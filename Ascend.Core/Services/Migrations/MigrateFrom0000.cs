using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using RedBranch.Hammock;

namespace Ascend.Core.Services.Migrations
{
    public class MigrateFrom0000 : MigrationBase
    {
        public override int From { get { return 0; } }
        public override int To { get { return 1; } }

        protected override void Migrate(Document d)
        {   
            if (d.Id.StartsWith("page-") ||
                d.Id.StartsWith("quiz-") ||
                d.Id.StartsWith("survey-") ||
                d.Id.StartsWith("award-"))
            {
                var x = Session.LoadRaw(d.Id) as JObject;
                if (null != x)
                {
                    if (MigrateContent(x) ||
                        MigrateAvailability(x) ||
                        MigrateBudget(x))
                    {
                        History.Updated++;
                        Session.SaveRaw(x);
                    }
                }
            }

            if (d.Id.StartsWith("user-"))
            {
                var x = Session.LoadRaw(d.Id) as JObject;
                if(null != x)
                {
                    if (MigrateUser(x))
                    {
                        History.Updated++;
                        Session.SaveRaw(x);
                    }
                }
            }
        }

        bool MigrateContent(JObject x)
        {
            if (x["Content"] is JObject)
            {
                // already converted
                return false;
            }

            var content = new Content();
            content.Title = x.Value<string>("Title");
            content.Body = x.Value<string>("Content");
            content.Format = (ContentFormat)x.Value<int>("Format");
            x.Remove("Title");
            x.Remove("Content");
            x.Remove("Format");
            x["Content"] = Session.Serializer.WriteFragment(content);
            return true;
        }

        bool MigrateAvailability(JObject x)
        {
            // determine our mode
            var a = new Availability();
            var availableToPublic = x.Value<bool>("AvailableToPublic ");
            var availableToAll = x.Value<bool>("AvailableToAllUsers");
            a.Mode = availableToPublic
                ? AvailabilityMode.AvailableToPublic
                : availableToAll 
                    ? AvailabilityMode.AvailableToAllUsers
                    : AvailabilityMode.AvailableOnlyTo;
            
            // copy users & groups arrays if there's any reason to
            if (AvailabilityMode.AvailableOnlyTo == a.Mode)
            {
                var users = x["Users"] as JArray;
                if (null != users)
                {
                    a.Users = users.Select(y => y.Value<string>()).ToArray();
                }
                var groups = x["Groups"] as JArray;
                if (null != groups)
                {
                    a.Groups = groups.Select(y => y.Value<string>()).ToArray();
                }
            }

            x["Availability"] = Session.Serializer.WriteFragment(a);
            return true;
        }

        bool MigrateBudget(JObject x)
        {
            return x.Remove("Budget");
        }

        bool MigrateUser(JObject x)
        {
            // move budget to budget account
            var budget = x["Budget"] as JObject;
            if (null == budget)
            {
                return false;
            }
            x.Remove("Budget");
            
            var budgetAccount = x.Value<string>("BudgetAccount");
            if (!String.IsNullOrEmpty(budgetAccount))
            {
                try
                {
                    var budgetAccountEntity = Session.LoadRaw(budgetAccount) as JObject;
                    if (null != budgetAccountEntity)
                    {
                        budgetAccountEntity["Budget"] = budget;
                    }
                    Session.SaveRaw(budgetAccountEntity);
                }
                catch
                {
                }
            }
            return true;
        }
    }
}
