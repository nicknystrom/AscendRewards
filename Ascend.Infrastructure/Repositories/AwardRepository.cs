using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core;
using Ascend.Core.Repositories;
using RedBranch.Hammock;

namespace Ascend.Infrastructure.Repositories
{
    public class AwardRepository : Repository<Award>, IAwardRepository
    {
        public AwardRepository(Session sx) : base(sx)
        {
        }
    }

    public class UserAwardRepository : Repository<UserAward>, IUserAwardRepository
    {
        public UserAwardRepository(Session sx) : base(sx)
        {
        }

        public IEnumerable<UserAward> GetAwardsSentByUser(User user)
        {
            return Where(x => x.Nominator).Eq(user.Document.Id);
        }

        public IEnumerable<UserAward> GetAwardsSentToUser(User user)
        {
            return Where(x => x.Recipient).Eq(user.Document.Id);
        }

        public int GetCountAwardsSentByUser(User user)
        {
            return Where(x => x.Nominator).Eq(user.Document.Id).Spec().Execute().Rows.Length;
        }

        public int GetCountAwardsSentToUser(User user)
        {
            return Where(x => x.Recipient).Eq(user.Document.Id).Spec().Execute().Rows.Length;
        }
    }
}
