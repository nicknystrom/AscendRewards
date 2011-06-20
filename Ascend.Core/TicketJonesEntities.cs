using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core.Services;
using RedBranch.Hammock;

namespace Ascend.Core
{
    public class TicketJonesCategory : Entity
    {
        public string TicketJonesId { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }

        public static string For(int id) { return Document.For<TicketJonesCategory>(id.ToString()); }
    }

    public class TicketJonesPerformer : Entity
    {
        public string TicketJonesId { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }

        public static string For(int id) { return Document.For<TicketJonesPerformer>(id.ToString()); }
    }

    public class TicketJonesVenue : Entity
    {
        public string TicketJonesId { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public Phone Phone { get; set; }

        public static string For(int id) { return Document.For<TicketJonesVenue>(id.ToString()); }
    }

    public class TicketJonesEvent : Entity
    {
        public string TicketJonesId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Venue { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public List<string> Performers { get; set; }

        public static string For(int id) { return Document.For<TicketJonesEvent>(id.ToString()); }
    }
}
