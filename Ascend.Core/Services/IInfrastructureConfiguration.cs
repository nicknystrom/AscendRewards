using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core.Services
{
    public interface IInfrastructureConfiguration
    {
        string CouchServer { get; }
        string CouchDatabase { get; }
        string CouchCatalogDatabase { get; }
        string CouchTenantsDatabase { get; }
        string CouchErrorsDatabase { get; }
        string CouchTicketJonesDatabase { get; }

        string EmailServer { get; }
        string EmailUsername { get; }
        string EmailPassword { get; }
        bool?  EmailEnableSsl { get; }
        
        string ImageFolder { get; }

        string AmazonAccessKey { get; }
        string AmazonSecretKey { get; }
    }
}
