using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Ascend.Core;
using Ascend.Core.Services;
using NUnit.Framework;

namespace Ascend.Test.Core
{
    [TestFixture]
    public class TemplateTests
    {
        class MockRequest : HttpRequestBase
        {
            Uri _url;
            string _app;

            public MockRequest(Uri url, string app)
            {
                _url = url;
                _app = app;
            }

            public override Uri Url  { get { return _url; } }
            public override string ApplicationPath { get { return _app; } } 
        }
    
        [Test]
        public void Template_engine_performs_data_replacements()
        {
            var service = new BasicTemplateService();
            var t = new Template { Content = @"This is a {foo}." };
            var d = new TemplateData {{ "foo", "bar" }};
            var w = service.Render(t.Content, d, null);
            Assert.That(w, Is.EqualTo(@"This is a bar."));
        }

        [Test]
        public void Template_engine_performs_url_replacements()
        {
            var service = new BasicTemplateService();
            var r = new MockRequest(new Uri("http://sandbox.ascendrewards.com/admin/foo"), "/");
            var t = new Template { Content = @"This is a <a href=""~/url"">link</a>." };
            var d = new TemplateData {{ "foo", "bar" }};
            var w = service.Render(t.Content, d, r);
            Assert.That(w, Is.EqualTo(@"This is a <a href=""http://sandbox.ascendrewards.com/url"">link</a>."));
        }

        [Test]
        public void Template_engine_performs_url_replacements_with_virtual_directory()
        {
            var service = new BasicTemplateService();
            var r = new MockRequest(new Uri("http://sandbox.ascendrewards.com/bar/admin/foo"), "/bar");
            var t = new Template { Content = @"This is a <a href=""~/url"">link</a>." };
            var d = new TemplateData {{ "foo", "bar" }};
            var w = service.Render(t.Content, d, r);
            Assert.That(w, Is.EqualTo(@"This is a <a href=""http://sandbox.ascendrewards.com/bar/url"">link</a>."));
        }

    }
}
