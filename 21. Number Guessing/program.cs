Assignment 21: Basic Number Guessing Challenge versus Computer

using System;
using System.Text;

namespace NumberGuessingGame;

public static class Program
{
    private static readonly Random Rnd = new();

    public static void Main()
    {
        Console.WriteLine("=== Number Guessing Challenge ===");

        while (true)
        {
            int target = Rnd.Next(1, 101);
            int attemptsLeft = 7;
            bool won = false;

            Console.WriteLine("\nI'm thinking of a number between 1 and 100.");
            Console.WriteLine($"You have {attemptsLeft} attempts.\n");

            while (attemptsLeft > 0)
            {
                Console.Write($"Attempts left {attemptsLeft}. Your guess: ");
                string? input = Console.ReadLine();
                if (!int.TryParse(input, out int guess) || guess < 1 || guess > 100)
                {
                    Console.WriteLine("  Please enter a valid integer between 1 and 100.");
                    continue;
                }

                attemptsLeft--;
                if (guess == target)
                {
                    Console.WriteLine($"  *** Correct! You guessed it in {7 - attemptsLeft} attempt(s). ***");
                    won = true;
                    break;
                }
                Console.WriteLine(guess < target ? "  Too low!" : "  Too high!");
            }

            if (!won)
                Console.WriteLine($"\nOut of attempts! The number was {target}.");

            // Computer's turn – output built once
            Console.WriteLine("\n--- Computer's turn ---");
            Console.Write(ComputerPlays());

            Console.Write("\nPlay again? (y/n): ");
            string? again = Console.ReadLine();
            if (again == null || (again.Length > 0 && again[0] != 'y' && again[0] != 'Y'))
                break;
        }

        Console.WriteLine("Thanks for playing!");
    }

    private static string ComputerPlays()
    {
        int secret = Rnd.Next(1, 101);
        int low = 1, high = 100, tries = 0;

        var sb = new StringBuilder(256);
        sb.AppendLine($"Computer is guessing a secret number in [1,100] (secret = {secret})");

        while (low <= high)
        {
            tries++;
            int guess = (low + high) / 2;
            sb.Append($"  Try {tries}: guess {guess} -> ");
            if (guess == secret)
            {
                sb.AppendLine("CORRECT!");
                break;
            }
            if (guess < secret)
            {
                sb.AppendLine("too low");
                low = guess + 1;
            }
            else
            {
                sb.AppendLine("too high");
                high = guess - 1;
            }
        }
        sb.AppendLine($"  Computer found it in {tries} tries (binary search).");
        return sb.ToString();
    }
}
