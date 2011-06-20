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
            var o = p.Options.First(x => x.Name == option);
            return GetFinalPriceForProduct(p, (o.Price ?? p.Pricing).Price);
        }

        public decimal GetFinalPriceForProduct(Product p)
        {
            return GetFinalPriceForProduct(p, p.Pricing.Price);
        }

        private decimal GetFinalPriceForProduct(Product p, decimal basePrice)
        {
            // get the price of the first item
            var x = basePrice;

            // use the lower of the two prices: either % markup, or % below msrp, but
            // ensure that we make a certain flat $ value.
            x = Math.Max(
                    Math.Min(
                        x * (1+MinMarkupPercent),
                        (p.Pricing.Msrp ?? decimal.MaxValue) * (1-MaxPriceBelowMsrp)
                    ),
                    x + MinMarkupDollars
                );

            // now add in flat costs like shipping
            if (null != p.Shipping)
            {
                x += p.Shipping.DropShipFee ?? 0;
                x += p.Shipping.Cost ?? 0;
            }

            return x;
        }
    }
}
