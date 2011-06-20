using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ascend.Web.Areas.Admin
{
    public class AdminRoutes : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Admin"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(null, "admin", new {Controller = "Home", Action = "Index"});
            
            context.MapRoute(null, "admin/tenant", new {Controller = "Tenant", Action = "Index"});
            context.MapRoute(null, "admin/tenant/{id}", new {Controller = "Tenant", Action = "Edit"});

            context.MapRoute(null, "admin/app", new {Controller = "Configuration", action = "Index"});
            
            context.MapRoute(null, "admin/accounts", new { Controller = "Account", Action = "Index"});
            context.MapRoute(null, "admin/accounts/choose", new { Controller = "Account", Action = "Choose" }); 
            context.MapRoute(null, "admin/accounts/{id}", new { Controller = "Account", Action = "Edit" });
            
            context.MapRoute(null, "admin/user", new {Controller = "User", Action = "Index"});
            context.MapRoute(null, "admin/user/search", new {Controller = "User", Action = "Search"});
            context.MapRoute(null, "admin/user/{id}/assume", new {Controller = "User", Action = "Assume"});
            context.MapRoute(null, "admin/user/{id}/reset", new {Controller = "User", Action = "Reset"});
            context.MapRoute(null, "admin/user/{id}", new {Controller = "User", Action = "Edit"});
            
            context.MapRoute(null, "admin/group", new {Controller = "Group", Action = "Index"});
            context.MapRoute(null, "admin/group/{id}", new {Controller = "Group", Action = "Edit"});

            context.MapRoute(null, "admin/concierge", new {Controller = "Concierge", Action = "Index"});
            context.MapRoute(null, "admin/concierge/{id}", new {Controller = "Concierge", Action = "Edit"});

            context.MapRoute(null, "admin/messages", new {Controller = "Messaging", Action = "Messages"});
            context.MapRoute(null, "admin/messages/activate", new {Controller = "Messaging", Action = "Activation"});
            context.MapRoute(null, "admin/messages/welcome", new {Controller = "Messaging", Action = "Welcome"});
            context.MapRoute(null, "admin/messages/{id}/resend", new {Controller = "Messaging", Action = "Resend"});
            context.MapRoute(null, "admin/messages/{id}", new {Controller = "Messaging", Action = "Display"});
            context.MapRoute(null, "admin/templates", new {Controller = "Messaging", Action = "Index"});
            context.MapRoute(null, "admin/template/{id}", new {Controller = "Messaging", Action = "Edit"});

            context.MapRoute(null, "admin/error", new {Controller = "Error", Action = "Index"});
            context.MapRoute(null, "admin/error/clear", new {Controller = "Error", Action = "Clear"});
            context.MapRoute(null, "admin/error/{id}", new {Controller = "Error", Action = "Display"});

            context.MapRoute(null, "admin/ledger", new {Controller = "Ledger", Action = "Post"});
            context.MapRoute(null, "admin/ledger/{id}", new {Controller = "Ledger", Action = "Index"});
            
            context.MapRoute(null, "admin/catalog", new {Controller = "Catalog", Action = "Index"});
            context.MapRoute(null, "admin/catalog/{id}", new {Controller = "Catalog", Action = "Edit"});
            
            context.MapRoute(null, "admin/products", new {Controller = "Product", Action = "Index"});
            context.MapRoute(null, "admin/products/renamecategory", new {Controller = "Product", Action = "RenameCategory"});
            context.MapRoute(null, "admin/products/enable", new {Controller = "Product", Action = "Enable"});
            context.MapRoute(null, "admin/products/categorize", new {Controller = "Product", Action = "Categorize"});
            context.MapRoute(null, "admin/products/tag", new {Controller = "Product", Action = "Tag"});
            context.MapRoute(null, "admin/products/clear-images", new {Controller = "Product", Action = "ClearImages" });
            context.MapRoute(null, "admin/products/refresh-images", new {Controller = "Product", Action = "RefreshImages"});
            context.MapRoute(null, "admin/products/{id}", new {Controller = "Product", Action = "Edit"});
            
            context.MapRoute(null, "admin/page", new {Controller = "Page", Action = "Index"});
            context.MapRoute(null, "admin/page/{id}", new {Controller = "Page", Action = "Edit"});
            
            context.MapRoute(null, "admin/game", new {Controller = "Game", Action = "Index"});
            context.MapRoute(null, "admin/game/{id}", new {Controller = "Game", Action = "Edit"});
            
            context.MapRoute(null, "admin/quiz", new {Controller = "Quiz", Action = "Index"});
            context.MapRoute(null, "admin/quiz/{id}", new {Controller = "Quiz", Action = "Edit"});
            
            context.MapRoute(null, "admin/survey", new {Controller = "Survey", Action = "Index"});
            context.MapRoute(null, "admin/survey/{id}", new {Controller = "Survey", Action = "Edit"});
            
            context.MapRoute(null, "admin/award", new {Controller = "Award", Action = "Index"});
            context.MapRoute(null, "admin/award/certificate", new {Controller = "Award", Action = "Certificate"});
            context.MapRoute(null, "admin/award/{id}", new {Controller = "Award", Action = "Edit"});

            context.MapRoute(null, "admin/files", new {Controller = "File", Action = "Index"});
            context.MapRoute(null, "admin/files/{file}",
                new {Controller = "File", Action = "Index"}, 
                new { httpMethod = new HttpMethodConstraint("DELETE")});
            
            context.MapRoute(null, "admin/migrations", new {Controller = "Migration", Action = "Index"});
            context.MapRoute(null, "admin/validations", new {Controller = "Validation", Action = "Index"});

            context.MapRoute(null, "admin/import", new {Controller = "Import", Action = "Index"});
            context.MapRoute(null, "admin/import/randomizer", new {Controller = "Import", Action = "Randomizer"});
            context.MapRoute(null, "admin/import/events", new {Controller = "Import", Action = "Events"});
            context.MapRoute(null, "admin/import/reset", new {Controller = "Import", Action = "Reset"});
            context.MapRoute(null, "admin/import/review", new {Controller = "Import", Action = "Review"});
            context.MapRoute(null, "admin/import/{id}", new {Controller = "Import", Action = "Edit"});
           
            context.MapRoute(null, "admin/menu", new {Controller = "Menu", Action = "Index"});
            context.MapRoute(null, "admin/menu/{id}", new {Controller = "Menu", Action = "Edit"});
            
            context.MapRoute(null, "admin/theme", new {Controller = "Theme", Action = "Index"});
            context.MapRoute(null, "admin/theme/{id}", new {Controller = "Theme", Action = "Edit"});
        }
    }
}
