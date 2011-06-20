using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;

namespace Ascend.Web.Areas.Site.Controllers
{
    public partial class HomeController : SiteController
    {
        public IUserRepository UsersRepository { get; set; }

        [HttpGet]
        public virtual ActionResult Index()
        {
            // look at either the application default home page or the group specific home page
            var page = Application.DefaultHomePage;
            if (null != CurrentGroup && 
                !String.IsNullOrEmpty(CurrentGroup.HomePage))
            {
                page = CurrentGroup.HomePage;
            }
            return RedirectToAction(MVC.Site.Page.Index(page));
        }

        [HttpGet]
        public virtual ActionResult Terms()
        {
            // look at either the application default tos or the group specific tos
            var page = Application.DefaultTermsOfService;
            if (null != CurrentGroup &&
                !String.IsNullOrEmpty(CurrentGroup.TermsOfService))
            {
                page = CurrentGroup.TermsOfService;
            }

            ViewData["updated"] = CurrentUser.DateAcceptedTermsOfService.HasValue;
            try
            {
                return View(ContentViewModel.FromDomain(Pages[page].Content));    
            }
            catch
            {
                throw new HttpException((int) HttpStatusCode.NotFound, "No terms of service page could be found.");
            }
        }

        [HttpPost]
        public virtual ActionResult Terms(bool accept)
        {
            if (accept)
            {
                CurrentUser.DateAcceptedTermsOfService = DateTime.UtcNow;
                UsersRepository.Save(CurrentUser);
                return RedirectToAction(Actions.Index);
            }
            return Terms();
        }
    }
}
