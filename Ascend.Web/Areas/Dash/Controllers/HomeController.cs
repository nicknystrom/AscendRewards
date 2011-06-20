using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core.Repositories;

namespace Ascend.Web.Areas.Dash.Controllers
{
    public partial class HomeController : DashController
    {
        [HttpGet]
        public virtual ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public virtual ActionResult Org()
        {
            return View();
        }
    }
}