using CA1_Words.Properties;
using System.Globalization;
using Timer = System.Timers.Timer;

class Program
{
    static bool isRussian = true;
    static readonly HashSet<string> usedWords = new HashSet<string>();

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Language selection
        Console.WriteLine("Выберите язык / Choose language:");
        Console.WriteLine("1. Русский\n2. English (By default)");
        Console.Write("> ");
        string langChoice = Console.ReadLine();
        isRussian = langChoice == "1";

        if (isRussian)
        {
            Resources.Culture = new CultureInfo("ru-RU");
        }
        else
        {
            Resources.Culture = new CultureInfo("en-US");
        }

        Console.Clear();

        Console.WriteLine(Resources.GameName);
        Console.WriteLine(Resources.Rules);

        Console.Write("> ");

        string baseWord;
        while (true)
        {
            baseWord = Console.ReadLine()?.Trim().ToLower() ?? "";

            if (baseWord.Length >= 8 && baseWord.Length <= 30 && baseWord.All(char.IsLetter))
                break;
            else
                Console.WriteLine(Resources.LenEror);
        }


        Console.WriteLine($"{Resources.BaseWord} {baseWord}");

        int currentPlayer = 1;
        bool isGameRunning = true;

        while (isGameRunning)
        {
            Console.WriteLine($"{Resources.Player} {currentPlayer}{Resources.TimeWarning}");

            string currentWord = null;

            // 10 second timer
            using (Timer timer = new Timer(10000))
            {
                timer.Elapsed += (sender, args) =>
                {
                    timer.Stop();

                    Console.WriteLine($"{Resources.Player} {currentPlayer} {Resources.LoseByTime}");

                    Environment.Exit(0); // instant exit
                };
                timer.AutoReset = false;
                timer.Start();

                // Entering a word
                currentWord = ReadWord();

                timer.Stop();

                // Word check
                if (string.IsNullOrEmpty(currentWord) || !IsWordFromBase(currentWord, baseWord) || usedWords.Contains(currentWord))
                {
                    Console.WriteLine($"{Resources.Player} {currentPlayer} {Resources.LoseByWord}");

                    break; // Exit the game
                }

                usedWords.Add(currentWord);
                currentPlayer = currentPlayer == 1 ? 2 : 1; // Change of player
            }
        }

        Console.WriteLine(Resources.GameEnd);
    }

    // Function for entering a word character by character
    static string ReadWord()
    {
        string input = "";
        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    input += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }
            Thread.Sleep(10);
        }
        return input.Trim().ToLower();
    }

    // Checking if a word consists of the letters of a base word
    static bool IsWordFromBase(string currentWord, string baseWord)
    {
        var baseLetters = baseWord.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
        foreach (char c in currentWord)
        {
            if (!baseLetters.ContainsKey(c)) return false;
            baseLetters[c]--;
            if (baseLetters[c] < 0) return false;
        }
        return true;
    }
}
