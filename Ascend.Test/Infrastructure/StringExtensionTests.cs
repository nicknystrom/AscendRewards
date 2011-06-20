using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ascend.Core;
using Ascend.Infrastructure;

using NUnit.Framework;

namespace Ascend.Test.Infrastructure
{
    [TestFixture]
    public class StringExtensionTests
    {
        #region MockRequest

        public class MockRequest : HttpRequestBase
        {
            private Uri _url;
            private string _appPath;

            public MockRequest(Uri url, string appPath)
            {
                _url = url;
                _appPath = appPath;
            }

            public override Uri Url
            {
                get
                {
                    return _url;
                }
            }
            public override string ApplicationPath
            {
                get
                {
                    return _appPath;
                }
            }
        }

        #endregion
 
        [Test]
        public void ToAbsoluteUrl_can_handle_tilde()
        {
            var r = new MockRequest(new Uri("http://ascendrewards.com/a1/dashboard/files"), "/a1");
            Assert.That("~/a/b/c".ToAbsoluteUrl(r).ToString(), Is.EqualTo("http://ascendrewards.com/a1/a/b/c"));
        } 
 
        [Test]
        public void ToAbsoluteUrl_can_handle_tilde_with_root_application_path()
        {
            var r = new MockRequest(new Uri("http://ascendrewards.com/a1/dashboard/files"), "/");
            Assert.That("~/a/b/c".ToAbsoluteUrl(r).ToString(), Is.EqualTo("http://ascendrewards.com/a/b/c"));
        } 

        [Test]
        public void ToAbsoluteUrl_can_handle_relative_url()
        {
            var r = new MockRequest(new Uri("http://ascendrewards.com/a1/dashboard/files"), "/a1");
            Assert.That("/a1/a/b/c".ToAbsoluteUrl(r).ToString(), Is.EqualTo("http://ascendrewards.com/a1/a/b/c"));
        }

        [Test]
        public void ToAbsoluteUrl_can_handle_absolute_http_url()
        {
            var r = new MockRequest(new Uri("http://ascendrewards.com/a1/dashboard/files"), "/a1");
            Assert.That("http://ascendrewards.com/a1/a/b/c".ToAbsoluteUrl(r).ToString(), Is.EqualTo("http://ascendrewards.com/a1/a/b/c"));
        }

        [Test]
        public void ToAbsoluteUrl_can_handle_absolute_https_url()
        {
            var r = new MockRequest(new Uri("http://ascendrewards.com/a1/dashboard/files"), "/a1");
            Assert.That("https://ascendrewards.com/a1/a/b/c".ToAbsoluteUrl(r).ToString(), Is.EqualTo("https://ascendrewards.com/a1/a/b/c"));
        }
    }
}
