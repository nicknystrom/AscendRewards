using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ascend.Core;
using NUnit.Framework;

namespace Ascend.Test.Core
{
    [TestFixture]
    public class UserPasswordTests
    {
        [Test]
        public void User_can_set_password()
        {
            var u = new User();
            u.SetPassword("asdf");
            Assert.That(u.PasswordHash.Length, Is.GreaterThan(0));
            Assert.That(u.PasswordSalt.Length, Is.GreaterThan(0));
        }

        [Test]
        public void User_can_check_password()
        {
            var u = new User();
            u.SetPassword("asdf");
            Assert.That(u.CheckPassword("asdf"), Is.True);
        }

        [Test]
        public void User_password_is_case_sensitive()
        {
            var u = new User();
            u.SetPassword("asdf");
            Assert.That(u.CheckPassword("asdF"), Is.False);
        }

        [Test]
        public void User_sets_unique_password_hash_even_for_same_password()
        {
            var u1 = new User();
            var u2 = new User();

            u1.SetPassword("asdf");
            u2.SetPassword("asdf");

            Assert.That(User.CompareHash(u1.PasswordHash, u2.PasswordHash), Is.False);
        }
    }
}
