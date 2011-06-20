using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ascend.Core.Services;

namespace Ascend.Infrastructure
{
    public class PassiveNotificationService : INotificationService
    {
        private IList<Notification> Notifications { get; set; }

        public IEnumerable<Notification> GetNotifications()
        {
            var x = Notifications;
            Notifications = null;
            return x ?? new Notification[0];
        }

        public bool Has(params Severity[] severities)
        {
            if (null == Notifications) return false;
            if (   0 == Notifications.Count) return false;
            if (null == severities ||
                   0 == severities.Length)
            {
                return Notifications.Count != 0;
            }

            return severities.Any(x => Notifications.Any(y => y.Severity == x));
        }

        public void Notify(
            Severity severity,
            string heading,
            string message,
            object topic)
        {
            (Notifications ?? (Notifications = new List<Notification>()))
                .Add(new Notification
                {
                    Severity = severity,
                    Heading = heading,
                    Message = message,
                    Topic = topic,
                });
        }

        public void Notify(
            string heading,
            string message,
            Exception ex)
        {
            Notify(
                Severity.Error,
                heading ?? "An unexpcted error has occurered processing this request.",
                message ?? "Please contact customer support and reference message: '" + ex.Message + "'.",
                ex);
        }

        public void Notify(
            Exception ex)
        {
            Notify(null, null, ex);
        }

    }
}
