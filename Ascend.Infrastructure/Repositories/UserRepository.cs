using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core;
using Ascend.Core.Repositories;
using Newtonsoft.Json.Linq;
using RedBranch.Hammock;

namespace Ascend.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(Session sx) : base(sx)
        {
        }



        public IEnumerable<UserSummary> GetSummaries()
        {
            var query = WithView(
                "_summary",
                @"
                function(doc) {
                  if (doc._id.indexOf('user-') === 0) {
                    emit(null,  [doc.Login, 
                                 (doc.FirstName == undefined ? null : doc.FirstName),
                                 (doc.LastName == undefined ? null : doc.LastName),
                                 (doc.Email == undefined ? null : doc.Email),
                                 doc.State,
                                 (doc.Manager == undefined ? null : doc.Manager),
                                 (doc.Group == undefined ? null : doc.Group),
                                 (doc.Title == undefined ? null : doc.Title)
                    ]);
                  }
                }");
            return query.All().Execute().Rows.Select(
                x => new UserSummary
                {
                    Id = x.Id,
                    Login = x.Value.Value<string>(0),
                    FirstName = x.Value.Value<string>(1),
                    LastName = x.Value.Value<string>(2),
                    Email = x.Value.Value<string>(3),
                    State = (UserState)x.Value.Value<int>(4),
                    Manager = x.Value.Value<string>(5),
                    Group = x.Value.Value<string>(6),
                    Title = x.Value.Value<string>(7),
                });
        }

        public IEnumerable<UserSummary> Search(string q)
        {
            var query = WithView(
                "_search",
                @"
                function(doc) {
                  if (doc._id.indexOf('user-') === 0) {
                    var vals = [doc.Login, 
                                (doc.FirstName == undefined ? null : doc.FirstName),
                                (doc.LastName == undefined ? null : doc.LastName),
                                (doc.Email == undefined ? null : doc.Email),
                                doc.State,
                                (doc.Manager == undefined ? null : doc.Manager),
                                (doc.Group == undefined ? null : doc.Group),
                                (doc.Title == undefined ? null : doc.Title)];
                    for (var i=0; i<4; i++) {
                      var x = vals[i]
                      if (x) {
                        x = x.toLowerCase();
                        for (var n=0; n < x.length-2; n++)
                          emit([x.substr(n)], vals);
                      }
                    }
                  }
                }");
            var a = new JArray {new JValue(q.ToLower())};
            var b = new JArray {new JValue(q.ToLower() + "Z")};
            var r = query.From(a).To(b).Limit(100).Execute();
            return r.GetUniqueDocumentRows().Select(
                x => new UserSummary
                         {
                             Id = x.Id,
                             Login = x.Value.Value<string>(0),
                             FirstName = x.Value.Value<string>(1),
                             LastName = x.Value.Value<string>(2),
                             Email = x.Value.Value<string>(3),
                             State = (UserState)x.Value.Value<int>(4),
                             Manager = x.Value.Value<string>(5),
                             Group = x.Value.Value<string>(6),
                             Title = x.Value.Value<string>(7),
                         });
        }

        public IDictionary<UserState, int> GetUsersCountsByStates()
        {
            return QueryCountsByState()
                .All().Group().Execute()
                .Rows.ToDictionary(
                    x => (UserState)x.Key.Value<int>(),
                    x => x.Value.Value<int>()
                );
        }

        public IList<User> GetUsersWithBudgets()
        {
            return WithView(
                "with-budget",
                @"
                function(doc) {
                  if (doc._id.indexOf('user-') === 0 &&
                      doc.BudgetAccount != undefined) {
                    emit(null, null);
                  }
                }")
                .All().WithDocuments().ToList();
        }

        public User FindUserByEmployeeId(string employeeId)
        {
            return Where(x => x.EmployeeId).Eq(employeeId).SingleOrDefault();
        }

        public User FindUserByLogin(string login)
        {
            return Where(x => x.Login).Eq(login).SingleOrDefault();
        }

        public IEnumerable<User> GetUsersWithoutWelcomeEmail()
        {
            return WithView(
                "without-welcome-email",
                @"
                function (doc) {
                  if (doc._id.indexOf('user-') === 0 &&
                      doc.LastWeclomeEmailSent == undefined) {
                    emit(null, null);
                  }
                }")
                .All().WithDocuments();
        }

        public IEnumerable<User> GetUsersWithoutActivationEmail()
        {
            return WithView(
                "without-activation-email",
                @"
                function (doc) {
                  if (doc._id.indexOf('user-') === 0 &&
                      doc.LastActivationEmailSent == undefined) {
                    emit(null, null);
                  }
                }")
                .All().WithDocuments();
        }

        public int GetUserCountByState(UserState state)
        {
            return QueryCountsByState()
                .All().Group().Execute()
                .Rows.Where(x => x.Key.Value<int>() == (int)state)
                     .Select(x => x.Value.Value<int>())
                     .FirstOrDefault();
        }

        protected Query<User> QueryCountsByState()
        {
            return WithView(
                "_counts-by-state",
                @"
                function(doc) {
                  if (doc._id.indexOf('user-') === 0) {
                    emit(doc.State, 1);
                  }
                }",
                @"
                function (keys, values, rereduce) {
                  return sum(values);
                }");
        }

        public int GetUniqueLoginsByDate(DateTime from, DateTime to)
        {
            return QueryLoginsByDate()
                .From(from).To(to)
                .Execute().Rows.Select(x => x.Key)
                .Distinct().Count();
        }

        protected Query<User> QueryLoginsByDate()
        {
            return WithView(
                "_logins-by-date", 
                @"
                function(doc) {
                  if (doc._id.indexOf('user-') === 0 &&
                      doc.Logins) {
                    for (var x in doc.Logins) {
                      if (doc.Logins[x].Success) {
                        emit(doc.Logins[x].Date, null);
                      }
                    }
                  }
                }");
        }

        public IEnumerable<User> GetUsersWithWishlists()
        {
            return WithView(
                "_users-with-wishlists",
                @"
                function(doc) {
                  if (doc._id.indexOf('user-') === 0 && doc.Wishlist) {
                    for (var i in doc.Wishlist) {
                      emit(null, null);
                      return;
                    }
                  }
                }")
            .All().WithDocuments();
        }
    }
}
