using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    /// <summary>
    /// Used to model a content area in a page, award, quiz, etc.
    /// </summary>
    public class Content
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public ContentFormat Format { get; set; }
    }

    public enum ContentFormat
    {
        Text = 0,
        Html = 1,
        Markdown = 2,
    }
}
