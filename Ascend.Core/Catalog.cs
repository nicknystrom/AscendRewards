using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Ascend.Core
{
    public class Catalog : Entity
    {
        public string Name { get; set; }

        /// <summary>
        /// Default Mark to use for products in categories that are otherwise
        /// overriden with a CategoryMark.
        /// </summary>
        public Mark DefaultMark { get; set; }
        public IList<CategoryMark> CategoryMarks { get; set; }

        public Mark GetMarkForCategory(string[] category)
        {
            var bestMatch = DefaultMark;
            if (null != category && category.Length > 0)
            {
                var bestMatchDepth = 0;
                foreach (var m in CategoryMarks.Where(m => m.Category.Length <= category.Length))
                {
                    if (m.Category.Length > bestMatchDepth &&
                        Enumerable.Range(0, m.Category.Length).All(x => m.Category[x] == category[x]))
                    {
                        bestMatch = m.Mark;
                        bestMatchDepth = m.Category.Length;
                    }
                }
            }
            return bestMatch;
        }

        public decimal GetPrice(Product p)
        {
            return GetMarkForCategory(p.Category).GetFinalPriceForProduct(p);
        }

        public decimal GetPrice(Product p, string option)
        {
            return GetMarkForCategory(p.Category).GetFinalPriceForProduct(p, option);
        }

        public bool IncludeProduct(Product p)
        {
            return p.Enabled == true;
        }
    }

    public class CategoryMark
    {
        /// <summary>
        /// The Category to apply the given Mark to. May be a 'leaf' category (a
        /// category with no sub-categories), or a 'parent' category, in which case
        /// the Mark is applied to it and all sub-categories, unless the sub-category
        /// has a more specific, overriding CategoryMark.
        /// </summary>
        public string[] Category { get; set; }
        public Mark Mark { get; set; }
    }

    public class Mark
    {
        public decimal MaxPriceBelowMsrp { get; set; }
        public decimal MinMarkupPercent { get; set; }
        public decimal MinMarkupDollars { get; set; }

        public decimal GetFinalPriceForProduct(Product p, string option)
        {
            return GetFinalPriceForProduct(p.Options.First(x => x.Name == option));
        }

        public decimal GetFinalPriceForProduct(ProductOption o)
        {
            return GetFinalPriceForProduct(o.GetBestSource().Pricing);
        }

        public decimal GetFinalPriceForProduct(Product p)
        {
            return GetFinalPriceForProduct(p.GetReferencePricing());
        }

        private decimal GetFinalPriceForProduct(ProductPricing pricing)
        {
            // use the lower of the two prices: either % markup, or % below msrp, but
            // ensure that we make a certain flat $ value.
            var x = Math.Max(
                    Math.Min(
                        pricing.Cost * (1+MinMarkupPercent),
                        (pricing.Msrp ?? decimal.MaxValue) * (1-MaxPriceBelowMsrp)
                    ),
                    pricing.Cost + MinMarkupDollars
                );

            // now add in flat costs like shipping
            x += pricing.ShippingFee ?? 0;
            x += pricing.ShippingCost ?? 0;

            return x;
        }
    }
}
