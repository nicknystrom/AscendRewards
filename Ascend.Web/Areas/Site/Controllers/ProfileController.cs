using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Ascend.Core;
using Ascend.Core.Repositories;
using System.ComponentModel.DataAnnotations;
using Ascend.Core.Services.Caching;

namespace Ascend.Web.Areas.Site.Controllers
{
    #region ProfileEditModel

    public class ProfileEditModel
    {
        public ProfileElements Elements { get; set; }

        public string Login { get; set; }
        [DataType(DataType.EmailAddress)] public string Email { get; set; }
        [DisplayName("First Name"), StringLength(20)] public string FirstName { get; set; }
        [DisplayName("Last Name"), StringLength(20)] public string LastName { get; set; }

        [DisplayName("New Password"), DataType(DataType.Password)] public string Password { get; set; }
        [DisplayName("Retype Password"), DataType(DataType.Password)] public string PasswordConfirmation { get; set; }

        [DisplayName("Date of Birth"), DataType(DataType.Date)] public DateTime? DateBirth { get; set; }
        [DisplayName("Date of Hire"), DataType(DataType.Date)] public DateTime? DateHired { get; set; }

        [DisplayName("Home Address")] public Address HomeAddress { get; set; }
        [DisplayName("Work Address")] public Address WorkAddress { get; set; }

        [DisplayName("Home Phone")] public Phone HomePhone { get; set; }
        [DisplayName("Work Phone")] public Phone WorkPhone { get; set; }
        [DisplayName("Mobile Phone")] public Phone MobilePhone { get; set; }

        public IDictionary<string, string> Custom { get; set; }

        public static ProfileEditModel FromDomain(ProfileElements elements, string[] custom, User u)
        {
            return new ProfileEditModel()
            {
                Elements = elements,
                Login = u.Login,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                DateBirth = u.DateBirth,
                DateHired = u.DateHired,
                HomeAddress = u.HomeAddress,
                WorkAddress = u.WorkAddress,
                HomePhone = u.HomePhone,
                WorkPhone = u.WorkPhone,
                MobilePhone = u.MobilePhone,
                Custom = custom.ToDictionary(
                    x => x,
                    x => (u.Custom != null && u.Custom.ContainsKey(x)) ? u.Custom[x] : ""),
            };
        }

        public void Apply(ProfileElements e, User u)
        {
            if (e.Email)
            {
                u.Email = Email;
            }
            if (e.Name)
            {
                u.FirstName = FirstName;
                u.LastName = LastName; 
            }
            if (e.DateOfBirth) 
            {
                u.DateBirth = DateBirth;
            }
            if (e.DateOfHire)
            {
                u.DateHired = DateHired;
            }
            if (e.HomeAddress)
            {
                u.HomeAddress = HomeAddress;
            }
            if (e.WorkAddress)
            { 
                u.WorkAddress = WorkAddress;
            }
            if (e.HomePhone)
            {
                u.HomePhone = HomePhone;
            }
            if (e.WorkPhone)
            {
                u.WorkPhone = WorkPhone;
            }
            if (e.MobilePhone)
            {
                u.MobilePhone = MobilePhone;
            }
            if (e.CustomFields)
            { 
                u.Custom = Custom;
            }
            if (e.Password &&
                !String.IsNullOrEmpty(Password) &&
                !String.IsNullOrEmpty(PasswordConfirmation) &&
                Password == PasswordConfirmation)
            {
                u.SetPassword(Password);
            }

            u.LastUpdatedProfile = DateTime.UtcNow;
        }
    }

    #endregion
    #region MyActivityViewModel

    public class MyActivityViewModel
    {
        public int Total { get; set; }
        public IEnumerable<MyActivityViewModelProgram> Programs { get; set; }
    }

    public class MyActivityViewModelProgram
    {
        public string Name { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }

        public int Total
        {
            get { return Transactions.Sum(x => x.Amount); }
        }
    }

    #endregion
    #region RecognitionsViewModel
     
    public class RecognitionsViewModel
    {
        public IEnumerable<Public.Controllers.UserAwardViewModel> Recognitions { get; set; }

        public static RecognitionsViewModel FromDomain(IEntityCache<User> users, IEnumerable<UserAward> awards)
        {
            return new RecognitionsViewModel
                       {
                           Recognitions = awards.OrderByDescending(x => x.Created.Date).Select(x => new Public.Controllers.UserAwardViewModel
                            {
                                Id = x.Document.Id,
                                Date = x.Created.Date,
                                Certificate = x.Certificate,
                                Message = x.Message,
                                Amount = x.Amount,
                                NominatorName = (users.TryGet(x.Nominator) ?? new User {Login = "Unknown User"}).DisplayName,
                                RecipientName = (users.TryGet(x.Recipient) ?? new User {Login = "Unknown User"}).DisplayName,
                            })
                       };
        }
    }

    #endregion

    
    public partial class ProfileController : SiteController
    {
        public IUserRepository UserRepository { get; set; }
        public IOrderRepository OrdersRepository { get; set; }
        public IUserAwardRepository UserAwardsRepository { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewData["awardsSent"] = UserAwardsRepository.GetCountAwardsSentByUser(CurrentUser);
            ViewData["awardsReceived"] = UserAwardsRepository.GetCountAwardsSentToUser(CurrentUser);
        }

        public bool IsForcedUpdate { get { return Application.ShowProfileOnActivate && !CurrentUser.LastUpdatedProfile.HasValue; } }

        [HttpGet]
        public virtual ActionResult Index()
        {
            ViewData["force"] = IsForcedUpdate;
            return View(ProfileEditModel.FromDomain(Application.Profile, Application.CustomFields ?? new string[0], CurrentUser));
        }

        [HttpPost]
        public virtual ActionResult Index(ProfileEditModel p)
        {
            // manually check for password validation
            p.Elements = Application.Profile ?? new ProfileElements();
            if (p.Elements.Password &&
                (!String.IsNullOrEmpty(p.Password) ||
                 !String.IsNullOrEmpty(p.PasswordConfirmation)))
            {
                if (String.IsNullOrEmpty(p.Password))
                {
                    ModelState.AddModelError("Password", "Fill both password fields to change your password.");
                }
                if (String.IsNullOrEmpty(p.PasswordConfirmation))
                {
                    ModelState.AddModelError("PasswordConfirmation", "Fill both password fields to change your password.");
                }
                if (p.Password != p.PasswordConfirmation)
                {
                    ModelState.AddModelError("Password", "Both password entries must match to change your password.");
                }
            }

            // password is required on initial forced-update
            if (IsForcedUpdate &&
                p.Elements.Password &&
                String.IsNullOrEmpty(p.Password) &&
                String.IsNullOrEmpty(p.PasswordConfirmation))
            {
                ModelState.AddModelError("Password", "You must set your password before you can access the site.");
            }

            // validate email uniqueness
            if (p.Elements.Email)
            {
                Admin.Controllers.UserController.ValidateEmail(CurrentUser, p.Email, UserRepository, ModelState);
            }

            if (!ModelState.IsValid)
            {
                ViewData["force"] = IsForcedUpdate;
                return View(p);
            }

            p.Apply(p.Elements, CurrentUser);
            UserRepository.Save(CurrentUser);
            return RedirectToAction(MVC.Site.Home.Index());
        }

        [HttpGet]
        public virtual ActionResult Activity()
        {
            var model = new MyActivityViewModel
                            {
                                Total = CurrentLedger.TotalCredits,
                                Programs = CurrentLedger.Credits
                                    .GroupBy(x =>
                                                 {
                                                    if (String.IsNullOrEmpty(x.Program))
                                                    {
                                                        return "Misc";
                                                    }
                                                    if (x.Program.StartsWith("quiz-")) return "Quiz";
                                                    if (x.Program.StartsWith("useraward-")) return "Recognition";
                                                    if (x.Program.StartsWith("survey-")) return "Survey";
                                                     return "Other";
                                                 })
                                    .Select(x => new MyActivityViewModelProgram {Transactions = x, Name=x.Key})
                            };
            
            return View(model);
        }

        [HttpGet]
        public virtual ActionResult Orders()
        {
            return View(OrdersRepository.GetOrdersForUser(CurrentUser));
        }

        [HttpGet]
        public virtual ActionResult RecognitionsSent()
        {
            var awards = UserAwardsRepository.GetAwardsSentByUser(CurrentUser);
            return View("Recognitions", RecognitionsViewModel.FromDomain(Users, awards));
        }

        [HttpGet]
        public virtual ActionResult RecognitionsReceived()
        {
            var awards = UserAwardsRepository.GetAwardsSentToUser(CurrentUser);
            return View("Recognitions", RecognitionsViewModel.FromDomain(Users, awards));
        }
    }
}