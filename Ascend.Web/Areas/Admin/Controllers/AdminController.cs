using System;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Ascend.Core.Services;
using Spark;

namespace Ascend.Web.Areas.Admin.Controllers
{
    public class BasicAuthenticationFailedResult : ActionResult
    {
        public string Realm { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.HttpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"" + (Realm ?? "ascendrewards") + "\"";

            // bypasses the forms authentication module, which would take our 401 response
            // and convert into a 302 redirect to the login page
            context.HttpContext.Response.Write("");
            context.HttpContext.Response.Flush();
        }
    }

    [Precompile("*")]
    public abstract partial class AdminController : Web.Controllers.BaseController
    {
        public IAdminService Admin { get; set; }
        public ITenantService TenantService { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // provide a list of tenants
            ViewData["activeTenants"] = TenantService.GetActiveTenants();
            ViewData["currentTenant"] = TenantService.GetTenantForRequest(Request);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            var auth = filterContext.HttpContext.Request.Headers["Authorization"];
            if (!String.IsNullOrEmpty(auth))
            {
                var tokens = auth.Split(' ');
                if (tokens.Length == 2 && tokens[0] == "Basic")
                {
                    var credentials = Encoding.ASCII.GetString(Convert.FromBase64String(tokens[1]));
                    if (!String.IsNullOrEmpty(credentials))
                    {
                        tokens = credentials.Split(':');
                        if (tokens.Length == 2 &&
                            Admin.Authenticate(tokens[0], tokens[1]))
                        {
                            // success
                            ViewData["adminUser"] = tokens[0];
                            return;
                        }
                    }
                }
            }
            filterContext.Result = new BasicAuthenticationFailedResult();
        }
    }
}