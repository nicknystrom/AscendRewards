using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Ascend.Core.Services;
using Ascend.Core.Services.Caching;
using Ascend.Web.Areas.Admin.Controllers;
using Ascend.Core.Repositories;

namespace Ascend.Web.Areas.Dash.Controllers
{
    public partial class UserController : DashController
    {
        public IAccountingService Accounting { get; set; }
        public IUserActivityRepository UserActivity { get; set; }
        public IUserSummaryCache UserSummaryCache { get; set; }

        [HttpGet]
        public ActionResult Index()
        {
            return View(UserSummaryCache.All);
        }

        [HttpGet]
        public ActionResult Edit(string userId)
        {
            var u = Users[userId];
            return View(UserEditModel.FromDomain(
                u,
                UserActivity.GetUserActivitySummary(u),
                Accounting
            ));
        }

        [HttpPost]
        public ActionResult Edit(string userId, UserEditModel model)
        {
            var u = Users[userId];
            if (ModelState.IsValid)
            {
                model.Apply(u, Accounting);
                return RedirectToAction("Index");
            }

            model.AddBudgets(u, Accounting);
            model.AddActivity(UserActivity.GetUserActivitySummary(u));
            return View(model);
        }
    }
}