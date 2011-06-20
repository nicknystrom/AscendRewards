using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    public interface IApplicationConfiguration
    {
        string ProgramName { get; set; }
        string DefaultTheme { get; set; }
        string DefaultHomePage { get; set; }
        string DefaultTermsOfService { get; set; } 

        string DefaultLoginMenu { get; set; }
        string DefaultHeaderMenu { get; set; }
        string DefaultNavigationMenu { get; set; }
        string DefaultFooterMenu { get; set; }

        string MaintenancePage { get; set; }

        PaymentType PaymentType { get; set; }
        string GeneralControlAccount { get; set; }
        string GeneralExpenseAccount { get; set; }

        string DefaultCatalog { get; set; }
        decimal PointsPerDollar { get; set; }

        EmailAddress EmailSender { get; set; }

        string SupportEmailAddress { get; set; }
        string SupportPhoneNumber { get; set; }

        string GoogleAnalyticsCode { get; set; }

        bool ShowProfileOnActivate { get; set; }
        ProfileElements Profile { get; set; }
        string[] CustomFields { get; set; }
        string TicketJonesKey { get; set; }
        string TicketJonesUrl { get; set; }
    }

    public class ApplicationConfiguration : Entity, IApplicationConfiguration
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

        public PaymentType PaymentType { get; set; }
        public string GeneralControlAccount { get; set; }
        public string GeneralExpenseAccount { get; set; }

        public string DefaultCatalog { get; set; }
        public decimal PointsPerDollar { get; set; }

        public EmailAddress EmailSender { get; set; }

        public string SupportEmailAddress { get; set; }
        public string SupportPhoneNumber { get; set; }

        public string GoogleAnalyticsCode { get; set; }

        public string TicketJonesKey { get; set; }
        public string TicketJonesUrl { get; set; }

        public bool ShowProfileOnActivate { get; set; }
        public ProfileElements Profile { get; set; }
        public string[] CustomFields { get; set; } 
    }

    public class ProfileElements
    {
        public bool Password { get; set; }
        public bool Email { get; set; }
        public bool Name { get; set; }
        public bool DateOfBirth { get; set; }
        public bool DateOfHire { get; set; }
        public bool HomeAddress { get; set; }
        public bool WorkAddress { get; set; }
        public bool HomePhone { get; set; }
        public bool WorkPhone { get; set; }
        public bool MobilePhone { get; set; }
        public bool CustomFields { get; set; }
    }

    public enum PaymentType
    {
        PayOnIssuance,
        PayOnRedemption,
    }
}
