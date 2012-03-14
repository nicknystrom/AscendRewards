using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;

using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;

using RedBranch.Hammock;

namespace Ascend.Web.Services
{
    public class UserMessaging : IUserMessaging
    {
        public IUserRepository UserRepository { get; set; }
        public IUserSummaryCache Users { get; set; }
        public IRepository<Template> Templates { get; set; }
        public ICacheStore Cache { get; set; }

        public IApplicationConfiguration Application { get; set; }
        public IMessagingBuilder Builder { get; set; }
        public IMessagingSender Sender { get; set; }
       
        public Template LoadTemplate(string id, string file)
        {
            if (!Cache.ContainsKey(id))
            {
                var t = Templates.TryGet(id);
                if (null == t)
                {
                    // load from disk
                    t = new Template();
                    using (var reader = File.OpenText(file))
                    {
                        t.Subject = reader.ReadLine();
                        t.Content = reader.ReadToEnd();
                    }
                }
                Cache.Put(id, t, TimeSpan.FromHours(1));
            }
            return Cache.Get<Template>(id);
        }

        public EmailSendAttempt SendRegistrationNotice(
            RequestContext request,
            string firstName,
            string lastName,
            string email,
            string password)
        {
            var template = LoadTemplate(
                "template-registration-notice",
                request.HttpContext.Server.MapPath("~/Messages/RegistrationNotice.template")
            );
            var e = Builder.Transform(
                template,
                new TemplateData
                    {
                        {"firstName", firstName },
                        {"lastName", lastName },
                        {"email", email },
                        {"password", password },
                    },
                request.HttpContext.Request
            );
            e.Recipient = new EmailAddress { Address = Application.SupportEmailAddress, Name = Application.ProgramName };

            return Sender.Send(e);
        }

        public EmailSendAttempt SendActivation(RequestContext request, User u)
        {
            var template = LoadTemplate(
                "template-user-activation",
                request.HttpContext.Server.MapPath("~/Messages/UserActivation.template")
            );
            var url = new UrlHelper(request).Action(MVC.Public.Login.Activate(u.ActivationCode));
            var e = Builder.Transform(
                template,
                new TemplateData
                    {
                        {"login", u.Login},
                        {"program", Application.ProgramName},
                        {"url", url.ToAbsoluteUrl(request.HttpContext.Request).ToString() },
                    },
                request.HttpContext.Request
            );
            e.Recipient = new EmailAddress {Address = u.Email, Name = u.DisplayName};

            var attempt = Sender.Send(e);
            if (attempt.Success)
            {
                u.LastActivationEmailSent = DateTime.UtcNow;
                UserRepository.Save(u);
            }
            return attempt;
        }

        public EmailSendAttempt SendWelcome(RequestContext request, User u)
        {
            var template = LoadTemplate(
                "template-user-welcome",
                request.HttpContext.Server.MapPath("~/Messages/UserWelcome.template")
            );
            var url = new UrlHelper(request).Action(MVC.Public.Login.Index());
            var e = Builder.Transform(
                template,
                new TemplateData
                    {
                        {"login", u.Login},
                        {"program", Application.ProgramName},
                        {"url", url.ToAbsoluteUrl(request.HttpContext.Request).ToString() },
                    },
                request.HttpContext.Request
            );
            e.Recipient = new EmailAddress { Address = u.Email, Name = u.DisplayName };
           
            var attempt = Sender.Send(e);
            if (attempt.Success)
            {
                u.LastWeclomeEmailSent = DateTime.UtcNow;
                UserRepository.Save(u);
            }
            return attempt;
        }

        public EmailSendAttempt SendReset(RequestContext request, User u, string password)
        {
            var template = LoadTemplate(
                "template-user-reset",
                request.HttpContext.Server.MapPath("~/Messages/UserReset.template")
            );
            var url = new UrlHelper(request).Action(MVC.Public.Login.Index());
            var e = Builder.Transform(
                template,
                new TemplateData
                    {
                        {"login", u.Login},
                        {"program", Application.ProgramName},
                        {"password", password},
                        {"url", url.ToAbsoluteUrl(request.HttpContext.Request).ToString() },
                    },
                request.HttpContext.Request
            );
            e.Recipient = new EmailAddress { Address = u.Email, Name = u.DisplayName };
            return Sender.Send(e);
        }

        public EmailSendAttempt SendAward(RequestContext request, UserAward award)
        {
            var template = LoadTemplate(
                "template-user-award",
                request.HttpContext.Server.MapPath("~/Messages/UserAward.template")
            );
            var recipient = Users[award.Recipient];
            var nominator = Users[award.Nominator];
            var url = new UrlHelper(request).Action(MVC.Public.Award.Index(award.Document.Id));
            var e = Builder.Transform(
                template,
                new TemplateData
                    {
                        {"program", Application.ProgramName},
                        {"url", url.ToAbsoluteUrl(request.HttpContext.Request).ToString() },
                        {"recipient", recipient.DisplayName},
                        {"nominator", nominator.DisplayName}
                    },
                request.HttpContext.Request
            );
            e.Recipient = new EmailAddress { Address = recipient.Email, Name = recipient.DisplayName };
            return Sender.Send(e);
        }
    }
}