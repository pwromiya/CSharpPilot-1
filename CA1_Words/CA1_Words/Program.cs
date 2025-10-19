using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Timer = System.Timers.Timer;

class Program
{
    static bool isRussian = true;
    static readonly HashSet<string> usedWords = new HashSet<string>();

    static string lang(string ru, string en) => isRussian ? ru : en;

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Выбор языка
        Console.WriteLine("Выберите язык / Choose language:");
        Console.WriteLine("1. Русский\n2. English (By default)");
        Console.Write("> ");
        string langChoice = Console.ReadLine();
        isRussian = langChoice == "1";
        Console.Clear();

        Console.WriteLine(lang("=== Игра в Слова ===", "=== Word Game ==="));
        Console.WriteLine(lang("Введите начальное слово (8-30 букв)", "Enter a base word (8-30 letters)"));
        Console.Write("> ");

        string baseWord;
        while (true)
        {
            baseWord = Console.ReadLine()?.Trim().ToLower() ?? "";

            if (baseWord.Length >= 8 && baseWord.Length <= 30 && baseWord.All(char.IsLetter))
                break;
            else
                Console.WriteLine(lang("Недопустимая длина слова или используются недопустимые символы",
                                       "Invalid length or characters"));
        }

        Console.WriteLine(lang($"Базовое слово: {baseWord}", $"Base word: {baseWord}"));

        int currentPlayer = 1;
        bool isGameRunning = true;

        while (isGameRunning)
        {
            Console.WriteLine(lang(
                $"Игрок {currentPlayer}, у вас 10 секунд на ввод слова:",
                $"Player {currentPlayer}, you have 10 seconds to enter a word:"
            ));

            string currentWord = null;

            // Таймер на 10 секунд
            using (Timer timer = new Timer(10000))
            {
                timer.Elapsed += (sender, args) =>
                {
                    timer.Stop();
                    Console.WriteLine(lang(
                        $"Время вышло! Игрок {currentPlayer} проиграл.",
                        $"Time is up! Player {currentPlayer} loses."
                    ));
                    Environment.Exit(0); // мгновенный выход
                };
                timer.AutoReset = false;
                timer.Start();

                // Ввод слова
                currentWord = ReadWord();

                timer.Stop();

                // Проверка слова
                if (string.IsNullOrEmpty(currentWord) || !IsWordFromBase(currentWord, baseWord) || usedWords.Contains(currentWord))
                {
                    Console.WriteLine(lang(
                        $"Игрок {currentPlayer} ввел неверное слово. Проигрыш!",
                        $"Player {currentPlayer} entered an invalid word. You lose!"
                    ));
                    break; // выход из игры
                }

                usedWords.Add(currentWord);
                currentPlayer = currentPlayer == 1 ? 2 : 1; // смена игрока
            }
        }

        Console.WriteLine(lang("\nИгра окончена!", "\nGame over!"));
    }

    // Функция для посимвольного ввода слова
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

    // Проверка, состоит ли слово из букв базового слова
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
