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
	#region SurveyCreateModel

	public class SurveyCreateModel
	{
		[Required, StringLength(100)] public string Title { get; set; }

		public Survey CreateSurvey()
		{
			return new Survey {
                Content = new Content {
                    Title = Title,
                },
                Document = new Document { Id = Document.For<Survey>(Title.ToSlug()) },
                MaxTimes = 1,
			};
		}
	}
	
	#endregion
	#region SurveyEditModel
	
	public class SurveyEditModel : ProgramEditModel
	{
		[DisplayName("Max times taken (leave empty for unlimited).")] public int? MaxTimes { get; set; }
        [DisplayName("Flat survey value")] public int? Points { get; set; }

        public IList<SurveyQuestion> Questions { get; set; }

		public static SurveyEditModel FromDomain(Survey s, IAccountingService accounting)
		{
			var x = new SurveyEditModel {
                Points = s.Points,
                Questions = s.Questions,
                MaxTimes = s.MaxTimes,
			};
            x.Init(s, accounting);
            return x;
		}
	
		public void Apply(Survey s, IAccountingService accounting)
		{
		    s.Questions = Questions;
		    s.Points = Points;
            s.MaxTimes = MaxTimes;
            base.Apply(s, accounting);
		}
	}
	
	#endregion
	
	
    public partial class SurveyController : AdminController
    {
		public ISurveyRepository Surveys { get; set; }
        public IAccountingService Accounting { get; set; }

        public IUserSummaryCache UserSummaries { get; set; }
        public IGroupSummaryCache GroupSummaries { get; set; }

		[HttpGet]
		public virtual ActionResult Index()
        {
            return View(Surveys.All().WithDocuments());
        }

        [HttpPost]
        public virtual ActionResult Index(SurveyCreateModel s)
        {
			ViewData["s"] = s;
			if (!ModelState.IsValid)
			{
                return View(Surveys.All().WithDocuments());
			}
			
			var x = s.CreateSurvey();
            try
            {
				Surveys.Save(x);
                return this.RedirectToAction(c => c.Edit(x.Document.Id));
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(Surveys.All().WithDocuments());
            }
        }

		[HttpGet]
        public virtual ActionResult Edit(string id)
        {
		    var model = SurveyEditModel.FromDomain(Surveys.Get(id), Accounting);
            ViewData["groups"] = GroupSummaries;
            ViewData["users"] = UserSummaries[model.Availability.Users ?? new string[0]];
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Edit(string id, SurveyEditModel s)
        {
			var x = Surveys.Get(id);
			if (!ModelState.IsValid)
			{
				return View(s);
			}
            try
            {
                s.Apply(x, Accounting);
				Surveys.Save(x);
                return this.RedirectToAction(c => c.Index());
            }
            catch (Exception ex)
            {
                ViewData["groups"] = GroupSummaries;
                ViewData["users"] = UserSummaries[s.Availability.Users ?? new string[0]];
				Notifier.Notify(ex);
                return View(s);
            }
        }
    }
}
