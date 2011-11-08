
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Ascend.Core.Services.Caching;
using RedBranch.Hammock;

using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;

namespace Ascend.Web.Areas.Admin.Controllers
{
    #region UserCreateModel

    public class UserCreateModel
    {
        public UserCreateModel()
        {
            Password = User.GeneratePassword();
        }

        [Required, StringLength(40)]
        public string Login { get; set; }
        [Required, DataType(DataType.Password), StringLength(20)]
        public string Password { get; set; }
        [DataType(DataType.EmailAddress), StringLength(100)]
        public string Email { get; set; }
        [DisplayName("First Name"), StringLength(50)]
        public string FirstName { get; set; }
        [DisplayName("Last Name"), StringLength(50)]
        public string LastName { get; set; }
        public bool Activate { get; set; }
        public bool Welcome { get; set; }

        public User CreateUser()
        {
            var u = new User
            {
                Login = Login,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                State = UserState.Registered,
                StateChanged = DateTime.UtcNow,
                DateRegistered = DateTime.UtcNow,
                Document = new Document { Id = Document.For<User>(Login.ToSlug()) }
            };
            u.State = Activate ? UserState.Active : UserState.Registered;
            u.SetPassword(Password);
            if (!Activate)
            {
                u.SetActivationCode();
            }
            return u;
        }
    }

    #endregion
    #region UserEditModel

    public class LoginIncrements
    {
        public int Count { get; set; }
        public DateTime? Last { get; set; }
    }

    public class UserEditModel
    {
        [DataType(DataType.Password), StringLength(20)] public string Password { get; set; }

        [UIHint("UserChooser")] public string Manager { get; set; }
        [UIHint("GroupChooser")] public string Group { get; set; }
        [UIHint("GroupMultiChooser")] public string[] ManagedGroups { get; set; }

        public UserState State { get; set; }

        [DisplayName("Birth Date")] public DateTime? DateBirth { get; set; }
        [DisplayName("Hire Date")] public DateTime? DateHired { get; set; }

        [DataType(DataType.EmailAddress), StringLength(100)] public string Email { get; set; }
        public string Title { get; set; }
        public string EmployeeId { get; set; }
        [DisplayName("First Name")] public string FirstName { get; set; }
        [DisplayName("Last Name")] public string LastName { get; set; }
        public Address HomeAddress { get; set; }
        public Address WorkAddress { get; set; }
        public Phone HomePhone { get; set; }
        public Phone WorkPhone { get; set; }
        public Phone MobilePhone { get; set; }
        public IDictionary<string, string> Custom { get; set; }
        public UserPermissions Permissions { get; set; }
        
        // fields for display only
        public string Login { get; set; }
        [DisplayName("Failed Logins")] public LoginIncrements FailedLogins { get; set; }
        [DisplayName("Logins")] public LoginIncrements SuccessfulLogins { get; set; }
        [DisplayName("ToS Accepted")] public DateTime? DateAcceptedTermsOfService { get; set; }
        [DisplayName("Registered")] public DateTime? DateRegistered { get; set; }
        [DisplayName("Activated")] public DateTime? DateActivated { get; set; }
        [DisplayName("Suspended")] public DateTime? DateSuspended { get; set; }
        [DisplayName("Terminated")] public DateTime? DateTerminated { get; set; }
        
        public Ledger PointsLedger { get; set; }
        public Ledger BudgetLedger { get; set; }
        public BudgetEditModel Budget { get; set; }

        public void AddBudgets(User u, IAccountingService accounting)
        {
            PointsLedger = accounting.GetPointsLedger(u);
            BudgetLedger = accounting.GetBudgetLedger(u);
            Budget = (null != BudgetLedger &&
            null != BudgetLedger.Account)
                ? BudgetEditModel.FromDomain(BudgetLedger.Account.Budget)
                : new BudgetEditModel();
        }

        public void AddActivity(UserActivitySummary logins)
        {
            FailedLogins = new LoginIncrements { Count = logins.FailedLogins, Last = logins.LastFailedLogin };
            SuccessfulLogins = new LoginIncrements { Count = logins.SuccessfulLogins, Last = logins.LastSuccesfulLogin };
        }

        public static UserEditModel FromDomain(User u, UserActivitySummary logins, IAccountingService accounting)
        {
            var x = new UserEditModel
                       {
                            Manager = u.Manager,
                            Group = u.Group,
                            ManagedGroups = u.ManagedGroups,
                            State = u.State,
                            DateBirth = u.DateBirth,
                            DateHired = u.DateHired,
                            Email = u.Email,
                            Title = u.Title,
                            EmployeeId = u.EmployeeId,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            HomeAddress = u.HomeAddress,
                            WorkAddress = u.WorkAddress,
                            HomePhone = u.HomePhone,
                            WorkPhone = u.WorkPhone,
                            MobilePhone = u.MobilePhone,
                            Custom = u.Custom,
                            Permissions = (u.Permissions ?? new UserPermissions()),
                            

                            // display only fields
                            Login = u.Login,
                            DateAcceptedTermsOfService = u.DateAcceptedTermsOfService,
                            DateRegistered = u.DateRegistered,
                            DateActivated = u.DateActivated,
                            DateSuspended = u.DateSuspended,
                            DateTerminated = u.DateTerminated,
                      };
            x.AddBudgets(u, accounting);
            x.AddActivity(logins);
            return x;
        }

        public void Apply(User u, IAccountingService accounting)
        {
            if (!String.IsNullOrEmpty(Password))
            {
                u.SetPassword(Password);
            }
            u.Manager = Manager;
            u.Group = Group;
            u.ManagedGroups = ManagedGroups;
            u.State = State;
            u.DateBirth = DateBirth;
            u.DateHired = DateHired;
            u.Title = Title;
            u.EmployeeId = EmployeeId;
            u.Email = Email;
            u.FirstName = FirstName;
            u.LastName = LastName;
            u.HomeAddress = HomeAddress;
            u.WorkAddress = WorkAddress;
            u.HomePhone = HomePhone;
            u.WorkPhone = WorkPhone;
            u.MobilePhone = MobilePhone;
            u.Custom = Custom;
            u.Permissions = Permissions;

            accounting.SetUserBudget(u, null == Budget ? null : Budget.ToBudget());
        }   
    }       

    #endregion

	
    public partial class UserController : AdminController
    {
		public IUserRepository Users { get; set; }
        public IUserActivityRepository UserActivity { get; set; }
        public IOrderRepository Orders { get; set; }
        public IUserMessaging Messaging { get; set; }
        public IAccountingService Accounting { get; set; }

        public IGroupSummaryCache GroupSummaryCache { get; set; }
        public IUserSummaryCache UserSummaryCache { get; set; }
		
		[HttpGet]
		public virtual ActionResult Index()
        {
            return View(Users.All().WithDocuments());
        }

		[HttpPost]
        public virtual ActionResult Index(UserCreateModel u)
        {
			ViewData["create"] = u;
            ValidateEmail(null, u.Email, Users, ModelState);
            ValidateLogin(null, u.Login, Users, ModelState);
			if (!ModelState.IsValid)
			{
                return View(Users.All().WithDocuments());
			}
			
			var x = u.CreateUser();
            try
            {
				Users.Save(x);
                if (u.Welcome)
                {
                    if (x.State == UserState.Active)
                    {
                        Messaging.SendWelcome(ControllerContext.RequestContext, x);
                    }
                    else if (x.State == UserState.Registered)
                    {
                        Messaging.SendActivation(ControllerContext.RequestContext, x);
                    }
                }
                return RedirectToAction(Actions.Edit, new { id = x.Document.Id });
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(Users.All().WithDocuments());
            }
        }

		[HttpGet]
        public virtual ActionResult Edit(string id)
        {
		    var x = Users.Get(id);
		    ViewData["id"] = id;
            ViewData["users"] = UserSummaryCache;
            ViewData["groups"] = GroupSummaryCache;
		    ViewData["orders"] = Orders.GetOrdersForUser(x);
		    return View(UserEditModel.FromDomain(x, UserActivity.GetUserActivitySummary(x), Accounting));
        }

        [HttpPost]
        public virtual ActionResult Edit(string id, UserEditModel u)
        {
			var x = Users.Get(id);
            ViewData["id"] = id;
            ViewData["users"] = UserSummaryCache;
            ViewData["groups"] = GroupSummaryCache;
            ValidateEmail(x, u.Email, Users, ModelState);
            ValidateLogin(x, u.Login, Users, ModelState);
			if (!ModelState.IsValid)
			{
				return View(u);
			}
            try
            {
                u.Apply(x, Accounting);
				Users.Save(x);
                return RedirectToAction(Actions.Index);
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(u);
            }
        }

        [HttpGet]
        public virtual JsonResult Search(string term)
        {
            return Json(
                Users.Search(term)
                    .Select(x => new {
                        label = x.DisplayName,
                        location = Url.Action(MVC.Admin.User.Edit(x.Id)),
                        value = x.Id,
                    }),                       
                JsonRequestBehavior.AllowGet
            );
        }

        [HttpGet]
        public virtual ActionResult Assume(string id)
        {
            var u = UserSummaryCache[id];

            FormsAuthentication.SetAuthCookie(
                u.Id,
                true);

            Notifier.Notify(
                Severity.Warn,
                "You are now impersonating " + u.DisplayName + ".",
                "To resume your own identity you will need to manually log out. Please be careful not to make unexpected changes to this user's account.",
                null);
            return RedirectToAction(MVC.Site.Home.Index());
        }

        [HttpPost]
        public virtual ActionResult Reset(string id, bool? sendEmail)
        {
            var u = Users.Get(id);
            var pwd = Core.User.GeneratePassword();
            u.SetPassword(pwd);
            Users.Save(u);

            Notifier.Notify(
                Severity.Success,
                "Password reset.",
                "The users's password has been reset to '" + pwd + "'.",
                u);

            if (sendEmail.GetValueOrDefault())
            {
                Messaging.SendReset(ControllerContext.RequestContext, u, pwd);
                Notifier.Notify(
                    Severity.Success,
                    "Pasword reset email sent.",
                    "User should receive their new password in a few moments.",
                    u);
            }

            return RedirectToAction(MVC.Admin.User.Edit(id));
        }

        public static void ValidateLogin(
            User u,
            string login,
            IUserRepository users,
            ModelStateDictionary state)
        {
            if (!String.IsNullOrEmpty(login) &&
                (null == u || u.Login != login))
            {
                var same = users.Where(x => x.Login).Eq(login).List();
                if (same.Rows.Length > 0 &&
                    (null == u || same.Rows.First().Id != u.Document.Id))
                {
                    // there's already a user with this login address (besides the current one)
                    state.AddModelError("Login", "Login name already in use.");
                }
            }
        }

        public static void ValidateEmail(
            User u,
            string email,
            IUserRepository users,
            ModelStateDictionary state)
        {
            if (!String.IsNullOrEmpty(email) &&
                (null == u || u.Email != email))
            {
                var same = users.Where(x => x.Email).Eq(email).List();
                if (same.Rows.Length > 0 &&
                    (null == u || same.Rows.First().Id != u.Document.Id))
                {
                    // there's already a user with this email address (besides the current one)
                    state.AddModelError("Email", "Email address already in use.");
                }
            }
        }
    }
}
