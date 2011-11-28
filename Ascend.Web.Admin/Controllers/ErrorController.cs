using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RedBranch.Hammock;

using Ascend.Core;
using Ascend.Core.Services;
using Ascend.Infrastructure.Web;

namespace Ascend.Web.Admin.Controllers
{
    public partial class ErrorController : AdminController
    {
        public IRepository<Error> Errors { get; set; }

        [HttpGet]
        public virtual ActionResult Index()
        {
            return View(
                Errors.Where(x => x.Created.Date).Bw(
                            DateTime.UtcNow,
                            DateTime.UtcNow.AddMonths(-1))
                      .Spec().Descending().WithDocuments().Limit(1000)
                      .ToList()
            );          
        }
       
        [HttpGet]
        public virtual ActionResult Display(string id)
        {
            return View(Errors.Get(id));
        }

        [HttpGet]
        public virtual ActionResult Clear()
        {
            var a = Errors.All();
            foreach (var row in a)
            {
                Errors.Delete(row);
            }
            Notifier.Notify(Severity.Info, "Errors cleared", String.Format("{0} errors deleted.", a.Count()), null);
            return this.RedirectToAction(c => c.Index());
        }
    }
}