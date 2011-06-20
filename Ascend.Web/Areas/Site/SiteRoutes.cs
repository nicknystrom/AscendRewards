using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ascend.Web.Areas.Site
{
    public class SiteRoutes : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Site"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(null, "", new { controller = "Home", action = "Index" });
            context.MapRoute(null, "index.html", new { controller = "Home", action = "Index" });
            context.MapRoute(null, "terms", new { controller = "Home", action = "Terms" });

            context.MapRoute(null, "p/{id}", new { controller = "Page", action = "Index" });
            context.MapRoute(null, "g/{id}", new { controller = "Game", action = "Index" });
            context.MapRoute(null, "g/{id}/{r}", new { controller = "Game", action = "Review" });
            context.MapRoute(null, "q/{id}", new { controller = "Quiz", action = "Index" });
            context.MapRoute(null, "q/{id}/{r}", new { controller = "Quiz", action = "Review" });
            context.MapRoute(null, "s/{id}", new { controller = "Survey", action = "Index" });
            context.MapRoute(null, "s/{id}/{r}", new { controller = "Survey", action = "Review" });
            context.MapRoute(null, "a/{id}", new { controller = "Award", action = "Index" });
            context.MapRoute(null, "a/{id}/users", new { controller = "Award", action = "Search" });

            context.MapRoute(null, "profile",  new { controller = "Profile", action = "Index" });
            context.MapRoute(null, "activity/recognitions/sent", new { controller = "Profile", action = "RecognitionsSent" });
            context.MapRoute(null, "activity/recognitions/received", new { controller = "Profile", action = "RecognitionsReceived" });
            context.MapRoute(null, "activity/orders", new { controller = "Profile", action = "Orders" });
            context.MapRoute(null, "activity", new { controller = "Profile", action = "Activity" });
            
            context.MapRoute(null, "budget", new { controller = "Budget", action = "Index" });
            context.MapRoute(null, "budget/{userId}", new { controller = "Budget", action = "Distribute" });

            context.MapRoute(null, "catalog/tickets", new { controller = "Ticket", action = "Index" });
            context.MapRoute(null, "catalog/tickets/local", new { controller = "Ticket", action = "Local" });
            context.MapRoute(null, "catalog/tickets/venues", new { controller = "Ticket", action = "Venues" });
            context.MapRoute(null, "catalog/tickets/performers", new { controller = "Ticket", action = "Performers" });
            context.MapRoute(null, "catalog/tickets/venue/{id}", new { controller = "Ticket", action = "Venue" });
            context.MapRoute(null, "catalog/tickets/performer/{id}", new { controller = "Ticket", action = "Performer" });
            context.MapRoute(null, "catalog/tickets/event/{id}", new { controller = "Ticket", action = "Event" });
            context.MapRoute(null, "catalog/tickets/event/{id}/{ticket}", new { controller = "Ticket", action = "Ticket" });
            context.MapRoute(null, "catalog/tickets/order", new { controller = "Ticket", action = "Order" });

            context.MapRoute(null, "catalog/travel",    new { controller = "Catalog", action = "Travel" });
            context.MapRoute(null, "catalog/giftcards", new { controller = "Catalog", action = "GiftCards" });
            context.MapRoute(null, "catalog/concierge", new { controller = "Catalog", action = "Concierge" });
            context.MapRoute(null, "catalog/tag/{tag}", new { controller = "Catalog", action = "Tagged" });

            context.MapRoute(null, "product/{productId}", new { controller = "Catalog", action = "Product" });
            
            context.MapRoute(
                "CatalogKeys5",
                "catalog/{key0}/{key1}/{key2}/{key3}",
                new {controller = "Catalog", action = "Index"});
            context.MapRoute(
                "CatalogKeys4",
                "catalog/{key0}/{key1}/{key2}",
                new {controller = "Catalog", action = "Index"});
            context.MapRoute(
                "CatalogKeys3",
                "catalog/{key0}/{key1}",
                new {controller = "Catalog", action = "Index"});
            context.MapRoute(
                "CatalogKeys2",
                "catalog/{key0}",
                new {controller = "Catalog", action = "Index"});
            context.MapRoute(
                "CatalogKeys1",
                "catalog",
                new {controller = "Catalog", action = "Index"});

            context.MapRoute(null, "thankyou", new { controller = "Order", action = "Complete" }); 
            context.MapRoute(null, "checkout", new { controller = "Order", action = "Checkout" });
            
            context.MapRoute(null, "cart",     new { controller = "Cart", action = "Index" });
            context.MapRoute(null, "wishlist", new { controller = "Cart", action = "Wishlist" });
            context.MapRoute(null, "cart/{productId}/{optionName}",
                new { controller = "Cart", action = "Update", type = "Cart", optionName = (string)null, }
            );
            context.MapRoute(null, "wishlist/{productId}/{optionName}",
                new { controller = "Cart", action = "Update", type = "Wishlist", optionName = (string)null, }
            );
        }
    }
}