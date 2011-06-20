using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Ascend.Core;
using RedBranch.Hammock;

namespace Ascend.Web.Areas.Site.Controllers
{
    public abstract class ResourceController<TResource> : SiteController where TResource : Resource
    {
        protected abstract TResource GetResource(string id);

        protected TResource CurrentResource { get; private set; }

        protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // all resource requests must be to a specific id
            var id = filterContext.RouteData.GetRequiredString("id");
            CurrentResource = GetResource(id);

            // validate current user's access
            var result = CurrentResource.IsAllowedAccess(CurrentUser, DateTime.UtcNow);
            if (!result.Available)
            {
                filterContext.Result = View("Unavailable", result);
            }
        }
    }
}