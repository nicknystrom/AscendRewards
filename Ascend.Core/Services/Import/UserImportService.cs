using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core.Repositories;
using RedBranch.Hammock;

namespace Ascend.Core.Services.Import
{
    public enum UserColumnMappingTargets
    {
        None = -1,
        EmployeeId = 0,
        Login,
        Password,
        Manager,
        Group,
        ManagedGroups,
        State,
        DateBirth,
        DateHired,
        DateTerminated,
        Title,
        Email,
        FirstName,
        LastName,
        HomeStreet1,
        HomeStreet2,
        HomeStreet3,
        HomeCity,
        HomeState,
        HomeZip,
        WorkStreet1,
        WorkStreet2,
        WorkStreet3,
        WorkCity,
        WorkState,
        WorkZip,
        HomePhoneNumber,
        HomePhoneExtension,
        WorkPhoneNumber,
        WorkPhoneExtension,
        MobilePhoneNumber,
        MobilePhoneExtension,
        Custom,
    }

    public class UserImportService : BaseImportService<User, UserColumnMappingTargets>
    {
        public IUserRepository Users { get; set; }
        public IGroupRepository Groups { get; set; }
        public INotificationService Notifier { get; set; }

        public override ImportResult ValidateLayout(UserColumnMappingTargets[] layout)
        {
            var result = new ImportResult();
            if (!layout.Any(x => x == UserColumnMappingTargets.EmployeeId) &&
                !layout.Any(x => x == UserColumnMappingTargets.Login))
            {
                result.AddProblem("Either EmployeeId or Login field is required.");
            }
            return result;
        }

        public override void ValidateRow(ImportRow row)
        {
            // employee id is required if its on the sheet.
            if (row.Has(UserColumnMappingTargets.EmployeeId) &&
                String.IsNullOrEmpty((string)row[UserColumnMappingTargets.EmployeeId]))
            {
                row.AddProblem("Row has an empty EmployeeId column. EmployeeId is an optional column, but if part of the upload sheet, it must be filled.");
            }

            // login is also required if on the sheet
            if (row.Has(UserColumnMappingTargets.Login) &&
                String.IsNullOrEmpty((string)row[UserColumnMappingTargets.Login]))
            {
                row.AddProblem("Row has an empty Login value. Login is a required field.");
            }
        }

        public override User Find(ImportRow row)
        {
            // search by employee id first
            if (row.Has(UserColumnMappingTargets.EmployeeId))
            {
                return Users.FindUserByEmployeeId((string)row[UserColumnMappingTargets.EmployeeId]);
            }
            
            // search by login next
            if (row.Has(UserColumnMappingTargets.Login))
            {
                return Users.FindUserByLogin((string)row[UserColumnMappingTargets.Login]);
            }

            row.AddProblem("Import contains neither EmployeeId nor Login.");
            return null;
        }

        public override User Create(ImportRow row)
        {
            // use either employee id or login, and add digits after login to create a unique id
            string id = null;
            if (row.Has(UserColumnMappingTargets.EmployeeId))
            {
                id = Document.For<User>((string)row[UserColumnMappingTargets.EmployeeId]);
                var dupe = Users.TryGet(id);
                if (null != dupe)
                {
                    row.Problems.Add("There is already an employee with this EmployeeId, '" + id + "'.");
                    return null;
                }
            }
            else if (row.Has(UserColumnMappingTargets.Login))
            {
                id = Document.For<User>((string)row[UserColumnMappingTargets.EmployeeId]);
                var dupe = Users.TryGet(id);
                while (null != dupe)
                {
                    if (id.Length > 2 &&
                        char.IsDigit(id[id.Length-1]) &&
                        '-' == id[id.Length-2])
                    {
                        id = id.Substring(id.Length-1) + (int.Parse(id[id.Length-1].ToString()) + 1);
                    }
                    else
                    {
                        id += "-1";
                    }
                    dupe = Users.TryGet(id);  
                }
            }
            else
            {
                row.Problems.Add("Row has neither EmployeeId nor Login.");
            }
            return new User
            {
                Document = new Document { Id = id },
                EmployeeId = (string)row[UserColumnMappingTargets.EmployeeId],
                Login = (string)row[UserColumnMappingTargets.Login],
                State = UserState.Registered,
            };
        }

        public override void Save(User entity)
        {
            Users.Save(entity);
        }

        public override void Completed()
        {
            Notifier.Notify(
                Severity.Info,
                "Don't forget to send Welcome emails.",
                @"Use the Messaging tab to send welcome/activation emails to all
                  all of the users that were uploaded. No emails are automatically
                  sent as a result of this upload.",
                null);
        }

        public override ImportDataType GetColumnType(UserColumnMappingTargets x)
        {
            switch (x)
            {
                case UserColumnMappingTargets.State:
                case UserColumnMappingTargets.DateBirth:
                case UserColumnMappingTargets.DateHired:
                case UserColumnMappingTargets.DateTerminated:
                    return ImportDataType.Date;

                default:
                    return ImportDataType.String;
            }    
        }

        public override void Apply(User u, ImportRow r)
        {
            for (var i=0; i<r.Layout.Length; i++)
            {
                var value = r.Values[i];
                var target = r.Layout[i];

                switch (target)
                {
                    case UserColumnMappingTargets.EmployeeId:           u.EmployeeId                 = (string)value; break;
                    case UserColumnMappingTargets.Login:                u.Login                      = (string)value; break;
                    case UserColumnMappingTargets.Password:             u.SetPassword((string)value); break;
                    case UserColumnMappingTargets.Manager:              ApplyManager((string)value, u); break;
                    case UserColumnMappingTargets.Group:                ApplyGroup((string)value, u); break;
                    case UserColumnMappingTargets.ManagedGroups:        ApplyManagedGroups((string)value, u); break;
                    case UserColumnMappingTargets.State:                ApplyState((string)value, u); break;
                    case UserColumnMappingTargets.DateBirth:            u.DateBirth                  = (DateTime?)value; break;
                    case UserColumnMappingTargets.DateHired:            u.DateHired                  = (DateTime?)value; break;
                    case UserColumnMappingTargets.DateTerminated:       u.DateTerminated             = (DateTime?)value; break;
                    case UserColumnMappingTargets.Title:                u.Title                      = (string)value; break;
                    case UserColumnMappingTargets.Email:                u.Email                      = (string)value; break;
                    case UserColumnMappingTargets.FirstName:            u.FirstName                  = (string)value; break;
                    case UserColumnMappingTargets.LastName:             u.LastName                   = (string)value; break;
                    case UserColumnMappingTargets.HomeStreet1:          GetHomeAddress(u).Address1   = (string)value; break;
                    case UserColumnMappingTargets.HomeStreet2:          GetHomeAddress(u).Address2   = (string)value; break;
                    case UserColumnMappingTargets.HomeStreet3:          GetHomeAddress(u).Address3   = (string)value; break;
                    case UserColumnMappingTargets.HomeCity:             GetHomeAddress(u).City       = (string)value; break;
                    case UserColumnMappingTargets.HomeState:            GetHomeAddress(u).State      = (string)value; break;
                    case UserColumnMappingTargets.HomeZip:              GetHomeAddress(u).PostalCode = (string)value; break;
                    case UserColumnMappingTargets.WorkStreet1:          GetWorkAddress(u).Address1   = (string)value; break;
                    case UserColumnMappingTargets.WorkStreet2:          GetWorkAddress(u).Address2   = (string)value; break;
                    case UserColumnMappingTargets.WorkStreet3:          GetWorkAddress(u).Address3   = (string)value; break;
                    case UserColumnMappingTargets.WorkCity:             GetWorkAddress(u).City       = (string)value; break;
                    case UserColumnMappingTargets.WorkState:            GetWorkAddress(u).State      = (string)value; break;
                    case UserColumnMappingTargets.WorkZip:              GetWorkAddress(u).PostalCode = (string)value; break;
                    case UserColumnMappingTargets.HomePhoneNumber:      GetHomePhone(u).Number       = (string)value; break;
                    case UserColumnMappingTargets.HomePhoneExtension:   GetHomePhone(u).Extension    = (string)value; break;
                    case UserColumnMappingTargets.WorkPhoneNumber:      GetWorkPhone(u).Number       = (string)value; break;
                    case UserColumnMappingTargets.WorkPhoneExtension:   GetWorkPhone(u).Extension    = (string)value; break;
                    case UserColumnMappingTargets.MobilePhoneNumber:    GetMobilePhone(u).Number     = (string)value; break;
                    case UserColumnMappingTargets.MobilePhoneExtension: GetMobilePhone(u).Extension  = (string)value; break;
                    //case UserColumnMappingTargets.Custom:               ApplyCustom(custom, (string)value, u); break;
                }
            }
        }

        static Address GetHomeAddress(User u)  { return u.HomeAddress ?? (u.HomeAddress = new Address()); }
        static Address GetWorkAddress(User u)  { return u.WorkAddress ?? (u.WorkAddress = new Address()); }
        static Phone GetHomePhone(User u)  { return u.HomePhone ?? (u.HomePhone = new Phone()); }
        static Phone GetWorkPhone(User u)  { return u.WorkPhone ?? (u.WorkPhone = new Phone()); }
        static Phone GetMobilePhone(User u)  { return u.MobilePhone ?? (u.MobilePhone = new Phone()); }

        void ApplyManager(string manager, User u)
        {
            var x = Users.FindUserByEmployeeId(manager) ??
                    Users.FindUserByLogin(manager) ??
                    Users.TryGet(manager);
            if (null != x)
            {
                u.Manager = x.Document.Id;
            }
        }

        void ApplyGroup(string group, User u)
        {
            var x = Groups.FindByNumber(group) ??
                    Groups.FindByName(group) ??
                    Groups.TryGet(group);
            if (null != x)
            {
                u.Group = x.Document.Id;
            }
        }

        void ApplyManagedGroups(string managedGroups, User u)
        {
            if (String.IsNullOrEmpty(managedGroups))
            {
                u.ManagedGroups = null;
                return;
            }
            u.ManagedGroups = 
                managedGroups.Split(',')
                             .Select(x => x.Trim())
                             .Select(x => {
                                var y = Groups.FindByNumber(x) ??
                                        Groups.FindByName(x) ??
                                        Groups.TryGet(x);
                                return (null == y) ? null : y.Document.Id;
                             })
                             .ToArray();
            if (u.ManagedGroups.Length == 0)
            {
                u.ManagedGroups = null;
            }
        }

        void ApplyState(string state, User u)
        {
            if (!String.IsNullOrEmpty(state))
            {
                var x = (UserState)Enum.Parse(typeof(UserState), state);
                u.State = x;
            }
        }

        void ApplyCustom(string custom, string value, User u)
        {
            if (null == u.Custom)
            {
                u.Custom = new Dictionary<string, string>();
            }
            u.Custom[custom] = value;
        }
    }
}
