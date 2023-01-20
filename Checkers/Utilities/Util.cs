using DAL;
using DAL.Database;
using DAL.FileSystem;

namespace Utilities;

public static class Util
{
    public static void WriteColored(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
    }

    public static void WriteLineColored(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
    
    public static string? AskUserInput(string title, string askText, string? errorText = null,
        bool clearConsole = true, bool nullCheck = true, Func<string, string?>? customCheck = null)
    {
        if (clearConsole)
            Console.Clear();

        while (true)
        {
            WriteLineColored($">  {title}  <", ConsoleColor.Green);
            Console.WriteLine("----------------------------------");
            
            Console.Write(askText);
            var userInput = Console.ReadLine()?.Trim()!;

            Console.Clear();

            if (!nullCheck && string.IsNullOrEmpty(userInput))
                return null;

            if (customCheck != null)
            {
                errorText = customCheck(userInput);

                if (string.IsNullOrEmpty(errorText))
                    return userInput;
            }

            if (customCheck == null && !string.IsNullOrEmpty(userInput))
                return userInput;

            WriteLineColored($"{errorText}\n", ConsoleColor.Red);
        }
    }

    public static IGameRepository GameRepo(string repoType)
    {
        if (repoType == "Filesystem")
            return new GameRepositoryFileSystem();
        
        return new GameRepositoryDatabase();
    }
    
    public static IGameOptionsRepository OptionsRepo(string repoType)
    {
        if (repoType == "Filesystem")
            return new GameOptionsRepositoryFileSystem();
        
        return new GameOptionsRepositoryDatabase();
    }
}