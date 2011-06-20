
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using RedBranch.Hammock;

using Ascend.Core;
using Ascend.Infrastructure;
using Ascend.Core.Repositories;
using Ascend.Core.Services;

namespace Ascend.Web.Areas.Admin.Controllers
{
    public partial class FileController : AdminController
    {
        public IFileStore Store { get; set; }
		
		[HttpGet, OutputCache(Location = OutputCacheLocation.None)]
		public virtual ActionResult Index()
        {
            var files = Store.List().ToDictionary(x => x, x => Store.GetUrl(x));
		    if (Request.IsAjaxRequest())
		    {
		        return PartialView("Files", files);
		    }
		    else
		    {
		        return View(files);
		    }
        }

        [HttpPost]
        public virtual ActionResult Index(HttpPostedFileBase file)
        {
            Store.Put(
                file.FileName,
                file.InputStream.ToArray(file.ContentLength),
                file.ContentType
            );
            return PartialView("Upload");
        }

        [HttpDelete]
        [ActionName("Index")]
        public virtual ActionResult Delete(string file)
        {
            Store.Delete(file);
            return Index();
        }
    }
}
