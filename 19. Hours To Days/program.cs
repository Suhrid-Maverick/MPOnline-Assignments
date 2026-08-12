Assignment 19: Convert hours to days (days = h/24)

using System;
using System.Text;

namespace HoursToDays;

public static class Program
{
    public static void Main()
    {
        double[] hoursList = { 12, 24, 36, 48, 72, 96, 100, 168, 365.25 * 24 };

        var sb = new StringBuilder(1024);
        sb.AppendLine($"{"Hours",-12}{"Days",-15}{"Days+Hours"}");
        sb.AppendLine(new string('-', 45));

        foreach (double h in hoursList)
        {
            double totalDays = h / 24.0;
            int wholeDays = (int)totalDays;
            int remainder = (int)(h % 24);
            sb.AppendLine($"{h,-12}{totalDays,-15:F2}{wholeDays}d {remainder}h");
        }

        // Write all buffered output at once
        Console.Write(sb.ToString());

        // Interactive part (direct I/O is fine for user interaction)
        Console.Write("\nEnter hours: ");
        if (double.TryParse(Console.ReadLine(), out double input))
        {
            double total = input / 24.0;
            Console.WriteLine($"{input} hours = {total:F4} days");
            Console.WriteLine($"  = {(int)total} full day(s) and {input % 24} hour(s)");
        }
        else
        {
            Console.WriteLine("Invalid number.");
        }
    }
}
