Assignment 14: Default values of C# types
int, bool, string, DateTime (and a few more for completeness)

using System;
using System.Text;

namespace DefaultValues;

public static class Program
{
    // Class‑level fields get default values automatically
    private static readonly int _defaultInt;
    private static readonly bool _defaultBool;
    private static readonly string _defaultString;   // null
    private static readonly DateTime _defaultDateTime;
    private static readonly double _defaultDouble;
    private static readonly decimal _defaultDecimal;
    private static readonly char _defaultChar;       // '\0'

    private static void AppendLine(StringBuilder sb, string format, params object[] args) =>
        sb.AppendLine(string.Format(format, args));

    public static void Main()
    {
        var sb = new StringBuilder(1024);

        sb.AppendLine("--- Default values of class-level fields ---");
        AppendLine(sb, "int       = {0}       (expected: 0)", _defaultInt);
        AppendLine(sb, "bool      = {0}      (expected: False)", _defaultBool);
        AppendLine(sb, "string    = {0}", _defaultString ?? "null");
        AppendLine(sb, "DateTime  = {0}  (expected: 01/01/0001 00:00:00)", _defaultDateTime);
        AppendLine(sb, "double    = {0}", _defaultDouble);
        AppendLine(sb, "decimal   = {0}", _defaultDecimal);
        AppendLine(sb, "char      = {0} (expected: 0 = '\\0')", (int)_defaultChar);

        sb.AppendLine("\n--- Using default(T) expression ---");
        AppendLine(sb, "default(int)      = {0}", default(int));
        AppendLine(sb, "default(bool)     = {0}", default(bool));
        AppendLine(sb, "default(string)   = {0}", default(string) ?? "null");
        AppendLine(sb, "default(DateTime) = {0}", default(DateTime));
        AppendLine(sb, "default(double)   = {0}", default(double));
        AppendLine(sb, "default(decimal)  = {0}", default(decimal));
        AppendLine(sb, "default(char)     = {0}", (int)default(char));
        AppendLine(sb, "default(Guid)     = {0}", default(Guid));

        sb.AppendLine("\n--- Local variables need explicit default ---");
        int localInt = default;
        bool localBool = default;
        string localString = default;
        DateTime localDt = default;
        AppendLine(sb, "Local int      = {0}", localInt);
        AppendLine(sb, "Local bool     = {0}", localBool);
        AppendLine(sb, "Local string   = {0}", localString ?? "null");
        AppendLine(sb, "Local DateTime = {0}", localDt);

        Console.Write(sb.ToString());
    }
}
