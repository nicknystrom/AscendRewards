using System;
using System.Collections.Generic;
using System.Web;

namespace Ascend.Core.Services
{
    public interface ITenantService
    {
        Tenant GetTenantForRequest(HttpRequest request);
        Tenant GetTenantForRequest(HttpRequestBase request);
        IEnumerable<Tenant> GetActiveTenants();
    }
}