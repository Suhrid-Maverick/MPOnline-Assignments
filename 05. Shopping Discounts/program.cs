Assignment 05: Shopping Application with Different Discount Types
-Prime loyalty discount
-Festival/seasonal discount
-Coupon code discount
-Demonstrates Strategy pattern + polymorphism

using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingDiscounts;

public abstract class Discount
{
    public string Name { get; }

    protected Discount(string name) => Name = name;

    public abstract decimal Apply(decimal amount);
}

public sealed class PrimeLoyaltyDiscount : Discount
{
    public PrimeLoyaltyDiscount() : base("Prime Loyalty (10%)") { }

    public override decimal Apply(decimal amount) => amount * 0.90m;
}

public sealed class FestivalDiscount : Discount
{
    private readonly decimal _percent;

    public FestivalDiscount(decimal percent) : base($"Festival ({percent}%)")
        => _percent = percent;

    public override decimal Apply(decimal amount) =>
        amount * (1m - _percent / 100m);
}

public sealed class CouponDiscount : Discount
{
    private readonly decimal _flatOff;

    public CouponDiscount(string code, decimal flatOff) : base($"Coupon '{code}' ({flatOff:C} off)")
        => _flatOff = flatOff;

    public override decimal Apply(decimal amount) =>
        Math.Max(0, amount - _flatOff);
}

public sealed class NoDiscount : Discount
{
    public NoDiscount() : base("No Discount") { }

    public override decimal Apply(decimal amount) => amount;
}

public sealed class ComposedDiscount : Discount
{
    private readonly Discount _first;
    private readonly Discount _second;

    public ComposedDiscount(Discount first, Discount second)
        : base($"{first.Name} + {second.Name}")
    {
        _first = first;
        _second = second;
    }

    public override decimal Apply(decimal amount) =>
        _second.Apply(_first.Apply(amount));
}

public sealed class ShoppingCart
{
    private readonly List<(string item, decimal price)> _items = new();
    public decimal SubTotal { get; private set; }

    public void Add(string item, decimal price)
    {
        _items.Add((item, price));
        SubTotal += price;
    }

    public string Checkout(Discount discount)
    {
        var sb = new StringBuilder(256 + _items.Count * 40);
        sb.AppendLine("-------- BILL --------");

        foreach (var (item, price) in _items)
            sb.AppendLine($"  {item,-15} {price,10:C}");

        sb.AppendLine($"  {"Subtotal",-15} {SubTotal,10:C}");
        sb.AppendLine($"  Applied: {discount.Name}");

        decimal final = discount.Apply(SubTotal);
        sb.AppendLine($"  {"Final Payable",-15} {final,10:C}");
        sb.AppendLine($"  You saved: {SubTotal - final:C}");
        sb.AppendLine();

        return sb.ToString();
    }
}

public static class Program
{
    public static void Main()
    {
        var cart = new ShoppingCart();
        cart.Add("Laptop", 55000m);
        cart.Add("Mouse", 500m);
        cart.Add("Keyboard", 1500m);
        cart.Add("Headset", 2000m);

        // Build all output strings in memory, then write once
        var output = new StringBuilder();

        output.AppendLine("----- No Discount -----");
        output.Append(cart.Checkout(new NoDiscount()));

        output.AppendLine("----- Prime Loyalty -----");
        output.Append(cart.Checkout(new PrimeLoyaltyDiscount()));

        output.AppendLine("----- Festival 15% -----");
        output.Append(cart.Checkout(new FestivalDiscount(15m)));

        output.AppendLine("----- Coupon SAVE250 -----");
        output.Append(cart.Checkout(new CouponDiscount("SAVE250", 250m)));

        output.AppendLine("----- Composed (Prime + Festival 5%) -----");
        var composed = new ComposedDiscount(new PrimeLoyaltyDiscount(), new FestivalDiscount(5m));
        output.Append(cart.Checkout(composed));

        Console.Write(output.ToString());
    }
}
