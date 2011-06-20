using System;
using System.Net.Mail;
using Ascend.Core;
using Ascend.Core.Services;
using RedBranch.Hammock;
using System.Net;

namespace Ascend.Infrastructure
{
    public class SmtpMessagingSender : IMessagingSender
    {
        private SmtpClient _smtp;

        public IInfrastructureConfiguration InfrastructureConfiguration { get; set; }
        public IApplicationConfiguration Configuration { get; set; }
        public IRepository<Email> Emails { get; set; }
        
        public SmtpClient Smtp
        {
            get
            {
                if (null == _smtp)
                {
                    _smtp = new SmtpClient(InfrastructureConfiguration.EmailServer);
                    if (!String.IsNullOrWhiteSpace(InfrastructureConfiguration.EmailUsername) ||
                        !String.IsNullOrWhiteSpace(InfrastructureConfiguration.EmailPassword))
                    {
                        _smtp.UseDefaultCredentials = false;
                        _smtp.Credentials = new NetworkCredential(
                            InfrastructureConfiguration.EmailUsername,
                            InfrastructureConfiguration.EmailPassword);
                    }
                    if (InfrastructureConfiguration.EmailEnableSsl.HasValue)
                    {
                        _smtp.EnableSsl = InfrastructureConfiguration.EmailEnableSsl.Value;
                    }
                }
                return _smtp;
            }   
        }

        public EmailSendAttempt Send(Email email)
        {
            email.Sender = email.Sender ?? Configuration.EmailSender;
            var attempt = new EmailSendAttempt
                  {
                      Date = DateTime.UtcNow,
                      Server = Smtp.Host,
                      Success = true,
                  };
            try
            {
                Smtp.Send(email.ToMailMessage());
                email.Sent = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                attempt.Success = false;
                attempt.Error = ex.Message;
            }

            email.AddAttempt(attempt);
            Emails.Save(email);
            return attempt;
        }
    }
}
