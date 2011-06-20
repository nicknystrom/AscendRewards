using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using RedBranch.Hammock;

namespace Ascend.Web.Areas.Site.Controllers
{
    #region OrderEditModel

    public class OrderEditModel
    {
        [DisplayName("Contact"), Required] public string ShippingName { get; set; }
        [DisplayName("Address")] public Address ShippingAddress { get; set; }
        [DisplayName("Phone Number")] public Phone ShippingPhone { get; set; }
    }

    #endregion

    
    public partial class OrderController : SiteController
    {
        public ICatalogService Catalog { get; set; }
        public IOrderRepository Orders { get; set; }
        public IUserRepository UserRepository { get; set; }

        public virtual ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public virtual ActionResult Checkout()
        {
            var m = new OrderEditModel
            {
                ShippingName = CurrentUser.DisplayName,
                ShippingAddress = CurrentUser.HomeAddress ?? CurrentUser.WorkAddress,
                ShippingPhone = CurrentUser.HomePhone ?? CurrentUser.WorkPhone,
            };

            return View(m);
        }

        [HttpPost]
        public virtual ActionResult Checkout(OrderEditModel model)
        {
            // validate form
            if (String.IsNullOrEmpty(model.ShippingAddress.Address1))
            {
                ModelState.AddModelError("ShippingAddress.Address1", "Please include a valid street address.");
            }
            if (String.IsNullOrEmpty(model.ShippingAddress.City))
            {
                ModelState.AddModelError("ShippingAddress.City", "Please specify your city.");
            }
            if (String.IsNullOrEmpty(model.ShippingAddress.State))
            {
                ModelState.AddModelError("ShippingAddress.State", "Please specify your state.");
            }
            if (String.IsNullOrEmpty(model.ShippingAddress.PostalCode))
            {
                ModelState.AddModelError("ShippingAddress.PostalCode", "Please specify your postal code.");
            }
            if (String.IsNullOrEmpty(model.ShippingPhone.Number))
            {
                ModelState.AddModelError("ShippingPhone.Number", "Please include a phone number where we can reach you for any questions regarding this order.");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // update the shopping cart to get current pricing
            CurrentCart.Update(Catalog, Application.DefaultCatalog);

            // create order
            var ticks = DateTime.Now.Ticks.ToString();
            var o = new Order
                        {
                            Document = new Document
                                           {
                                               Id = Document.For<Order>(string.Format(
                                                "{0}-{1}-{2}",
                                                CurrentUser.Login,
                                                DateTime.UtcNow.ToShortDateString(),
                                                ticks.Substring(ticks.Length-4)).ToSlug())
                                           },
                            User = CurrentUser.Document.Id,
                            State = OrderStateMachine.Create(),
                            ShippingName = model.ShippingName,
                            ShippingAddress = model.ShippingAddress,
                            ShippingPhone = model.ShippingPhone,
                            Items = CurrentCart.Select(x => x.Value.ToOrderItem()).ToArray(),
                        };

            // pay for order
            try
            {
                var tx = Accounting.CreateOrderPayment(CurrentUser, o);
                o.Transaction = tx.Document.Id;
            }
            catch
            {
                throw;   
            }

            CurrentUser.Cart = null;
            Orders.Save(o);
            UserRepository.Save(CurrentUser);

            Notifier.Notify(
                Severity.Success,
                "Thank you for your order!",
                "You should receive an email confirmation with your order number shortly, and you can always review your order in My Activity.",
                o);

            return RedirectToAction(MVC.Site.Home.Index());
        }

        [HttpGet]
        public virtual ActionResult Complete()
        {
            return View();   
        }
    }
}