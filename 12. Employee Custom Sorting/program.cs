Assignment 12: Custom sorting of Employees
Sort by: 1) Salary 2) Joining Date 3) Employee ID
Uses IComparer<Employee> for each criterion + LINQ equivalents

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmployeeCustomSorting;

public sealed class Employee
{
    public int EmpId { get; }
    public string Name { get; }
    public decimal Salary { get; }
    public DateTime JoinDate { get; }

    public Employee(int id, string name, decimal salary, DateTime joinDate) =>
        (EmpId, Name, Salary, JoinDate) = (id, name, salary, joinDate);

    public override string ToString() =>
        $"Id={EmpId,-4} Name={Name,-10} Salary={Salary,8:C} Joined={JoinDate:dd-MM-yyyy}";
}

// Comparer 1: by salary descending
public sealed class SalaryComparer : IComparer<Employee>
{
    public static readonly SalaryComparer Instance = new();
    private SalaryComparer() { }

    public int Compare(Employee x, Employee y)
    {
        if (x is null) return y is null ? 0 : 1;
        if (y is null) return -1;
        return y.Salary.CompareTo(x.Salary); // descending
    }
}

// Comparer 2: by joining date ascending (oldest first)
public sealed class JoinDateComparer : IComparer<Employee>
{
    public static readonly JoinDateComparer Instance = new();
    private JoinDateComparer() { }

    public int Compare(Employee x, Employee y)
    {
        if (x is null) return y is null ? 0 : 1;
        if (y is null) return -1;
        return x.JoinDate.CompareTo(y.JoinDate);
    }
}

// Comparer 3: by employee ID ascending
public sealed class EmpIdComparer : IComparer<Employee>
{
    public static readonly EmpIdComparer Instance = new();
    private EmpIdComparer() { }

    public int Compare(Employee x, Employee y)
    {
        if (x is null) return y is null ? 0 : 1;
        if (y is null) return -1;
        return x.EmpId.CompareTo(y.EmpId);
    }
}

public static class Program
{
    private static void AppendEmployees(StringBuilder sb, IEnumerable<Employee> employees, string header)
    {
        sb.AppendLine(header);
        foreach (var e in employees)
            sb.AppendLine("  " + e);
        sb.AppendLine();
    }

    public static void Main()
    {
        var employees = new List<Employee>
        {
            new(105, "Arjun",  55000m, new DateTime(2021, 7,  1)),
            new(102, "Neha",   72000m, new DateTime(2019, 3, 15)),
            new(108, "Kabir",  45000m, new DateTime(2023, 1, 10)),
            new(101, "Ananya", 72000m, new DateTime(2018, 9,  5)),
            new(109, "Rohit",  60000m, new DateTime(2020, 12, 1))
        };

        var output = new StringBuilder();

        AppendEmployees(output, employees, "--- Original list ---");

        // Sort by Salary (descending) using IComparer – no intermediate list
        AppendEmployees(output,
            employees.OrderBy(e => e, SalaryComparer.Instance),
            "--- Sorted by Salary (desc) ---");

        // Sort by JoiningDate (ascending) using IComparer
        AppendEmployees(output,
            employees.OrderBy(e => e, JoinDateComparer.Instance),
            "--- Sorted by Joining Date (asc) ---");

        // Sort by Employee ID (ascending) using IComparer
        AppendEmployees(output,
            employees.OrderBy(e => e, EmpIdComparer.Instance),
            "--- Sorted by Employee ID (asc) ---");

        // Multi-level: Salary desc, then JoinDate asc, then EmpId asc (via LINQ)
        var multi = employees
            .OrderByDescending(e => e.Salary)
            .ThenBy(e => e.JoinDate)
            .ThenBy(e => e.EmpId);
        AppendEmployees(output, multi,
            "--- Multi-level: Salary desc, JoinDate asc, EmpId asc ---");

        Console.Write(output.ToString());
    }
}
