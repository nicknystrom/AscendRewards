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

            context.MapRoute(null, "/",                       new { Controller = "Home",      Action = "Index"});

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

            context.MapRoute(null, "app", new {Controller = "Configuration", action = "Index"});

            context.MapRoute(null, "accounts", new { Controller = "Account", Action = "Index"});
            context.MapRoute(null, "accounts/choose", new { Controller = "Account", Action = "Choose" });
            context.MapRoute(null, "accounts/{id}", new { Controller = "Account", Action = "Edit" });

            context.MapRoute(null, "user", new {Controller = "User", Action = "Index"});
            context.MapRoute(null, "user/search", new {Controller = "User", Action = "Search"});
            context.MapRoute(null, "user/{id}/assume", new {Controller = "User", Action = "Assume"});
            context.MapRoute(null, "user/{id}/reset", new {Controller = "User", Action = "Reset"});
            context.MapRoute(null, "user/{id}", new {Controller = "User", Action = "Edit"});

            context.MapRoute(null, "group", new {Controller = "Group", Action = "Index"});
            context.MapRoute(null, "group/{id}", new {Controller = "Group", Action = "Edit"});

            context.MapRoute(null, "concierge", new {Controller = "Concierge", Action = "Index"});
            context.MapRoute(null, "concierge/{id}", new {Controller = "Concierge", Action = "Edit"});

            context.MapRoute(null, "messages/activate", new {Controller = "Messaging", Action = "Activation"});
            context.MapRoute(null, "messages/welcome", new {Controller = "Messaging", Action = "Welcome"});
            context.MapRoute(null, "templates", new {Controller = "Messaging", Action = "Index"});
            context.MapRoute(null, "template/{id}", new {Controller = "Messaging", Action = "Edit"});

            context.MapRoute(null, "ledger", new {Controller = "Ledger", Action = "Post"});
            context.MapRoute(null, "ledger/{id}", new {Controller = "Ledger", Action = "Index"});
            
            context.MapRoute(null, "catalog", new {Controller = "Catalog", Action = "Index"});
            context.MapRoute(null, "catalog/{id}", new {Controller = "Catalog", Action = "Edit"});

            context.MapRoute(null, "page", new {Controller = "Page", Action = "Index"});
            context.MapRoute(null, "page/{id}", new {Controller = "Page", Action = "Edit"});
            
            context.MapRoute(null, "game", new {Controller = "Game", Action = "Index"});
            context.MapRoute(null, "game/{id}", new {Controller = "Game", Action = "Edit"});
            
            context.MapRoute(null, "quiz", new {Controller = "Quiz", Action = "Index"});
            context.MapRoute(null, "quiz/{id}", new {Controller = "Quiz", Action = "Edit"});
            
            context.MapRoute(null, "survey", new {Controller = "Survey", Action = "Index"});
            context.MapRoute(null, "survey/{id}", new {Controller = "Survey", Action = "Edit"});
            
            context.MapRoute(null, "award", new {Controller = "Award", Action = "Index"});
            context.MapRoute(null, "award/certificate", new {Controller = "Award", Action = "Certificate"});
            context.MapRoute(null, "award/{id}", new {Controller = "Award", Action = "Edit"});

            context.MapRoute(null, "files", new {Controller = "File", Action = "Index"});
            context.MapRoute(null, "files/{file}",
                new { Controller = "File", Action = "Index" },
                new { httpMethod = new HttpMethodConstraint("DELETE") });
            
            context.MapRoute(null, "migrations", new {Controller = "Migration", Action = "Index"});
            context.MapRoute(null, "validations", new {Controller = "Validation", Action = "Index"});

            context.MapRoute(null, "import", new {Controller = "Import", Action = "Index"});
            context.MapRoute(null, "import/randomizer", new {Controller = "Import", Action = "Randomizer"});
            context.MapRoute(null, "import/events", new {Controller = "Import", Action = "Events"});
            context.MapRoute(null, "import/reset", new {Controller = "Import", Action = "Reset"});
            context.MapRoute(null, "import/review", new {Controller = "Import", Action = "Review"});
            context.MapRoute(null, "import/{id}", new {Controller = "Import", Action = "Edit"});
           
            context.MapRoute(null, "menu", new {Controller = "Menu", Action = "Index"});
            context.MapRoute(null, "menu/{id}", new {Controller = "Menu", Action = "Edit"});
            
            context.MapRoute(null, "theme", new {Controller = "Theme", Action = "Index"});
            context.MapRoute(null, "theme/{id}", new {Controller = "Theme", Action = "Edit"});
        }
    }
}
