using Utilities;

namespace MenuSystem;

public class Menu
{
    private string Title { get; set; }
    private EMenuType MenuType { get; set; }
    private bool ClearScene { get; set; }

    private readonly Option _exit = new Option("Exit", null);
    private readonly Option _back = new Option("Back", null);

    private readonly Dictionary<string, Option> _options = new Dictionary<string, Option>();

    public Menu(string title, List<Option> options, EMenuType menuType = EMenuType.Sub, bool clearScene = true)
    {
        Title = title;
        MenuType = menuType;
        ClearScene = clearScene;

        foreach (var option in options)
            _options.Add(option.Title, option);

        if (menuType == EMenuType.Choice)
            return;
        
        if (menuType != EMenuType.Main)
            _options.Add(_back.Title, _back);

        _options.Add(_exit.Title, _exit);
    }

    private string SelectOption(int selectedOption, int previousOption, int cursor)
    {
        // Deselect previously selected option
        Console.SetCursorPosition(0, cursor + previousOption);
        Console.WriteLine(_options.ElementAt(previousOption).Value.Title);

        // Set cursor on next option 
        Console.SetCursorPosition(0, cursor + selectedOption);
        
        // Color it
        Util.WriteColored(_options.ElementAt(selectedOption).Value.Title, ConsoleColor.Cyan);

        // return selected option's value
        return _options.ElementAt(selectedOption).Value.Title;
    }

    public string Run()
    {
        var reset = true;
        var selectedOption = 0;
        var previousOption = 0;
        var lastOption = 0;
        var cursor = Console.CursorTop;

        while (true)
        {
            if (reset)
            {
                // Clear console when necessary
                if (ClearScene)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.Clear();
                }

                selectedOption = 0;
                previousOption = 0;
                cursor = Console.CursorTop;
                reset = false;

                // Print title
                Util.WriteLineColored(Title, ConsoleColor.Green);
                Console.WriteLine("----------------------------------");
                
                // Jump cursor over 1st position since it's reserved for selected option
                Console.SetCursorPosition(0, Console.CursorTop + 1);
                
                // Print options
                for (var i = 0; i < _options.Count; i++)
                {
                    // Skip 1st option since it's added later on as selected option
                    if (i == selectedOption)
                    {
                        cursor = Console.CursorTop - 1;
                        continue;
                    }
                    
                    Console.WriteLine(_options.ElementAt(i).Value.Title);
                    
                    // Store cursor's last position in case we don't want to clear console after switching scene
                    lastOption = Console.CursorTop + 1;
                }
            }
            
            // Add/Change selected option, color it & store it's value
            var choice = SelectOption(selectedOption, previousOption, cursor);
            
            // Store previous option to deselect it
            previousOption = selectedOption;

            var key = Console.ReadKey().Key;

            switch (key)
            {
                case ConsoleKey.DownArrow:
                {
                    if (selectedOption + 1 < _options.Count)
                        selectedOption++;
                    else
                        selectedOption = 0;

                    break;
                }
                case ConsoleKey.UpArrow:
                {
                    if (selectedOption > 0)
                        selectedOption--;
                    else
                        selectedOption = _options.Count - 1;

                    break;
                }
                case ConsoleKey.Enter:
                {
                    // Reset menu when switching scene
                    Console.SetCursorPosition(0, lastOption);
                    reset = true;

                    string? subMenuReturnValue = null;

                    if (_options[choice].Method != null)
                        subMenuReturnValue = _options[choice].Method!();

                    if (subMenuReturnValue == _exit.Title)
                        return subMenuReturnValue;

                    if (MenuType == EMenuType.Choice || choice == _back.Title || choice == _exit.Title)
                        return choice;

                    break;
                }
            }
        }
    }
}