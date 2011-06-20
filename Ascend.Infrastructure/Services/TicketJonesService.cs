using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;
using Ascend.Infrastructure.TicketJones;
using RedBranch.Hammock;
using tj = Ascend.Infrastructure.TicketJones;

namespace Ascend.Infrastructure.Services
{
    public class TicketJonesService : IEventTicketingService
    {
        public IApplicationConfiguration Application { get; set; }

        public ITicketJonesCategoryRepository Categories { get; set; }
        public ITicketJonesVenueRepository Venues { get; set; }
        public ITicketJonesPerformerRepository Performers { get; set; }
        public ITicketJonesEventRepository Events { get; set; }

        static object _sync = new object();
        static bool _updating = false;

        static DateTime? LastUpdated { get; set; }
        static IDictionary<string, EventCategory> _cacheCategories { get; set; }
        static IDictionary<string, EventVenue> _cacheVenues { get; set; }
        static IDictionary<string, EventPerformer> _cachePerformers { get; set; }

        tj.xtics_rpc _channel;
        
        tj.xtics_rpc Channel 
        {
            get
            {
                return _channel ?? (_channel = new tj.xtics_rpc { Url = Application.TicketJonesUrl });
            }
        }

        #region Updating

        public void Repopulate()
        {
            DoUpdate(DateTime.MinValue);
        }

        static bool NeedsUpdate()
        {
            return !LastUpdated.HasValue ||
                (DateTime.UtcNow - LastUpdated.Value).TotalHours > 1;
        }

        void CheckUpdate()
        {
            if (NeedsUpdate())
            {
                var willupdate = true;
                lock(_sync)
                {
                    willupdate = NeedsUpdate() && !_updating;
                    if (willupdate)
                    {
                        _updating = true;
                    }
                }
                if (willupdate)
                {
                    new Thread(DoUpdate).Start(LastUpdated ?? Events.LastUpdated());
                }
            }
        }

        void DoUpdate(object ots)
        {
            var ts = (DateTime) ots;

            var k = Application.TicketJonesKey;

            // update lookup lists first
            lock (_sync)
            {    
                Parallel.Invoke(
                    () => DoUpdateCategories(k, ts),
                    () => DoUpdateVenues(k, ts),
                    () => DoUpdatePeformers(k, ts)
                );
            }
            
            // update events -- never get more than 3 months worth
            if (ts < DateTime.UtcNow.AddMonths(-3))
            {
                ts = DateTime.UtcNow.AddMonths(-3);
            }
            foreach (var a in GetEvents(k, ts))
            {
                lock (_sync)
                {
                    foreach (var x in a)
                    {
                        var id = TicketJonesEvent.For(x.id);
                        var e = Events.TryGet(id) ?? new TicketJonesEvent
                                                         {
                                                             Document = new Document {Id = id},
                                                         };
                        e.TicketJonesId = x.id.ToString();
                        e.Date = x.date;
                        e.Time = x.time;
                        e.Title = x.name;
                        e.Performers = new List<string>();
                        if (x.performer1 > 1)
                        {
                            e.Performers.Add(TicketJonesPerformer.For(x.performer1));
                        }
                        if (x.performer2 > 1)
                        {
                            e.Performers.Add(TicketJonesPerformer.For(x.performer2));
                        }
                        e.Venue = TicketJonesVenue.For(x.venue_id);
                        e.Category = TicketJonesCategory.For(x.event_category_id);
                        Events.Save(e);
                    }
                }
            }

            LastUpdated = DateTime.UtcNow;
            _updating = false;
        }

        private IEnumerable<tj.@event[]> GetEvents(string key, DateTime since)
        {
            int count = Channel.get_event_count(key, since);
            int offset = 0;
            while (offset < count)
            {
                // get the next batch
                var a = Channel.get_events(key, since, offset);
                offset += a.Length;
                yield return a;
            }
        }

        private void DoUpdatePeformers(string key, DateTime since)
        {
            var count = Channel.get_performer_count(key, since);
            var offset = 0;
            if (count > 0)
            {
                _cachePerformers = null;
            }
            while (offset < count)
            {
                // get the next batch
                var a = Channel.get_performers(key, since, offset);
                offset += a.Length;
                foreach (var x in a)
                {
                    var id = TicketJonesPerformer.For(x.id);
                    var performer = Performers.TryGet(id) ?? new TicketJonesPerformer
                                                                 {
                                                                     Document = new Document { Id = id },
                                                                 };
                    performer.TicketJonesId = x.id.ToString();
                    performer.Name = x.name;
                    performer.Category = TicketJonesCategory.For(x.event_category_id);
                    Performers.Save(performer);
                }
            }
        }

        private void DoUpdateVenues(string key, DateTime since)
        {
            var count = Channel.get_venue_count(key, since);
            var offset = 0;
            if (count > 0)
            {
                _cacheVenues = null;
            }
            while (offset < count)
            {
                // get the next batch
                var a = Channel.get_venues(key, since, offset);
                offset += a.Length;
                foreach (var x in a)
                {
                    var id = TicketJonesVenue.For(x.id);
                    var venue = Venues.TryGet(id) ?? new TicketJonesVenue
                                                         {
                                                             Document = new Document { Id = id },
                                                         };
                    venue.TicketJonesId = x.id.ToString();
                    venue.Address = new Address {
                                                    Address1 = x.address1,
                                                    Address2 = x.address2,
                                                    City = x.city,
                                                    State = x.province,
                                                    Country = x.country,
                                                };
                    venue.Phone = new Phone { Number = x.phone };
                    venue.Name = x.name;
                    Venues.Save(venue);
                }
            }
        }

        private void DoUpdateCategories(string key, DateTime since)
        {
            var count = Channel.get_event_category_count(key, since);
            var offset = 0;
            if (count > 0)
            {
                _cacheCategories = null;
            }
            while (offset < count)
            {
                // get the next batch
                var a = Channel.get_event_categories(key, since, offset);
                offset += a.Length;
                foreach (var x in a)
                {
                    var id = TicketJonesCategory.For(x.id);
                    var category = Categories.TryGet(id) ?? new TicketJonesCategory
                                                                {
                                                                    Document = new Document { Id = id },
                                                                };
                    category.TicketJonesId = x.id.ToString();
                    category.Active = x.active;
                    category.Description = x.name;
                    Categories.Save(category);
                }
            }
        }

        #endregion
        #region Category/Venue/Performer Caching

        IDictionary<string, EventCategory> GetCategories()
        {
            if (null == _cacheCategories)
            {
                lock (_sync)
                {
                    if (null == _cacheCategories)
                    {
                        _cacheCategories = Categories.GetCategories();
                    }
                }
            }
            return _cacheCategories;
        }

        IDictionary<string, EventVenue> GetVenues()
        {
            if (null == _cacheVenues)
            {
                lock (_sync)
                {
                    if (null == _cacheVenues)
                    {
                        _cacheVenues = Venues.GetVenues();
                    }
                }
            }
            return _cacheVenues;
        }

        IDictionary<string, EventPerformer> GetPerformers()
        {
            if (null == _cachePerformers)
            {
                lock (_sync)
                {
                    if (null == _cachePerformers)
                    {
                        _cachePerformers = Performers.GetPerformers(GetCategories());
                    }
                }
            }
            return _cachePerformers;
        }

        public EventCategory GetCategory(string id)
        {
            return GetCategories()[id];
        }

        public EventVenue GetVenue(string id)
        {
            return GetVenues()[id];
        }

        public EventPerformer GetPerformer(string id)
        {
            return GetPerformers()[id];
        }

        public Event GetEvent(string id)
        {
            var cx = GetCategories();
            var vn = GetVenues();
            var pf = GetPerformers();
            var x = Events.Get(id);
            return new Event {
                Id = x.Document.Id,
                Date = x.Date,
                Time = x.Time,
                Title = x.Title,
                Venue = vn[x.Venue],
                Category = cx[x.Category],
                Performers = x.Performers.Select(y => pf[y]).ToList(),
            };
        }

        #endregion

        public IEnumerable<EventVenue> FindVenues(string q)
        {
            CheckUpdate();
            q = q.ToLower();
            return GetVenues().Where(x => x.Value.Name.ToLower().Contains(q)).Select(x => x.Value);
        }

        public IEnumerable<EventPerformer> FindPerformers(string q)
        {
            CheckUpdate();
            q = q.ToLower();
            return GetPerformers().Where(x => x.Value.Name.ToLower().Contains(q)).Select(x => x.Value);
        }

        Event[] MapEvents(IEnumerable<TicketJonesEvent> events)
        {
            var cx = GetCategories();
            var vn = GetVenues();
            var pf = GetPerformers();
            return events.Select(x => new Event {
                Id = x.Document.Id,
                Title = x.Title,
                Date = x.Date,
                Category = cx[x.Category],
                Venue = vn[x.Venue],
                Performers = x.Performers.Select(y => pf[y]).ToList(),
            }).ToArray();
        }

        public Event[] FindEventsAt(string venue, DateTime from, DateTime to)
        {
            CheckUpdate();
            return MapEvents(Events.GetEventsAt(venue, from, to));
        }

        public Event[] FindEventsBy(string performer, DateTime from, DateTime to)
        {
            CheckUpdate();
            return MapEvents(Events.GetEventsBy(performer, from, to));
        }

        public Event[] FindEventsNear(string zip)
        {
            CheckUpdate();
            var cx = GetCategories();
            var vn = GetVenues();
            var pf = GetPerformers();
            try
            {
               var a = Channel.get_events_by_zipcode(Application.TicketJonesKey, zip, DateTime.Now, 90);
               return a.Select(x => new Event
                {
                    Id = TicketJonesEvent.For(x.id),
                    Title = x.name,
                    Date = x.date,
                    Category = cx[TicketJonesCategory.For(x.event_category_id)],
                    Venue = vn[TicketJonesVenue.For(x.venue_id)],
                    Performers = new [] { x.performer1, x.performer2 }.Where(y => y > 0).Select(y => pf[TicketJonesPerformer.For(y)]).ToList(),
                }).ToArray();  
            }
            catch
            {
                return new Event[0];
            }
        }

        public IEnumerable<EventTicket> GetTickets(string id)
        {
            if (id.StartsWith("ticketjonesevent-"))
            {
                id = id.Substring("ticketjonesevent-".Length);
            }
            try 
            {
                return Channel
                        .get_tickets(
                            Application.TicketJonesKey,
                            int.Parse(id))
                        .Where(x => x.qty > 0)
                        .Select(x => new EventTicket {
                            Id = x.id,
                            Quantity = x.qty,
                            Section = x.section,
                            Row = x.row,
                            SeatFrom = x.seat_from,
                            SeatThrough = x.seat_thru,
                            Description = x.description,
                            Points = (int)Math.Ceiling(((double)Application.PointsPerDollar * x.price) * 1.3d),
                        });
            }
            catch
            {
                // todo: log this bad boy
                // sometimes ticket jones responds with a text/html page.. not sure what it
                // says, doesn't really matter though.
                return new EventTicket[0];
            }
        }
    }
}
