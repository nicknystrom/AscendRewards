using System;
using System.Collections.Generic;
using System.Web;

namespace Ascend.Core.Services
{
    public interface ITenantResolverService
    {
        Tenant GetTenantForRequest(HttpContext context);
        Tenant GetTenantForRequest(HttpContextBase context);
        IEnumerable<Tenant> GetActiveTenants();
    }
}