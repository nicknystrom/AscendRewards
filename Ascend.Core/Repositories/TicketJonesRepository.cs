using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core.Services;
using RedBranch.Hammock;

namespace Ascend.Core.Repositories
{
    public interface ITicketJonesCategoryRepository : IRepository<TicketJonesCategory>
    {
        IDictionary<string, EventCategory> GetCategories();
    }

    public interface ITicketJonesVenueRepository : IRepository<TicketJonesVenue>
    {
        IDictionary<string, EventVenue> GetVenues();
    }

    public interface ITicketJonesPerformerRepository : IRepository<TicketJonesPerformer>
    {
        IDictionary<string, EventPerformer> GetPerformers(IDictionary<string, EventCategory> categories);
    }

    public interface ITicketJonesEventRepository : IRepository<TicketJonesEvent>
    {
        DateTime LastUpdated();
        IEnumerable<TicketJonesEvent> GetEventsAt(string venue, DateTime from, DateTime to);
        IEnumerable<TicketJonesEvent> GetEventsBy(string performer, DateTime from, DateTime to);
    }
}