using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core;
using Ascend.Core.Repositories;
using Newtonsoft.Json;
using RedBranch.Hammock;

namespace Ascend.Infrastructure.Repositories
{
    public class MenuRepository : Repository<Menu>, IMenuRepository
    {
        public MenuRepository(Session sx) : base(sx)
        {
        }

        //private static JsonSerializer Serializer = new JsonSerializer();

        private static AllowedResource DeserializeResource<T>(Query<T>.Result.Row row) where T : class
        {
            var a = new AllowedResource { Id = row.Id };
            if (row.Id.StartsWith("page-")) a.Type = MenuItemType.Page;
            if (row.Id.StartsWith("game-")) a.Type = MenuItemType.Game;
            if (row.Id.StartsWith("quiz-")) a.Type = MenuItemType.Quiz;
            if (row.Id.StartsWith("award-")) a.Type = MenuItemType.Award;
            if (row.Id.StartsWith("survey-")) a.Type = MenuItemType.Survey;
            if (row.Value != null)
            {
                //a.Availability = Serializer.Deserialize(r)
            }
            return a;
        }

        public IEnumerable<AllowedResource> GetResourcesForUser(User u)
        {
            var a = 
                WithView(
                    "allowed-resources-public",
                    @"function(doc) {
                      if (doc.Enabled && doc.Availability != undefined && (
                            doc.Availability.Mode == 0 || 
                            doc.Availability.Mode == 1))
                        emit(null, doc.Availability);
                    }")
                    .All().Execute().Rows.Select(x => DeserializeResource<Menu>(x))
                .Union(WithView(
                    "allowed-resources-by-user",
                    @"function(doc) {
                      if (doc.Enabled && doc.Availability != undefined && doc.Availability.Users)
                        for (var x in doc.Availability.Users)
                          emit(doc.Users[x], doc.Availability);
                    }")
                    .From(u.Document.Id)
                    .To(u.Document.Id)
                    .Execute().Rows.Select(x => DeserializeResource<Menu>(x)));
            if (!String.IsNullOrEmpty(u.Group))
            {
                a = a.Union(WithView(
                    "allowed-resources-by-group",
                    @"function(doc) {
                      if (doc.Enabled && doc.Availability != undefined && doc.Availability.Groups)
                        for (var x in doc.Availability.Groups)
                          emit(doc.Groups[x], doc.Availability);
                    }")
                    .From(u.Group)
                    .To(u.Group)
                    .Execute().Rows.Select(x => DeserializeResource<Menu>(x)));
            }
            return a;
        }
    }
}
