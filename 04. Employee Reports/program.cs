Assignment 04: Manager Application
-Three employee types: Developer, Tester, Manager
-All can GenerateReport() — different content per role
-Demonstrates abstract class + polymorphism

using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeReports;

public abstract class Employee
{
    public int Id { get; }
    public string Name { get; }

    protected Employee(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public abstract string GenerateReport();
}

public sealed class Developer : Employee
{
    public string ProjectName { get; }
    public int ModulesBuilt { get; }
    public int BugsFixed { get; }

    public Developer(int id, string name, string projectName, int modulesBuilt, int bugsFixed)
        : base(id, name)
    {
        ProjectName = projectName;
        ModulesBuilt = modulesBuilt;
        BugsFixed = bugsFixed;
    }

    public override string GenerateReport()
    {
        return $@"----- DEVELOPER REPORT -----
Developer   : {Name} (Id={Id})
Project     : {ProjectName}
Modules built: {ModulesBuilt}
Bugs fixed   : {BugsFixed}
Status      : Project development in progress.
";
    }
}

public sealed class Tester : Employee
{
    public string ProjectName { get; }
    public int TestCasesRun { get; }
    public int BugsReported { get; }
    public int BugsClosed { get; }

    public Tester(int id, string name, string projectName, int testCasesRun, int bugsReported, int bugsClosed)
        : base(id, name)
    {
        ProjectName = projectName;
        TestCasesRun = testCasesRun;
        BugsReported = bugsReported;
        BugsClosed = bugsClosed;
    }

    public override string GenerateReport()
    {
        int openBugs = BugsReported - BugsClosed;
        return $@"----- TESTER REPORT -----
Tester       : {Name} (Id={Id})
Project      : {ProjectName}
Test cases run: {TestCasesRun}
Bugs reported : {BugsReported}
Bugs closed   : {BugsClosed}
Open bugs     : {openBugs}
";
    }
}

public sealed class Manager : Employee
{
    private readonly List<Employee> _team = new();

    public Manager(int id, string name) : base(id, name) { }

    public void AddToTeam(Employee employee) => _team.Add(employee);

    public override string GenerateReport()
    {
        // Pre‑size StringBuilder to avoid reallocations
        var sb = new StringBuilder(256 + _team.Count * 256);
        sb.AppendLine("===== MANAGER REPORT =====");
        sb.AppendLine($"Manager: {Name} (Id={Id})");
        sb.AppendLine($"Team size: {_team.Count}");
        sb.AppendLine();

        foreach (var employee in _team)
        {
            sb.Append(employee.GenerateReport());
        }

        sb.AppendLine("----- Manager Summary -----");
        sb.AppendLine($"Reviewed reports from all {_team.Count} team members.");
        sb.AppendLine();

        return sb.ToString();
    }
}

public static class Program
{
    public static void Main()
    {
        var dev1 = new Developer(101, "Arjun", "InventoryApp", modulesBuilt: 12, bugsFixed: 8);
        var dev2 = new Developer(102, "Neha", "InventoryApp", modulesBuilt: 9, bugsFixed: 6);
        var tester = new Tester(201, "Kabir", "InventoryApp", testCasesRun: 240, bugsReported: 25, bugsClosed: 18);

        var manager = new Manager(301, "Mr. Sharma");
        manager.AddToTeam(dev1);
        manager.AddToTeam(dev2);
        manager.AddToTeam(tester);

        Console.WriteLine("### Individual reports ###\n");
        Console.WriteLine(dev1.GenerateReport());
        Console.WriteLine(tester.GenerateReport());

        Console.WriteLine("### Manager's consolidated report ###\n");
        Console.WriteLine(manager.GenerateReport());
    }
}
