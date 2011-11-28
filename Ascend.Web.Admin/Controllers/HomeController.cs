
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RedBranch.Hammock;

using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;

using Spark;

namespace Ascend.Web.Admin.Controllers
{
    public partial class HomeController : AdminController
    {		
		[HttpGet]
		public virtual ActionResult Index()
        {
		    return View();
        }		
   }
}
