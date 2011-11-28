using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RedBranch.Hammock;

using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Infrastructure.Web;

namespace Ascend.Web.Admin.Controllers
{
    #region TenantCreateModel

    public class TenantCreateModel
    {
        public string Name { get; set; }

        public Tenant CreateTenant()
        {
            return new Tenant
            {
                Document = new Document { Id = Document.For<Tenant>(Name.ToSlug()) },
                Enabled = true,
                Name = Name,
                Database = "ascend-" + Name.ToSlug(),
                Match = new [] { Name.ToSlug() + ".ascend-rewards.com" },
            };
        }
    }

    #endregion
    #region TenantEditModel

    public class TenantEditModel
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string Database { get; set; }
        public string[] Match { get; set; }
        public string EmailDomain { get; set; }

        public static TenantEditModel FromDomain(Tenant t)
        {
            return new TenantEditModel
            {
                Enabled = t.Enabled,
                Name = t.Name,
                Database = t.Database,
                Match = t.Match,
                EmailDomain = t.EmailDomain
            };
        }

        public void Apply(Tenant t)
        {
            t.Enabled = Enabled;
            t.Name = Name;
            t.Database = Database;
            t.Match = Match.Where(x => !String.IsNullOrEmpty(x)).ToArray();
            t.EmailDomain = EmailDomain;
        }
    }

    #endregion

    
    public partial class TenantController : AdminController
    {
        public ITenantRepository Tenants { get; set; }

        [HttpGet]
        public virtual ActionResult Index()
        {
            return View(Tenants.All().WithDocuments());
        }

        [HttpPost]
        public virtual ActionResult Index(TenantCreateModel t)
        {
            ViewData["create"] = t;
            if (!ModelState.IsValid)
            {
                return View(Tenants.All().WithDocuments());
            }

            var x = t.CreateTenant();
            try
            {
                Tenants.Save(x);
                return this.RedirectToAction(c => c.Edit(x.Document.Id));
            }
            catch (Exception ex)
            {
                Notifier.Notify(ex);
                return View(Tenants.All().WithDocuments());
            }

        }

        [HttpGet]
        public virtual ActionResult Edit(string id)
        {
            return View(TenantEditModel.FromDomain(Tenants.Get(id)));
        }

        [HttpPost]
        public virtual ActionResult Edit(string id, TenantEditModel t)
        {
            var x = Tenants.Get(id);
            if (!ModelState.IsValid)
            {
                return View(t);
            }
            try
            {
                t.Apply(x);
                Tenants.Save(x);
                return this.RedirectToAction(c => c.Index());
            }
            catch (Exception ex)
            {
                Notifier.Notify(ex);
                return View(x);
            }
        }
    }
}