using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core;
using Ascend.Core.Services;

namespace Ascend.Web.Controllers
{
    public abstract partial class BaseController : Controller
    {
        public INotificationService Notifier { get; set; }
        public IApplicationConfiguration Application { get; set; }
        public IProductImageService Images { get; set; }

        protected override FilePathResult File(string fileName, string contentType, string fileDownloadName)
        {
            if (fileName.StartsWith("~/"))
            {
                fileName = Server.MapPath(fileName);
            }
            return base.File(fileName, contentType, fileDownloadName);
        }

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
        
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // place a default 'validation failed' notification
            if (!ModelState.IsValid &&
                !Notifier.Has(Severity.Warn, Severity.Error))
            {
                Notifier.Notify(
                    Severity.Warn,
                    "Some of the input data was invalid.",
                    "Please review the form for specific problem to correct before resubmitting.",
                    ModelState);
            }

            // push notifications to the view
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                // if we're redirecting, pull any notifications already in tempdata and concat 
                // with new ones
                var notifications = Notifier.GetNotifications();
                if (!(filterContext.Result is ViewResult))
                {
                    var existing = filterContext.Controller.TempData["__notifications"] as IEnumerable<Notification>;
                    if (null != existing)
                    {
                        notifications = notifications.Concat(existing);
                    }
                }
                
                // place notification into ViewData for a ViewResult, into TempData for everything else
                (filterContext.Result is ViewResult
                     ? (IDictionary<string, object>)filterContext.Controller.ViewData
                     : (IDictionary<string, object>)filterContext.Controller.TempData
                )["__notifications"] = notifications;
            }
        }
    }
}