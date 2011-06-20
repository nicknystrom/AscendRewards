using System;
using Ascend.Core.Repositories;
using RedBranch.Hammock;
using Ascend.Core;

namespace Ascend.Infrastructure
{
    public class UserActivityRepository : Repository<LoginAttempt>, IUserActivityRepository
    {
        public UserActivityRepository(Session sx) : base(sx) {}
        
        public UserActivitySummary GetUserActivitySummary(User user)
        {
            return new UserActivitySummary(this.Where(x => x.User).Eq(user.Document.Id).And(x => x.Date).Ge(DateTime.MinValue).Spec());
        }
    }
}

