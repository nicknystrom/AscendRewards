
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RedBranch.Hammock;

using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Infrastructure.Web;

namespace Ascend.Web.Admin.Controllers
{
    #region ConfigurationEditModel

	public class ConfigurationEditModel
	{
        public string ProgramName { get; set; }
        public string DefaultTheme { get; set; }
        public string DefaultHomePage { get; set; }
        public string DefaultTermsOfService { get; set; }

        public string DefaultLoginMenu { get; set; }
        public string DefaultHeaderMenu { get; set; }
        public string DefaultNavigationMenu { get; set; }
        public string DefaultFooterMenu { get; set; }

        public string MaintenancePage { get; set; }

        [UIHint("Enum")] public PaymentType PaymentType { get; set; }
        public string GeneralControlAccount { get; set; }
        public string GeneralExpenseAccount { get; set; }

        public string DefaultCatalog { get; set; }
        public decimal PointsPerDollar { get; set; }

        public string EmailSenderAddress { get; set; }
        public string EmailSenderName { get; set; }

        public string SupportEmailAddress { get; set; }
        public string SupportPhoneNumber { get; set; }

        public string TicketJonesKey { get; set; }
        public string TicketJonesUrl { get; set; }

        public string GoogleAnalyticsCode { get; set; }

        public bool ShowProfileOnActivate { get; set; }
        public ProfileElements Profile { get; set; }
        public string[] CustomFields { get; set; }

		public static ConfigurationEditModel FromDomain(ApplicationConfiguration c)
		{
		    return new ConfigurationEditModel
		               {
		                   ProgramName = c.ProgramName,
		                   DefaultTheme = c.DefaultTheme,
                           DefaultHomePage = c.DefaultHomePage,
                           DefaultTermsOfService = c.DefaultTermsOfService,

                           DefaultLoginMenu = c.DefaultLoginMenu,
                           DefaultHeaderMenu = c.DefaultHeaderMenu,
                           DefaultNavigationMenu = c.DefaultNavigationMenu,
                           DefaultFooterMenu = c.DefaultFooterMenu,

                           MaintenancePage = c.MaintenancePage,

                           PaymentType = c.PaymentType,
                           GeneralControlAccount = c.GeneralControlAccount,
                           GeneralExpenseAccount = c.GeneralExpenseAccount,

                           DefaultCatalog = c.DefaultCatalog,
                           PointsPerDollar = c.PointsPerDollar,

		                   EmailSenderAddress = c.EmailSender.Address,
		                   EmailSenderName = c.EmailSender.Name,

                           SupportEmailAddress = c.SupportEmailAddress,
                           SupportPhoneNumber = c.SupportPhoneNumber,

                           GoogleAnalyticsCode = c.GoogleAnalyticsCode,

                           TicketJonesKey = c.TicketJonesKey,
                           TicketJonesUrl = c.TicketJonesUrl,

                           ShowProfileOnActivate = c.ShowProfileOnActivate,
                           Profile = c.Profile ?? new ProfileElements(),
                           CustomFields = c.CustomFields,
		               };
		}
	
		public void Apply(ApplicationConfiguration c)
		{
		    c.ProgramName = ProgramName;
		    c.DefaultTheme = DefaultTheme;
		    c.DefaultHomePage = DefaultHomePage;
		    c.DefaultTermsOfService = DefaultTermsOfService;

            c.DefaultLoginMenu = DefaultLoginMenu;
            c.DefaultHeaderMenu = DefaultHeaderMenu;
            c.DefaultNavigationMenu = DefaultNavigationMenu;
            c.DefaultFooterMenu = DefaultFooterMenu;

		    c.MaintenancePage = MaintenancePage;

		    c.PaymentType = PaymentType;
		    c.GeneralControlAccount = GeneralControlAccount;
		    c.GeneralExpenseAccount = GeneralExpenseAccount;

		    c.DefaultCatalog = DefaultCatalog;
		    c.PointsPerDollar = PointsPerDollar;

		    c.EmailSender = new EmailAddress
		                        {
		                            Address = EmailSenderAddress,
		                            Name = EmailSenderName,
		                        };

            c.SupportEmailAddress = SupportEmailAddress;
            c.SupportPhoneNumber = SupportPhoneNumber;

            c.GoogleAnalyticsCode = GoogleAnalyticsCode;

            c.TicketJonesKey = TicketJonesKey;
            c.TicketJonesUrl = TicketJonesUrl;

            c.ShowProfileOnActivate = ShowProfileOnActivate;
            c.Profile = Profile;
            c.CustomFields = CustomFields.Where(x => !String.IsNullOrEmpty(x)).ToArray();
		}
	}

    #endregion
	
	
    public partial class ConfigurationController : AdminController
    {
		public IApplicationConfigurationRepository Configurations { get; set; }
        public IApplicationConfiguration CurrentConfiguration { get; set; }
		
		[HttpGet]
		public virtual ActionResult Index()
        {
		    return View(
                ConfigurationEditModel.FromDomain(
                    Configurations.All().First())
            );
        }

        [HttpPost]
        public virtual ActionResult Index(ConfigurationEditModel c)
        {
            if (!ModelState.IsValid)
			{
				return View(c);
			}
            var cfg = Configurations.All().First();
            try
            {
                c.Apply(cfg);
				Configurations.Save(cfg);
                Notifier.Notify(
                    Severity.Info,
                    "Configuraton udpated.",
                    "The configuration has been updated. You must either wait for the application to reset (typically at midnight), or manually reseset the application for changes to take immediate effect.",
                    cfg);
                return this.RedirectToAction(d => d.Index());
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(c);
            }
        }
    }
}
