Assignment 25: Expense Sharing Application
EF Core + Code-First + SQLite (no server needed)
Covers:
-Users, Groups, Members (many-to-many)
-Expenses and ExpenseSplits
-Full CRUD operations
-Auto calculated settlement (who owes whom)

Steps to build (run inside this folder):
-dotnet restore
-dotnet ef migrations add InitialCreate
-dotnet ef database update
-dotnet run

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSharingApp
{
    // ENTITIES
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = "";

        [Required, StringLength(100)]
        public string Email { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public List<GroupMember> Memberships { get; set; } = new();
        public List<Expense>      PaidExpenses { get; set; } = new();
        public List<ExpenseSplit> Splits { get; set; } = new();

        public override string ToString() => $"{Name} <{Email}>";
    }

    public class Group
    {
        [Key]
        public int GroupId { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<GroupMember> Members  { get; set; } = new();
        public List<Expense>     Expenses { get; set; } = new();

        public override string ToString() => $"Group '{Name}' (Id={GroupId})";
    }

    // Join table for many-to-many between User and Group
    public class GroupMember
    {
        [Key]
        public int GroupMemberId { get; set; }

        [ForeignKey(nameof(Group))]
        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }

    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        [ForeignKey(nameof(Group))]
        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;

        [ForeignKey(nameof(User))]
        public int PaidByUserId { get; set; }
        public User PaidBy { get; set; } = null!;

        [Required, StringLength(120)]
        public string Description { get; set; } = "";

        [Column(TypeName = "decimal(12,2)")]
        public decimal Amount { get; set; }

        public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;

        public List<ExpenseSplit> Splits { get; set; } = new();
    }

    public class ExpenseSplit
    {
        [Key]
        public int SplitId { get; set; }

        [ForeignKey(nameof(Expense))]
        public int ExpenseId { get; set; }
        public Expense Expense { get; set; } = null!;

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Column(TypeName = "decimal(12,2)")]
        public decimal Share { get; set; }   // what this user owes for this expense
    }
  
    // DbContext
    public class AppDbContext : DbContext
    {
        public DbSet<User>          Users          { get; set; } = null!;
        public DbSet<Group>         Groups         { get; set; } = null!;
        public DbSet<GroupMember>   GroupMembers   { get; set; } = null!;
        public DbSet<Expense>       Expenses       { get; set; } = null!;
        public DbSet<ExpenseSplit>  ExpenseSplits  { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // SQLite — no server required, file created automatically
                optionsBuilder.UseSqlite("Data Source=expenses.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            // Unique: same user cannot be in same group twice
            b.Entity<GroupMember>()
                .HasIndex(gm => new { gm.GroupId, gm.UserId })
                .IsUnique();

            b.Entity<Expense>()
                .HasOne(e => e.PaidBy)
                .WithMany(u => u.PaidExpenses)
                .HasForeignKey(e => e.PaidByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    // Application class with all CRUD + settlement logic
    public class ExpenseApp
    {
        private readonly AppDbContext _db;

        public ExpenseApp(AppDbContext db) { _db = db; }

        // USER CRUD
        public User CreateUser(string name, string email)
        {
            var u = new User { Name = name, Email = email };
            _db.Users.Add(u);
            _db.SaveChanges();
            return u;
        }

        public List<User> GetAllUsers() =>
            _db.Users.OrderBy(u => u.Name).ToList();

        public User? GetUser(int id) => _db.Users.Find(id);

        public void UpdateUser(int id, string name, string email)
        {
            var u = _db.Users.Find(id);
            if (u == null) return;
            u.Name = name; u.Email = email;
            _db.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            var u = _db.Users.Find(id);
            if (u == null) return;
            _db.Users.Remove(u);
            _db.SaveChanges();
        }

        // GROUP CRUD
        public Group CreateGroup(string name)
        {
            var g = new Group { Name = name };
            _db.Groups.Add(g);
            _db.SaveChanges();
            return g;
        }

        public List<Group> GetAllGroups() =>
            _db.Groups
               .Include(g => g.Members).ThenInclude(m => m.User)
               .OrderBy(g => g.Name)
               .ToList();

        public void AddMember(int groupId, int userId)
        {
            if (_db.GroupMembers.Any(gm => gm.GroupId == groupId && gm.UserId == userId))
                return;
            _db.GroupMembers.Add(new GroupMember { GroupId = groupId, UserId = userId });
            _db.SaveChanges();
        }

        public void RemoveMember(int groupId, int userId)
        {
            var gm = _db.GroupMembers.FirstOrDefault(x => x.GroupId == groupId && x.UserId == userId);
            if (gm == null) return;
            _db.GroupMembers.Remove(gm);
            _db.SaveChanges();
        }

        // EXPENSE CRUD
        public Expense AddExpense(int groupId, int paidByUserId, string description,
                                  decimal amount, DateTime date)
        {
            var exp = new Expense
            {
                GroupId       = groupId,
                PaidByUserId  = paidByUserId,
                Description   = description,
                Amount        = amount,
                ExpenseDate   = date
            };
            _db.Expenses.Add(exp);
            _db.SaveChanges();

            // Split equally among all members (including payer)
            var memberIds = _db.GroupMembers
                                .Where(gm => gm.GroupId == groupId)
                                .Select(gm => gm.UserId)
                                .ToList();

            if (memberIds.Count == 0)
                throw new InvalidOperationException("Group has no members to split the expense.");

            decimal share = Math.Round(amount / memberIds.Count, 2);
            decimal sum   = share * memberIds.Count;
            // Adjust last share for rounding so total == amount
            decimal diff  = amount - sum;

            for (int i = 0; i < memberIds.Count; i++)
            {
                decimal s = share + (i == memberIds.Count - 1 ? diff : 0);
                _db.ExpenseSplits.Add(new ExpenseSplit
                {
                    ExpenseId = exp.ExpenseId,
                    UserId    = memberIds[i],
                    Share     = s
                });
            }
            _db.SaveChanges();
            return exp;
        }

        public List<Expense> GetExpensesForGroup(int groupId) =>
            _db.Expenses
               .Include(e => e.PaidBy)
               .Include(e => e.Splits).ThenInclude(s => s.User)
               .Where(e => e.GroupId == groupId)
               .OrderByDescending(e => e.ExpenseDate)
               .ToList();

        public void DeleteExpense(int expenseId)
        {
            var exp = _db.Expenses
                         .Include(e => e.Splits)
                         .FirstOrDefault(e => e.ExpenseId == expenseId);
            if (exp == null) return;
            _db.ExpenseSplits.RemoveRange(exp.Splits);
            _db.Expenses.Remove(exp);
            _db.SaveChanges();
        }

        // SETTLEMENT (who owes whom)
        public Dictionary<(int from, int to), decimal> CalculateSettlement(int groupId)
        {
            var expenses = _db.Expenses
                              .Include(e => e.Splits)
                              .Where(e => e.GroupId == groupId)
                              .ToList();

            // Net balance per user: positive => should receive, negative => should pay
            var balance = new Dictionary<int, decimal>();

            foreach (var e in expenses)
            {
                balance[e.PaidByUserId] =
                    balance.GetValueOrDefault(e.PaidByUserId) + e.Amount;
                foreach (var s in e.Splits)
                {
                    balance[s.UserId] =
                        balance.GetValueOrDefault(s.UserId) - s.Share;
                }
            }

            // Greedy settlement: debtors pay creditors
            var debtors   = balance.Where(kv => kv.Value < 0)
                                   .Select(kv => (id: kv.Key, amt: -kv.Value))
                                   .OrderByDescending(x => x.amt)
                                   .ToList();
            var creditors = balance.Where(kv => kv.Value > 0)
                                   .Select(kv => (id: kv.Key, amt: kv.Value))
                                   .OrderByDescending(x => x.amt)
                                   .ToList();

            var result = new Dictionary<(int, int), decimal>();
            int i = 0, j = 0;
            while (i < debtors.Count && j < creditors.Count)
            {
                decimal pay = Math.Min(debtors[i].amt, creditors[j].amt);
                result[(debtors[i].id, creditors[j].id)] = pay;
                debtors[i]   = (debtors[i].id,   debtors[i].amt   - pay);
                creditors[j] = (creditors[j].id, creditors[j].amt - pay);
                if (debtors[i].amt   < 0.01m) i++;
                if (creditors[j].amt < 0.01m) j++;
            }
            return result;
        }
    }

    // Program/UI
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Expense Sharing App (EF Core) ===\n");

            using var db = new AppDbContext();
            db.Database.EnsureCreated();   // ensures DB exists (alternative to migrations)

            var app = new ExpenseApp(db);
            RunDemo(app);
        }

        static void RunDemo(ExpenseApp app)
        {
            // Create users
            var arjun = app.CreateUser("Arjun", "arjun@example.com");
            var neha  = app.CreateUser("Neha",  "neha@example.com");
            var kabir = app.CreateUser("Kabir", "kabir@example.com");

            Console.WriteLine("Users created:");
            foreach (var u in app.GetAllUsers())
                Console.WriteLine("  - " + u);

            // Create a group
            var flat = app.CreateGroup("Flat 2024");
            app.AddMember(flat.GroupId, arjun.UserId);
            app.AddMember(flat.GroupId, neha.UserId);
            app.AddMember(flat.GroupId, kabir.UserId);

            Console.WriteLine("\nGroup created:");
            foreach (var g in app.GetAllGroups())
            {
                Console.WriteLine("  - " + g);
                foreach (var m in g.Members)
                    Console.WriteLine($"      member: {m.User.Name} (joined {m.JoinedAt:dd-MM-yyyy})");
            }

            // Add expenses
            app.AddExpense(flat.GroupId, arjun.UserId, "Rent",        3000m, DateTime.Today.AddDays(-5));
            app.AddExpense(flat.GroupId, neha.UserId,  "Groceries",    900m, DateTime.Today.AddDays(-3));
            app.AddExpense(flat.GroupId, kabir.UserId, "Electricity",  600m, DateTime.Today.AddDays(-1));

            Console.WriteLine("\nExpenses:");
            foreach (var e in app.GetExpensesForGroup(flat.GroupId))
            {
                Console.WriteLine($"  [{e.ExpenseDate:dd-MM}] {e.Description,-12} {e.Amount,8:C} paid by {e.PaidBy.Name}");
                foreach (var s in e.Splits)
                    Console.WriteLine($"           - {s.User.Name,-8} share: {s.Share,6:C}");
            }

            // Settlement report
            Console.WriteLine("\nSettlement (who owes whom):");
            var settlement = app.CalculateSettlement(flat.GroupId);
            if (settlement.Count == 0)
                Console.WriteLine("  All settled up!");
            else
            {
                var users = app.GetAllUsers().ToDictionary(u => u.UserId);
                foreach (var kv in settlement)
                    Console.WriteLine($"  {users[kv.Key.from].Name} owes {users[kv.Key.to].Name}: {kv.Value:C}");
            }
        }
    }
}
