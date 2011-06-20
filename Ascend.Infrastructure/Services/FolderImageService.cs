using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Ascend.Core;
using Ascend.Core.Services;
using RedBranch.Hammock;

using Size = Ascend.Core.Size;

namespace Ascend.Infrastructure.Services
{
    public abstract class BaseImageService : IProductImageService
    {
        public IInfrastructureConfiguration Configuration { get; set; }
        public IImageResizer Resizer { get; set; }
        public IHttpClientService Http { get; set; }
        public ILog Log { get; set; }

        public abstract FileResult GetItemImage(Product product, int imageIndex);
        public abstract FileResult GetItemImage(Product product, int imageIndex, Size size);
        public abstract string GetExternalImageUrl(string productId, int imageIndex, Size size);
        public abstract void ClearExternalImages();

        protected byte[] LoadOriginalImage(Product product, int imageIndex)
        {
            // fetch an image from the web
            string url;
            if (null == product.Images ||
                product.Images.Length == 0 ||
                null == (url = product.Images[imageIndex]))
            {
                Log.ErrorFormat("No image url for product '{0}'.", product.Name);
                return null;
            }

            Log.InfoFormat("Loading original for product '{0}' from '{1}'.", product.Name, url);
            try
            {
                var web = new WebClient();
                return web.DownloadData(url);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Failed to load image with exception '{0}'.", ex.Message);
                return null;
            }
        }
    }

    [IgnoreService]
    public class FolderImageService : BaseImageService
    {
        protected virtual string GetImageName(Product p, int i, Size size)
        {
            string path = Configuration.ImageFolder;
            if (null != size)
            {
                path = Path.Combine(path, String.Format("{0}x{1}", size.Width, size.Height));
            }
            if (i > 0)
            {
                path = Path.Combine(path, i.ToString());
            }
            path = Path.Combine(path, p.ProductId + ".jpeg");
            return path;
        }

        private FileResult GetFileResult(string path)
        {
            return new FileStreamResult(File.OpenRead(path), "image/jpeg");              
        }

        public override FileResult GetItemImage(Product product, int imageIndex)
        {
            var file = GetImageName(product, imageIndex, null);
            if (!File.Exists(file))
            {
                var buf = LoadOriginalImage(product, imageIndex);

                var fi = new FileInfo(file);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                File.WriteAllBytes(file, buf);
            }
            return GetFileResult(file);
        }

        public override FileResult GetItemImage(Product product, int imageIndex, Size size)
        {
            if (null == size) return GetItemImage(product, imageIndex); 
            var file = GetImageName(product, imageIndex, size);
            if (!File.Exists(file))
            {
                // load the original
                var original = (FileStreamResult)GetItemImage(product, imageIndex);
                using (var stream = original.FileStream)
                {
                    var fi = new FileInfo(file);
                    if (!fi.Directory.Exists)
                    {
                        fi.Directory.Create();
                    }
                    using (var output = File.Create(file))
                    {
                        Resizer.ResizeImage(
                            Image.FromStream(stream),
                            new System.Drawing.Size(size.Width, size.Height),
                            output,
                            ImageFormat.Jpeg);
                    }
                }    
            }
            return GetFileResult(file);
        }

        public override string GetExternalImageUrl(string productId, int imageIndex, Size size)
        {
            return null;
        }

        public override void ClearExternalImages()
        {
            // nothing
        }
    }
}
