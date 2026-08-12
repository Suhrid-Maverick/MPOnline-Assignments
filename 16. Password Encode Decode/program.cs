Assignment 16: Password Encode/Decode
Encode: shift each char by +2 in ASCII table, then reverse the string
Decode: reverse the process (reverse first, then shift by -2)

using System;
using System.Text;

namespace PasswordCodec;

public static class PasswordEncoder
{
    // Encode: shift each char by +2 then reverse the string
    public static string Encode(string plain)
    {
        if (plain is null) return null;

        char[] result = new char[plain.Length];
        for (int i = 0; i < plain.Length; i++)
            result[plain.Length - 1 - i] = (char)(plain[i] + 2);

        return new string(result);
    }

    // Decode: reverse the encoded string then shift each char by -2
    public static string Decode(string encoded)
    {
        if (encoded is null) return null;

        char[] result = new char[encoded.Length];
        for (int i = 0; i < encoded.Length; i++)
            result[i] = (char)(encoded[encoded.Length - 1 - i] - 2);

        return new string(result);
    }
}

public static class Program
{
    private static void AppendPasswordResult(StringBuilder sb, string original)
    {
        string enc = PasswordEncoder.Encode(original);
        string dec = PasswordEncoder.Decode(enc);
        sb.AppendLine($"Original : {original}");
        sb.AppendLine($"Encoded  : {enc}");
        sb.AppendLine($"Decoded  : {dec}");
        sb.AppendLine($"Round-trip OK : {original == dec}");
        sb.AppendLine(new string('-', 40));
    }

    public static void Main()
    {
        // Batch all test outputs
        var sb = new StringBuilder(1024);
        string[] passwords = { "Hello123", "Arjun@2004", "AbC", "z" };

        foreach (var pwd in passwords)
            AppendPasswordResult(sb, pwd);

        Console.Write(sb.ToString());

        // Interactive demo (needs immediate prompt)
        Console.Write("\nEnter a password to encode: ");
        string input = Console.ReadLine();
        if (!string.IsNullOrEmpty(input))
        {
            string encodedInput = PasswordEncoder.Encode(input);
            string decodedInput = PasswordEncoder.Decode(encodedInput);
            Console.WriteLine($"Encoded  : {encodedInput}");
            Console.WriteLine($"Decoded  : {decodedInput}");
        }
    }
}
