using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using RedBranch.Hammock;

using Size = Ascend.Core.Size;

namespace Ascend.Infrastructure.Services
{
    // obsolete.. use FileImageService. CouchDb just doesnt like storing
    // 160,000+ images: go figure.

    //public class AttachmentImageService : IProductImageService
    //{
    //    public IImageResizer Resizer { get; set; }
    //    public IHttpClientService Http { get; set; }
    //    public IProductRepository Products { get; set; }

    //    public Attachment GetItemImage(
    //        Product product,
    //        int imageIndex)
    //    {
    //        return GetItemImage(product, imageIndex, false);
    //    }
        
    //    public Attachment GetItemImage(
    //        Product product,
    //        int imageIndex,
    //        bool reloadImage)
    //    {
    //        string imageName = imageIndex.ToString();
    //        if (!reloadImage && null != product.Attachments && product.Attachments.ContainsKey(imageName))
    //        {
    //            return product.Attachments[imageName];
    //        }

    //        // fetch an image from the web
    //        string url;
    //        if (null == product.Images ||
    //            product.Images.Length == 0 ||
    //            null == (url = product.Images[imageIndex]))
    //        {
    //            return null;
    //        }
            
    //        var web = new WebClient();
    //        var buf = web.DownloadData(url); 

            
    //        Products.AttachImage(
    //            product,
    //            imageName,
    //            "image/jpeg",
    //            new MemoryStream(buf));
    //        return product.Attachments[imageName];
    //    }
        
    //    public Attachment GetItemImage(
    //        Product product,
    //        int imageIndex,
    //        Size size)
    //    {
    //        var itemImage = GetItemImage(product, imageIndex);
    //        if (null == itemImage) return null;

    //        if (null == size) return itemImage;

    //        var k = imageIndex + "-" + size.Width + "-" + size.Height;
    //        if (null != product.Attachments && product.Attachments.ContainsKey(k))
    //        {
    //            return product.Attachments[k];
    //        }

    //        // load the original
    //        using (var stream = itemImage.LoadStream())
    //        {
    //            var buf = new MemoryStream();
    //            Resizer.ResizeImage(
    //                Image.FromStream(stream),
    //                new System.Drawing.Size(size.Width, size.Height),
    //                buf,
    //                ImageFormat.Jpeg);
    //            buf.Seek(0, SeekOrigin.Begin);
    //            Products.AttachImage(
    //                product,
    //                k,
    //                "image/jpeg",
    //                buf);
    //        }

    //        return product.Attachments[k];
    //    }

    //    public void ClearImages(Product entity)
    //    {
    //        if (null != entity.Attachments)
    //        {
    //            entity.Attachments.Clear();
    //            Products.Save(entity);
    //        }
    //    }
    //}
}
