using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core.Services;

namespace Ascend.Web.Areas.Admin.Controllers
{
    public partial class ValidationController : AdminController
    {
        public IValidationService Validations { get; set; }

        [HttpGet]
        public virtual ActionResult Index()
        {
            return View(Validations.GetValidations());
        }
    }
}