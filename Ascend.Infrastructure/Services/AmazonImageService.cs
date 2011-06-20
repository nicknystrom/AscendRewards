using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Amazon.S3.Model;
using Ascend.Core;
using Ascend.Core.Services;

using Amazon.S3;
using System.IO;
using Size = Ascend.Core.Size;
using System.Threading;

namespace Ascend.Infrastructure.Services
{
    public class AmazonImageService : BaseImageService
    {
        private const string _imageUrlFormat = "https://s3.amazonaws.com/{0}/{1}";

        public IFileStoreProvider Files { get; set; }

        protected IFileStore Store { get { return Files.GetFileStore(Configuration.ImageFolder, false); } }

        protected static string GetImageName(Product p, int i, Size size)
        {
            return GetImageName(p.ProductId, i, size);
        }

        protected static string GetImageName(string productId, int i, Size size)
        {
            return (null == size)
                ? String.Format("{0}.jpeg", productId)
                : String.Format("{0}-{1}-{2}x{3}.jpeg", productId, i, size.Width, size.Height);
        }

        protected FileResult GetFileResult(byte[] buf)
        {
            return new FileContentResult(buf, "image/jpeg");
        }

        protected FileResult GetFileResult(string name)
        {
            return GetFileResult(Store.Get(name).Item1);
        }

        public override FileResult GetItemImage(Product product, int imageIndex)
        {
            // load from s3
            var name = GetImageName(product, imageIndex, null);
            if (Store.Contains(name))
            {
                return GetFileResult(name);
            }

            // add the image to s3
            var buf = LoadOriginalImage(product, imageIndex);
            Store.Put(name, buf, "image/jpeg"); 
            return GetFileResult(buf);
        }

        public override FileResult GetItemImage(Product product, int imageIndex, Size size)
        {
            // load directly from s3
            var name = GetImageName(product, imageIndex, size);
            if (Store.Contains(name))
            {
                return GetFileResult(name);
            }

            // resize and add to s3
            var original = (FileContentResult)GetItemImage(product, imageIndex);
            var output = new MemoryStream();
            using (var stream = new MemoryStream(original.FileContents))
            {
                Log.DebugFormat("Resizing image for '{0}' to {1}x{2}px.", product.Name, size.Width, size.Height);
                Resizer.ResizeImage(
                    Image.FromStream(stream),
                    new System.Drawing.Size(size.Width, size.Height),
                    output,
                    ImageFormat.Jpeg);
                var buf = output.ToArray();
                Store.Put(name, buf, "image/jpeg"); 
                return GetFileResult(buf);
            }
        }

        public override string GetExternalImageUrl(string productId, int imageIndex, Size size)
        {
            var name = GetImageName(productId, imageIndex, size);
            return Store.Contains(name) ? String.Format(_imageUrlFormat, Store.Name, name) : null;
        }

        public override void ClearExternalImages()
        {
            var n = 0;
            var items = Store.List().Where(x => x != "index.html").ToArray();
            Log.InfoFormat("Deleting {0} images from S3...", items.Length);
            foreach (var x in items)
            {
                Store.Delete(x);
                Log.DebugFormat(" ... deleted {2} ({0}/{1})", ++n, items.Length, x);
            }
        }
    }
}
