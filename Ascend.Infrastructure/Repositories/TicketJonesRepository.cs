using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core.Services;
using RedBranch.Hammock;

namespace Ascend.Core.Repositories
{
    public class TicketJonesCategoryRepository : Repository<TicketJonesCategory>, ITicketJonesCategoryRepository
    {
        public TicketJonesCategoryRepository(Session sx) : base(sx)
        {
        }

        public IDictionary<string, EventCategory> GetCategories()
        {
            return
                WithView(
                    "_all-with-values",
                    @"
                    function (doc) {
                        if (doc._id.indexOf('ticketjonescategory-') === 0) {
                            emit(null, [
                                doc.Active,
                                doc.Description
                            ]);
                        }
                    }")
                    .All().Execute()
                    .Rows.ToDictionary(
                        x => x.Id,
                        x => new EventCategory {
                            Id = x.Id,
                            Description = x.Value.Value<string>(1),
                        });
        }
    }

    public class TicketJonesVenueRepository : Repository<TicketJonesVenue>, ITicketJonesVenueRepository
    {
        public TicketJonesVenueRepository(Session sx) : base(sx)
        {
        }

        public IDictionary<string, EventVenue> GetVenues()
        {
            return 
            WithView(
                "_all-with-values",
                @"
                function (doc) {
                    if (doc._id.indexOf('ticketjonesvenue-') === 0) {
                        emit(null, [
                            doc.Name,
                            doc.Address.Address1, 
                            doc.Address.Address2, 
                            doc.Address.City, 
                            doc.Address.State, 
                            doc.Address.Country, 
                            doc.Phone.Number
                        ]);
                    }
                }")
                .All().Execute()
                .Rows.ToDictionary(
                    x => x.Id,
                    x => new EventVenue {
                        Id = x.Id,
                        Name = x.Value.Value<string>(0),
                        Address = new Address {
                            Address1 = x.Value.Value<string>(1),
                            Address2 = x.Value.Value<string>(2),
                            City = x.Value.Value<string>(3),
                            State = x.Value.Value<string>(4),
                            Country = x.Value.Value<string>(5),
                        },
                        Phone = new Phone {
                            Number = x.Value.Value<string>(6),
                        },
                    });
        }
    }

    public class TicketJonesPerformerRepository : Repository<TicketJonesPerformer>, ITicketJonesPerformerRepository
    {
        public TicketJonesPerformerRepository(Session sx) : base(sx)
        {
        }

        public IDictionary<string, EventPerformer> GetPerformers(IDictionary<string, EventCategory> categories)
        {
            return
                WithView(
                    "_all-with-values",
                    @"
                    function (doc) {
                        if (doc._id.indexOf('ticketjonesperformer-') === 0) {
                            emit(null, [
                                doc.Name,
                                doc.Category
                            ]);
                        }
                    }")
                    .All().Execute()
                    .Rows.ToDictionary(
                        x => x.Id,
                        x => new EventPerformer
                        {
                            Id = x.Id,
                            Name = x.Value.Value<string>(0),
                            Category = categories[x.Value.Value<string>(1)],
                        });
        }
    }

    public class TicketJonesEventRepository : Repository<TicketJonesEvent>, ITicketJonesEventRepository
    {
        public TicketJonesEventRepository(Session sx) : base(sx)
        {
        }

        public DateTime LastUpdated()
        {
            var rows = 
                WithView(
                    "_by-date",
                    @"function(doc) {
                        if (doc.Updated != undefined)
                            emit(doc.Updated.Date, null);
                        if (doc.Created != undefined)
                            emit(doc.Created.Date, null);
                    }")
                    .Limit(1).Descending()
                    .Execute().Rows;
            if (null == rows ||
                rows.Length == 0)
            {
                return DateTime.MinValue;
            }
            return (DateTime)rows[0].Key;
        }

        public IEnumerable<TicketJonesEvent> GetEventsAt(string venue, DateTime from, DateTime to)
        {
            return Where(x => x.Venue).Eq(venue)
                    .And(x => x.Date).Bw(from, to)
                    .Spec().WithDocuments();
        }

        public IEnumerable<TicketJonesEvent> GetEventsBy(string performer, DateTime from, DateTime to)
        {
            return WithView(
                "by-performer-date",
                @"
                function(doc) {
                    if (doc._id.indexOf('ticketjonesevent-') === 0 &&
                        doc.Performers != undefined &&
                        doc.Date != undefined)
                    {
                        if (doc.Performers.length > 0) emit([doc.Performers[0], doc.Date], null);    
                        if (doc.Performers.length > 1) emit([doc.Performers[1], doc.Date], null);    
                        if (doc.Performers.length > 2) emit([doc.Performers[2], doc.Date], null);    
                        if (doc.Performers.length > 3) emit([doc.Performers[3], doc.Date], null);    
                    }
                }")
                .From(new object[] { performer, from })
                .To(new object[] {performer, to})
                .WithDocuments();
        }
    }
}