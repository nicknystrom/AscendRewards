using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;

using RedBranch.Hammock;

namespace Ascend.Infrastructure.Web
{
    public class RouteBasedTenantResolverService : ITenantResolverService
    {
        readonly ITenantRepository _tenants;

        public RouteBasedTenantResolverService(ITenantRepository tenants)
        {
            _tenants = tenants;
        }
		
		public Tenant GetTenantForRequest(HttpContextBase context)
		{
            var mvc = context.CurrentHandler as MvcHandler;
            return GetTenantForRoute(mvc.RequestContext.RouteData);
		}
		
		public Tenant GetTenantForRequest(HttpContext context)
		{
            var mvc = context.CurrentHandler as MvcHandler;
            return GetTenantForRoute(mvc.RequestContext.RouteData);
		}
		
        public Tenant GetTenantForRoute(RouteData data)
        {
			// return a tenant directly from our host map
            Tenant t = null;
            if (data.Values.ContainsKey("tenant"))
            {
                t = _tenants.Get(Document.For<Tenant>((string)data.Values["tenant"]));
            }
            return t;
        }

        public IEnumerable<Tenant> GetActiveTenants()
        {
            return _tenants.Where(x => x.Enabled).Eq(true);
        }
    }
}