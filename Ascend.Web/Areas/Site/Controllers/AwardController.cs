using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Infrastructure;

namespace Ascend.Web.Areas.Site.Controllers
{
    #region AwardViewModel

    public class AwardViewModel
    {
        public string From { get; set; }
        public ContentViewModel Content { get; set; }
        public IEnumerable<CertificateViewModel> Certificates { get; set; }
        public IList<UserSummary> Nominees { get; set; }
        public Issuance Issuance { get; set; }

        public int? MaxAward { get; set; }

        public static AwardViewModel FromDomain(User u, Award a, IUserRepository userRepository, IAccountingService accounting, HttpRequestBase request)
        {
            return new AwardViewModel
                        {
                            From = u.DisplayName,
                            Content = ContentViewModel.FromDomain(a.Content),
                            Certificates =
                               (a.Certificates ?? new List<Certificate>()).Where(x => x.Enabled).Select(
                                   x => CertificateViewModel.FromDomain(x, request)),
                            Issuance = a.Issuance,
                            MaxAward = accounting.GetMaxProgramAward(a, u) ?? 0,
                            Nominees = userRepository
                                        .GetSummaries()
                                        .Where(x => x.Id != u.Document.Id && a.IsValidNominee(x))
                                        .ToList(),
                        };
        }
    }

    #endregion
    #region CertificateViewModel

    public class CertificateViewModel
    {
        public string Name { get; set; }
        public string BackgroundUrl { get; set; }
        public Size BackgroundSize { get; set; }
        public string FontFace { get; set; }
        public int FontSize { get; set; }
        public string FontColor { get; set; }
        public string DefaultMessage { get; set; }
        public int? MessageWordLimit { get; set; }
        public CertificateArea FromBox { get; set; }
        public CertificateArea ToBox { get; set; }
        public CertificateArea DateBox { get; set; }
        public CertificateArea MessageBox { get; set; }
        public CertificateArea AwardBox { get; set; }

        public static CertificateViewModel FromDomain(Certificate c, HttpRequestBase request)
        {
            return new CertificateViewModel
                       {
                           Name = c.Name,
                           BackgroundUrl = c.BackgroundUrl.ToAbsoluteUrl(request).ToString(),
                           BackgroundSize = c.BackgroundSize,
                           DefaultMessage = c.DefaultMessage,
                           MessageWordLimit = c.MessageWordLimit,
                           FromBox = c.FromBox,
                           ToBox = c.ToBox,
                           DateBox = c.DateBox,
                           MessageBox = c.MessageBox,
                           AwardBox = c.AwardBox,
                       };
        }
    }

    #endregion
    #region UserAwardViewModel

    public class UserAwardViewModel
    {
        public string Recipient { get; set; }
        public int? Amount { get; set; }
        public string Message { get; set; }
        public int? Certificate { get; set; }
        public bool Email { get; set; }

        public UserAward CreateAward(User nominator, Award award)
        {
            return new UserAward
             {
                 Nominator = nominator.Document.Id,
                 Recipient = Recipient,
                 Award = award.Document.Id,

                 Amount = Amount,
                 Message = Message,
                 Sent = DateTime.UtcNow,
                 Certificate = Certificate.HasValue
                    ? award.Certificates.Where(x => x.Enabled).ElementAt(Certificate.Value)
                    : null
             };
        }
    }

    #endregion

    
    public partial class AwardController : ProgramController<Award>
    {
        public IAwardRepository Awards { get; set; }
        public IUserAwardRepository UserAwards { get; set; }
        public IUserRepository UserRepository { get; set; }
        public IUserMessaging Messaging { get; set; }

        protected override Award GetResource(string id)
        {
            return Awards.Get(id);
        }

        [HttpGet]
        public virtual ActionResult Index(string id)
        {
            var model = AwardViewModel.FromDomain(CurrentUser, CurrentResource, UserRepository, Accounting, Request);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Index(string id, UserAwardViewModel model)
        {
            var ua = model.CreateAward(CurrentUser, CurrentResource);
            if (ua.Amount.HasValue)
            {
                TryAwardPointsTo(
                    Users[ua.Recipient],
                    ua.Amount.Value);
            }
            UserAwards.Save(ua);

            if (model.Email)
            {
                Messaging.SendAward(ControllerContext.RequestContext, ua);
                Notifier.Notify(
                    Severity.Success,
                    "Your award has been emailed!",
                    "The recipient should receive an email shortly with a link they can use to view or print their certificate.",
                    ua);
                
            }
            else
            {
                Notifier.Notify(
                    Severity.Success,
                    "Your award has been sent.",
                    "You indicated that you would print and deliver the certificate yourself, so we didn't email the recipient. Have fun delivering the good news!",
                    ua);
                TempData["popup-certificate"] = ua.Document.Id;
            }
            return RedirectToAction(MVC.Site.Home.Index());
        }

        [HttpGet]
        public virtual JsonResult Search(string id, string q)
        {
            return Json(
                UserRepository
                    .Search(q)
                    .Where(x => x.Id != CurrentUser.Document.Id &&
                                CurrentResource.IsValidNominee(x))
                    .ToDictionary(
                        x => x.Id,
                        x => x.ToString()),
                JsonRequestBehavior.AllowGet
            );
        }

    }
}