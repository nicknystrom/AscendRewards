using Ascend.Infrastructure.Web;
using System;
using Ascend.Core;
using Ascend.Core.Services;
using System.Web.Mvc;

namespace Ascend.Web
{
    public class AscendController : BaseController
    {
        public IApplicationConfiguration Application { get; set; }
        public IProductImageService Images { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["application"] = Application;
            ViewData["today"] = DateTime.Now;
   
            // specifically add an 'id' field to viewdata -- this isnt needed on windows, but
            // on mono it seems like viewdata won't pull the id automatically from routedata
            if (!ViewData.ContainsKey("id") &&
                filterContext.RouteData.Values.ContainsKey("id"))
            {
                ViewData["id"] = filterContext.RouteData.Values["id"];
            }
            
            base.OnActionExecuting(filterContext);
        }
    }
}

