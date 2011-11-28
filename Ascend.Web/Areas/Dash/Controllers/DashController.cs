using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Ascend.Core;
using Ascend.Core.Services.Caching;
using Ascend.Infrastructure.Web;

using Spark;

namespace Ascend.Web.Areas.Dash.Controllers
{
    [Authorize]
    [Precompile("*")]
    public abstract partial class DashController : BaseController
    {
        public IEntityCache<User> Users { get; set; }

        public User CurrentUser { get; private set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            CurrentUser = Users[filterContext.HttpContext.User.Identity.Name];
            ViewData["currentUser"] = CurrentUser;

            // we need to have at least dashboard permissions to access this area
            if (null ==  CurrentUser.Permissions ||
                false == CurrentUser.Permissions.Dashboard)
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, "You do not have permission to access the dashboard.");
            }
        }
    }
}