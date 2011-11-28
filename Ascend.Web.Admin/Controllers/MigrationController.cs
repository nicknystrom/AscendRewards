using System;
using System.Web.Mvc;

using Ascend.Core.Services.Migrations;
using Ascend.Core.Services;
using Ascend.Infrastructure.Web;

namespace Ascend.Web.Admin.Controllers
{
	
    public partial class MigrationController : AdminController
    {
        public IMigrationService Migrations { get; set; }
		
		[HttpGet]
		public virtual ActionResult Index()
        {
            var v = Migrations.GetVersion();
            ViewData["available"] = Migrations.GetMigrationsAvailble();
            return View(v);
        }

        [HttpPost]
        public virtual ActionResult Index(bool confirm)
        {
            if (!confirm)
            {
                ModelState.AddModelError("confirm", "*");
                return Index();
            }    

            try
            {
                Migrations.Migrate();
                Notifier.Notify(
                    Severity.Success,
                    "Migrations successfull.",
                    "",
                    null
                );

            }
            catch (Exception ex)
            {
                Notifier.Notify(
                    Severity.Error,
                    "Migrations failed.",
                    String.Format(
                        "An unexpected error occurered during migration: {0}.",
                        ex.Message),
                    null
                );
            }
            return this.RedirectToAction(c => c.Index());
	    }		
    }
}
