Assignment 10: Menu-driven Playlist of Songs
-Add song (with auto index)
-Remove song by index
-Display playlist (index + title)
-Play next/previous

using System;
using System.Collections.Generic;
using System.Text;

namespace PlaylistApp;

public static class Program
{
    private static readonly List<string> _songs = new();
    private static int _currentIndex = -1;

    private const string Menu =
        "\n========= PLAYLIST MENU =========" +
        "\n 1. Add song" +
        "\n 2. Remove song by index" +
        "\n 3. Show playlist" +
        "\n 4. Play next" +
        "\n 5. Play previous" +
        "\n 6. Show currently playing" +
        "\n 7. Exit" +
        "\nChoice: ";

    public static void Main()
    {
        bool running = true;
        while (running)
        {
            Console.Write(Menu);
            string input = Console.ReadLine();

            switch (input)
            {
                case "1": AddSong(); break;
                case "2": RemoveSong(); break;
                case "3": ShowPlaylist(); break;
                case "4": PlayNext(); break;
                case "5": PlayPrevious(); break;
                case "6": ShowCurrent(); break;
                case "7": running = false; break;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }
    }

    private static void AddSong()
    {
        Console.Write("Enter song title: ");
        string title = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Title cannot be empty.");
            return;
        }
        _songs.Add(title);
        Console.WriteLine($"Added at index {_songs.Count - 1}: \"{title}\"");
    }

    private static void RemoveSong()
    {
        Console.Write("Enter index to remove: ");
        if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 0 && idx < _songs.Count)
        {
            Console.WriteLine($"Removed \"{_songs[idx]}\".");
            _songs.RemoveAt(idx);
            if (_currentIndex >= _songs.Count)
                _currentIndex = _songs.Count - 1;
        }
        else
            Console.WriteLine("Invalid index.");
    }

    private static void ShowPlaylist()
    {
        int count = _songs.Count;
        if (count == 0)
        {
            Console.WriteLine("(playlist is empty)");
            return;
        }

        // Build the entire output in one StringBuilder to reduce console I/O
        var sb = new StringBuilder();
        sb.AppendLine("\nIndex | Title");
        sb.AppendLine(new string('-', 30));

        for (int i = 0; i < count; i++)
        {
            string marker = (i == _currentIndex) ? " > " : "   ";
            sb.AppendLine($"{marker}{i,-5}| {_songs[i]}");
        }

        Console.Write(sb.ToString());
    }

    private static void PlayNext()
    {
        if (_songs.Count == 0)
        {
            Console.WriteLine("Playlist empty.");
            return;
        }
        _currentIndex = (_currentIndex + 1) % _songs.Count;
        Console.WriteLine($"Now playing [#{_currentIndex}]: \"{_songs[_currentIndex]}\"");
    }

    private static void PlayPrevious()
    {
        if (_songs.Count == 0)
        {
            Console.WriteLine("Playlist empty.");
            return;
        }
        _currentIndex = _currentIndex <= 0 ? _songs.Count - 1 : _currentIndex - 1;
        Console.WriteLine($"Now playing [#{_currentIndex}]: \"{_songs[_currentIndex]}\"");
    }

    private static void ShowCurrent()
    {
        if (_currentIndex < 0 || _currentIndex >= _songs.Count)
            Console.WriteLine("No song is currently playing.");
        else
            Console.WriteLine($"Now playing [#{_currentIndex}]: \"{_songs[_currentIndex]}\"");
    }
}
