using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ascend.Core;
using RedBranch.Hammock;

namespace Ascend.Infrastructure
{
    public class EntityAuditObserver : BaseObserver
    {
        public override Disposition BeforeSave(object entity, Document document)
        {
            // only interested in entities
            var e = entity as Entity;
            if (null == e)
            {
                return Disposition.Continue;
            }

            // attempt to determine the current username
            string user = null;
            if (HttpContext.Current != null &&
                HttpContext.Current.User != null &&
                HttpContext.Current.User.Identity != null &&
                HttpContext.Current.User.Identity.IsAuthenticated)
            {
                user = HttpContext.Current.User.Identity.Name;  
            }

            // set the date and user fields
            var audit = new EntityActivity
                            {
                                Date = DateTime.UtcNow,
                                User = user,
                            };

            // set the source, currently only http is supported.
            if (HttpContext.Current != null)
            {
                audit.Source = "http";
                audit.SourceId = HttpContext.Current.Request.RawUrl;
            }

            // assign to either created or updated
            if (e.Document == null ||
                e.Document.Id == null ||
                e.Document.Revision == null)
            {
                e.Created = audit;
            }
            else
            {
                e.Updated = audit;
            }
            
            return Disposition.Continue;
        }
    }
}
