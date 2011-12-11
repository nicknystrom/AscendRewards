using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Ascend.Core.Services;

namespace Ascend.Infrastructure
{
    public class WebConfigInfrastructureConfiguration : IInfrastructureConfiguration
    {
        public string CouchServer
        {
            get { return ConfigurationManager.AppSettings["couch-server"];  }
        }

        public string CouchDatabase
        {
            get { return ConfigurationManager.AppSettings["couch-database"]; }
        }

        public string EmailServer
        {
            get { return ConfigurationManager.AppSettings["email-server"]; }
        }

        public string EmailUsername
        {
            get { return ConfigurationManager.AppSettings["email-username"]; }
        }

        public string EmailPassword
        {
            get { return ConfigurationManager.AppSettings["email-password"]; }
        }

        public bool? EmailEnableSsl
        {
            get
            {
                var a = ConfigurationManager.AppSettings["email-enable-ssl"];
                return String.IsNullOrWhiteSpace(a) ? false : bool.Parse(a);
            }
        }

        public string ImageFolder
        {
            get { return ConfigurationManager.AppSettings["image-folder"]; }
        }

        public string CouchCatalogDatabase
        {
            get { return ConfigurationManager.AppSettings["couch-catalog-database"]; }
        }

        public string CouchTenantsDatabase
        {
            get { return ConfigurationManager.AppSettings["couch-tenants-database"]; }
        }

        public string CouchTicketJonesDatabase
        {
            get { return ConfigurationManager.AppSettings["couch-ticketjones-database"]; }
        }

        public string CouchErrorsDatabase
        {
            get { return ConfigurationManager.AppSettings["couch-errors-database"]; }
        }

        public string AmazonAccessKey
        {
            get { return ConfigurationManager.AppSettings["amazon-access-key"]; }
        }

        public string AmazonSecretKey
        {
            get { return ConfigurationManager.AppSettings["amazon-secret-key"]; }
        }
    }
}
