using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Ascend.Core;
using Ascend.Core.Repositories;
using Newtonsoft.Json;

namespace Ascend.Web.Areas.Mobile.Controllers
{
    public partial class LoginController : MobileController
    {
        public IUserRepository UserRepository { get; set; }

        [HttpGet]
        public virtual ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult Index(string login, string password)
        {
            // validate credentials
            var auth = false;
            var user = UserRepository.Where(x => x.Login).Eq(login).SingleOrDefault();
            if (null == user || !user.CheckPassword(password))
            {
                ViewData["err"] = "Your username or password was not correct.";
            }
            else if (user.State == UserState.Registered)
            {
                ViewData["err"] = "Please activate your account before logging in.";  
            }
            else if (user.State == UserState.Suspended || user.State == UserState.Terminated)
            {
                ViewData["err"] = "This account is no longer active.";
            }
            else
            {
                auth = true;
                FormsAuthentication.SetAuthCookie(
                       user.Document.Id,
                       true);
            }

            // update state
            if (null != user)
            {
                user.IncrementLogins(auth, Request);
                UserRepository.Save(user);
                Init(user.Document.Id);
            }

            if (auth)
            {
                return RedirectToRoute(MobileRoutes.Home);
            }
            return PartialView();
        }

        [HttpPost]
        public virtual ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Init(null);

            return RedirectToRoute(MobileRoutes.Login);
        }
    }
}