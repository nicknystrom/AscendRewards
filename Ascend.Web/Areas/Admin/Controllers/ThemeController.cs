
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
	#region ThemeCreateModel

	public class ThemeCreateModel
	{
		[Required, StringLength(100)] public string Name { get; set; }
		[Required, StringLength(100)] public string Stylesheet { get; set; }

        public ThemeCreateModel()
	    {
            Stylesheet = "Site";
	    }

	    public Theme CreateTheme()
		{
			return new Theme {
				Name = Name,
                Stylesheet = Stylesheet,
				Document = new Document { Id = Document.For<Theme>(Name.ToSlug()) },
			};
		}
	}
	
	#endregion
	#region ThemeEditModel
	
	public class ThemeEditModel
	{
		[Required, StringLength(100)] public string Name { get; set; }
		
        [Required, StringLength(100),
         Description("Filename of the stylesheet that renders this theme. Use 'Site' as a default.")]
        public string Stylesheet { get; set; }
        
        public string Banner { get; set; }

        [UIHint("TextArea"), Description("Custom Site CSS")] public string CustomSite { get; set; }
        [UIHint("TextArea"), Description("Custom Login CSS")] public string CustomLogin { get; set; }

        [UIHint("Strings"), Description("Banners images rotated at the top of the login screens.")] public string[] LoginBanners { get; set; }
        [UIHint("Strings"), Description("Images rotated to the right of the username/password entry.")] public string[] LoginInfos { get; set; }

        public string[] ColorNames { get; set; }
        public string[] FontNames { get; set; }
        public string[] BoxNames { get; set; }

        public string[] ColorValues { get; set; }
        public ThemeFont[] FontValues { get; set; }
        public ThemeBox[] BoxValues { get; set; }

        public string[] FontPreviews { get; set; }
		
		public static ThemeEditModel FromDomain(Theme t)
		{
            t.Colors = (t.Colors ?? new Dictionary<string, string>());
            t.Fonts = (t.Fonts ?? new Dictionary<string, ThemeFont>());
            t.Boxes = (t.Boxes ?? new Dictionary<string, ThemeBox>());
			return new ThemeEditModel {
				Name = t.Name,	
                Stylesheet = t.Stylesheet,
                Banner = t.Banner,
                LoginBanners = t.LoginBanners,
                LoginInfos = t.LoginInfos,
                CustomSite = t.CustomSite,
                CustomLogin = t.CustomLogin,
                ColorNames = t.Colors.Keys.ToArray(),
                ColorValues = t.Colors.Values.ToArray(),
                FontNames = t.Fonts.Keys.ToArray(),
                FontValues = t.Fonts.Values.ToArray(),
                FontPreviews = t.Fonts.Values.Select(x => x.ToString(t)).ToArray(),
                BoxNames = t.Boxes.Keys.ToArray(),
                BoxValues = t.Boxes.Values.ToArray(),
			};
		}
	
		public void Apply(Theme t)
		{
			t.Name = Name;
            t.Banner = Banner;
            t.Stylesheet = Stylesheet;
		    t.LoginBanners = LoginBanners.Clean();
		    t.LoginInfos = LoginInfos.Clean();
            t.CustomLogin = CustomLogin;
            t.CustomSite = CustomSite;
            if (null != ColorNames && null != ColorValues)
            {
                t.Colors = new Dictionary<string, string>();
                for (var i = 0; i < ColorNames.Length; i++)
                {
                    t.Colors.Add(ColorNames[i], ColorValues[i]);
                }
            }
            if (null != FontNames && null != FontValues)
            {
                t.Fonts = new Dictionary<string, ThemeFont>();
                for (var i=0; i < FontNames.Length; i++)
                {
                    t.Fonts.Add(FontNames[i], FontValues[i]);
                }
            }
            if (null != BoxNames && null != BoxValues)
            {
                t.Boxes = new Dictionary<string, ThemeBox>();
                for (var i=0; i < BoxNames.Length; i++)
                {
                    t.Boxes.Add(BoxNames[i], BoxValues[i]);
                }
            }
		}
	}
	
	#endregion
	
	
    public partial class ThemeController : AdminController
    {
		public IThemeRepository Themes { get; set; }
		
		[HttpGet]
		public virtual ActionResult Index()
        {
            return View(Themes.All().WithDocuments());
        }

        [HttpPost]
        public virtual ActionResult Index(ThemeCreateModel t)
        {
			ViewData["t"] = t;
			if (!ModelState.IsValid)
			{
                return View(Themes.All().WithDocuments());
			}
			
			var x = t.CreateTheme();
            try
            {
				Themes.Save(x);
                return RedirectToAction(Actions.Edit, new { id = x.Document.Id });
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(Themes.All().WithDocuments());
            }
        }

		[HttpGet]
        public virtual ActionResult Edit(string id)
        {
            return View(ThemeEditModel.FromDomain(Themes.Get(id)));
        }

        [HttpPost]
        public virtual ActionResult Edit(string id, ThemeEditModel t)
        {
			var x = Themes.Get(id);
            if (!ModelState.IsValid)
			{
				return View(t);
			}
            try
            {
                t.Apply(x);
				Themes.Save(x);
                Notifier.Notify(Severity.Success, "Theme saved.", "You may continue editing this theme.", null);
                return Redirect(Request.RawUrl);
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(x);
            }
        }
    }
}
