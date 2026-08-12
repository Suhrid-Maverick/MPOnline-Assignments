Assignment 20: Print groups of anagrams from a given list

using System;
using System.Collections.Generic;
using System.Text;

namespace AnagramGroups;

public static class Program
{
    private static List<List<string>> GroupAnagrams(IEnumerable<string> words)
    {
        var groups = new Dictionary<string, List<string>>();

        foreach (string word in words)
        {
            // Normalise: lower‑case and sort characters
            char[] chars = word.ToLowerInvariant().ToCharArray();
            Array.Sort(chars);
            string key = new string(chars);

            if (!groups.TryGetValue(key, out List<string>? list))
            {
                list = new List<string>();
                groups[key] = list;
            }
            list.Add(word);
        }

        return new List<List<string>>(groups.Values);
    }

    private static void AppendGroups(StringBuilder sb, List<List<string>> groups, string? header = null)
    {
        if (header is not null)
            sb.AppendLine(header);

        int i = 1;
        foreach (var g in groups)
        {
            sb.AppendLine($"Group {i++}: [{string.Join(", ", g)}]");
        }
        sb.AppendLine();
    }

    public static void Main()
    {
        string[] words =
        {
            "listen", "silent", "enlist", "google", "gogole", "cat", "act", "tac",
            "rat", "tar", "art", "hello", "world"
        };

        var output = new StringBuilder(1024);

        var groups = GroupAnagrams(words);
        AppendGroups(output, groups, "--- Anagrams ---");

        Console.Write(output.ToString());

        // Interactive part
        Console.Write("\nEnter words separated by spaces: ");
        string? line = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(line))
        {
            var userWords = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var userGroups = GroupAnagrams(userWords);
            Console.WriteLine($"\nFound {userGroups.Count} anagram group(s):");
            foreach (var g in userGroups)
                Console.WriteLine($"  [{string.Join(", ", g)}]");
        }
    }
}
