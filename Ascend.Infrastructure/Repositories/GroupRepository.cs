using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core;
using Ascend.Core.Repositories;
using RedBranch.Hammock;

namespace Ascend.Infrastructure.Repositories
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        public GroupRepository(Session sx) : base(sx)
        {
        }

        public IEnumerable<GroupSummary> GetSummaries()
        {
            var query = WithView(
                "_summary",
                @"
                function(doc) {
                  if (doc._id.indexOf('group-') === 0) {
                    emit(null, [doc.Name]);
                  }
                }");
            return query.All().Execute().Rows.Select(
                x => new GroupSummary
                {
                    Id = x.Id,
                    Name = x.Value.Value<string>(0),
                });
        }

        public Group FindByNumber(string number)
        {
            return Where(x => x.Number).Eq(number)
                .Spec().WithDocuments().FirstOrDefault();
        }

        public Group FindByName(string name)
        {
            return Where(x => x.Name).Eq(name)
                .Spec().WithDocuments().FirstOrDefault();
        }
    }
}
