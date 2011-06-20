using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedBranch.Hammock;

namespace Ascend.Core.Repositories
{
    public class UserSummary
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public UserState State { get; set; }
        public string Manager { get; set; }
        public string Group { get; set; }
        public string Title { get; set; }

        public static UserSummary FromDomain(User u)
        {
            return new UserSummary
                       {
                           Id = u.Document.Id,
                           Login = u.Login,
                           FirstName = u.FirstName,
                           LastName = u.LastName,
                           Email = u.Email,
                           State = u.State,
                           Manager = u.Manager,
                           Group = u.Group,
                           Title = u.Title,
                       };
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

        public override string ToString()
        {
            var sb = new StringBuilder(DisplayName, 128);
            if (!String.IsNullOrEmpty(Email))
            {
                sb.Append(" (");
                sb.Append(Email);
                sb.Append(")");
            }
            return sb.ToString();
        }
    }

    public interface IUserRepository : IRepository<User>
    {
        IEnumerable<UserSummary> Search(string q);
        IEnumerable<UserSummary> GetSummaries();
        int GetUniqueLoginsByDate(DateTime from, DateTime to);
        IDictionary<UserState, int> GetUsersCountsByStates();
        IList<User> GetUsersWithBudgets();

        User FindUserByEmployeeId(string employeeId);
        User FindUserByLogin(string login);
        IEnumerable<User> GetUsersWithoutWelcomeEmail();
        IEnumerable<User> GetUsersWithoutActivationEmail();
        IEnumerable<User> GetUsersWithWishlists();
    }
    

}
