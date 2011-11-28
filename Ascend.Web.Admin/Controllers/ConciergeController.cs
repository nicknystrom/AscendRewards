
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core.Services.Caching;
using RedBranch.Hammock;

using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;

namespace Ascend.Web.Admin.Controllers
{
	#region ConciergeEditModel

	public class ConciergeEditModel
	{
        [UIHint("Enum")] public ConciergeStage Stage { get; set; }
        [UIHint("TextArea")] public string Notes { get; set; }
	}
	
	#endregion
	
	public partial class ConciergeController : AdminController
    {
        public IUserSummaryCache Users { get; set; }
		public IConciergeRepository ConciergeRepository { get; set; }
        
		[HttpGet]
		public virtual ActionResult Index()
		{
		    ViewData["users"] = Users;
            return View(ConciergeRepository.All().WithDocuments());
        }

		[HttpGet]
        public virtual ActionResult Edit(string id)
		{
		    var x = ConciergeRepository.Get(id);
		    ViewData["concierge"] = x;
            ViewData["user"] = Users[x.User];
		    return View(new ConciergeEditModel { Notes = x.Notes, Stage = x.Stage });
        }

        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Edit(string id, ConciergeEditModel model)
        {
            var x = ConciergeRepository.Get(id);
            ViewData["concierge"] = x;
            ViewData["user"] = Users[x.User];
			if (!ModelState.IsValid)
			{
				return View(model);
			}
            try
            {
                x.Notes = model.Notes;
                x.Stage = model.Stage;
				ConciergeRepository.Save(x);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Notifier.Notify(ex);
                return View(model);
            }
        }
    }
}
