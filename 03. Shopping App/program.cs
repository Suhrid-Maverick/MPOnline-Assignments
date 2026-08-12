Assignment 03: Shopping Experience
-User can be a Customer or a DeliveryAgent
-Customer can place an order and pay
-Demonstrates abstraction + polymorphism

using System;
using System.Collections.Generic;

namespace ShoppingApp
{
    // Abstract base user
    public abstract class User
    {
        public int    UserId { get; }
        public string Name   { get; }
        protected User(int id, string name) { UserId = id; Name = name; }
        public abstract void ShowRole();
    }

    public class Customer : User
    {
        public Customer(int id, string name) : base(id, name) { }

        public override void ShowRole() =>
            Console.WriteLine($"[Customer] {Name} (Id={UserId})");

        public Order PlaceOrder(Cart cart)
        {
            Console.WriteLine($"{Name} is placing an order...");
            var order = new Order(this, cart);
            order.ShowSummary();
            return order;
        }

        public void Pay(Order order, PaymentMethod method)
        {
            Console.WriteLine($"{Name} is paying {order.Total:C} via {method}...");
            order.MarkPaid();
            Console.WriteLine("Payment successful. Order confirmed.");
        }
    }

    public class DeliveryAgent : User
    {
        public string VehicleNo { get; }

        public DeliveryAgent(int id, string name, string vehicleNo)
            : base(id, name) { VehicleNo = vehicleNo; }

        public override void ShowRole() =>
            Console.WriteLine($"[DeliveryAgent] {Name} (Id={UserId}, Vehicle={VehicleNo})");

        public void PickUp(Order order)
        {
            if (!order.IsPaid)
            {
                Console.WriteLine($"Cannot pick up order #{order.OrderId} — not paid yet.");
                return;
            }
            Console.WriteLine($"{Name} picked up order #{order.OrderId}. Out for delivery.");
        }

        public void Deliver(Order order)
        {
            Console.WriteLine($"{Name} delivered order #{order.OrderId} to customer.");
            order.MarkDelivered();
        }
    }

    // Cart/Order/Product
    public class Product
    {
        public int    ProductId { get; }
        public string Name      { get; }
        public decimal Price    { get; }
        public Product(int id, string name, decimal price)
        { ProductId = id; Name = name; Price = price; }
    }

    public class Cart
    {
        private readonly List<(Product p, int qty)> _items = new();
        public IReadOnlyList<(Product p, int qty)> Items => _items;
        public decimal Total { get; private set; }

        public void Add(Product p, int qty)
        {
            _items.Add((p, qty));
            Total += p.Price * qty;
        }
    }

    public class Order
    {
        private static int _nextId = 1001;
        public int      OrderId   { get; } = _nextId++;
        public Customer Customer  { get; }
        public Cart     Cart      { get; }
        public decimal  Total     => Cart.Total;
        public bool     IsPaid    { get; private set; }
        public bool     IsDelivered { get; private set; }

        public Order(Customer c, Cart cart) { Customer = c; Cart = cart; }

        public void MarkPaid()      => IsPaid = true;
        public void MarkDelivered() => IsDelivered = true;

        public void ShowSummary()
        {
            Console.WriteLine($"\n=== Order #{OrderId} ===");
            foreach (var (p, qty) in Cart.Items)
                Console.WriteLine($"  - {p.Name} x {qty} = {p.Price * qty:C}");
            Console.WriteLine($"  Total: {Total:C}\n");
        }
    }

    public enum PaymentMethod { UPI, Card, Cash, Wallet }

    class Program
    {
        static void Main()
        {
            var alice    = new Customer(1, "Alice");
            var agentRaj = new DeliveryAgent(2, "Raj", "DL-01-AB-1234");

            alice.ShowRole();
            agentRaj.ShowRole();

            var cart = new Cart();
            cart.Add(new Product(101, "Laptop",   55000m), 1);
            cart.Add(new Product(102, "Mouse",       500m), 2);
            cart.Add(new Product(103, "Keyboard",   1500m), 1);

            var order = alice.PlaceOrder(cart);
            alice.Pay(order, PaymentMethod.UPI);

            agentRaj.PickUp(order);
            agentRaj.Deliver(order);

            Console.WriteLine($"\nFinal order state: Paid={order.IsPaid}, Delivered={order.IsDelivered}");
        }
    }
}
