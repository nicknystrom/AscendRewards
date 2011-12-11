using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ascend.Core.Services.Import;
using Ascend.Core.Services.Validations;
using Autofac;

using Ascend.Core;
using Ascend.Core.Services;
using Ascend.Core.Repositories;
using Ascend.Core.Services.Caching;
using Ascend.Infrastructure;
using Ascend.Infrastructure.Repositories;
using Ascend.Infrastructure.Services.Caching;
using Ascend.Web.Services;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Newtonsoft.Json;

using Spark;
using Spark.Web.Mvc;
using Spark.Web.Mvc.Descriptors;

using RedBranch.Hammock;
using Ascend.Infrastructure.Services;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Ascend.Infrastructure.Web;


namespace Ascend.Web
{
    #region Spark/MVC2 Fixups

    public class AreaDescriptorFilter : DescriptorFilterBase
    {
        private static string areaVirtualPath = Path.Combine("~/Areas", "{0}", "Views");
            
        public override void ExtraParameters(ControllerContext context, IDictionary<string, object> extra)
        {
            object value;
            if (context.RouteData.DataTokens.TryGetValue("area", out value))
                extra["area"] = value;
        }

        public override IEnumerable<string> PotentialLocations(IEnumerable<string> locations, IDictionary<string, object> extra)
        {
            string areaName;
            return TryGetString(extra, "area", out areaName)
                ? locations.Select(x => Path.Combine(string.Format(areaVirtualPath, areaName), x)).Concat(locations)
                : locations;
        }
    }

    public class CssDescriptorFilter : DescriptorFilterBase
    {
        public override void ExtraParameters(ControllerContext context, IDictionary<string, object> extra)
        {
        }

        public override IEnumerable<string> PotentialLocations(IEnumerable<string> locations, IDictionary<string, object> extra)
        {
            return locations.Select(x => x.Replace(".spark", ".css"));
        }
    }

    #endregion

    public class AscendApplication : HttpApplication, IServiceLocator
    {
        private static Autofac.IContainer _container;

        public static void RegisterRoutes(RouteCollection routes)
        {
            AreaRegistration.RegisterAllAreas();
        }

        protected void RegisterControllers(ContainerBuilder builder)
        {
            builder.RegisterControllers(Assembly.GetExecutingAssembly()).PropertiesAutowired();
            builder.RegisterModule(new AutofacWebTypesModule());
        }

        protected void InstallPersistence(ContainerBuilder builder)
        {
            // register the connection used for products, tenants, and ticket jones
            builder.Register(c => {
                    var cfg = c.Resolve<IInfrastructureConfiguration>();
                    var x = new Connection(new Uri(cfg.CouchServer));
                    var dbs = x.ListDatabases();
                    if (!dbs.Contains(cfg.CouchCatalogDatabase)) { x.CreateDatabase(cfg.CouchCatalogDatabase); }
                    if (!dbs.Contains(cfg.CouchTenantsDatabase)) { x.CreateDatabase(cfg.CouchTenantsDatabase); }
                    if (!dbs.Contains(cfg.CouchTicketJonesDatabase)) { x.CreateDatabase(cfg.CouchTicketJonesDatabase); }
                    if (!dbs.Contains(cfg.CouchErrorsDatabase)) { x.CreateDatabase(cfg.CouchErrorsDatabase); }
                    x.Observers.Add(y => new EntityAuditObserver());
                    return x;
                })
                .Named<Connection>("global-connection")
                .As<Connection>()
                .SingleInstance();
                
            // register a single connection, shared globally, and also create the database if needed
            builder
                .Register(c =>
                              {
                                  var cfg = c.Resolve<IInfrastructureConfiguration>();
                                  var x = new Connection(new Uri(cfg.CouchServer));

                                  // todo: SessionObserver's are VERY hard to wire with IoC in the 
                                  // multi-tenant context. I think the solution is one connection
                                  // instance per tenant, but not sure how that works with autofac.
                                  // Tagged containers?

                                  var h = c.ResolveNamed<ICacheStore>("http-context-cache-store");
                                  var s = c.Resolve<ITenantResolverService>();

                                  x.Observers.Add(y => new EntityAuditObserver());
                                  x.Observers.Add(y => new EntityCache<User>.SessionObserver(h, s));
                                  x.Observers.Add(y => new EntityCache<Group>.SessionObserver(h, s));
                                  x.Observers.Add(y => new EntityCache<Menu>.SessionObserver(h, s));
                                  x.Observers.Add(y => new EntityCache<Page>.SessionObserver(h, s));
                                  x.Observers.Add(y => new EntityCache<ApplicationConfiguration>.SessionObserver(h, s));
                                  x.Observers.Add(y => new EntityCache<Theme>.SessionObserver(h, s));
                                  x.Observers.Add(y => new UserSummaryCache.SessionObserver(h, s));
                                  x.Observers.Add(y => new GroupSummaryCache.SessionObserver(h, s));
                                  return x;
                              })
                .As(typeof(Connection))
                .SingleInstance();

            // register sessions, using the connection as a factory. register the two shared sessions first (catalog and tenants),
            // then register the tenant-specific session last, which makes it the default when an ISession is asked for. Catalog and 
            // tenant session must therefore be requested explicitly by service name.
            builder
                .Register(c => c.ResolveNamed<Connection>("global-connection").CreateSession(c.Resolve<IInfrastructureConfiguration>().CouchCatalogDatabase))
                .InstancePerLifetimeScope()
                .Named<Session>("catalog-session");
            builder
                .Register(c => c.ResolveNamed<Connection>("global-connection").CreateSession(c.Resolve<IInfrastructureConfiguration>().CouchTenantsDatabase))
                .SingleInstance()
                .Named<Session>("tenants-session");            
            builder
                .Register(c => c.ResolveNamed<Connection>("global-connection").CreateSession(c.Resolve<IInfrastructureConfiguration>().CouchTicketJonesDatabase))
                .InstancePerLifetimeScope()
                .Named<Session>("ticketjones-session");
            builder
                .Register(c => c.ResolveNamed<Connection>("global-connection").CreateSession(c.Resolve<IInfrastructureConfiguration>().CouchErrorsDatabase))
                .SingleInstance()
                .Named<Session>("errors-session");
            builder
                .Register(c => 
                {
                    // default session service must determine the current tenant
                    // and create a session on that tenant's db
                    var t = c.Resolve<Tenant>();
                    if (null == t)
                    {
                        throw new Exception("No tenant found to respond to this url.");
                    }
                    var cx = c.Resolve<Connection>();
                    if (!cx.ListDatabases(true).Contains(t.Database))
                    {
                        cx.CreateDatabase(t.Database);
                    }
                    return cx.CreateSession(t.Database);
                })
                .InstancePerLifetimeScope();
                
            // explicit repositories (i.e. classes in .Infrastructure that inherit from Repository<>) will
            // be registered as services and override this registration. this means that requesting an 
            // IRepository<User> will return UserRepository instead of Repository<User>.
            builder
                .RegisterGeneric(typeof (Repository<>))
                .As(typeof (IRepository<>))
                .InstancePerLifetimeScope();

            // register the catalog and tenant repositories seperatly, so that they are forced to attach to their special
            // databases, and not the current tenant's database
            builder
                .Register(c => new ProductRepository(c.ResolveNamed<Session>("catalog-session")))
                .As(typeof(ProductRepository), typeof(IProductRepository), typeof(IRepository<Product>))
                .InstancePerLifetimeScope();
            builder
                .Register(c => new TenantRepository(c.ResolveNamed<Session>("tenants-session")))
                .As(typeof(TenantRepository), typeof(ITenantRepository), typeof(IRepository<Tenant>))
                .InstancePerLifetimeScope();
            builder
                .Register(c => new ErrorRepository(c.ResolveNamed<Session>("errors-session")))
                .As(typeof(ErrorRepository), typeof(IErrorRepository), typeof(IRepository<Error>))
                .InstancePerLifetimeScope();
            builder
                .Register(c => new TicketJonesCategoryRepository(c.ResolveNamed<Session>("ticketjones-session")))
                .As(typeof(TicketJonesCategoryRepository), typeof(ITicketJonesCategoryRepository), typeof(IRepository<TicketJonesCategory>))
                .InstancePerLifetimeScope();
            builder
                .Register(c => new TicketJonesVenueRepository(c.ResolveNamed<Session>("ticketjones-session")))
                .As(typeof(TicketJonesVenueRepository), typeof(ITicketJonesVenueRepository), typeof(IRepository<TicketJonesVenue>))
                .InstancePerLifetimeScope();
            builder
                .Register(c => new TicketJonesPerformerRepository(c.ResolveNamed<Session>("ticketjones-session")))
                .As(typeof(TicketJonesPerformerRepository), typeof(ITicketJonesPerformerRepository), typeof(IRepository<TicketJonesPerformer>))
                .InstancePerLifetimeScope();
            builder
                .Register(c => new TicketJonesEventRepository(c.ResolveNamed<Session>("ticketjones-session")))
                .As(typeof(TicketJonesEventRepository), typeof(ITicketJonesEventRepository), typeof(IRepository<TicketJonesEvent>))
                .InstancePerLifetimeScope();

            // register the 2nd-level caching system
            builder
                .RegisterGeneric(typeof (EntityCache<>))
                .As(typeof (IEntityCache<>))
                .InstancePerLifetimeScope();

            // register the product entity cache using the http cache store instead
            // of the default per-tenant cache store
            builder.Register(c => new EntityCache<Product>(
                    c.Resolve<IRepository<Product>>(),
                    c.ResolveNamed<ICacheStore>("http-context-cache-store")
                ))
                .As<IEntityCache<Product>>()
                .InstancePerLifetimeScope();
        }

        protected void RegisterServices(ContainerBuilder builder)
        {
            var nongenericCoreInterfaces = 
                Assembly.Load("Ascend.Core")
                    .GetTypes()
                    .Where(x => x.IsPublic &&
                                x.IsVisible &&
                                x.IsInterface && 
                                !x.IsGenericTypeDefinition);
            var services =
                Assembly.Load("Ascend.Core").GetTypes().Concat(
                Assembly.Load("Ascend.Infrastructure").GetTypes().Concat(
                Assembly.Load("Ascend.Web").GetTypes())
                    .Where(t => t.IsPublic &&
                                t.IsVisible &&
                                !t.IsGenericTypeDefinition &&
                                !t.IsAbstract &&
                                !t.IsInterface &&
                                t.GetCustomAttributes(typeof(IgnoreServiceAttribute), false).Length == 0 &&
                                t.GetInterfaces().Any(x => nongenericCoreInterfaces.Contains(x))));

            foreach (var t in services)
            {
                builder
                    .RegisterType(t)
                    .As(t.GetInterfaces().Where(x => (x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRepository<>)) || nongenericCoreInterfaces.Contains(x)).ToArray())
                    .PropertiesAutowired()
                    .InstancePerLifetimeScope();
            }

            // register the import services
           builder
              .Register(x => new ProductImportService())
              .As(typeof (IImportService<Product>))
              .PropertiesAutowired()
              .InstancePerLifetimeScope();
            builder
              .Register(x => new UserImportService())
              .As(typeof (IImportService<User>))
              .PropertiesAutowired()
              .InstancePerLifetimeScope();
            builder
              .Register(x => new PointsImportService())
              .As(typeof (IImportService<Transaction>))
              .PropertiesAutowired()
              .InstancePerLifetimeScope();

            // ticket jones service is singleton
            //builder.Register<TicketJonesService>()
            //    .As<IEventTicketingService>()
            //    .SingleInstance();

            // tenant service is singleton
            builder.RegisterType<HostBasedTenantResolverService>()
                .As<ITenantResolverService>()
                .SingleInstance();

            // the http context cache is entirely stateless
            builder.Register(c => new HttpContextCacheStore())
                .Named<ICacheStore>("http-context-cache-store")
                .SingleInstance();

            // register the current tenant as the 'tenant' service
            builder
                .Register(c => c.Resolve<ITenantResolverService>().GetTenantForRequest(HttpContext.Current))
                .As<Tenant>()
                .InstancePerLifetimeScope();

            // IFileStoreProvider is a singleton
            builder.Register(c => new AmazonFileStoreProvider())
                   .As<IFileStoreProvider>()
                   .PropertiesAutowired()
                   .SingleInstance();

            // register an IFileStore service that provides tenant-specific file stores
            builder.Register(c => c.Resolve<IFileStoreProvider>().GetFileStore(c.Resolve<Tenant>().Database))
                .As<IFileStore>()
                .InstancePerLifetimeScope();
            
            // most caching uses the tenant based cache, which puts each tenants data
            // in a seperate bucket (by appending the tenant's database name before each entry)
            builder.Register(c => new TenantCacheStore(
                    c.ResolveNamed<ICacheStore>("http-context-cache-store"),
                    c.Resolve<Tenant>()))
                .As<ICacheStore>()
                .InstancePerLifetimeScope();

            // allow the default (tenant-specific) cache store to be created
            // using a factory. this is used by the connection's entitycache observers,
            // which invalidate cache entries when entities are updated.
            //builder.RegisterGeneratedFactory<Func<ICacheStore>>();

            // register the admin authentication service, which relies on a session from the tenants db
            builder.Register(c => new AdminService(c.ResolveNamed<Session>("tenants-session")))
                   .As<IAdminService>()
                   .SingleInstance();

        }

        protected void InstallConfiguration(ContainerBuilder builder)
        {
            var configLocation = Server.MapPath("~/Config.js");
            builder
                .Register(c => {
                    var cache = c.Resolve<IEntityCache<ApplicationConfiguration>>();
                    return cache[Document.For<ApplicationConfiguration>("default")];
                })
                .As<IApplicationConfiguration>()
                .InstancePerLifetimeScope();
        }

        protected void InstallContainer()
        {
            // install persistence after services, as there are some very specific registrations for catalog
            // and tenanat repositories that must override the default autowiring behavior of RegisterServices()
            var builder = new ContainerBuilder();
            RegisterControllers(builder);
            RegisterServices(builder);
            InstallPersistence(builder);
            InstallConfiguration(builder);

            _container = builder.Build();
            ServiceLocator.CurrentServiceLocator = this;
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
        }

        public static SparkSettings CreateSparkSettings()
        {
            var settings = new SparkSettings();
            settings.Debug = true;
            
            settings.AddAssembly("RedBranch.Hammock");
            settings.AddAssembly("Newtonsoft.Json");
            settings.AddAssembly("Ascend.Core");
            settings.AddAssembly("Ascend.Web");
            settings.AddAssembly("Ascend.Infrastructure");

            settings.AddNamespace("System");
            settings.AddNamespace("System.Collections.Generic");
            settings.AddNamespace("System.Linq");
            settings.AddNamespace("System.Web");
            settings.AddNamespace("System.Web.Routing");
            settings.AddNamespace("System.Web.Mvc");
            settings.AddNamespace("System.Web.Mvc.Html");
            settings.AddNamespace("RedBranch.Hammock");
            settings.AddNamespace("Newtonsoft.Json");
            settings.AddNamespace("Ascend.Core");
            settings.AddNamespace("Ascend.Core.Repositories");
            settings.AddNamespace("Ascend.Core.Services");
            settings.AddNamespace("Ascend.Infrastructure");
            settings.AddNamespace("Ascend.Web");
            settings.AddNamespace("Ascend.Infrastructure");
            return settings;
        }
        
        public static DefaultDescriptorBuilder CreateSparkDescriptorBuilder()
        {
            var builder = new DefaultDescriptorBuilder();
            builder.Filters.Add(new AreaDescriptorFilter());
            return builder;
        }

        protected void InstallViewEngine()
        {
            InstallDefaultViewEngine();
            InstallCssViewEngine();
        }

        protected void InstallDefaultViewEngine()
        {
            // install custom AreaDescriptorBuilder to support MVC2 areas
            var services = SparkEngineStarter.CreateContainer();
            services.SetServiceBuilder<IDescriptorBuilder>(c => CreateSparkDescriptorBuilder());
            services.SetService<ISparkSettings>(CreateSparkSettings());
            SparkEngineStarter.RegisterViewEngine(services);
            
            var views = Path.Combine(Server.MapPath("~"), "bin", "Ascend.Web.Views.dll");
            if (File.Exists(views))
            {
                var factory = services.GetService<IViewEngine>() as SparkViewFactory;
                factory.Engine.LoadBatchCompilation(Assembly.LoadFile(views));
            }
        }

        protected void InstallCssViewEngine()
        {
            // replace the standard '#' spark statement marker with something
            // that no css line ever starts with
            var settings = CreateSparkSettings();
            settings.StatementMarker = ">";

            // install custom AreaDescriptorBuilder to support MVC2 areas, then
            // apply the css filter, which just replaces all .spark extensions
            // with .css extensions.
            var builder = CreateSparkDescriptorBuilder();
            builder.Filters.Add(new CssDescriptorFilter());
            var services = SparkEngineStarter.CreateContainer();
            services.SetServiceBuilder<IDescriptorBuilder>(c => builder);
            services.SetService<ISparkSettings>(settings);
            SparkEngineStarter.RegisterViewEngine(services);
        }

        protected void Application_Start()
        {
            // This will trust *any* certificate, sadly required for sending mail through gmail, as nothing
            // it seems will cause mono to validate those certs
            ServicePointManager.ServerCertificateValidationCallback +=
                delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };

            // build the technology stack
            InstallContainer();
            InstallViewEngine();
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                var ex = Server.GetLastError().GetBaseException();
                var http = ex as HttpException;
                if (null != http)
                {
                    // ignore 404's for favicon (causes db error spam)
                    if (http.GetHttpCode() == 404 && http.Message.Contains("/favicon.ico"))
                        return;
                }
                var err = new Error {
                    
                    Url = Request.RawUrl,
                    FormValues = Request.Form.AllKeys.ToDictionary(
                        x => x,
                        x => Request.Form[x]),
                    QueryValues = Request.QueryString.AllKeys.ToDictionary(
                        x => x,
                        x => Request.QueryString[x]),

                    Type = ex.GetType().Name,
                    Message = ex.Message,       
                    Stack = ex.StackTrace,

                };
                
                var repo = DependencyResolver.Current.GetService<IErrorRepository>();
                repo.Save(err);
            }
            catch
            {
            }
        }

        TService IServiceLocator.Resolve<TService>()
        {
            return _container.Resolve<TService>();
        }
    }
}
