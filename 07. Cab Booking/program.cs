Assignment 07: Cab Booking Application
-User enters pickup location
-If GPS service fails OR location is invalid, a custom exception is thrown and handled gracefully

using System;
using System.Collections.Generic;
using System.Text;

namespace CabBooking;

public sealed class GpsServiceUnavailableException : Exception
{
    public GpsServiceUnavailableException(string msg) : base(msg) { }
}

public sealed class InvalidLocationException : Exception
{
    public string EnteredLocation { get; }

    public InvalidLocationException(string location)
        : base($"Invalid pickup location: '{location}'. Please enter a valid address.")
    {
        EnteredLocation = location;
    }
}

public sealed class GpsService
{
    private static readonly HashSet<string> ValidLocations = new(
        new[] { "Connaught Place", "IGI Airport", "Cyber Hub", "Noida Sector 62", "Karol Bagh" },
        StringComparer.OrdinalIgnoreCase
    );

    private static readonly Random Random = new();

    public string ResolveLocation(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
            throw new InvalidLocationException(userInput ?? "<empty>");

        // Simulate flaky GPS (20% chance)
        if (Random.Next(1, 6) == 3)
            throw new GpsServiceUnavailableException(
                "GPS service is temporarily unavailable. Please try again.");

        if (!ValidLocations.Contains(userInput))
            throw new InvalidLocationException(userInput);

        return userInput + " (resolved coordinates: 28.61N, 77.20E)";
    }
}

public sealed class CabBookingApp
{
    private readonly GpsService _gps = new();

    // Append booking result directly to the provided StringBuilder (no intermediate string)
    public void BookCab(StringBuilder sb, string pickup, string drop)
    {
        sb.AppendLine($"\nBooking cab: {pickup} -> {drop}");

        try
        {
            string resolved = _gps.ResolveLocation(pickup);
            sb.AppendLine($"  Pickup confirmed: {resolved}");
            sb.AppendLine("  Driver 'Ramesh' (DL-01-XY-9999) arriving in 5 min.");
            sb.AppendLine($"  Booking ID: CAB-{DateTime.Now.Ticks.ToString()[^6..]}");
        }
        catch (InvalidLocationException ex)
        {
            sb.AppendLine("  [Invalid Location] " + ex.Message);
            sb.AppendLine("  Tip: try one of the supported locations.");
        }
        catch (GpsServiceUnavailableException ex)
        {
            sb.AppendLine("  [GPS Error] " + ex.Message);
            sb.AppendLine("  Fallback: dispatching based on last known location...");
        }
        finally
        {
            sb.AppendLine("  --- Booking attempt finished ---");
        }
    }
}

public static class Program
{
    public static void Main()
    {
        var app = new CabBookingApp();
        var output = new StringBuilder();

        app.BookCab(output, "Cyber Hub", "IGI Airport");
        app.BookCab(output, "", "Noida Sector 62");
        app.BookCab(output, "Nowhere Random Place", "Karol Bagh");
        app.BookCab(output, "Connaught Place", "Cyber Hub");

        for (int i = 0; i < 3; i++)
            app.BookCab(output, "Cyber Hub", "IGI Airport");

        Console.Write(output.ToString());
    }
}
