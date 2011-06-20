using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core.Services
{
    /// <summary>
    /// Provides the ability for non-presentation services to send notifications that will be
    /// (possibly) displayed to the user.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Places a notification into a queue of message to be shown to the user, typically on the next page load.
        /// </summary>
        /// <param name="severity">In escalating order: Debug, Info, Warn, Sucess, Error. User may not see notifications at lower severities.</param>
        /// <param name="heading">A very short phrase describing the notification. Try not to exceed 75 characters.</param>
        /// <param name="message">Longer text describing the notification, and perhaps giving guidance to the user receiving it.</param>
        /// <param name="topic">Optional. If not null, the presentation layer may attempt to create a hyperlink to this object when displaying the notification.</param>
        void Notify(
            Severity severity,
            string heading,
            string message,
            object topic);

        /// <summary>
        /// Provides a more completely formated notification for unexpected exceptions, including expanding details
        /// regarding the exception object itself (optional).
        /// </summary>
        /// <param name="heading"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        void Notify(
            string heading,
            string message,
            Exception ex);

        /// <summary>
        /// Simplest form of exception notification, provides generic, unfhelpful message.
        /// </summary>
        /// <param name="ex"></param>
        void Notify(
            Exception ex);

        /// <summary>
        /// Reads all notifications that have been posted to the service, and clears the service's queue.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Notification> GetNotifications();

        /// <summary>
        /// Determines if there are currently any notifications pending with the given severities.
        /// </summary>
        /// <param name="severities">If null or empty, return true if any notifications of any sevrity are pending.</param>
        /// <returns></returns>
        bool Has(params Severity[] severities);
    }

    public class Notification
    {
        public Severity Severity { get; set; }
        public string Heading { get; set; }
        public string Message { get; set; }
        public object Topic { get; set; }

        public static readonly Notification[] None = new Notification[0];
    }

    public enum Severity
    {
        Debug,
        Info,
        Warn,
        Success,
        Error,
    }
}
