using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedBranch.Hammock;

namespace Ascend.Core.Repositories
{
    public interface IAwardRepository : IRepository<Award>
    {
    }

    public interface IUserAwardRepository : IRepository<UserAward>
    {
        IEnumerable<UserAward> GetAwardsSentByUser(User user);
        IEnumerable<UserAward> GetAwardsSentToUser(User user);
        int GetCountAwardsSentByUser(User user);
        int GetCountAwardsSentToUser(User user);
    }
}