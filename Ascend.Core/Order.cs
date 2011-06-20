using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;
using Newtonsoft.Json;

namespace Ascend.Core
{
    public class Order : Entity 
    {
        public string User { get; set; }
        public OrderItem[] Items { get; set; }
        public OrderStateMachine State { get; set; }
        public string Transaction { get; set; }
        
        public string ShippingName { get; set; }
        public Address ShippingAddress { get; set; }
        public Phone ShippingPhone { get; set; }

        [JsonIgnore]
        public int Total
        {
            get { return Items.Sum(x => x.SubTotal); }   
        }

        [JsonIgnore]
        public DateTime Ordered
        {
            get
            {
                if (null == State ||
                    State.Count == 0) return Created.Date;
                return State.First().Changed;
            }
        }

        /// <summary>
        /// An Order is in a given state if and only if each Item in the order has a status which falls within the associated list.
        /// Evaluate in listed order, as each following OrderState is a superset of the states preceding it.
        /// </summary>
        public static Dictionary<OrderState, OrderItemState[]> ItemStateMap = new Dictionary<OrderState, OrderItemState[]> {
            { OrderState.Closed, new[] {    OrderItemState.Returned,
                                            OrderItemState.Canceled,
                                            OrderItemState.Delivered,
                                            OrderItemState.Declined }},

            { OrderState.Processed, new[] { OrderItemState.Approved,
                                            OrderItemState.Backordered,
                                            OrderItemState.Returned,
                                            OrderItemState.Canceled,
                                            OrderItemState.Delivered,
                                            OrderItemState.Declined }},

            { OrderState.Shipped, new[] {   OrderItemState.Shipped,
                                            OrderItemState.Approved,
                                            OrderItemState.Backordered,
                                            OrderItemState.Returned,
                                            OrderItemState.Canceled,
                                            OrderItemState.Delivered,
                                            OrderItemState.Declined }},
        };
    }

    public class ShoppingCart : Dictionary<string, ShoppingCartItem>
    {
        [JsonIgnore]
        public int Total
        {
            get { return this.Sum(x => x.Value.SubTotal); }
        }

        public void Update(ICatalogService catalog, string catalogId)
        {
            foreach (var i in this)
            {
                var p = catalog.GetProduct(catalogId, i.Value.ProductId);
                i.Value.Stock = p.Stock;

                if (!String.IsNullOrEmpty(i.Value.OptionName))
                {
                    var o = p.Options.FirstOrDefault(x => x.Name == i.Value.OptionName);
                    if (null != o && null != o.Price)
                    {
                        i.Value.UnitPrice = o.Price.Value;
                        return;
                    }
                    
                }
                i.Value.UnitPrice = p.Price;                 
            }
        }

        public bool Contains(string productId, string optionName)
        {
            return ContainsKey(CreateItemKey(productId, optionName));
        }

        public void Add(CatalogProduct product, string optionName, int quantity)
        {
            var option = String.IsNullOrWhiteSpace(optionName)
                             ? null
                             : product.Options.First(x => x.Name == optionName);
            Add(CreateItemKey(product.Id, optionName),
                new ShoppingCartItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        OptionName = option == null ? null : option.Name,
                        OptionSku = option == null ? null : option.Sku,
                        Description = product.Description,
                        UnitPrice = option == null ? product.Price : option.Price ?? product.Price,
                        Quantity = quantity,
                        Stock = product.Stock,
            });    
        }

        public void Remove(string productId, string optionName)
        {
            Remove(CreateItemKey(productId, optionName));
        }

        public ShoppingCartItem this[string productId, string optionName]
        {
            get { return this[CreateItemKey(productId, optionName)]; }
        }

        public static string CreateItemKey(string productId, string optionName)
        {
            if (String.IsNullOrWhiteSpace(optionName))
            {
                return productId;
            }
            return String.Format("{0}-{1}", productId, optionName);
        }
    }

    public class ShoppingCartItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string OptionName { get; set; }
        public string OptionSku { get; set; }
        public string Description { get; set; }
        public int UnitPrice { get; set; }
        public int Quantity { get; set; }

        [JsonProperty("StockData")]
        public ProductStock Stock { get; set; }

        [JsonIgnore]
        public int SubTotal
        {
            get { return UnitPrice * Quantity; }
        }

        public OrderItem ToOrderItem()
        {
            return new OrderItem
                       {
                            ProductId = ProductId,
                            ProductName = ProductName,
                            OptionName = OptionName,
                            Description = Description,
                            UnitPrice = UnitPrice,
                            Quantity = Quantity,
                            Stock = Stock,
                            State = OrderItemStateMachine.Create(),
                       };
        }
    }

    public class OrderItem : ShoppingCartItem
    {
        public OrderItemStateMachine State { get; set; }
    }

    public class OrderItemStateMachine : List<OrderItemStateEntry>
    {
        public static OrderItemStateMachine Create()
        {
            return new OrderItemStateMachine
                       {new OrderItemStateEntry {Value = OrderItemState.Pending, Changed = DateTime.UtcNow}};
        }

        [JsonIgnore]
        public OrderItemState Current
        {
            get { return this.Last().Value; }
        }

        public OrderItemState[] GetAllowedTransitionStates()
        {
            return ((OrderItemState[])Enum.GetValues(typeof(OrderItemState)))
                .Where(x => GetDisposition(x) != OrderItemStateTransitionDisposition.Forbidden)
                .ToArray();
        }


        public OrderItemStateTransitionDisposition GetDisposition(OrderItemState state)
        {
            return Transitions[Current].ContainsKey(state)
                       ? Transitions[Current][state]
                       : OrderItemStateTransitionDisposition.Forbidden;
        } 

        public void Set(OrderItemState state)
        {
            if (state != Current)
            {
                var disposition = GetDisposition(state);
                if (disposition == OrderItemStateTransitionDisposition.Forbidden)
                {
                    throw new InvalidOperationException("This state transition (from " + Current + " to " + state + ") is not allowed.");
                }
                Add(new OrderItemStateEntry { Value = state, Changed = DateTime.UtcNow });
            }
        }

        public static IDictionary<OrderItemState, IDictionary<OrderItemState, OrderItemStateTransitionDisposition>> Transitions =
            new Dictionary<OrderItemState, IDictionary<OrderItemState, OrderItemStateTransitionDisposition>>
            {
                { OrderItemState.Pending, new Dictionary<OrderItemState, OrderItemStateTransitionDisposition> {
                        { OrderItemState.Approved,    OrderItemStateTransitionDisposition.Allow },
                        { OrderItemState.Declined,    OrderItemStateTransitionDisposition.Refund },
                        { OrderItemState.Backordered, OrderItemStateTransitionDisposition.Allow },
                        { OrderItemState.Shipped,     OrderItemStateTransitionDisposition.Allow },
                        { OrderItemState.Delivered,   OrderItemStateTransitionDisposition.Allow },
                        { OrderItemState.Canceled,    OrderItemStateTransitionDisposition.Refund },
                }},
                { OrderItemState.Approved, new Dictionary<OrderItemState, OrderItemStateTransitionDisposition> {
                        { OrderItemState.Backordered, OrderItemStateTransitionDisposition.Allow },
                        { OrderItemState.Shipped,     OrderItemStateTransitionDisposition.Allow },
                        { OrderItemState.Delivered,   OrderItemStateTransitionDisposition.Allow },
                        { OrderItemState.Canceled,    OrderItemStateTransitionDisposition.Refund },
                }},
                { OrderItemState.Declined, new Dictionary<OrderItemState, OrderItemStateTransitionDisposition> { }},
                { OrderItemState.Backordered, new Dictionary<OrderItemState, OrderItemStateTransitionDisposition> {
                        { OrderItemState.Approved,    OrderItemStateTransitionDisposition.Allow },
                        { OrderItemState.Declined,    OrderItemStateTransitionDisposition.Refund },
                        { OrderItemState.Shipped,     OrderItemStateTransitionDisposition.Allow },
                        { OrderItemState.Delivered,   OrderItemStateTransitionDisposition.Allow },
                        { OrderItemState.Canceled,    OrderItemStateTransitionDisposition.Refund },
                }},
                { OrderItemState.Shipped, new Dictionary<OrderItemState, OrderItemStateTransitionDisposition> {
                        { OrderItemState.Delivered,   OrderItemStateTransitionDisposition.Allow },
                        { OrderItemState.Returned,    OrderItemStateTransitionDisposition.Refund },
                }},
                { OrderItemState.Delivered, new Dictionary<OrderItemState, OrderItemStateTransitionDisposition> {
                        { OrderItemState.Returned,    OrderItemStateTransitionDisposition.Refund },
                }},
                { OrderItemState.Canceled, new Dictionary<OrderItemState, OrderItemStateTransitionDisposition> { }},
                { OrderItemState.Returned, new Dictionary<OrderItemState, OrderItemStateTransitionDisposition> { }},
            };
    }

    public class OrderStateMachine : List<OrderStateEntry>
    {
        public static OrderStateMachine Create()
        {
            return new OrderStateMachine { new OrderStateEntry { Value = OrderState.Pending, Changed = DateTime.UtcNow } };
        }

        [JsonIgnore]
        public OrderState Current
        {
            get { return this.Last().Value; }
        }

        public void Set(OrderState state)
        {
            if (state != Current)
            {
                Add(new OrderStateEntry { Value = state, Changed = DateTime.UtcNow });
            }
        }
    }

    public class OrderItemStateEntry
    {
        public OrderItemState Value { get; set; }
        public DateTime Changed { get; set; } 
    }

    public class OrderStateEntry
    {
        public OrderState Value { get; set; }
        public DateTime Changed { get; set; } 
    }

    public enum OrderState
    {
        Pending,
        Processed,
        Shipped,
        Closed,
    }

    public enum OrderItemStateTransitionDisposition
    {
        Allow,
        Forbidden,
        Refund,
        Ignore,
    }

    public enum OrderItemState
    {
        Pending,
        Approved,
        Declined,
        Backordered,
        Shipped,
        Delivered,
        Canceled,
        Returned,
    }
}
