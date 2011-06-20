using System;
using Spark.FileSystem;
using System.Collections.Generic;

namespace Ascend.Web.PreCompiler
{
    public class VirtualPathCompatableViewFolder : IViewFolder
    {
        IViewFolder _base;

        public VirtualPathCompatableViewFolder(string basePath)
        {
            _base = new FileSystemViewFolder(basePath);
        }

        protected string Convert(string path)
        {
            return path.StartsWith("~/")
                ? path.Substring(2)
                : path;
        }

        public bool HasView (string path)
        {
            return _base.HasView(Convert(path));
        }

        public IViewFile GetViewSource (string path)
        {
            return _base.GetViewSource(Convert(path));
        }

        public IList<string> ListViews (string path)
        {
            return _base.ListViews(Convert(path));
        }
    }
}

