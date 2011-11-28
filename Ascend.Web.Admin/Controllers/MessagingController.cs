using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
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
    public class TemplateEditModel
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        [UIHint("TextArea")] public string Content { get; set; }

        public static TemplateEditModel FromDomain(Template t)
        {
            return new TemplateEditModel {
                Name = t.Name,
                Subject = t.Subject,
                Content = t.Content,
            };
        }

        public void Apply(Template t)
        {
            t.Name = Name;
            t.Subject = Subject;
            t.Content = Content;
        }
    }

    public class TemplateCreateModel
    {
        public string Name { get; set; }

        public Template ToTemplate()
        {
            return new Template {
                Document = new Document { Id = Document.For<Template>(Name.ToSlug()) },
                Name = Name,
                Subject = Name,
                Content = "",
            };
        }
    }

    
    public partial class MessagingController : AdminController
    {
        public IRepository<Template> Templates { get; set; }
        public IRepository<Email> Emails { get; set; }
        public IUserRepository Users { get; set; }
        public IMessagingSender Sender { get; set; }
        public IUserMessaging Messaging { get; set; }

        [HttpGet]
        public virtual ActionResult Index()
        {
            return View(Templates.All().WithDocuments());
        }

        [HttpPost]
        public virtual ActionResult Index(TemplateCreateModel t)
        {
            ViewData["create"] = t;
            if (!ModelState.IsValid)
            {
                return View(Templates.All().WithDocuments());
            }

            var x = t.ToTemplate();
            try
            {
                // attempt to load a template from disk as a starting point
                string f = null;
                if (x.Document.Id == "template-user-activation") f = Server.MapPath("~/Messages/UserActivation.template");
                if (x.Document.Id == "template-user-welcome") f = Server.MapPath("~/Messages/UserWelcome.template");
                if (x.Document.Id == "template-user-reset") f = Server.MapPath("~/Messages/UserReset.template");
                if (x.Document.Id == "template-user-award") f = Server.MapPath("~/Messages/UserAward.template");
                if (!String.IsNullOrEmpty(f))
                {
                    using (var reader = System.IO.File.OpenText(f))
                    {
                        x.Subject = reader.ReadLine();
                        x.Content = reader.ReadToEnd();
                    }
                }
            }
            catch
            {
            }
            try
            {
                Templates.Save(x);
                return this.RedirectToAction(c => c.Edit(x.Document.Id));
            }
            catch (Exception ex)
            {
                Notifier.Notify(ex);
                return View(Templates.All().WithDocuments());
            }
        }

        [HttpGet]
        public virtual ActionResult Messages(int? last, string address)
        {
            if (String.IsNullOrEmpty(address))
            {
                return View(Emails.Where(x => x.Created.Date)
                                  .Ge(DateTime.UtcNow.AddYears(-1))
                                  .Spec().Limit(last ?? 100).WithDocuments().ToList()
                );
            }
            else
            {
                return View(Emails.Where(x => x.Recipient.Address)
                                  .Eq(address)
                                  .And(x => x.Created.Date)
                                  .Ge(DateTime.UtcNow.AddYears(-1))
                                  .Spec().Limit(last ?? 100).WithDocuments().ToList());
            }
        }

        [HttpGet]
        public virtual ActionResult Activation()
        {
            return View(Users.GetUsersWithoutActivationEmail().Where(x => x.State == UserState.Registered));
        }

        [HttpPost]
        public virtual ActionResult Activation(string[] id, bool send)
        {
            if (!send)
            {
                ModelState.AddModelError("send", "You must confirm that you wish to send these emails.");
            }
            else
            {
                foreach (var x in id)
                {
                    try
                    {
                        Messaging.SendActivation(ControllerContext.RequestContext, Users.Get(x));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("id", ex);
                    }
                }
            }

            return View(Users.GetUsersWithoutActivationEmail());
        }

        [HttpGet]
        public virtual ActionResult Welcome()
        {
            return View(Users.GetUsersWithoutWelcomeEmail().Where(x => x.State == UserState.Active));
        }

        [HttpPost]
        public virtual ActionResult Welcome(string[] id, bool send)
        {
            if (!send)
            {
                ModelState.AddModelError("send", "You must confirm that you wish to send these emails.");
            }
            else
            {
                foreach (var x in id)
                {
                    try
                    {
                        Messaging.SendWelcome(ControllerContext.RequestContext, Users.Get(x));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("id", ex);
                    }
                }
            }

            return View(Users.GetUsersWithoutWelcomeEmail().Where(x => x.State == UserState.Active));
        }

        [HttpGet]
        public virtual ActionResult Display(string id)
        {
            return View(Emails.Get(id));
        }

        [HttpPost]
        public virtual ActionResult Resend(string id)
        {
            var msg = Emails.Get(id);
            Sender.Send(msg);
            return this.RedirectToAction(c => c.Display(id));
        }

        [HttpGet]
        public virtual ActionResult Edit(string id)
        {
            return View(TemplateEditModel.FromDomain(Templates.Get(id)));
        }

        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Edit(string id, TemplateEditModel t)
        {
            var x = Templates.Get(id);
            if (!ModelState.IsValid)
            {
                return View(t);
            }
            try
            {
                t.Apply(x);
                Templates.Save(x);
                return this.RedirectToAction(c => c.Index());
            }
            catch (Exception ex)
            {
                Notifier.Notify(ex);
                return View(t);
            }
        }
    }
}