using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core;
using Ascend.Core.Services;

namespace Ascend.Web.Areas.Site.Controllers
{
    public class VenueViewModel
    {
        public EventVenue Venue { get; set; }
        [UIHint("Events")] public Event[] Events { get; set; }
    }

    public class PerformerViewModel
    {
        public EventPerformer Performer { get; set; }
        [UIHint("Events")] public Event[] Events { get; set; }
    }

    public class EventViewModel
    {
        public Event Event { get; set; }
        public EventTicket[] Tickets { get; set; }
    }

    public class TicketViewModel
    {
        public Event Event { get; set; }
        public EventTicket Ticket { get; set; }
    }

    public class TicketPurchaseModel
    {
        public string Event { get; set; }
        public int Ticket { get; set; }
        public int Quantity { get; set; }
        public Address Address { get; set; }
        public Phone Phone { get; set; }
    }

    public static partial class __HtmlExtensions
    {
        public static MvcHtmlString VenueLink(this HtmlHelper html, EventVenue v)
        {
            return html.ActionLink(
                v.Name,
                MVC.Site.Ticket.Venue(v.Id.Substring("ticketjonesvenue-".Length))
            );
        }

        public static MvcHtmlString PerformerLink(this HtmlHelper html, EventPerformer p)
        {
            return html.ActionLink(
                p.Name,
                MVC.Site.Ticket.Performer(p.Id.Substring("ticketjonesperformer-".Length))
            );
        }

        public static MvcHtmlString EventLink(this HtmlHelper html, Event e)
        {
            return html.ActionLink(
                e.Title,
                MVC.Site.Ticket.Event(e.Id.Substring("ticketjonesevent-".Length))
            );
        }
    }

    
    public partial class TicketController : SiteController
    {
        public IEventTicketingService Events { get; set; }

        [HttpGet]
        public virtual ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public virtual ActionResult Local(string q)
        {
            return View(Events.FindEventsNear(q));
        }

        [HttpGet]
        public virtual ActionResult Venues(string q)
        {
            var a = Events.FindVenues(q);
            return a.Count() == 1 
                ? (ActionResult) RedirectToAction(MVC.Site.Ticket.Venue(a.First().Id.Substring("ticketjonesvenue-".Length)))
                : View(a);
        }

        [HttpGet]
        public virtual ActionResult Performers(string q)
        {
            var a = Events.FindPerformers(q);
            foreach (var g in a.GroupBy(x => x.Category.Description))
            {
                
            }
            return a.Count() == 1
                ? (ActionResult)RedirectToAction(MVC.Site.Ticket.Performer(a.First().Id.Substring("ticketjonesperformer-".Length)))
                : View(a);
        }

        [HttpGet]
        public virtual ActionResult Venue(string id)
        {
            id = "ticketjonesvenue-" + id;
            return View(new VenueViewModel {
                Venue = Events.GetVenue(id),
                Events = Events.FindEventsAt(
                    id,
                    DateTime.UtcNow.AddDays(-1),
                    DateTime.UtcNow.AddDays(90)),
            });
        }

        [HttpGet]
        public virtual ActionResult Performer(string id)
        {
            id = "ticketjonesperformer-" + id;
            return View(new PerformerViewModel {
                Performer = Events.GetPerformer(id),
                Events = Events.FindEventsBy(
                    id,
                    DateTime.UtcNow.AddDays(-1),
                    DateTime.UtcNow.AddDays(90)),
            });
        }

        [HttpGet]
        public virtual ActionResult Event(string id)
        {
            id = "ticketjonesevent-" + id;
            return View(new EventViewModel {
                Event = Events.GetEvent(id),   
                Tickets = Events.GetTickets(id).ToArray(),
            });
        }

        [HttpGet]
        public virtual ActionResult Ticket(string id, int ticket)
        {
            return View(new TicketViewModel {
                Event = Events.GetEvent(id),
                Ticket = Events.GetTickets(id).First(x => x.Id == ticket),
            });
        }

        [HttpPost]
        public virtual ActionResult Order(TicketPurchaseModel model)
        {
            return View();
        }
    }
}