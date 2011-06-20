using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedBranch.Hammock;

namespace Ascend.Core.Repositories
{
    public struct AllowedResource
    {
        public MenuItemType Type;
        public string Id;
        public Availability Availability;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(AllowedResource)) return false;
            return Equals((AllowedResource)obj);
        }

        public bool Equals(AllowedResource other)
        {
            return Equals(other.Id, Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public bool Allows(MenuItem item)
        {
            return (item.Type == this.Type &&
                    item.Location == this.Id);
        }
    }

    public interface IMenuRepository : IRepository<Menu>
    {
        IEnumerable<AllowedResource> GetResourcesForUser(User u);
    }
}