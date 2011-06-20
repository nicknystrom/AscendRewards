using System.Collections.Generic;
using System.Web.Routing;
using Ascend.Core.Repositories;

namespace Ascend.Core.Services.Caching
{
    public interface IEntityCache<TEntity> where TEntity : Entity
    {
        TEntity this[string id] { get; set; }

        TEntity TryGet(string id);
    }
}