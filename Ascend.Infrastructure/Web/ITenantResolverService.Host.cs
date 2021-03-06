﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;
using RedBranch.Hammock;

namespace Ascend.Infrastructure.Web
{
    public class HostBasedTenantResolverService : ITenantResolverService
    {
        readonly ITenantRepository _tenants;
        readonly Dictionary<string, Tenant> _hosts;

        public HostBasedTenantResolverService(ITenantRepository tenants)
        {
            _tenants = tenants;
            _hosts = new Dictionary<string,Tenant>();
        }

        // HTTP_HOST works on both IIS and Mono's FastCGI server. SERVER_NAME does not (it is
        // set to the virtual server match string *.ascendrewards.com on Mono FastCGI).
		
		public Tenant GetTenantForRequest(HttpContextBase context)
		{
			return GetTenantForRequest(context.Request.ServerVariables["HTTP_HOST"]);
		}
		
		public Tenant GetTenantForRequest(HttpContext context)
		{
            return GetTenantForRequest(context.Request.ServerVariables["HTTP_HOST"]);
		}
		
        public Tenant GetTenantForRequest(string host)
        {
			// return a tenant directly from our host map
            Tenant t = null;
            lock (_hosts)
            {
                if (!_hosts.ContainsKey(host))
                {
                    _hosts[host] = _tenants.All().WithDocuments().FirstOrDefault(x => x.Match != null && x.Match.Any(y => 0 == String.Compare(host, y, true)));
                }
                t = _hosts[host];
            }
            if (null == t)
            {
                throw new HttpException(404, "No tenant found for host '" + host + "'");    
            }
            return t;
        }

        public IEnumerable<Tenant> GetActiveTenants()
        {
            return _tenants.Where(x => x.Enabled).Eq(true);
        }
    }
}