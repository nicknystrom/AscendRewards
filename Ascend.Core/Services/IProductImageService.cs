using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using RedBranch.Hammock;

namespace Ascend.Core.Services
{
    public interface IProductImageService
    {
        FileResult GetItemImage(
            Product product,
            int imageIndex);

        FileResult GetItemImage(
            Product product,
            int imageIndex,
            Size size);

        string GetExternalImageUrl(
            string productId,
            int imageIndex,
            Size size);

        void ClearExternalImages();
    }
}
