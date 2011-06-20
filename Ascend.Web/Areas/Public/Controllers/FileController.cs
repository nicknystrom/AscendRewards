using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Infrastructure;
using Ascend.Core.Services;

namespace Ascend.Web.Areas.Public.Controllers
{
    public partial class FileController : PublicController
    {
        public IFileStore Store { get; set; }
        
        [OutputCache(Duration=3600, VaryByParam="*", VaryByHeader="Host")]
        public virtual ActionResult Index(
            string file,
            int? w,
            int? h)
        {
            // find the file
            var f = Store.Get(file);
                        
            // resize the image
            if (w.HasValue && h.HasValue)
            {
                var resizer = new ImageResizer();
                var image = Image.FromStream(new MemoryStream(f.Item1));
                var stream = new MemoryStream();
                resizer.ResizeImage(
                    image,
                    new System.Drawing.Size(w.Value, h.Value),
                    stream,
                    ImageFormat.Jpeg);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, f.Item2);
            }

            return File(f.Item1, f.Item2);
        }
    }
}