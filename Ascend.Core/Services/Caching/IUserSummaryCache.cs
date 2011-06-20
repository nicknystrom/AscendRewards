using System.Collections.Generic;
using System.Web.Routing;
using Ascend.Core.Repositories;

namespace Ascend.Core.Services.Caching
{
    public interface IUserSummaryCache
    {
        UserSummary this[string userId] { get; }
        IEnumerable<UserSummary> this[IEnumerable<string> userIds] { get; }
        IEnumerable<UserSummary> All { get; }
        UserSummary TryGet(string userId);
    }
}