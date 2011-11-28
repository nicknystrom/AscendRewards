
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ascend.Core.Services.Caching;
using RedBranch.Hammock;

using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Infrastructure;
using Ascend.Infrastructure.Web;

namespace Ascend.Web.Admin.Controllers
{
    #region ResourceEditModel

    public abstract class ResourceEditModel
    {
        public AvailabilityEditModel Availability { get; set; }

        protected virtual void Init(Resource r)
        {
            Availability = AvailabilityEditModel.FromResource(r);
        }

        protected virtual void Apply(Resource r)
        {
            if (null != Availability)
            {
                Availability.Apply(r);
            }
        }
    }

    #endregion
    #region ProgramEditModel

    public abstract class ProgramEditModel : ResourceEditModel
    {
        public string Title { get; private set; }
		public ContentEditModel Content { get; set; }
        public IssuanceEditModel Issuance { get; set; }
        
        protected virtual void Init(Program p, IAccountingService accounting)
        {
            base.Init(p);
            Content = ContentEditModel.FromDomain(p.Content);
            Issuance = IssuanceEditModel.FromDomain(p.Issuance);
            if (Issuance.Type == IssuanceType.ProgramBudget &&
                !String.IsNullOrEmpty(p.Issuance.Account))
            {
                var a = accounting.GetProgramAwardSource(p, null);
                if (null != a)
                {
                    Issuance.Ledger = accounting.GetLedger(a);
                    Issuance.Budget = BudgetEditModel.FromDomain(a.Budget);
                }
            }

            Title = p.Title;
        }

        protected virtual void Apply(Program p, IAccountingService accounting)
        {
            base.Apply(p);
            p.Content = null != Content ? Content.ToDomain() : new Content();
            if (null != Issuance)
            {
                Issuance.Apply(p.Issuance ?? (p.Issuance = new Issuance()));
                accounting.UpdateProgramAwardSource(p, (null == Issuance.Budget) ? null : Issuance.Budget.ToBudget());
            }
            else
            {
                p.Issuance = new Issuance();
            }
        }
    }

    #endregion
	#region AwardCreateModel

	public class AwardCreateModel
	{
		[Required, StringLength(100)] public string Title { get; set; }
        [UIHint("Enum")] public IssuanceType IssuanceType { get; set; }

		public Award CreateAward()
		{
			return new Award {
				Document = new Document { Id = Document.For<Award>(Title.ToSlug()) },
				Content = new Content {
                    Title = Title,
                },
                Issuance = new Issuance { Type = IssuanceType, },
			};
		}
	}
	
	#endregion
	#region AwardEditModel
	
	public class AwardEditModel : ProgramEditModel
	{
        public IList<Certificate> Certificates { get; set; }

        public bool OpenNomination { get; set; }
        [UIHint("UserMultiChooser")] public IList<string> NomineeUsers { get; set; }
        [UIHint("GroupMultiChooser")] public IList<string> NomineeGroups { get; set; }

		public static AwardEditModel FromDomain(Award a, IAccountingService accounting)
		{
            var x = new AwardEditModel {
                Certificates = a.Certificates,
                OpenNomination = a.OpenNomination,
                NomineeUsers = a.NomineeUsers,
                NomineeGroups = a.NomineeGroups,
			};
            x.Init(a, accounting);
            return x;
		}
	
		public void Apply(Award a, IAccountingService accounting)
		{
            base.Apply(a, accounting);

            a.Certificates = Certificates;
            a.OpenNomination = OpenNomination;
            a.NomineeUsers = NomineeUsers;
            a.NomineeGroups = NomineeGroups;
		}
	}
	
	#endregion
    #region CertificateCreateModel

    public class CertificateCreateModel
    {
        public string Url { get; set; }
     
        public Certificate CreateCertificate(HttpRequestBase request)
        {
            // load the image
            var url = Url.ToAbsoluteUrl(request);
            var req = WebRequest.Create(url);
            var response = (HttpWebResponse)req.GetResponse();
            var img = Image.FromStream(response.GetResponseStream());

            return new Certificate()
               {
                    Enabled = true,
                    BackgroundUrl = Url,
                    BackgroundContentType = response.ContentType,
                    BackgroundSize = new Core.Size { Width = img.Size.Width, Height = img.Size.Height },
                    Name = "Certificate",
                    FromBox =    new CertificateArea { 
                        Left = 0, Top = 0, Width = img.Size.Width/2, Height = 20,
                        Enabled = true,
                        FontFace = "Arial",
                        FontSize = 12,
                        FontColor = "#000",
                        Alignment = "center",
                    },
                    ToBox =      new CertificateArea {
                        Left = 0, Top = 25, Width = img.Size.Width/2, Height = 20,
                        Enabled = true,
                        FontFace = "Arial",
                        FontSize = 12,
                        FontColor = "#000",
                        Alignment = "center",
                    },
                    DateBox =      new CertificateArea {
                        Left = 0, Top = 50, Width = img.Size.Width/2, Height = 20,
                        Enabled = true,
                        FontFace = "Arial",
                        FontSize = 12,
                        FontColor = "#000",
                        Alignment = "center",
                    },
                    AwardBox =   new CertificateArea {
                        Left = 0, Top = 75, Width = img.Size.Width/2, Height = 20,
                        Enabled = true,
                        FontFace = "Arial",
                        FontSize = 12,
                        FontColor = "#000",
                        Alignment = "center",
                    },
                    MessageBox = new CertificateArea {
                        Left = 0, Top = 100, Width = img.Size.Width/2, Height = 20,
                        Enabled = true,
                        FontFace = "Arial",
                        FontSize = 12,
                        FontColor = "#000",
                        Alignment = "center",
                    },
               };
        }
    }

    #endregion
    #region IssuanceEditModel

    public class IssuanceEditModel
    {
        [UIHint("Enum")]    public IssuanceType Type { get; set; }
        [DisplayName("Fixed")] public int? FixedIssuance { get; set; }
        [DisplayName("Min.")] public int? MinIssuance { get; set; }
        [DisplayName("Max.")] public int? MaxIssuance { get; set; }
        [DisplayName("Default")] public int? DefaultIssuance { get; set; }

        [UIHint("AccountChooser")]
        public string Account { get; set; }
        public BudgetEditModel Budget { get; set; }
        public Ledger Ledger { get; set; }

        public static IssuanceEditModel FromDomain(Issuance i)
        {
            i = (i ?? new Issuance());
            return new IssuanceEditModel
                       {
                           Type = i.Type,
                           Account = i.Account,
                           FixedIssuance = i.FixedIssuance,
                           MinIssuance = i.MinIssuance,
                           MaxIssuance = i.MaxIssuance,
                           DefaultIssuance = i.DefaultIssuance,
                       };
        }

        public void Apply(Issuance i)
        {
            i.Type = Type;
            i.Account = Account;
            i.FixedIssuance = FixedIssuance;
            i.MinIssuance = MinIssuance;
            i.MaxIssuance = MaxIssuance;
            i.DefaultIssuance = DefaultIssuance;
        }
    }

    #endregion

    public partial class AwardController : AdminController
    {
        public IAccountingService Accounting { get; set; }
		public IAwardRepository Awards { get; set; }

        public IUserSummaryCache UserSummaries { get; set; }
        public IGroupSummaryCache GroupSummaries { get; set; }

		[HttpGet]
		public virtual ActionResult Index()
        {
            return View(Awards.All().WithDocuments());
        }


        public virtual ActionResult Details(int id)
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult Index(AwardCreateModel a)
        {
			ViewData["a"] = a;
			if (!ModelState.IsValid)
			{
                return View(Awards.All().WithDocuments());
			}
			
			var x = a.CreateAward();
            try
            {
				Awards.Save(x);
                return this.RedirectToAction(c => c.Edit(x.Document.Id));
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(Awards.All().WithDocuments());
            }
        }

		[HttpGet]
        public virtual ActionResult Edit(string id)
        {
		    var model = AwardEditModel.FromDomain(Awards.Get(id), Accounting);
            ViewData["groups"] = GroupSummaries;
            ViewData["users"] = UserSummaries[
                (model.Availability.Users ?? new string[0]).Union(
                 model.NomineeUsers       ?? new string[0])
            ];
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public virtual ActionResult Edit(string id, AwardEditModel a)
        {
			var x = Awards.Get(id);
			if (!ModelState.IsValid)
			{
				return View(a);
			}
            try
            {
                a.Apply(x, Accounting);
				Awards.Save(x);
                return this.RedirectToAction(c => c.Index());
            }
            catch (Exception ex)
            {
                ViewData["groups"] = GroupSummaries;
                ViewData["users"] = UserSummaries[
                    (a.Availability.Users ?? new string[0]).Union(
                     a.NomineeUsers       ?? new string[0])
                ];
				Notifier.Notify(ex);
                return View(x);
            }
        }

        [HttpPost]
        public virtual ActionResult Certificate(CertificateCreateModel c)
        {
            return PartialView("Certificate", c.CreateCertificate(Request));
        }
    }
}
