using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ascend.Web.Admin
{
    public static class Routes
    {
        public static void RegisterRoutes(RouteCollection context)
        {
            // global admin routes

            context.MapRoute(null, "",                        new { Controller = "Home",      Action = "Index"});

            context.MapRoute(null, "tenant",                  new { Controller = "Tenant",    Action = "Index"});
            context.MapRoute(null, "tenant/{id}",             new { Controller = "Tenant",    Action = "Edit"});

            context.MapRoute(null, "messages",                new { Controller = "Messaging", Action = "Messages"});
            context.MapRoute(null, "messages/{id}/resend",    new { Controller = "Messaging", Action = "Resend"});
            context.MapRoute(null, "messages/{id}",           new { Controller = "Messaging", Action = "Display"});

            context.MapRoute(null, "error",                   new { Controller = "Error",     Action = "Index"});
            context.MapRoute(null, "error/clear",             new { Controller = "Error",     Action = "Clear"});
            context.MapRoute(null, "error/{id}",              new { Controller = "Error",     Action = "Display"});

            /*
            context.MapRoute(null, "products",                new { Controller = "Product",   Action = "Index"});
            context.MapRoute(null, "products/renamecategory", new { Controller = "Product",   Action = "RenameCategory"});
            context.MapRoute(null, "products/enable",         new { Controller = "Product",   Action = "Enable"});
            context.MapRoute(null, "products/categorize",     new { Controller = "Product",   Action = "Categorize"});
            context.MapRoute(null, "products/tag",            new { Controller = "Product",   Action = "Tag"});
            context.MapRoute(null, "products/clear-images",   new { Controller = "Product",   Action = "ClearImages" });
            context.MapRoute(null, "products/refresh-images", new { Controller = "Product",   Action = "RefreshImages"});
            context.MapRoute(null, "products/{id}",           new { Controller = "Product",   Action = "Edit"});
            */

            // tenant admin routes

            context.MapRoute(null, "{tenant}/app", new {Controller = "Configuration", Action = "Index"});

            context.MapRoute(null, "{tenant}/accounts", new { Controller = "Account", Action = "Index"});
            context.MapRoute(null, "{tenant}/accounts/choose", new { Controller = "Account", Action = "Choose" });
            context.MapRoute(null, "{tenant}/accounts/{id}", new { Controller = "Account", Action = "Edit" });

            context.MapRoute(null, "{tenant}/user", new {Controller = "User", Action = "Index"});
            context.MapRoute(null, "{tenant}/user/search", new {Controller = "User", Action = "Search"});
            context.MapRoute(null, "{tenant}/user/{id}/assume", new {Controller = "User", Action = "Assume"});
            context.MapRoute(null, "{tenant}/user/{id}/reset", new {Controller = "User", Action = "Reset"});
            context.MapRoute(null, "{tenant}/user/{id}", new {Controller = "User", Action = "Edit"});

            context.MapRoute(null, "{tenant}/group", new {Controller = "Group", Action = "Index", Tenant = ""});
            context.MapRoute(null, "{tenant}/group/{id}", new {Controller = "Group", Action = "Edit"});

            context.MapRoute(null, "{tenant}/concierge", new {Controller = "Concierge", Action = "Index"});
            context.MapRoute(null, "{tenant}/concierge/{id}", new {Controller = "Concierge", Action = "Edit"});

            context.MapRoute(null, "{tenant}/messages/activate", new {Controller = "Messaging", Action = "Activation"});
            context.MapRoute(null, "{tenant}/messages/welcome", new {Controller = "Messaging", Action = "Welcome"});
            context.MapRoute(null, "{tenant}/templates", new {Controller = "Messaging", Action = "Index"});
            context.MapRoute(null, "{tenant}/template/{id}", new {Controller = "Messaging", Action = "Edit"});

            context.MapRoute(null, "{tenant}/ledger", new {Controller = "Ledger", Action = "Post"});
            context.MapRoute(null, "{tenant}/ledger/{id}", new {Controller = "Ledger", Action = "Index"});
            
            context.MapRoute(null, "{tenant}/catalog", new {Controller = "Catalog", Action = "Index"});
            context.MapRoute(null, "{tenant}/catalog/{id}", new {Controller = "Catalog", Action = "Edit"});

            context.MapRoute(null, "{tenant}/page", new {Controller = "Page", Action = "Index"});
            context.MapRoute(null, "{tenant}/page/{id}", new {Controller = "Page", Action = "Edit"});
            
            context.MapRoute(null, "{tenant}/game", new {Controller = "Game", Action = "Index"});
            context.MapRoute(null, "{tenant}/game/{id}", new {Controller = "Game", Action = "Edit"});
            
            context.MapRoute(null, "{tenant}/quiz", new {Controller = "Quiz", Action = "Index"});
            context.MapRoute(null, "{tenant}/quiz/{id}", new {Controller = "Quiz", Action = "Edit"});
            
            context.MapRoute(null, "{tenant}/survey", new {Controller = "Survey", Action = "Index"});
            context.MapRoute(null, "{tenant}/survey/{id}", new {Controller = "Survey", Action = "Edit"});
            
            context.MapRoute(null, "{tenant}/award", new {Controller = "Award", Action = "Index"});
            context.MapRoute(null, "{tenant}/award/certificate", new {Controller = "Award", Action = "Certificate"});
            context.MapRoute(null, "{tenant}/award/{id}", new {Controller = "Award", Action = "Edit"});

            context.MapRoute(null, "{tenant}/files", new {Controller = "File", Action = "Index"});
            context.MapRoute(null, "{tenant}/files/{file}",
                new { Controller = "File", Action = "Index" },
                new { httpMethod = new HttpMethodConstraint("DELETE") });
            
            context.MapRoute(null, "{tenant}/migrations", new {Controller = "Migration", Action = "Index"});
            context.MapRoute(null, "{tenant}/validations", new {Controller = "Validation", Action = "Index"});

            context.MapRoute(null, "{tenant}/import", new {Controller = "Import", Action = "Index"});
            context.MapRoute(null, "{tenant}/import/randomizer", new {Controller = "Import", Action = "Randomizer"});
            context.MapRoute(null, "{tenant}/import/events", new {Controller = "Import", Action = "Events"});
            context.MapRoute(null, "{tenant}/import/reset", new {Controller = "Import", Action = "Reset"});
            context.MapRoute(null, "{tenant}/import/review", new {Controller = "Import", Action = "Review"});
            context.MapRoute(null, "{tenant}/import/{id}", new {Controller = "Import", Action = "Edit"});
           
            context.MapRoute(null, "{tenant}/menu", new {Controller = "Menu", Action = "Index"});
            context.MapRoute(null, "{tenant}/menu/{id}", new {Controller = "Menu", Action = "Edit"});
            
            context.MapRoute(null, "{tenant}/theme", new {Controller = "Theme", Action = "Index"});
            context.MapRoute(null, "{tenant}/theme/{id}", new {Controller = "Theme", Action = "Edit"});
        }
    }
}
