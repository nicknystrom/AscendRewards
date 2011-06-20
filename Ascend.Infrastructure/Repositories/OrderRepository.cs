using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core;
using Ascend.Core.Repositories;
using RedBranch.Hammock;

namespace Ascend.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(Session sx) : base(sx)
        {
        }

        public IEnumerable<Order> GetOrdersForUser(User user)
        {
            return Where(x => x.User).Eq(user.Document.Id);
        }
    }
}
