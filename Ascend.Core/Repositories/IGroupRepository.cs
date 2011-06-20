using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedBranch.Hammock;

namespace Ascend.Core.Repositories
{
    public class GroupSummary
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static GroupSummary FromDomain(Group g)
        {
            return new GroupSummary
            {
                Id = g.Document.Id,
                Name = g.Name,
            };
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public interface IGroupRepository : IRepository<Group>
    {
        IEnumerable<GroupSummary> GetSummaries();
        Group FindByNumber(string number);
        Group FindByName(string name);
    }
}