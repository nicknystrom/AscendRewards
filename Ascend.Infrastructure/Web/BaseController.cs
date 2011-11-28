﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Ascend.Core;
using Ascend.Core.Services;
using System.Reflection;

namespace Ascend.Infrastructure.Web
{
    #region RedirectToRouteResult<T>

    // from MVCContrib and Microsoft.Web.Mvc, aka "MVC futures"

    public delegate RouteValueDictionary ExpressionToRouteValueConverter<TController>(Expression<Action<TController>> expression) where TController : Controller;

    public class RedirectToRouteResult<T> : RedirectToRouteResult, IControllerExpressionContainer where T : Controller
    {
        static RouteValueDictionary GetRouteValuesFromExpression<TController>(Expression<Action<TController>> action) where TController : Controller
        {
            if (action == null) {
                throw new ArgumentNullException("action");
            }

            MethodCallExpression call = action.Body as MethodCallExpression;
            if (call == null) {
                throw new ArgumentException("Must be a method call.", "action");
            }

            string controllerName = typeof(TController).Name;
            if (!controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)) {
                throw new ArgumentException("Target must end in controller.", "action");
            }
            controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);
            if (controllerName.Length == 0) {
                throw new ArgumentException("Cannot route to controller.", "action");
            }

            var rvd = new RouteValueDictionary();
            rvd.Add("Controller", controllerName);
            rvd.Add("Action", call.Method.Name);
            AddParameterValuesFromExpressionToDictionary(rvd, call);
            return rvd;
        }

        static void AddParameterValuesFromExpressionToDictionary(RouteValueDictionary rvd, MethodCallExpression call)
        {
            var parameters = call.Method.GetParameters();
            if (parameters.Length > 0)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    var arg = call.Arguments[i];
                    object value = null;
                    var ce = arg as ConstantExpression;
                    if (ce != null)
                    {
                        value = ce.Value;
                    }
                    else
                    {
                        // Otherwise, convert the argument subexpression to type object,
                        // make a lambda out of it, compile it, and invoke it to get the value
                        Expression<Func<object>> lambdaExpression =
                            System.Linq.Expressions.Expression.Lambda<Func<object>>(
                                System.Linq.Expressions.Expression.Convert(arg, typeof(object))
                            );
                        Func<object> func = lambdaExpression.Compile();
                        value = func();
                    }
                    rvd.Add(parameters[i].Name, value);
                }
            }
        }

        MethodCallExpression IControllerExpressionContainer.Expression
        {
            get { return Expression.Body as MethodCallExpression; }
        }

        public Expression<Action<T>> Expression { get; private set; }

        public RedirectToRouteResult(Expression<Action<T>> expression) : this(expression, expr => GetRouteValuesFromExpression(expr)) {}

        public RedirectToRouteResult(Expression<Action<T>> expression, ExpressionToRouteValueConverter<T> expressionParser) : base(expressionParser(expression))
        {
            Expression = expression;
        }
    }

    public interface IControllerExpressionContainer
    {
        MethodCallExpression Expression { get; }
    }

    #endregion

    public static class ControllerExtensions
    {
        public static RedirectToRouteResult RedirectToAction<T>(this T controller, Expression<Action<T>> action)
            where T : Controller
        {
            return ((Controller)controller).RedirectToAction(action);
        }
        
        public static RedirectToRouteResult RedirectToAction<T>(this Controller controller, Expression<Action<T>> action)
            where T : Controller
        {
            return new RedirectToRouteResult<T>(action);
        }
    }

    public abstract class BaseController : Controller
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