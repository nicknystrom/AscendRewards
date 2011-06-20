using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedBranch.Hammock;

namespace Ascend.Core.Repositories
{
    public class UserActivitySummary
    {
        public UserActivitySummary(IEnumerable<LoginAttempt> logins)
        {
            Logins = logins;
        }
        
        private IEnumerable<LoginAttempt> Logins { get; set; }
        
        public int FailedLogins
        { 
            get { return null == Logins ? 0 : Logins.Count(x => !x.Success); }
        }
        
        public DateTime? LastFailedLogin
        {
            get { return null == Logins ? null : Logins.Where(x => !x.Success).Select(x => (DateTime?)x.Date).LastOrDefault(); }
        }
        
        public int SuccessfulLogins
        {
            get { return null == Logins ? 0 : Logins.Count(x => x.Success); }
        }
        
        public DateTime? LastSuccesfulLogin
        {
            get { return null == Logins ? null : Logins.Where(x => x.Success).Select(x => (DateTime?)x.Date).LastOrDefault(); }
        }
    }
    
    public interface IUserActivityRepository : IRepository<LoginAttempt>
    {        
        UserActivitySummary GetUserActivitySummary(User user);
    }
}
