using System.Collections.Generic;
using System.Web.Routing;
using Ascend.Core.Repositories;

namespace Ascend.Core.Services.Caching
{
    public interface IGroupSummaryCache
    {
        GroupSummary this[string id] { get; }
        IEnumerable<GroupSummary> this[IEnumerable<string> ids] { get; }
        IEnumerable<GroupSummary> All { get; }
    }
}