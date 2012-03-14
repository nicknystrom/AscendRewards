using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;
using Ascend.Web.Areas.Site.Controllers;

using Spark;

namespace Ascend.Web.Areas.Public.Controllers
{
    public class PasswordResetViewModel
    {
        [Required,
         StringLength(100),
         DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }

    public class PublicMenuItemViewModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class RegisterViewModel
    {
        [Required, StringLength(100)] public string FirstName { get; set; }
        [Required, StringLength(100)] public string LastName { get; set; }
        [Required, StringLength(100)] public string Email { get; set; }
        [Required, StringLength(100)] public string Password { get; set; }
        [Required, StringLength(100)] public string Confirm { get; set; }
    }

    [Precompile("*")]
    public partial class LoginController : PublicController
    {
        public IUserRepository Users { get; set; }
        public IUserMessaging Messaging { get; set; }
        public ITenantRepository Tenants { get; set; }

        public IEntityCache<Theme> Themes { get; set; }
        public IEntityCache<Page> Pages { get; set; }
        public IEntityCache<Menu> Menus { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // rotate the splash image
            var theme = Themes[Application.DefaultTheme];
            ViewData["splashCount"] = (null == theme.LoginBanners) ? 0 : theme.LoginBanners.Length;
            ViewData["infoCount"] = (null == theme.LoginInfos) ? 0 : theme.LoginInfos.Length;

            // supply the footer menu
            var menu = Menus.TryGet(Application.DefaultLoginMenu);
            if (null != menu && null != menu.Items)
            {
                var menuViewModel =
                    menu.Items.Where(x => x.Type == MenuItemType.Url || (x.Type == MenuItemType.Page && Pages[x.Location].IsAllowedAccess(null, DateTime.UtcNow).Available))
                              .Select(x => new PublicMenuItemViewModel
                              {
                                  Name = x.Name,
                                  Url = x.Type == MenuItemType.Page ? Url.Action(MVC.Public.Login.Page(x.Location)) : x.Location
                              })
                              .ToList();
                ViewData["footer"] = menuViewModel;
            }
        }

        [HttpGet]
        public virtual ActionResult Page(string page)
        {
            var p = Pages[page];
            if (!p.IsAllowedAccess(null, DateTime.UtcNow).Available) throw new HttpException((int)HttpStatusCode.Forbidden, "Forbidden");
            return Content(ContentViewModel.FromDomain(p.Content).Html);
        }

        [HttpGet]
        public virtual ActionResult Find(string e)
        {
            if (String.IsNullOrWhiteSpace(e))
            {
                return View("NoDomain");
            }
            var t = Tenants.All().FirstOrDefault(x => !String.IsNullOrWhiteSpace(x.EmailDomain) && e.Contains(x.EmailDomain));
            if (null == t)
            {
                ViewData["e"] = e;
                return View("NoDomain");
            }

            Session["RedirectedLogin"] = e;
            var host = t.Match[0];
            if (!host.StartsWith("http://") ||
                !host.StartsWith("https://"))
            {
                host = "http://" + host;
            }
            var uri = new Uri(host, UriKind.Absolute);
            uri = new Uri(uri, "/login");
            return Redirect(uri.ToString());
        }

        [HttpGet]
        public virtual ActionResult Index()
        {
            if (!String.IsNullOrEmpty(Application.MaintenancePage))
            {
                return View("Maintenance", PageViewModel.FromDomain(Pages[Application.MaintenancePage]));
            }

            ViewData["login"] = Session["RedirectedLogin"];
            return View();
        }

        protected void Login(User user)
        {
            user.IncrementLogins(ModelState.IsValid, Request);
            FormsAuthentication.SetAuthCookie(
                user.Document.Id,
                true);
        }

        [HttpPost]
        public virtual ActionResult Index(string login, string password)
        {
            ViewData["login"] = login;

            var user = Users.Where(x => x.Login).Eq(login).SingleOrDefault();
            if (null == user)
            {
                ModelState["login"].Errors.Add("Your username or password was not correct.");
                ModelState["password"].Errors.Add("");
                return View();
            }
            else if (!user.CheckPassword(password))
            {
                ModelState["login"].Errors.Add("Your username or password was not correct.");
                ModelState["password"].Errors.Add("");
            }
            else if (user.State == UserState.Registered)
            {
                ModelState["login"].Errors.Add("Please activate your account before logging in.");
            }
            else if (user.State == UserState.Suspended || user.State == UserState.Terminated)
            {
                ModelState["login"].Errors.Add("This account is no longer active.");
            }

            if (!ModelState.IsValid)
            {
                user.IncrementLogins(false, Request);
                Users.Save(user);
                return View();
            }
            
            Login(user);
            Users.Save(user);
            
            var returnUrl = Request.QueryString["ReturnUrl"];
            if (!String.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(MVC.Site.Home.Index());
        }

        public virtual ActionResult Logout()
        {
            // Session.Abandon(); <-- crashes mono xsp4 for now
            FormsAuthentication.SignOut();
            return RedirectToAction(Actions.Index);
        }

        [HttpGet]
        public virtual ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var attempt = Messaging.SendRegistrationNotice(
                ControllerContext.RequestContext,
                model.FirstName,
                model.LastName,
                model.Email,
                model.Password
            );
            return View("RegisterSuccess");
        }

        [HttpGet]
        public virtual ActionResult Activate(string code)
        {
            User u = null;
            if (String.IsNullOrEmpty(code) ||
                null == (u = Users.Where(x => x.ActivationCode).Eq(code).SingleOrDefault()))
            {
                Notifier.Notify(
                    Severity.Warn,
                    "Activation code not found.",
                    @"We're sorry, but we couldn't find an account with that activation code. Activation codes
                    only work once; so if you've already activated simply enter your username and password to login.",
                    null);
            }
            else
            {
                try
                {
                    // activate the user
                    u.ActivationCode = null;
                    u.State = UserState.Active;
                    Users.Save(u);

                    // send the welcome message
                    try
                    {
                        Messaging.SendWelcome(
                            ControllerContext.RequestContext,
                            u);
                    }
                    catch
                    {
                    }

                    Login(u);

                    // if the show profile setting is on, log the user in and send them to the profile page
                    if (!Application.ShowProfileOnActivate)
                    {
                        Notifier.Notify(
                            Severity.Success,
                            "Account succesfully activated!",
                            @"You may now login to the site with the username and password you chose during registration.",
                            null);
                    }

                    return RedirectToAction(MVC.Site.Home.Index());
                }
                catch
                {
                    Notifier.Notify(
                        Severity.Error,
                        "Error activating account.",
                        @"We're sorry, but we encountered a technical problem activating your account. Please
                        contact customer service immediatley and we will manually activate your account for you.",
                        null);
                }
            }

            if (null != u)
            {
                Users.Save(u);
            }
            return RedirectToAction(Actions.Index);
        }

        [HttpGet]
        public virtual ActionResult Reset()
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult Reset(PasswordResetViewModel reset)
        {
            if (!ModelState.IsValid)
            {
                return View(reset);
            }
            var email = reset.Email;

            var u = Users.Where(x => x.Email).Eq(email).SingleOrDefault();
            if (null == u)
            {
                Notifier.Notify(
                    Severity.Warn,
                    "No user found.",
                    @"We couldn't find a user with the email address, '" + email +
                    @"'. Although
                    capitalization is not important, you must spell the address exactly as you did
                    when you registerd your account, including punctuation (like periods) and the
                    domain name (like '@gmail.com'). If you've entered your email address correctly and
                    still receive this message, please contact customer supoprt immediatley.",
                    null);
                ModelState.AddModelError("Email", "No user found with that email address.");
                return View(reset);
            }

            var newPassowrd = Core.User.GeneratePassword();
            try
            {
                u.SetPassword(newPassowrd);
                Users.Save(u);    
            }
            catch
            {
                Notifier.Notify(
                    Severity.Error,
                    "Error reseting pasword.",
                    @"We found your account but encountered an unexpected technical error while reseting
                    your password. Please contact customer support immediatley to resolve the issue.",
                    null);
                return RedirectToAction(Actions.Index);
            }

            try
            {
                Messaging.SendReset(ControllerContext.RequestContext, u, newPassowrd);
            }
            catch
            {
                Notifier.Notify(
                    Severity.Warn,
                    "Password reset, but we couldn't send you an email.",
                    @"We found your account and reset your password, but an unexpected technical error occurered
                    when we tried to send you an email with your new password. Please try once more then contact
                    customer support immediatley if this problem contiues.",
                    null);
                return RedirectToAction(Actions.Index);
            }

            Notifier.Notify(
                Severity.Success,
                "Your password has been reset.",
                @"You should be receiving an email shortly with instructions for logging into the site. If you
                don't receive this email in the next few minutes, double check that it hasn't been marked as spam,
                try to reset your password again, and if that doesn't work, simply contact customer service
                to resolve the issue. Thanks!",
                null);

            return View("ResetSuccess");
        }
    }
}