
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
	#region MenuCreateModel

	public class MenuCreateModel
	{
		[Required, StringLength(100)] public string Name { get; set; }

		public Menu CreateMenu()
		{
			return new Menu {
				Name = Name,
				Document = new Document { Id = Document.For<Menu>(Name.ToSlug()) },
			};
		}
	}
	
	#endregion
	#region MenuEditModel
	
	public class MenuEditModel
	{
		[Required, StringLength(100)] public string Name { get; set; }
        public IList<MenuItemEditModel> Items { get; set; }

		private static void _FillItems(int depth, IMenuItemContainer container, IList<MenuItemEditModel> items)
		{
		    if (null == container.Items) return;
		    foreach (var i in container.Items)
		    {
		        items.Add(new MenuItemEditModel
		                      {
		                          Indent = depth,
		                          Name = i.Name,
		                          Type = i.Type,
		                          Location = i.Location,
		                      });
		        _FillItems(depth + 1, i, items);
		    }
		}

	    public static MenuEditModel FromDomain(Menu m)
		{
		    var a = new List<MenuItemEditModel>();
		    _FillItems(0, m, a);

			return new MenuEditModel {
				Name = m.Name,	
                Items = a,
			};
		}
	
		public void Apply(Menu m)
		{
			m.Name = Name;
		    m.Items = new List<MenuItem>();

		    var j = new Stack<MenuItem>();
            foreach (var i in Items)
            {
                var a = new MenuItem {Name = i.Name, Type = i.Type, Location = i.Location};
                if (i.Indent == 0)
                {
                    j.Clear();
                    j.Push(a);
                    m.AddItem(a);
                }
                else
                {
                    // rewind the that stack until we reach the parent node. creates the
                    // effect  the stack always represents a 'stair-step' pattern
                    while (j.Count > i.Indent) j.Pop();
                    j.Peek().AddItem(a);
                    j.Push(a);
                }
            }
		}
	}

    public class MenuItemEditModel
    {
        public int Indent { get; set; }
        public string Name { get; set; }
        public MenuItemType Type { get; set; }
        public string Location { get; set; }
    }
	
	#endregion
	
	
    public partial class MenuController : AdminController
    {
		public IMenuRepository Menus { get; set; }
		
		[HttpGet]
		public virtual ActionResult Index()
        {
            return View(Menus.All().WithDocuments());
        }


        public virtual ActionResult Details(int id)
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult Index(MenuCreateModel m)
        {
			ViewData["m"] = m;
			if (!ModelState.IsValid)
			{
                return View(Menus.All().WithDocuments());
			}
			
			var x = m.CreateMenu();
            try
            {
				Menus.Save(x);
                return RedirectToAction(Actions.Edit, new { id = x.Document.Id });
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(Menus.All().WithDocuments());
            }
        }

		[HttpGet]
        public virtual ActionResult Edit(string id)
        {
            return View(MenuEditModel.FromDomain(Menus.Get(id)));
        }

        [HttpPost]
        public virtual ActionResult Edit(string id, MenuEditModel m)
        {
			var x = Menus.Get(id);
			if (!ModelState.IsValid)
			{
				return View(m);
			}
            try
            {
                m.Apply(x);
				Menus.Save(x);
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
