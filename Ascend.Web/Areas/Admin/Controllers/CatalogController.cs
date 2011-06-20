
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

namespace Ascend.Web.Areas.Admin.Controllers
{
    #region CatalogCreateModel

    public class CatalogCreateModel
	{
        [Required, StringLength(100)] public string Name { get; set; }
        
		public Catalog CreateCatalog()
		{
			return new Catalog {
                Name = Name,
				Document = new Document { Id = Document.For<Catalog>(Name.ToSlug()) },
			};
		}
	}

    #endregion
    #region CatalogEditModel

    public class CatalogEditModel
	{
        [Required, StringLength(100)] public string Name { get; set; }
        public MarkEditModel Default { get; set; }
        public MarkEditModel[] Marks { get; set; }

		public static CatalogEditModel FromDomain(Catalog g)
		{
			return new CatalogEditModel {
			    Name = g.Name,
                Default = MarkEditModel.FromDomain(g.DefaultMark ?? new Mark()),
                Marks = (g.CategoryMarks ?? new List<CategoryMark>()).Select(x => MarkEditModel.FromDomain(x)).ToArray(),
			};
		}
	
		public void Apply(Catalog c)
		{
		    c.Name = Name;
		    c.DefaultMark = (Default ?? new MarkEditModel()).ToDomain().Mark;
		    c.CategoryMarks = (Marks ?? new MarkEditModel[0]).Select(x => x.ToDomain()).ToList();
		}
	}

    #endregion
    #region MarkEditModel

    public class MarkEditModel
    {
        public string Category { get; set; }
        
        [DisplayName("Max. Price Below Msrp")] public decimal MaxPriceBelowMsrp { get; set; }
        [DisplayName("Min. Markup in Percent")] public decimal MinMarkupPercent { get; set; }
        [DisplayName("Min. Markup in Dollars")] public decimal MinMarkupDollars { get; set; }

        public static MarkEditModel FromDomain(CategoryMark m)
        {
            var x = FromDomain(m.Mark);
            x.Category = String.Join("/", m.Category);
            return x;
        }        
        
        public static MarkEditModel FromDomain(Mark m)
        {
            return new MarkEditModel
                       {
                           MaxPriceBelowMsrp = (m.MaxPriceBelowMsrp * 100),
                           MinMarkupPercent = (m.MinMarkupPercent * 100),
                           MinMarkupDollars = m.MinMarkupDollars,
                       };
        }

        public CategoryMark ToDomain()
        {
            return new CategoryMark
                       {
                           Category = (null == Category) ? new string[0] : Category.Split('/').Select(x => x.Trim()).ToArray(),
                           Mark = new Mark
                                      {
                                          MaxPriceBelowMsrp = (MaxPriceBelowMsrp / 100),
                                          MinMarkupPercent = (MinMarkupPercent / 100),
                                          MinMarkupDollars = MinMarkupDollars,
                                      }
                       };
        }
    }

    #endregion

    
    public partial class CatalogController : AdminController
    {
		public ICatalogRepository Catalogs { get; set; }
        public ICatalogService CatalogService { get; set; }
		
		[HttpGet]
		public virtual ActionResult Index()
        {
            return View(Catalogs.All());
        }

        [HttpPost]
        public virtual ActionResult Index(CatalogCreateModel c)
        {
			ViewData["create"] = c;
			if (!ModelState.IsValid)
			{
                return View(Catalogs.All());
			}
			
			var x = c.CreateCatalog();
            try
            {
				Catalogs.Save(x);
                return RedirectToAction(Actions.Edit, new { id = x.Document.Id });
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(Catalogs.All());
            }
        }

		[HttpGet]
        public virtual ActionResult Edit(string id)
        {
            return View(CatalogEditModel.FromDomain(Catalogs.Get(id)));
        }

        [HttpPost]
        public virtual ActionResult Edit(string id, CatalogEditModel c)
        {
			var x = Catalogs.Get(id);
			if (!ModelState.IsValid)
			{
				return View(c);
			}
            try
            {
  			    c.Apply(x);
			    Catalogs.Save(x);
                return RedirectToAction(Actions.Index);
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(c);
            }
        }
    }
}
