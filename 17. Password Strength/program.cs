Assignment 17: Password Strength Checker
Rules:
1. Length should be exactly 8 (treated as min length 8 here)
2. Must have at least one capital letter
3. One digit is compulsory
Returns: Weak/Medium/Strong based on how many rules pass

using System;
using System.Text;

namespace PasswordStrength;

public readonly struct PasswordResult
{
    public string Strength { get; }
    public bool LengthOk { get; }
    public bool HasUpper { get; }
    public bool HasDigit { get; }

    public PasswordResult(string strength, bool lengthOk, bool hasUpper, bool hasDigit) =>
        (Strength, LengthOk, HasUpper, HasDigit) = (strength, lengthOk, hasUpper, hasDigit);
}

public static class PasswordChecker
{
    public static PasswordResult Check(string pwd)
    {
        if (pwd is null) pwd = string.Empty;

        int length = pwd.Length;
        bool lengthOk = length >= 8;
        bool hasUpper = false;
        bool hasDigit = false;

        for (int i = 0; i < length; i++)
        {
            char c = pwd[i];
            if (!hasUpper && char.IsUpper(c)) hasUpper = true;
            if (!hasDigit && char.IsDigit(c)) hasDigit = true;
            if (hasUpper && hasDigit) break; // early exit
        }

        int score = (lengthOk ? 1 : 0) + (hasUpper ? 1 : 0) + (hasDigit ? 1 : 0);
        string strength = score switch
        {
            3 => "Strong",
            2 => "Medium",
            _ => "Weak"
        };

        return new PasswordResult(strength, lengthOk, hasUpper, hasDigit);
    }
}

public static class Program
{
    private static void AppendResult(StringBuilder sb, string pwd)
    {
        var r = PasswordChecker.Check(pwd);
        sb.AppendLine($"Password : {pwd}");
        sb.AppendLine($"  Length>=8 : {r.LengthOk}");
        sb.AppendLine($"  Has upper : {r.HasUpper}");
        sb.AppendLine($"  Has digit : {r.HasDigit}");
        sb.AppendLine($"  Strength  : {r.Strength}\n");
    }

    public static void Main()
    {
        var samples = new[] { "abc", "abcdefgh", "Abcdefgh", "Abcdefg1", "Password1", "short1A" };

        var sb = new StringBuilder(512);
        foreach (var pwd in samples)
            AppendResult(sb, pwd);

        Console.Write(sb.ToString());

        Console.Write("Enter your own password to check: ");
        string input = Console.ReadLine() ?? "";
        var res = PasswordChecker.Check(input);
        Console.WriteLine($"Strength: {res.Strength} " +
                          $"(len={res.LengthOk}, upper={res.HasUpper}, digit={res.HasDigit})");
    }
}
