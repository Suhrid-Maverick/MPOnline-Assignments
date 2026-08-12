# Assignments

My coding assignments — C# (.NET) and SQL. Each folder contains a self-contained, runnable solution with comments.

## Index

| # | Folder | Topic | Language |
|---|--------|-------|----------|
| 01 | `01. SQL Users`                          | Users table (Age > 18 CHECK) + 5 SELECT queries (avg, IN, EXISTS, correlated avg, overall avg) | SQL |
| 02 | `02. Drone System`                       | Drone surveillance system — encapsulation, state, battery/altitude private | C# |
| 03 | `03. Shopping App`                       | Customer + DeliveryAgent + Order + Payment (abstract User) | C# |
| 04 | `04. Employee Reports`                   | Developer / Tester / Manager polymorphic reports | C# |
| 05 | `05. Shopping Discounts`                 | Prime loyalty, Festival, Coupon — Strategy pattern | C# |
| 06 | `06. Exam Form Exception`                | Custom exception when exam form submitted after deadline | C# |
| 07 | `07. Cab Booking`                        | Cab booking with GPS service failure / invalid location exceptions | C# |
| 08 | `08. Notes Collection Framework`         | Short note on C# Collection Framework | Notes |
| 09 | `09. Notes Object Methods`               | Short note on Equals / ToString / GetHashCode | Notes |
| 10 | `10. Playlist Menu`                      | Menu-driven playlist with index + title | C# |
| 11 | `11. Age Sorting`                        | Age-based sorting of persons (IComparable + LINQ) | C# |
| 12 | `12. Employee Custom Sorting`            | Custom sorting by salary, joining date, employee ID (IComparer) | C# |
| 13 | `13. SQL Customer Product Order`         | Customer/Product/Order joins (INNER, LEFT, RIGHT) | SQL |
| 14 | `14. Default Values`                     | Default values of int / bool / string / DateTime / etc. | C# |
| 15 | `15. Nullable Int`                       | Nullable&lt;int&gt; demo: null, HasValue, GetValueOrDefault, comparisons | C# |
| 16 | `16. Password Encode Decode`             | Encode: shift +2 then reverse; Decode: reverse then shift -2 | C# |
| 17 | `17. Password Strength`                  | Length ≥ 8, capital, digit → Weak/Medium/Strong | C# |
| 18 | `18. Age From DOB`                       | Calculate age (years/months/days) from DOB | C# |
| 19 | `19. Hours To Days`                      | Convert hours → days (days = h / 24) | C# |
| 20 | `20. Anagram Groups`                     | Print groups of anagrams from a word list | C# |
| 21 | `21. Number Guessing`                    | Number guessing challenge vs computer (binary search) | C# |
| 22 | `22. SQL Sales`                          | Sales by Salesman & Category + monthly sales grouped by salesman | SQL |
| 23 | `23. SQL Shipments`                      | Shipment status, delivered today, avg transit, in-transit | SQL |
| 24 | `24. FizzBuzz Variant`                   | 1..50 with rules: 3 / 5 / 3-5 / Prime / number | C# |
| 25 | `25. Expense Sharing EF Core`             | Expense sharing app with EF Core (SQLite by default) | C# |
| 26 | `26. Homework Notes`                     | Notes: SDLC / EF / ORM / HTML / CSS / ER / DB design | Notes |

## How to Run

### C# Assignments

Each C# folder contains a single `Program.cs`. Pick one of these options:

**Option A — run with `dotnet` (recommended)**

```bash
cd 02_drone_system
dotnet new console -n temp -o . --force
# Program.cs will be picked up automatically
dotnet run
```

Or convert each folder into its own SDK-style project by adding a `.csproj` like the one in `25_expense_sharing_efcore`.

**Option B — .NET Fiddle / online**

Paste `Program.cs` contents into https://dotnetfiddle.net/ and click Run.

### SQL Assignments

Open in SSMS, Azure Data Studio, DBeaver, or any SQL client. Each `.sql` file is self-contained:
- Creates tables (drop them first if rerunning)
- Inserts sample data
- Runs all the required queries

Tested with SQL Server syntax (`IDENTITY`, `GETDATE()`, `DATEDIFF`, `DATENAME`). For MySQL/PostgreSQL, minor tweaks to `IDENTITY`/`GETDATE()` may be needed.

### EF Core Expense Sharing App (`25_expense_sharing_efcore`)

```bash
cd 25_expense_sharing_efcore
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Uses **SQLite** by default (file-based — no server required).

## Notes

- All code is original and written to demonstrate clean C# / SQL practices.
- Comments at the top of each file explain the assignment objective.
- The repository is **private** — only the owner can see it.

## Author

Suhrid Paul — `Suhrid-Maverick`
