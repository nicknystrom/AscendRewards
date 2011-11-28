using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Ascend.Core.Services;
using Ascend.Core.Repositories;
using System.Web.Mvc;

namespace Ascend.Core
{
    public class User : Entity
    {
        private UserState _state;

        public string ActivationCode { get; set; }
        [Required] public string Login { get; set; }
        [Required] public byte[] PasswordHash { get; set; }
        [Required] public byte[] PasswordSalt { get; set; }

        #region Password Management

        public void SetActivationCode()
        {
            ActivationCode = Guid.NewGuid().ToString("N");
        }

        private static readonly Random _random = new Random();

        public void SetPassword(string password)
        {
            PasswordSalt = new byte[4];
            lock (_random)
            {
                _random.NextBytes(PasswordSalt);
            }
            PasswordHash = GetPasswordHash(password, PasswordSalt);
        }

        public bool CheckPassword(string password)
        {
            return CompareHash(GetPasswordHash(password, PasswordSalt), PasswordHash);
        }

        private const string PasswordCharacters =
           @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWYX1234567890";

        public static string GeneratePassword()
        {
            lock (_random)
            {
                var buf = new char[6];
                for (int n = 0; n < buf.Length; n++)
                {
                    var i = _random.Next(0, PasswordCharacters.Length - 1);
                    buf[n] = PasswordCharacters[i];
                }
                return new string(buf);
            }
        }

        public static bool CompareHash(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length) return false;
            for (var n = 0; n < hash1.Length; n++)
            {
                if (hash1[n] != hash2[n]) return false;
            }
            return true;
        }

        public static byte[] GetPasswordHash(string password, byte[] salt)
        {
            // merge the password and salt into one buffer
            var buf = Encoding.UTF8.GetBytes(password);
            var buf2 = new byte[buf.Length + salt.Length];
            Array.Copy(buf, buf2, buf.Length);
            Array.Copy(salt, 0, buf2, buf.Length, salt.Length);

            var sha256 = SHA256.Create();
            return sha256.ComputeHash(buf2);
        }

        #endregion

        public string Manager { get; set; }
        public string Group { get; set; }
        public string[] ManagedGroups { get; set; }

        public DateTime? DateAcceptedTermsOfService { get; set; }
        public DateTime? LastSuccesfulLogin { get; set; }
        public DateTime? LastFailedLogin { get; set; }
        
        public void IncrementLogins(bool success, HttpRequestBase request, DateTime? date = null)
        {
            date = date ?? DateTime.UtcNow;
            var r = ServiceLocator.Resolve<IUserActivityRepository>();
            if (success) LastSuccesfulLogin = date;
            if (null != r)
            {
                r.Save(new LoginAttempt { 
                    User = this.Document.Id,
                    Date = date.Value,
                    Success = success,
                    Host = request.UserHostAddress,
                    Referrer = (null == request.UrlReferrer) ? String.Empty :  request.UrlReferrer.ToString()
                });
            }
        }
        
        public UserState State
        {
            get{ return _state; }
            set
            {
                if (value == UserState.Unset) { throw new InvalidOperationException("State cannot be set to Unset."); }
                if (_state == value) return;

                if (_state == UserState.Unset)
                {
                    _state = value;
                }
                else if (value == UserState.Registered)
                {
                    throw new InvalidOperationException("State cannot transition back to Registered.");
                }
                else
                {
                    var d = DateTime.UtcNow;
                    switch (value)
                    {
                        case UserState.Active:
                            ActivationCode = null;
                            DateActivated = d;
                            break;
                        case UserState.Suspended:
                            DateSuspended = d;
                            break;
                        case UserState.Terminated:
                            DateTerminated = d;
                            break;
                    }
                    _state = value;
                    StateChanged = d;
                }
            }
        }
        public DateTime StateChanged { get; set; }

        public DateTime? DateBirth { get; set; }
        public DateTime? DateHired { get; set; }
        public DateTime? DateRegistered { get; set; }
        public DateTime? DateActivated { get; set; }
        public DateTime? DateSuspended { get; set; }
        public DateTime? DateTerminated { get; set; }

        public DateTime? LastUpdatedProfile { get; set; }
        public DateTime? LastWeclomeEmailSent { get; set; }
        public DateTime? LastActivationEmailSent { get; set; }

        public string EmployeeId { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address HomeAddress { get; set; }
        public Address WorkAddress { get; set; }
        public Phone HomePhone { get; set; }
        public Phone WorkPhone { get; set; }
        public Phone MobilePhone { get; set; }
        public IDictionary<string, string> Custom { get; set; }

        public UserPermissions Permissions { get; set; }
        public bool HasPermissions(UserPermissions p)
        {
            if (null == Permissions) return false;
            if (p.Dashboard && !Permissions.Dashboard) return false;
            if (p.StandardReports && !Permissions.StandardReports) return false;
            if (p.FinancialReports && !Permissions.FinancialReports) return false;
            if (p.Users && !Permissions.Users) return false;
            return true;
        }

        public string DisplayName
        {
            get
            {
                if (!String.IsNullOrEmpty(FirstName) ||
                    !String.IsNullOrEmpty(LastName))
                {
                    return FirstName + " " + LastName;
                }
                return Login;
            }
        }

        public ShoppingCart Cart { get; set; }
        public ShoppingCart Wishlist { get; set; }

        public string PointsAccount { get; set; }
        public string BudgetAccount { get; set; }

        public override string ToString()
        {
            return DisplayName;
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

    public class LoginAttempt : Entity
    {
        public string User { get; set; }
        public DateTime Date { get; set; }
        public bool Success { get; set; }
        public string Host { get; set; }
        public string Referrer { get; set; }

        public override string ToString()
        {
            return String.Format("{0}: {1}", Success ? "Success" : "Fail", Date.ToShortDateString());
        }
    }

    public class Phone
    {
        public string Number { get; set;}
        [DisplayName("Ext.")] public string Extension { get; set; }

        public override string ToString()
        {
            if (!String.IsNullOrWhiteSpace(Extension))
            {
                return String.Format("{0} x{1}", Number, Extension);
            }
            return Number;
        }
    }

    public class Address
    {
        [DisplayName("Street")] public string Address1 { get; set; }
        [DisplayName("")] public string Address2 { get; set; }
        [DisplayName("")] public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [DisplayName("Zip")] public string PostalCode { get; set; }
        public string Country { get; set; }

        public override string ToString()
        {
            return String.Format(
                "{0}\n\r{1}, {2} {3}",
                String.Join("\n\r", new[] {Address1, Address2, Address3}.Where(x => !String.IsNullOrWhiteSpace(x))),
                City,
                State,
                PostalCode);
        }
    }

    public enum UserState
    {
        Unset = 0,
        Registered = 1,
        Active = 2,
        Suspended = 3,
        Terminated = 4
    }

    public class UserPermissions
    {
        public bool Dashboard { get; set; }
        public bool StandardReports { get; set; }
        public bool FinancialReports { get; set; }
        public bool Users { get; set; }

        public override string ToString()
        {
            return String.Join(", ", new[]
            {
                Dashboard ? "Dashboard" : "",
                StandardReports ? "Reports" : "",
                FinancialReports ? "Financials" : "",
                Users ? "Users" : ""
            }.Where(x => !String.IsNullOrWhiteSpace(x)));
        }
    }
}
