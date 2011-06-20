using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using RedBranch.Hammock;

namespace Ascend.Core.Services.Migrations
{
    public interface IMigrationService
    {
        Version GetVersion();
        int GetMigrationsAvailble();
        void Migrate();
    }

    public interface IMigration
    {
        MigrationHistory Migrate(Session session);
    }

    public abstract class MigrationBase : IMigration
    {
        public MigrationHistory History { get; set; }
        public Session Session { get; set; }
        public abstract int From { get; }
        public abstract int To { get; }

        public MigrationHistory Migrate(Session session)
        {
            History = new MigrationHistory {
                FromRevision = From,
                ToRevision = To,
                Applied = DateTime.UtcNow,
            };
            Session = session;
            var docs = Session.ListDocuments();
            foreach (var d in docs)
            {
                Migrate(d);
            }
            return History;
        }

        protected abstract void Migrate(Document d);
    }
        
    public class MigrationService : IMigrationService
    {
        public Session Session { get; set; } 
        public IRepository<Core.Version> Versions { get; set; }

        public static Dictionary<int, Type> Migrations = new Dictionary<int,Type> {
            { 0, typeof(MigrateFrom0000) },
        };

        public Version GetVersion()
        {
            return Versions.TryGet("version") ??
                new Version { 
                    Document = new Document { Id = "version" },
                    History = new List<MigrationHistory>(),
                };
        }

        protected IMigration GetNextMigration()
        {
            var v = GetVersion();
            var a = Migrations.Where(x => x.Key >= v.Revision).ToArray();
            return a.Length == 0 
                ? null
                : (IMigration) Activator.CreateInstance(a[0].Value);
        }

        public int GetMigrationsAvailble()
        {
            var v = GetVersion();
            return Migrations.Count(x => x.Key >= v.Revision);
        }   

        public void Migrate()
        {
            var v = GetVersion();
            var m = GetNextMigration();
            while (null != m)
            {
                var x = m.Migrate(Session);
                v.History.Add(x);
                v.Revision = x.ToRevision;
                Versions.Save(v);
                m = GetNextMigration();
            }
        }
    }
}
