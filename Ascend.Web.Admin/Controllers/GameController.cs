using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RedBranch.Hammock;

using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;
using Ascend.Infrastructure.Web;

namespace Ascend.Web.Admin.Controllers
{
	#region GameCreateModel

	public class GameCreateModel
	{
        [UIHint("Enum")]
        public GameType Type { get; set; }
		[Required, StringLength(100)] public string Title { get; set; }

		public Game CreateGame()
		{
			return new Game {
				Content = new Content {
                    Title = Title,
                },
                Type = Type,
                Document = new Document { Id = Document.For<Game>(Title.ToSlug()) },
			};
		}
	}
	
	#endregion
	#region GameEditModel
	
	public class GameEditModel : ProgramEditModel
	{
        [UIHint("Enum")]
        public GameType Type { get; set; }
        public int? Award { get; set; }
        public int? TimeLimit { get; set; }

		
		public static GameEditModel FromDomain(Game g, IAccountingService accounting)
		{
			var x = new GameEditModel {
                Type = g.Type,
                Award = g.Award,
                TimeLimit = g.TimeLimit,
			};
            x.Init(g, accounting);
            return x;
		}

        public void Apply(Game g, IAccountingService accounting)
		{
		    g.Type = Type;
            g.Award = Award;
            g.TimeLimit = TimeLimit;
            base.Apply(g, accounting);
        }
	}
	
	#endregion
	
	
    public partial class GameController : AdminController
    {
		public IGameRepository Games { get; set; }
        public IAccountingService Accounting { get; set; }

        public IUserSummaryCache UserSummaries { get; set; }
        public IGroupSummaryCache GroupSummaries { get; set; }

		[HttpGet]
		public virtual ActionResult Index()
        {
            return View(Games.All().WithDocuments());
        }

        [HttpPost]
        public virtual ActionResult Index(GameCreateModel g)
        {
			ViewData["g"] = g;
			if (!ModelState.IsValid)
			{
                return View(Games.All().WithDocuments());
			}
			
			var x = g.CreateGame();
            try
            {
				Games.Save(x);
                return this.RedirectToAction(c => c.Edit(x.Document.Id));
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(Games.All().WithDocuments());
            }
        }

		[HttpGet]
        public virtual ActionResult Edit(string id)
        {
		    var model = GameEditModel.FromDomain(Games.Get(id), Accounting);
            ViewData["groups"] = GroupSummaries;
            ViewData["users"] = UserSummaries[model.Availability.Users ?? new string[0]];
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Edit(string id, GameEditModel g)
        {
			var x = Games.Get(id);
			if (!ModelState.IsValid)
			{
				return View(g);
			}
            try
            {
                g.Apply(x, Accounting);
				Games.Save(x);
                return this.RedirectToAction(c => c.Index());
            }
            catch (Exception ex)
            {
                ViewData["groups"] = GroupSummaries;
                ViewData["users"] = UserSummaries[g.Availability.Users ?? new string[0]];
				Notifier.Notify(ex);
                return View(g);
            }
        }
    }
}
