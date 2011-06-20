using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core;
using NUnit.Framework;

namespace Ascend.Test.Core
{
    [TestFixture]
    public class UserTests
    {
        [Test]
        public void User_state_cant_be_set_to_unset()
        {
            Assert.Throws<InvalidOperationException>(() => new User().State = UserState.Unset);
        }

        [Test]
        public void User_state_cant_transition_back_to_registered(
            [Values(UserState.Active, UserState.Suspended, UserState.Terminated)] UserState source)
        {
            var u = new User { State = source };
            Assert.Throws<InvalidOperationException>(() => u.State = UserState.Registered);
        }

        [Test]
        public void User_state_doesnt_set_dates_when_previous_state_is_unset(
            [Values(UserState.Active, UserState.Registered, UserState.Suspended, UserState.Terminated)] UserState target)
        {
            var u = new User { State = target };
            Assert.That(u.StateChanged, Is.EqualTo(new DateTime()));
            Assert.That(u.DateRegistered.HasValue, Is.False);
            Assert.That(u.DateActivated.HasValue, Is.False);
            Assert.That(u.DateSuspended.HasValue, Is.False);
            Assert.That(u.DateTerminated.HasValue, Is.False);
        }

        [Test]
        public void User_state_doesnt_set_dates_when_state_set_to_previous_state(
            [Values(UserState.Active, UserState.Registered, UserState.Suspended, UserState.Terminated)] UserState target)
        {
            var u = new User { State = target };
            u.State = target;
            Assert.That(u.StateChanged, Is.EqualTo(new DateTime()));
            Assert.That(u.DateRegistered.HasValue, Is.False);
            Assert.That(u.DateActivated.HasValue, Is.False);
            Assert.That(u.DateSuspended.HasValue, Is.False);
            Assert.That(u.DateTerminated.HasValue, Is.False);
        }

        [Test]
        public void User_state_sets_dates_when_changed(
            [Values(UserState.Active, UserState.Registered, UserState.Suspended, UserState.Terminated)] UserState source,
            [Values(UserState.Active, UserState.Suspended, UserState.Terminated)] UserState target)
        {
            if (source == target) return;

            var u = new User {State = source};
            var d = u.StateChanged;
            u.State = target;
            Assert.That(u.StateChanged, Is.GreaterThan(d));
            switch (target)
            {
                case UserState.Registered: Assert.That(u.DateRegistered.HasValue, Is.True); break;
                case UserState.Active:     Assert.That(u.DateActivated.HasValue, Is.True); break;
                case UserState.Suspended:  Assert.That(u.DateSuspended.HasValue, Is.True); break;
                case UserState.Terminated: Assert.That(u.DateTerminated.HasValue, Is.True); break;
            }
        }
    }
}