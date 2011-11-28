using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Ascend.Core;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;
using Ascend.Core.Repositories;


namespace Ascend.Web.Areas.Dash.Controllers
{
    #region UserEditModel

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
        [DisplayName("ToS Accepted")] public DateTime? DateAcceptedTermsOfService { get; set; }
        [DisplayName("Registered")] public DateTime? DateRegistered { get; set; }
        [DisplayName("Activated")] public DateTime? DateActivated { get; set; }
        [DisplayName("Suspended")] public DateTime? DateSuspended { get; set; }
        [DisplayName("Terminated")] public DateTime? DateTerminated { get; set; }
        
        public Ledger PointsLedger { get; set; }
        public Ledger BudgetLedger { get; set; }

        public void AddBudgets(User u, IAccountingService accounting)
        {
            PointsLedger = accounting.GetPointsLedger(u);
            BudgetLedger = accounting.GetBudgetLedger(u);
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
        }
    }       

    #endregion

    public partial class UserController : DashController
    {
        public IAccountingService Accounting { get; set; }
        public IUserActivityRepository UserActivity { get; set; }
        public IUserSummaryCache UserSummaryCache { get; set; }

        [HttpGet]
        public ActionResult Index()
        {
            return View(UserSummaryCache.All);
        }

        [HttpGet]
        public ActionResult Edit(string userId)
        {
            var u = Users[userId];
            return View(UserEditModel.FromDomain(
                u,
                UserActivity.GetUserActivitySummary(u),
                Accounting
            ));
        }

        [HttpPost]
        public ActionResult Edit(string userId, UserEditModel model)
        {
            var u = Users[userId];
            if (ModelState.IsValid)
            {
                model.Apply(u, Accounting);
                return RedirectToAction("Index");
            }

            model.AddBudgets(u, Accounting);
            return View(model);
        }
    }
}