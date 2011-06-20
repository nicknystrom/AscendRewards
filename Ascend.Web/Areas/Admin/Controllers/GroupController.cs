
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
using Spark;

namespace Ascend.Web.Areas.Admin.Controllers
{
    #region GroupCreateModel

	public class GroupCreateModel
	{
        [Required, StringLength(100)] public string Name { get; set; }
        [StringLength(100)] public string Catalog { get; set; }

		public Group CreateGroup()
		{
			return new Group {
                Name = Name,
                Catalog = Catalog,
				Document = new Document { Id = Document.For<Group>(Name.ToSlug()) },
			};
		}
	}

    #endregion
	#region GroupEditModel

	public class GroupEditModel
	{
        [Required, StringLength(100)] public string Name { get; set; }
        [DisplayName("Client Group Number")] public string Number { get; set; }
        [StringLength(100)] public string Catalog { get; set; }
        [StringLength(200), DisplayName("Home Page")] public string HomePage { get; set; }
        [StringLength(200), DisplayName("Terms of Service Page")] public string TermsOfService { get; set; }
        [StringLength(200), DisplayName("Banner Image")] public string BannerImage { get; set; }

		public static GroupEditModel FromDomain(Group g)
		{
			return new GroupEditModel {
			    Name = g.Name,
                Number = g.Number,
                Catalog = g.Catalog,
                HomePage = g.HomePage,
                TermsOfService = g.TermsOfService,
                BannerImage = g.BannerImage,
			};
		}
	
		public void Apply(Group g)
		{
		    g.Name = Name;
            g.Number = Number;
		    g.Catalog = Catalog;
		    g.HomePage = HomePage;
		    g.TermsOfService = TermsOfService;
		    g.BannerImage = BannerImage;
		}
	}
	
    #endregion

	[Precompile("*")]
    public partial class GroupController : AdminController
    {
		public IGroupRepository Groups { get; set; }
		
		[HttpGet]
		public virtual ActionResult Index()
        {
            return View(Groups.All());
        }

        [HttpPost]
        public virtual ActionResult Index(GroupCreateModel g)
        {
			ViewData["create"] = g;
			if (!ModelState.IsValid)
			{
                return View(Groups.All());
			}
			
			var x = g.CreateGroup();
            try
            {
				Groups.Save(x);
                return RedirectToAction(Actions.Edit, new { id = x.Document.Id });
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(Groups.All());
            }
        }

		[HttpGet]
        public virtual ActionResult Edit(string id)
        {
            return View(GroupEditModel.FromDomain(Groups.Get(id)));
        }

        [HttpPost]
        public virtual ActionResult Edit(string id, GroupEditModel g)
        {
			var x = Groups.Get(id);
			if (!ModelState.IsValid)
			{
				return View(g);
			}
            try
            {
  			    g.Apply(x);
			    Groups.Save(x);
                return RedirectToAction(Actions.Index);
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(x);
            }
        }
    }
}
