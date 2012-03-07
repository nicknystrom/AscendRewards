
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core.Services.Caching;
using Ascend.Infrastructure;
using RedBranch.Hammock;

using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;

namespace Ascend.Web.Areas.Admin.Controllers
{
    #region AvailabilityEditModel

    public class AvailabilityEditModel
    {
        [DisplayName("Enabled")] public bool Enabled { get; set; }
        [DisplayName("Archived")] public bool Archived { get; set; }
        [DisplayName("Mode"), UIHint("Enum")] public AvailabilityMode Mode { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; } 
        [UIHint("GroupMultiChooser")] public string[] Groups { get; set; }
        [UIHint("UserMultiChooser")] public string[] Users { get; set; }

        public static AvailabilityEditModel FromResource(Resource r)
        {
            var availability = r.Availability ?? Availability.Public;
            return new AvailabilityEditModel
            {
                Enabled = r.Enabled,
                Archived = r.Archived,
                Mode = availability.Mode,
                From = availability.From,
                To = availability.To,
                Users = availability.Users,
                Groups = availability.Groups,
            };
        }

        public void Apply(Resource r)
        {
            r.Enabled = Enabled;
            r.Archived = Archived;
            r.Availability = new Availability
            {
                Mode = Mode,
                From = From,
                To = To,
                Users = Users,
                Groups = Groups,
            };
        }

        public override string ToString()
        {
            if (!Enabled) return "Disabled";
            if (Archived) return "Archived";
            switch (Mode)
            {
                case AvailabilityMode.AvailableToPublic:
                    return "Public";
                case AvailabilityMode.AvailableToAllUsers:
                    return "Registered Users";
                case AvailabilityMode.AvailableOnlyTo:
                    return "Specific Users";
                case AvailabilityMode.AvailableToEveryoneBut:
                    return "Registered Users with Exceptions";
            }
            return base.ToString();
        }
    }

    #endregion

    #region PageCreateModel

    public class PageCreateModel
	{
        public PageCreateModel()
        {
            Format = ContentFormat.Markdown;
        }

		[Required, StringLength(100)] public string Title { get; set; }
		[UIHint("Enum")] public ContentFormat Format { get; set; }

		public Page CreatePage()
		{
			return new Page {
				Document = new Document { Id = Document.For<Page>(Title.ToSlug()) },
				Content = new Content {
                    Title = Title,
                    Format = Format,
                },
			};
		}
	}
	
	#endregion
    #region ContentEditModel

    public class ContentEditModel
    {
        [StringLength(100)] public string Title { get; set; }
        [UIHint("Enum")] public ContentFormat Format { get; set; }
        [StringLength(1024*100), UIHint("TextArea")] public string Body { get; set; }

        public static ContentEditModel FromDomain(Content c)
        {
            c = (c ?? new Content());
            return new ContentEditModel {
                Title = c.Title.Or(String.Empty),
                Format = c.Format,
                Body = c.Body.Or(String.Empty),
            };
        }

        public Content ToDomain()
        {
            return new Content {
                Title = Title,
                Format = Format,
                Body = Body,    
            };
        }
    }

    #endregion
	#region PageEditModel
	
	public class PageEditModel
	{
        public AvailabilityEditModel Availability { get; set; }
        public ContentEditModel Content { get; set; }

		public static PageEditModel FromDomain(Page p)
		{
			return new PageEditModel {
			    Content = ContentEditModel.FromDomain(p.Content),
                Availability = AvailabilityEditModel.FromResource(p),
			};
		}
	
		public void Apply(Page p)
		{
            if (null != Content)
            {
                p.Content = Content.ToDomain();
            }
            if (null != Availability)
            {
                Availability.Apply(p);
            }
		}
	}
	
	#endregion
	
	
    public partial class PageController : AdminController
    {
		public IPageRepository Pages { get; set; }

        public IUserSummaryCache UserSummaries { get; set; }
        public IGroupSummaryCache GroupSummaries { get; set; }

		[HttpGet]   
		public virtual ActionResult Index()
        {
		    var pages = Pages.All().WithDocuments().ToArray();
            return View(pages);
        }


        public virtual ActionResult Details(int id)
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult Index(PageCreateModel p)
        {
			ViewData["p"] = p;
			if (!ModelState.IsValid)
			{
                return View(Pages.All().WithDocuments());
			}
			
			var x = p.CreatePage();
            try
            {
				Pages.Save(x);
                return RedirectToAction(Actions.Edit, new { id = x.Document.Id });
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(Pages.All().WithDocuments());
            }
        }

		[HttpGet]
        public virtual ActionResult Edit(string id)
        {
		    var model = PageEditModel.FromDomain(Pages.Get(id));
		    ViewData["groups"] = GroupSummaries;
		    ViewData["users"] = UserSummaries[model.Availability.Users ?? new string[0]];
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Edit(string id, PageEditModel p)
        {
			var x = Pages.Get(id);
			if (!ModelState.IsValid)
			{
				return View(p);
			}
            try
            {
                p.Apply(x);
				Pages.Save(x);
                return RedirectToAction(Actions.Index);
            }
            catch (Exception ex)
            {
                ViewData["groups"] = GroupSummaries;
                ViewData["users"] = UserSummaries[p.Availability.Users ?? new string[0]];
				Notifier.Notify(ex);
                return View(x);
            }
        }
    }
}
