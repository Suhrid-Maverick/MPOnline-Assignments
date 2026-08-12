Assignment 11: Age-based sorting for a collection of Persons
Uses IComparable<Person> + LINQ OrderBy

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgeSorting;

public sealed class Person : IComparable<Person>
{
    public string Name { get; }
    public int Age { get; }

    public Person(string name, int age) => (Name, Age) = (name, age);

    // Default comparison: by Age ascending
    public int CompareTo(Person other) =>
        other is null ? 1 : Age.CompareTo(other.Age);

    public override string ToString() => $"{Name,-15} (Age {Age})";
}

public static class Program
{
    public static void Main()
    {
        var people = new List<Person>
        {
            new("Arjun", 21),
            new("Neha", 19),
            new("Kabir", 35),
            new("Ananya", 25),
            new("Zoya", 18),
            new("Rohit", 28)
        };

        var output = new StringBuilder();

        output.AppendLine("--- Original order ---");
        AppendPeople(output, people);

        // Sort using IComparable (default = age ascending) – in-place
        people.Sort();
        output.AppendLine("\n--- Sorted by Age (ascending, via IComparable) ---");
        AppendPeople(output, people);

        // Sort by age descending using LINQ – no ToList, print directly from enumerable
        output.AppendLine("\n--- Sorted by Age (descending, via LINQ) ---");
        AppendPeople(output, people.OrderByDescending(p => p.Age));

        // Sort by Name ascending – no ToList
        output.AppendLine("\n--- Sorted by Name (ascending, via LINQ) ---");
        AppendPeople(output, people.OrderBy(p => p.Name, StringComparer.Ordinal));

        Console.Write(output.ToString());
    }

    private static void AppendPeople(StringBuilder sb, IEnumerable<Person> people)
    {
        foreach (var p in people)
            sb.AppendLine("  " + p);
    }
}
