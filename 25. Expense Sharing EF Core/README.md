# Assignment 25 — Expense Sharing App (EF Core + MVC) — Comprehensive Notes

This document provides an in‑depth walkthrough of the **Expense Sharing Application**, a .NET solution that demonstrates the practical use of **Entity Framework Core** with a Code‑First approach. The application manages users, groups, group memberships, expenses, and expense splits, and automatically computes a settlement report to show who owes whom. Although the sample implementation is a console application, the concepts are directly transferable to an **ASP.NET Core MVC** web application.

---

## 1. Introduction to the Expense Sharing App

The **Expense Sharing App** is a lightweight system designed to help groups of people track shared costs and settle debts. Typical use cases include flatmates splitting rent and utilities, friends sharing dinner bills, or team members dividing project expenses. The app models the following domain:

- **User** – a person who participates in the system.
- **Group** – a collection of users who share expenses together (e.g., "Flat 2024").
- **GroupMember** – a join entity that links users to groups (many‑to‑many).
- **Expense** – a cost incurred by a group, paid by a single user (the payer).
- **ExpenseSplit** – how the expense is divided among the group members (each member's share).

By storing this information in a relational database, the application can answer important questions:
- How much has each member spent in total?
- For a given expense, what is each member’s share?
- Who owes whom, and how much, after all expenses are settled?

The application uses **Entity Framework Core** with a SQLite database (by default) and demonstrates key EF Core features: entity classes, `DbContext`, `DbSet<T>` properties, relationships (one‑to‑many, many‑to‑many via join table), migrations, and LINQ queries.

---

## 2. Project Setup and Dependencies

The project is structured as a .NET console application using the SDK‑style project file. It references the following NuGet packages (as defined in the `.csproj` file):

| Package | Purpose |
|---------|---------|
| `Microsoft.EntityFrameworkCore` | Core ORM functionality. |
| `Microsoft.EntityFrameworkCore.Sqlite` | Database provider for SQLite (lightweight, serverless). |
| `Microsoft.EntityFrameworkCore.Tools` | Provides `dotnet ef` commands for migrations and scaffolding. |
| `Microsoft.EntityFrameworkCore.Design` | Needed for design‑time migrations (referenced by tools). |

To switch to SQL Server, you would replace the SQLite provider with `Microsoft.EntityFrameworkCore.SqlServer` and adjust the connection string.

**Directory Structure:**

```
25_expense_sharing_efcore/
├── ExpenseSharingApp.csproj   ← project file (SDK-style)
├── Program.cs                 ← menu-driven UI + CRUD demo
├── Models/                    ← (optional) entity classes
├── Data/                      ← (optional) DbContext and factories
└── README.md                  ← project documentation
```

For better organisation, entity classes and the `DbContext` are typically placed in separate folders. The provided implementation may keep everything in `Program.cs` for simplicity, but in a real project you would separate concerns.

---

## 3. Database Design: Entities and Relationships

The core of the application is the **domain model**, which maps to the database tables. The following table summarises the main entities:

| Entity | Description | Key Properties |
|--------|-------------|----------------|
| **User** | A person in the system. | `UserId` (PK), `Name`, `Email` |
| **Group** | A collection of users who share expenses. | `GroupId` (PK), `Name`, `CreatedAt` |
| **GroupMember** | Junction table for many‑to‑many between User and Group. | `UserId` (FK), `GroupId` (FK) – composite PK. May include `JoinedAt` and `Role`. |
| **Expense** | A cost incurred by a group, paid by one user. | `ExpenseId` (PK), `Description`, `Amount`, `PaidByUserId` (FK), `GroupId` (FK), `Date` |
| **ExpenseSplit** | How a specific expense is divided among users. | `ExpenseSplitId` (PK), `ExpenseId` (FK), `UserId` (FK), `ShareAmount` |

### Relationships

| Relationship | Type | Details |
|--------------|------|---------|
| **User → Group** | Many‑to‑Many | Through `GroupMember` table. A user can be in many groups; a group has many users. |
| **User → Expense** | One‑to‑Many | A user (as payer) can have many expenses. An expense is paid by exactly one user. |
| **Group → Expense** | One‑to‑Many | A group can have many expenses. An expense belongs to exactly one group. |
| **Expense → ExpenseSplit** | One‑to‑Many | An expense can have many splits (one per member who owes a share). A split belongs to exactly one expense. |
| **User → ExpenseSplit** | One‑to‑Many | A user can have many splits (they owe money for various expenses). A split is associated with exactly one user (the debtor). |

### Entity Class Examples (C#)

```csharp
public class User
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    // Navigation properties
    public ICollection<GroupMember> GroupMemberships { get; set; }
    public ICollection<Expense> ExpensesPaid { get; set; }
    public ICollection<ExpenseSplit> ExpenseSplits { get; set; }
}

public class Group
{
    public int GroupId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<GroupMember> Members { get; set; }
    public ICollection<Expense> Expenses { get; set; }
}

public class GroupMember
{
    public int UserId { get; set; }
    public int GroupId { get; set; }
    public DateTime JoinedAt { get; set; } // optional

    // Navigation properties
    public User User { get; set; }
    public Group Group { get; set; }
}

public class Expense
{
    public int ExpenseId { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }

    // Foreign keys
    public int PaidByUserId { get; set; }
    public int GroupId { get; set; }

    // Navigation
    public User PaidByUser { get; set; }
    public Group Group { get; set; }
    public ICollection<ExpenseSplit> Splits { get; set; }
}

public class ExpenseSplit
{
    public int ExpenseSplitId { get; set; }
    public int ExpenseId { get; set; }
    public int UserId { get; set; } // the member who owes this share
    public decimal ShareAmount { get; set; }

    public Expense Expense { get; set; }
    public User User { get; set; }
}
```

### Fluent API Configuration (in `DbContext.OnModelCreating`)

For the many‑to‑many relationship, we configure the composite key for `GroupMember`:

```csharp
modelBuilder.Entity<GroupMember>()
    .HasKey(gm => new { gm.UserId, gm.GroupId });

modelBuilder.Entity<GroupMember>()
    .HasOne(gm => gm.User)
    .WithMany(u => u.GroupMemberships)
    .HasForeignKey(gm => gm.UserId);

modelBuilder.Entity<GroupMember>()
    .HasOne(gm => gm.Group)
    .WithMany(g => g.Members)
    .HasForeignKey(gm => gm.GroupId);
```

Similar configurations can be added for the other relationships to define cascade delete behaviour and precision for decimal amounts (e.g., `HasPrecision(18,2)`).

---

## 4. Code‑First Migrations

The application uses the **Code‑First** approach: we define the entity classes and `DbContext`, and then generate the database schema using **migrations**.

### Step‑by‑Step Migration Process

1. **Create the DbContext** – a class that derives from `DbContext` and includes `DbSet<T>` properties for each entity. Override `OnConfiguring` to set the connection string (or use `AddDbContext` in an ASP.NET Core host).

2. **Install EF Core Tools** (if not already):
   ```
   dotnet tool install --global dotnet-ef
   ```

3. **Add a migration** – This command introspects the current model, compares it with the previous migration (if any), and generates a C# migration class containing `Up` and `Down` methods.
   ```
   dotnet ef migrations add InitialCreate
   ```

4. **Update the database** – Executes the `Up` method to apply the schema changes.
   ```
   dotnet ef database update
   ```

After these steps, the database is created (if it does not exist) with all tables and relationships defined by the model. Subsequent changes to the model (e.g., adding a new property) require creating a new migration and updating the database again.

**Connection String** – By default, the app uses SQLite with `Data Source=expenses.db`. The database file is created in the project’s output folder. For SQL Server, you would change the provider and connection string (see Section 7).

---

## 5. CRUD Operations

The application demonstrates the basic **Create, Read, Update, Delete** operations using EF Core.

### Create a User
```csharp
using (var context = new AppDbContext())
{
    var user = new User { Name = "Arjun", Email = "arjun@example.com" };
    context.Users.Add(user);
    context.SaveChanges();
}
```

### Read Users with Expenses
```csharp
var users = context.Users
    .Include(u => u.ExpensesPaid)
    .ThenInclude(e => e.Splits)
    .ToList();
```

### Update an Expense
```csharp
var expense = context.Expenses.Find(1);
expense.Amount = 3200;
context.SaveChanges();
```

### Delete a Group and all related Expenses (Cascade)
Because of configured cascading delete, removing a group will automatically remove its expenses and their splits.

```csharp
var group = context.Groups.Include(g => g.Expenses).First(g => g.GroupId == 1);
context.Groups.Remove(group);
context.SaveChanges();
```

### Creating an Expense with Splits
When a new expense is added, we must also insert its splits (one for each member who shares the cost). The typical algorithm is:
- Determine the group members.
- Calculate each member's share (e.g., equal division, or custom percentages).
- Create an `ExpenseSplit` record for each member (including the payer, who may owe themselves zero if they paid, but often we include them with a share to track net balance).

Example:
```csharp
var expense = new Expense
{
    Description = "Rent",
    Amount = 3000,
    PaidByUserId = arjun.UserId,
    GroupId = flatGroup.GroupId,
    Date = DateTime.Now,
    Splits = new List<ExpenseSplit>()
};

foreach (var member in groupMembers)
{
    expense.Splits.Add(new ExpenseSplit
    {
        UserId = member.UserId,
        ShareAmount = 3000 / groupMembers.Count // equal split
    });
}

context.Expenses.Add(expense);
context.SaveChanges();
```

---

## 6. Settlement Report – "Who Owes Whom"

The standout feature of this application is the **settlement report**, which calculates the net balance for each member in a group and then derives a list of debts.

### Algorithm Overview

1. **Compute total paid and total owed for each user in the group.**
   - `TotalPaid` = sum of all expenses where the user is the payer.
   - `TotalOwed` = sum of all `ShareAmount`s in expense splits where the user is the debtor.

2. **Calculate net balance** for each user: `Net = TotalPaid - TotalOwed`.
   - Positive net means the user is owed money (they paid more than they owe).
   - Negative net means the user owes money.

3. **Settle the debts** by pairing creditors (positive net) and debtors (negative net) until all balances reach zero.
   - Use a greedy approach: take the largest creditor and the largest debtor, transfer the minimum of their absolute balances, and reduce both.
   - Output each transfer as a debt: "User A owes User B X amount".

### Example Output
For the demo data:
- Arjun paid ₹3000 (Rent) – split equally among 3 → each share ₹1000.
- Neha paid ₹900 (Groceries) – each share ₹300.
- Kabir paid ₹600 (Electricity) – each share ₹200.

Compute totals:
- Arjun: Paid 3000, Owed 1000+300+200=1500 → Net +1500 (creditor).
- Neha: Paid 900, Owed 1000+300+200=1500 → Net -600 (debtor, owes 600).
- Kabir: Paid 600, Owed 1500 → Net -900 (debtor, owes 900).

The settlement pairs Arjun (creditor) with Neha (debtor) and Kabir (debtor):
- Kabir owes Arjun 900.
- Neha owes Arjun 600.

This report can be presented as a list of tuples: `(DebtorName, CreditorName, Amount)`.

### Implementation in LINQ
```csharp
var balances = groupMembers
    .Select(m => new {
        User = m.User,
        Net = m.User.ExpensesPaid.Sum(e => e.Amount)
              - m.User.ExpenseSplits.Sum(s => s.ShareAmount)
    })
    .ToList();

var creditors = balances.Where(b => b.Net > 0).OrderByDescending(b => b.Net).ToList();
var debtors = balances.Where(b => b.Net < 0).OrderBy(b => b.Net).ToList();

var settlements = new List<Settlement>();
int i = 0, j = 0;
while (i < creditors.Count && j < debtors.Count)
{
    var credit = creditors[i];
    var debt = debtors[j];
    decimal amount = Math.Min(credit.Net, -debt.Net);

    settlements.Add(new Settlement(debt.User.Name, credit.User.Name, amount));

    creditors[i] = new { User = credit.User, Net = credit.Net - amount };
    debtors[j] = new { User = debt.User, Net = debt.Net + amount };

    if (creditors[i].Net == 0) i++;
    if (debtors[j].Net == 0) j++;
}
```

This settlement algorithm is efficient and yields a minimal number of transactions (at most n‑1, where n is the number of members).

---

## 7. Switching to SQL Server

The application uses SQLite by default for its simplicity (no server installation required). In a production environment, you might prefer SQL Server (or PostgreSQL, MySQL). To switch, follow these steps:

1. **Replace the provider package** – In the `.csproj`, remove `Microsoft.EntityFrameworkCore.Sqlite` and add `Microsoft.EntityFrameworkCore.SqlServer`.

2. **Update the connection string** – In the `DbContext` (or in a factory), replace:
   ```csharp
   optionsBuilder.UseSqlite("Data Source=expenses.db");
   ```
   with
   ```csharp
   optionsBuilder.UseSqlServer(
       "Server=localhost;Database=ExpenseDb;Trusted_Connection=True;TrustServerCertificate=True;");
   ```

3. **Create a new migration** – Since the provider changes, it’s best to start fresh: delete the existing `Migrations` folder and run `dotnet ef migrations add InitialCreate` again.

4. **Apply the migration** – Use `dotnet ef database update` to create the database on SQL Server.

The rest of the code remains unchanged, demonstrating the **database‑agnostic** nature of EF Core.

---

## 8. Extending to MVC (Web Application)

Although the current implementation is a console application, the same domain model and business logic can be exposed via an **ASP.NET Core MVC** frontend. The steps would be:

- Create an ASP.NET Core MVC project.
- Add the same entity classes and `DbContext` (or reference them from a class library).
- Configure the `DbContext` in `Program.cs` using `AddDbContext<T>` and the connection string from `appsettings.json`.
- Create **Controllers** (e.g., `ExpensesController`, `GroupsController`) that use the `DbContext` to perform CRUD operations.
- Use **ViewModels** to shape data for views (e.g., a settlement report view that displays the list of debts).
- Leverage Razor Views to render HTML, with forms for adding expenses and groups.
- Implement the settlement algorithm as a service method that can be called by a controller and passed to a view.

This separation of concerns aligns with the MVC pattern:
- **Model** – the EF Core entities and business logic (settlement service).
- **View** – Razor pages that display data and forms.
- **Controller** – handles HTTP requests, interacts with the model, and returns views.

The console application serves as a solid foundation that can be easily adapted to a full‑featured web application.

---

## 9. Testing and Running the Application

To test the application locally:

1. Clone the repository or download the source.
2. Navigate to the project folder.
3. Restore dependencies:
   ```
   dotnet restore
   ```
4. Ensure the EF Core tools are installed globally (or use `dotnet ef` from the project’s tools).
5. Create the database and apply migrations:
   ```
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
6. Run the application:
   ```
   dotnet run
   ```
   The console will execute the demo: create sample data, insert expenses, and display the settlement report.

The application is self‑contained and does not require any external database server when using SQLite.

---

## 10. Conclusion

The **Expense Sharing App** is a practical illustration of how **Entity Framework Core** simplifies data access in .NET applications. Through the Code‑First approach, developers can focus on the domain model while EF Core handles the database schema generation and object‑relational mapping. Key takeaways include:

- **Entity modelling** – defining classes with relationships (one‑to‑many, many‑to‑many).
- **DbSet and DbContext** – the core building blocks for querying and persisting data.
- **Migrations** – enabling version‑controlled schema evolution.
- **CRUD operations** – using `Add`, `Update`, `Remove`, and `SaveChanges`.
- **Complex queries** – using LINQ and navigation properties to compute balances.
- **Business logic** – implementing a settlement algorithm that reduces debts to minimal transactions.

This application can serve as a template for more sophisticated expense‑tracking systems, and its principles are directly applicable to other domains such as inventory management, order processing, and financial record‑keeping. With the foundation laid here, you can confidently extend the app to include authentication, user‑specific groups, custom split ratios, and real‑time updates in a web or mobile environment.
