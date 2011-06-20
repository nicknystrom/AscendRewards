using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core.Services;
using Ascend.Infrastructure;
using NUnit.Framework;

namespace Ascend.Test.Infrastructure
{
    [TestFixture]
    class PassiveNotificationServiceTests
    {
        [Test]
        public void Passive_notification_service_accepts_notification()
        {
            var p = new PassiveNotificationService();
            p.Notify(Severity.Warn, "foo", "bar", null);
        }

        [Test]
        public void Passive_notification_service_reports_notifications()
        {
            var p = new PassiveNotificationService();
            p.Notify(Severity.Warn, "foo", "bar", null);
            Assert.That(p.GetNotifications().Count(), Is.EqualTo(1));
        }

        [Test]
        public void Passive_notification_service_clears_its_buffer_after_reporting_notifications()
        {
            var p = new PassiveNotificationService();
            p.Notify(Severity.Warn, "foo", "bar", null);
            p.GetNotifications();
            Assert.That(p.GetNotifications().Count(), Is.EqualTo(0));
        }
    }
}
