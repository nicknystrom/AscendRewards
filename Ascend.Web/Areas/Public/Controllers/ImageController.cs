using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Ascend.Core;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;

namespace Ascend.Web.Areas.Public.Controllers
{
    public partial class ImageController : PublicController
    {
        public IEntityCache<Product> Products { get; set; }
        public ICacheStore Cache { get; set; }
		public ILog Log { get; set; }

        [HttpGet]
        [OutputCache(
            Location = OutputCacheLocation.Downstream,
            Duration = 3600 * 24 * 30,
            VaryByParam = "*")]
        public virtual ActionResult Index(string productId, int imageIndex, int? w, int? h)
        {
            // return the unprocessed original
            try
            {
                var itemImage = Images.GetItemImage(
                    Products["product-" + productId],
                    imageIndex,
                    (w.HasValue && h.HasValue) ? new Size {Width = w.Value, Height = h.Value} : null);
				if (null == itemImage)
				{
					Log.WarnFormat(
						"Image {0} for {1} was requested but not found.",
						imageIndex,
						productId);
				}
                return itemImage ?? File("~/Content/Images/no-image.png", "image/png");
            }
            catch (Exception ex)
            {
				Log.ErrorFormat(
					"Failed to render image {0} for {1} at ({2}x{3}): {4}",
					imageIndex,
					productId,
					w,
					h,
					ex.Message);
                return File("~/Content/Images/no-image.png", "image/png");
            }
        }
    }
}