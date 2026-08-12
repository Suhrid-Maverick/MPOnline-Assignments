Assignment 15: Nullable<int> behaviour
-assign null
-compare with another nullable
-HasValue and GetValueOrDefault

using System;
using System.Text;

namespace NullableIntDemo;

public static class Program
{
    public static void Main()
    {
        var sb = new StringBuilder(1024);

        // Declare and assign null
        int? a = null;
        sb.AppendLine("a = null");
        sb.AppendLine($"  HasValue           = {a.HasValue}");
        sb.AppendLine($"  GetValueOrDefault()= {a.GetValueOrDefault()}");
        sb.AppendLine($"  GetValueOrDefault(99)= {a.GetValueOrDefault(99)}");
        // sb.AppendLine(a.Value); // would throw

        // Assign a real value
        int? b = 42;
        sb.AppendLine("\nb = 42");
        sb.AppendLine($"  HasValue = {b.HasValue}");
        sb.AppendLine($"  Value    = {b.Value}");

        // Compare two nullables
        int? c = null;
        int? d = 42;
        int? e = 42;

        sb.AppendLine("\n--- Comparisons (lifted operators) ---");
        sb.AppendLine($"  (c == null)         = {c == null}");   // True
        sb.AppendLine($"  (d == e)            = {d == e}");      // True
        sb.AppendLine($"  (c == d)            = {c == d}");      // False
        sb.AppendLine($"  (d > 10)            = {d > 10}");      // True
        sb.AppendLine($"  (c > 10)            = {c > 10}");      // False
        sb.AppendLine($"  (c >= d)            = {c >= d}");      // False

        // Use in arithmetic
        int? x = 10, y = null;
        int? sum = x + y;
        sb.AppendLine($"\n10 + null = {sum ?? -1} (sum.HasValue = {sum.HasValue})");

        // Coalescing operator - ??
        int actual = a ?? 0;
        sb.AppendLine($"a ?? 0  = {actual}");
        int fromB = b ?? 0;
        sb.AppendLine($"b ?? 0  = {fromB}");

        // GetValueOrDefault in expression
        int result = a.GetValueOrDefault() + b.GetValueOrDefault();
        sb.AppendLine($"\na.GetValueOrDefault() + b.GetValueOrDefault() = {result}");

        Console.Write(sb.ToString());
    }
}
