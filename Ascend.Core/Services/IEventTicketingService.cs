using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core.Services
{
    public class EventCategory
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }

    public class EventPerformer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public EventCategory Category { get; set; }
    }

    public class EventVenue
    {
        public string Id { get; set; }
        public string Name { get; set; } 
        public Address Address { get; set; }
        public Phone Phone { get; set; }
    }

    public class Event
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public EventCategory Category { get; set; }
        public EventVenue Venue { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public List<EventPerformer> Performers { get; set; }
    }

    public class EventTicket
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string Section { get; set; }
        public string Row { get; set; }
        public string SeatFrom { get; set; }
        public string SeatThrough { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
    }

    public interface IEventTicketingService
    {
        void Repopulate();
        IEnumerable<EventVenue> FindVenues(string q);
        IEnumerable<EventPerformer> FindPerformers(string q);
        Event[] FindEventsAt(string venue, DateTime from, DateTime to);
        Event[] FindEventsBy(string performer, DateTime from, DateTime to);
        Event[] FindEventsNear(string zip);
        EventCategory GetCategory(string id);
        EventVenue GetVenue(string id);
        EventPerformer GetPerformer(string id);
        Event GetEvent(string id);
        IEnumerable<EventTicket> GetTickets(string id);
    }
}
