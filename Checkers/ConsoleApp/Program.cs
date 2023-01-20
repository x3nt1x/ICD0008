using System.Text.RegularExpressions;
using ConsoleUI;
using Domain;
using GameBrain;
using MenuSystem;
using Utilities;

Console.CursorVisible = false;

var game = new Game(null);

var repoType = "Database";

var playMenu = new Menu(
    ">  Play  <",
    new List<Option>()
    {
        new Option("New Game", NewGame),
        new Option("Load Save", LoadGame),
        new Option("Delete Save", DeleteSave)
    }
);

var optionsMenu = new Menu(
    ">  Options  <",
    new List<Option>()
    {
        new Option("Show Options", ShowOptions),
        new Option("Save Options", SaveOptions),
        new Option("Load Options", LoadOptions),
        new Option("Change Options", ChangeOptions),
        new Option("Delete Options", DeleteOptions),
        new Option("Persistence Engine", PersistenceEngine)
    }
);

var mainMenu = new Menu(
    @"      __           __
 ____/ /  ___ ____/ /_____ _______
/ __/ _ \/ -_) __/  '_/ -_) __(_-<
\__/_//_/\__/\__/_/\_\\__/_/ /___/",
    new List<Option>()
    {
        new Option("Play", playMenu.Run),
        new Option("Options", optionsMenu.Run)
    },
    EMenuType.Main
);

mainMenu.Run();

// --- Options Stuff --- \\

string ShowOptions()
{
    Console.Clear();

    Util.WriteLineColored(">  Current Options  <", ConsoleColor.Green);
    Console.WriteLine("----------------------------------");
    
    Console.WriteLine($"Width: {game.Options.Height}");
    Console.WriteLine($"Height: {game.Options.Width}");
    Console.WriteLine($"Multiplayer: {game.Options.Multiplayer}");
    Console.WriteLine($"Highlight moves: {game.Options.HighlightMoves}");
    Console.WriteLine($"Starting player: {game.Options.StartingPlayer}\n");

    Console.WriteLine($"Persistence Engine: {repoType}\n");

    Util.WriteLineColored("Press any key to return", ConsoleColor.Cyan);

    return Console.ReadKey().Key.ToString();
}

string SaveOptions()
{
    string? Check(string userInput)
    {
        if (userInput.Length > 32)
            return "Max options name length is 32 characters!";

        if (Util.OptionsRepo(repoType).OptionsExist(userInput))
            return $"Options named \"{userInput}\" already exist!";

        Util.OptionsRepo(repoType).SaveOptions(userInput, game.Options);

        return null;
    }

    Util.AskUserInput("Save Options", "Options name: ", nullCheck: false, customCheck: Check);

    return "-";
}

string LoadOptions()
{
    var loadMenu = new Menu(
        ">  Load Options  <",
        new List<Option>()
        {
            new Option("Search"),
            new Option("List All"),
            new Option("Back")
        },
        EMenuType.Choice
    );

    while (true)
    {
        var menu = loadMenu.Run();
        if (menu == "Back")
            return "-";

        var options = Util.OptionsRepo(repoType).ListOptions().Select(file => new Option(file!)).ToList();

        if (menu == "Search")
        {
            var search = Util.AskUserInput("Search", "Search options: ", nullCheck: false);

            if (!string.IsNullOrEmpty(search))
                options = options.Where(option => option.Title.Contains(search)).ToList();
        }

        if (!options.Any())
            continue;

        options.Add(new Option("Back"));

        var id = new Menu(">  Load Options  <", options, EMenuType.Choice).Run();
        if (id == "Back")
            continue;

        game.Options = Util.OptionsRepo(repoType).GetOptions(id);

        return "-";
    }
}

string ChangeOptions()
{
    var width = game.Options.Height;
    var height = game.Options.Width;
    var multiplayer = game.Options.Multiplayer;
    var highlightMoves = game.Options.HighlightMoves;
    var startingPlayer = game.Options.StartingPlayer;
    
    string? CheckWidth(string userInput)
    {
        if (!int.TryParse(userInput, out var size) || size < 4)
            return "Board width can only be a number greater than 3!";

        width = size;

        return null;
    }

    string? CheckHeight(string userInput)
    {
        if (!int.TryParse(userInput, out var size) || size < 4)
            return "Board height can only be a number greater than 3!";

        height = size;

        return null;
    }
    
    var multiplayerMenu = new Menu(
        ">  Multiplayer  <",
        new List<Option>()
        {
            new Option("Yes"),
            new Option("No")
        },
        EMenuType.Choice);

    var highlightMovesMenu = new Menu(
        ">  Highlight Moves  <",
        new List<Option>()
        {
            new Option("Yes"),
            new Option("No")
        },
        EMenuType.Choice);

    var startingPlayerMenu = new Menu(
        ">  Starting Player  <",
        new List<Option>()
        {
            new Option("Player 1"),
            new Option("Player 2")
        },
        EMenuType.Choice);
    
    var changeOptionsMenu = new Menu(
        ">  Change Options  <",
        new List<Option>()
        {
            new Option("Width"),
            new Option("Height"),
            new Option("Multiplayer"),
            new Option("Highlight Moves"),
            new Option("Starting Player"),
            new Option("Back")
        },
        EMenuType.Choice);

    while (true)
    {
        switch (changeOptionsMenu.Run())
        {
            case "Width":
                Util.AskUserInput("Width", "Board width: ", nullCheck: false, customCheck: CheckWidth);
                continue;
            case "Height":
                Util.AskUserInput("Height", "Board height: ", nullCheck: false, customCheck: CheckHeight);
                continue;
            case "Multiplayer":
                multiplayer = multiplayerMenu.Run() == "Yes";
                continue;
            case "Highlight Moves":
                highlightMoves = highlightMovesMenu.Run() == "Yes";
                continue;
            case "Starting Player":
                startingPlayer = startingPlayerMenu.Run() == "Player 2" ? EGamePiece.Player2 : EGamePiece.Player1;
                continue;
        }

        break;
    }

    game.Options = new GameOptions
    {
        Width = height,
        Height = width,
        Multiplayer = multiplayer,
        HighlightMoves = highlightMoves,
        StartingPlayer = startingPlayer
    };

    return "-";
}

string DeleteOptions()
{
    var options = Util.OptionsRepo(repoType).ListOptions().Select(file => new Option(file!)).ToList();
    if (!options.Any())
        return "-";
    
    var deleteOptionsMenu = new Menu(">  Delete Options  <", options, EMenuType.Choice);

    var id = deleteOptionsMenu.Run();
    Util.OptionsRepo(repoType).DeleteOptions(id);
    
    return "-";
}

string PersistenceEngine()
{
    repoType = new Menu(
        ">  Persistence Engine  <",
        new List<Option>()
        {
            new Option("Database"),
            new Option("Filesystem")
        },
        EMenuType.Choice).Run();

    return "-";
}

// --- Play Stuff --- \\

string NewGame()
{
    game.Options.Name = null;
    game = new Game(game.Options);

    return PlayGame(new CheckersBrain(game));
}

string LoadGame()
{
    var saves = Util.GameRepo(repoType).ListGames().Select(file => new Option(file!)).ToList();
    if (!saves.Any())
        return "-";
    
    var loadGameMenu = new Menu(">  Load Game  <", saves, EMenuType.Choice);
    
    var id = loadGameMenu.Run();
    game = Util.GameRepo(repoType).GetGame(id);
    
    return PlayGame(new CheckersBrain(game, false));
}

string DeleteSave()
{
    var saves = Util.GameRepo(repoType).ListGames().Select(file => new Option(file!)).ToList();
    if (!saves.Any())
        return "-";
    
    var deleteSaveMenu = new Menu(">  Delete Save  <", saves, EMenuType.Choice);

    var id = deleteSaveMenu.Run();
    Util.GameRepo(repoType).DeleteGame(id);
    
    return "-";
}

string PlayGame(CheckersBrain brain)
{
    var actionMenu = new Menu(
        ">  Action  <",
        new List<Option>()
        {
            new Option("Make Move"),
            new Option("Save Game"),
            new Option("Return To Menu")
        },
        EMenuType.Choice,
        false
    );
    
    var alternateMenu = new Menu(
        ">  Action  <",
        new List<Option>()
        {
            new Option("Save Game"),
            new Option("Return To Menu")
        },
        EMenuType.Choice,
        false
    );

    var exitMenu = new Menu(
        ">  Are you sure you want to exit? (Unsaved progress will be lost!)  <",
        new List<Option>()
        {
            new Option("Yes"),
            new Option("No")
        },
        EMenuType.Choice
    );

    string? save = null;

    while (true)
    {
        UI.DrawGameBoard(brain);
        
        if (brain.State.Winner != null)
        {
            Util.WriteLineColored($"{brain.State.Winner} won the game!\n", ConsoleColor.Green);
        }
        else if (brain.State.Turn == EGamePiece.Player2 && !brain.Options.Multiplayer)
        {
            brain.AiMove(brain.Piece?.X, brain.Piece?.Y);

            Thread.Sleep(1000);
            
            continue;
        }

        if (save != null)
        {
            Util.WriteLineColored($"Saved game: {save}\n", ConsoleColor.DarkGreen);
            save = null;
        }
        
        if (brain.Piece != null)
        {
            var regex = new Regex(@"\D;\d+");

            string? Check(string userInput)
            {
                UI.DrawGameBoard(brain);
                
                var moves = new List<(int x, int y)>();
                
                foreach (Match match in regex.Matches(userInput))
                {
                    var coordinates = match.Value.ToUpper().Split(";");

                    var x = Convert.ToInt32(coordinates[1]);
                    var y = Convert.ToChar(coordinates[0]) - 65;
                    
                    moves.Add((x, y));
                }

                if (!moves.Any())
                    return "Input must match format!";

                // Move returns string when there's problem with the move
                var error = brain.Move(moves);

                return !string.IsNullOrEmpty(error) ? error : null;
            }

            Util.AskUserInput(
                "Move Piece",
                "Move (format: A;0-B;1-C;2): ",
                null,
                false,
                false,
                Check);

            if (brain.Piece != null)
            {
                brain.EndMove();
                brain.Piece = null;
            }

            continue;
        }
        
        switch (brain.State.Winner != null ? alternateMenu.Run() : actionMenu.Run())
        {
            case "Make Move":
            {
                var regex = new Regex(@"^\D;\d+$");
                
                string? Check(string userInput)
                {
                    UI.DrawGameBoard(brain);

                    if (!regex.IsMatch(userInput))
                        return "Input must match format!";

                    var coordinates = userInput.ToUpper().Split(";");

                    var x = Convert.ToInt32(coordinates[1]);
                    var y = Convert.ToChar(coordinates[0]) - 65;

                    // SelectPiece returns string when there's problem with selecting piece
                    var error = brain.SelectPiece(x, y);

                    return !string.IsNullOrEmpty(error) ? error : null;
                }

                Util.AskUserInput(
                    "Make Move",
                    "Select piece (format: A;0): ",
                    null,
                    false,
                    false,
                    Check);

                break;
            }
            case "Save Game":
            {
                string? Check(string userInput)
                {
                    if (userInput.Length > 32)
                        return "Max game name length is 32 characters!";

                    if (userInput != game.Name)
                    {
                        if (Util.GameRepo(repoType).GameExist(userInput))
                            return $"Game named \"{userInput}\" already exist!";
                    }

                    save = userInput;
                    
                    Util.GameRepo(repoType).SaveGame(save, game);

                    return null;
                }

                Util.AskUserInput("Save Game", "Game name: ", nullCheck: false, customCheck: Check);
                
                break;
            }
            case "Return To Menu":
            {
                if (exitMenu.Run() == "Yes")
                    return "-";

                break;
            }
        }
    }
}