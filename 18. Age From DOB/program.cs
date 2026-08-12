Assignment 18: Find Age from entered Date of Birth

using System;
using System.Globalization;
using System.Text;

namespace AgeFromDob;

public static class Program
{
    private static (int years, int months, int days) CalculateAge(DateTime dob, DateTime today)
    {
        if (dob > today) return (0, 0, 0);

        int years = today.Year - dob.Year;
        int months = today.Month - dob.Month;
        int days = today.Day - dob.Day;

        if (days < 0)
        {
            months--;
            days += DateTime.DaysInMonth(dob.Year, today.AddMonths(-1).Month);
        }
        if (months < 0)
        {
            years--;
            months += 12;
        }

        return (years, months, days);
    }

    private static void AppendAgeResult(StringBuilder sb, DateTime dob, DateTime today)
    {
        var (y, m, d) = CalculateAge(dob, today);
        sb.AppendLine($"DOB: {dob:dd-MM-yyyy}  ->  Age: {y}y {m}m {d}d");
    }

    public static void Main()
    {
        var samples = new[]
        {
            new DateTime(2004, 5, 14),
            new DateTime(1990, 12, 25),
            new DateTime(2010, 1, 1),
            DateTime.Today.AddYears(-20).AddDays(1)
        };

        var today = DateTime.Today;
        var sb = new StringBuilder(512);

        foreach (var dob in samples)
            AppendAgeResult(sb, dob, today);

        // Interactive
        Console.Write(sb.ToString()); // flush buffered output

        Console.Write("\nEnter your Date of Birth (dd-MM-yyyy): ");
        string input = Console.ReadLine();
        if (DateTime.TryParseExact(input, "dd-MM-yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime userDob))
        {
            var (y, m, d) = CalculateAge(userDob, today);
            Console.WriteLine($"Your age: {y} years, {m} months, {d} days.");
            Console.WriteLine($"Total days alive: {(today - userDob).TotalDays:F0}");
        }
        else
        {
            Console.WriteLine("Invalid date format.");
        }
    }
}
