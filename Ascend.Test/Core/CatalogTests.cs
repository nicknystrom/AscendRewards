using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core;
using NUnit.Framework;

namespace Ascend.Test.Core
{
    [TestFixture]
    public class CatalogTests
    {
        public static Catalog GetCatalog()
        {
            return new Catalog
            {
                DefaultMark = new Mark
                                  {
                                      MaxPriceBelowMsrp = (decimal) 0.10,
                                      MinMarkupPercent = (decimal) 0.20,
                                      MinMarkupDollars = 1,
                                  },
                CategoryMarks = new List<CategoryMark>
                {
                    new CategoryMark
                    {
                        Category = new[] { "Catalog", "Books" },
                        Mark  = new Mark
                          {
                              MaxPriceBelowMsrp = (decimal) 0.20,
                              MinMarkupPercent = (decimal) 0.20,
                              MinMarkupDollars = 2,
                          },
                    },
                    new CategoryMark
                    {
                        Category = new[] { "Catalog", "Electronics" },
                        Mark  = new Mark
                          {
                              MaxPriceBelowMsrp = (decimal) 0.30,
                              MinMarkupPercent = (decimal) 0.20,
                              MinMarkupDollars = 3,
                          },
                    },
                    new CategoryMark
                    {
                        Category = new[] { "Catalog", "Electronics", "Computers" },
                        Mark  = new Mark
                          {
                              MaxPriceBelowMsrp = (decimal) 0.40,
                              MinMarkupPercent = (decimal) 0.20,
                              MinMarkupDollars = 4,
                          },
                    },
                }
            };
        }

        [Test]
        public void Catalog_returns_default_mark_when_no_categories_match()
        {
            Assert.That(GetCatalog().GetMarkForCategory(new[] {"Catalog", "Apparel"}).MinMarkupDollars, Is.EqualTo(1));
        }

        [Test]
        public void Catalog_returns_closest_matching_mark_when_no_exact_match_exists()
        {
            Assert.That(GetCatalog().GetMarkForCategory(new[] {"Catalog", "Electronics", "Televisions"}).MinMarkupDollars, Is.EqualTo(3));
            Assert.That(GetCatalog().GetMarkForCategory(new[] {"Catalog", "Electronics", "Computers", "Apple"}).MinMarkupDollars, Is.EqualTo(4));
        }

        [Test]
        public void Catalog_returns_exact_match_mark()
        {
            Assert.That(GetCatalog().GetMarkForCategory(new[] { "Catalog", "Electronics", "Computers" }).MinMarkupDollars, Is.EqualTo(4));
        }
    }
}
