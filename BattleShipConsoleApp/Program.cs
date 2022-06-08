
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using BattleShipGameBrain;
using BattleShipConsoleUI;
using BattleShipGameBrain.Interfaces;
using ColorString;
using DAL;
using Domain;
using Domain.Enums;
using GameSaveLoad;
using MenuSystem;
namespace BattleShipConsoleApp
{
    internal static class Program
    {
        private static BattleshipBrain? _brain;
        
       

        private static void Main(string[] args)
        {
            CheckConsoleSettings();

            var loadGameMenu = new Menu("Load Game", EMenuLevel.First);
            loadGameMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "Load from local computer", JsonSavesMenu, true),
                new MenuItem("2", "Load from database", DataBaseSavesMenu)
            
            });
            
            var newGameMenu = new Menu("New Game", EMenuLevel.First);
            newGameMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "Standard game", () => RunConfigurationGame(new StandardGameConfig()), true),
                new MenuItem("2", "Small game", () => RunConfigurationGame(new SmallGameConfig())),
                new MenuItem("3", "Custom Rules", RunCustomGameMenu)
            });

            var mainMenu = new Menu("Battleships main menu", EMenuLevel.Root);
            mainMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "Start new game", newGameMenu.Run),
                new MenuItem("2", "Load Game", loadGameMenu.Run),
            });

            mainMenu.Run();
        }

        private static void CheckConsoleSettings()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ConsoleSettings.ShowWindow(ConsoleSettings.GetConsoleWindow(), 3);
            }
            else
            {
                ColoredString.WriteLineString("Please maximize your console window for better experience", ConsoleColor.Red);
                ColoredString.WriteLineString("Press any key to continue...", ConsoleColor.Magenta);
                Console.ReadKey();
            }
        }

        private static string JsonSavesMenu()
        {
            var savedGamesMenu = new Menu("JSON saves", EMenuLevel.SecondOrMore);
            
            var separator = Path.DirectorySeparatorChar;

            var filePath = Directory.GetCurrentDirectory() + separator + "JSONSaves" + separator;
            
            string[] files = Directory.GetFiles(filePath);
            
            for (var i = files.Length - 1; i >= 0; i--)
            {
                var name = Path.GetFileName(files[i]);
                savedGamesMenu.AddMenuItem(new MenuItem((i + 1).ToString(), name, () => SetJsonGame(files,name)));

            }
            savedGamesMenu.InitializeMenu();
            
            var result = savedGamesMenu.Run();
            
            return result;
        }

        private static string DataBaseSavesMenu()
        {
            using var dbContext = new ApplicationDbContext();

            var games = dbContext.Games.Where(game => game.PredefinedGame == false 
                                                      && game.GameName != "initial custom" 
                                                      && game.GameName !=  "Small" 
                                                      && game.GameName != "Classic").ToList();
         

            var savedGamesMenu = new Menu("Database saves", EMenuLevel.SecondOrMore);

            var menuItems = new List<MenuItem>();
            
            for (var i = 0; i < games.Count; i++)
            {
                var index = i;
                menuItems.Add(new MenuItem((index + 1).ToString(), games[index].GameName, 
                    () => SetDatabaseGame(games[index].GameId)));
            }
            
            savedGamesMenu.AddMenuItems(menuItems);
            
            savedGamesMenu.InitializeMenu();

            var result = savedGamesMenu.Run();

            return result;
        }

        private static void SetDatabaseGame(int gameId)
        {
            _brain = new BattleshipBrain();
            var game = SaveLoad.GetGame(gameId);
            SaveLoad.SetDatabaseGame(game, _brain);
            RunGameMenu();
        }

        private static string SetJsonGame(string[] files, string fileName)
        {
            string? file = null;
            foreach (var save in files)
            {
                if (Path.GetFileName(save) == fileName)
                {
                    file = File.ReadAllText(save);
                    break;
                }
            }

            var indented = new JsonSerializerOptions() { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve};
            var game = JsonSerializer.Deserialize<Game>(file!, indented );

            _brain = new BattleshipBrain();

            SaveLoad.SetJsonGame(game!, _brain); 
           

            RunGameMenu();
            
            return EReturn.M.ToString();
        }

        private static string RunSaveMenu()
        {
            var saveMenu = new Menu("Saving menu", EMenuLevel.CustomReturnOnly);
            
            saveMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "Save to Local machine", SavingJson, true),
                new MenuItem("2", "Save to database", SaveGameDatabase)
            });

            var result = saveMenu.Run();
            return result;
        }

        private static void SaveGameDatabase()
        {
            if (_brain!.Game is not null && _brain.Game!.InDatabase && _brain.Game!.PredefinedGame == false)
            {
              SaveLoad.SaveGameToDb(_brain.Game!.GameName, _brain);
            }
            else
            {
                var gameName = CustomRules.AskString("Enter save name: ");
                SaveLoad.SaveGameToDb(gameName, _brain);
            }
        }

        private static void SavingJson()
        {
            if (_brain!.Game is null)
            {
                NewSave();
            }
            else if (_brain.Game!.SaveType is ESaveType.Json)
            {
                RunAlreadySavedMenu();
            }
            else
            {
                NewSave();
            }
        }

        private static void NewSave()
        {
            var gameName = CustomRules.AskString("Enter save name: ");
            
            SaveLoad.SaveGameJson(gameName, _brain!);
        }

        private static void RunAlreadySavedMenu()
        {
            var alreadySavedMenu = new Menu("Already saved", EMenuLevel.CustomReturnOnly);
            
            alreadySavedMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "Save to current", SaveToCurrent, true),
                new MenuItem("2", "New save", NewSave)
            });
            
            alreadySavedMenu.Run();

        }

        private static void SaveToCurrent()
        {
            SaveLoad.SaveGameJson(_brain!.Game!.GameName, _brain);
        }

        private static string RunCustomGameMenu()
        {
            _brain = new BattleshipBrain();
            var customRulesMenu = new Menu("Custom Rules", EMenuLevel.SecondOrMore);
            customRulesMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "Opponent (Pc or Human)", () => CustomGameMenu.RunOpponentMenu(_brain), true),
                new MenuItem("2", "Boats", () => CustomGameMenu.RunBoatsMenu(_brain)),
                new MenuItem("3", "Move after hit",() => CustomGameMenu.RunMoveAfterHitMenu(_brain)),
                new MenuItem("4", "Boat touch Rule", () => CustomGameMenu.RunBoatTouchMenu(_brain)),
                new MenuItem("5", "Board", () => CustomGameMenu.RunBoardSizeMenu(_brain)),
                new MenuItem("6", "Players", () => CustomGameMenu.RunPlayerMenu(_brain)),
                new MenuItem("7", "Start CUSTOM game", RunGameMenu)
            });
            var result = customRulesMenu.Run();
            return result;
        }

        private static string RunGameMenu()
        {

            var gameMenu = new Menu("Game menu", EMenuLevel.CustomMain);
            gameMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "Resume Game", PlayGame, true),
                new MenuItem("2", "Game rules", () => Information.ShowGameRules(_brain!)),
                new MenuItem("3", "Undo Move", UndoMove),
                new MenuItem("4", "Save", RunSaveMenu)
            });

            if (CustomRules.CheckPlayerName(_brain!, 0))
            {
                _brain!.SetPlayer(0, CustomRules.AskName());
            }
            if (CustomRules.CheckPlayerName(_brain!, 1))
            {
                _brain!.SetPlayer(1, CustomRules.AskName());
            }
            
            var settings = CustomRules.CheckGameSettings(_brain!);
            
            if (string.IsNullOrEmpty(settings))
            {
                
                var result = gameMenu.Run();
                return result;
            }
            return settings;
        }
        
        private static string RunConfigurationGame(IGameConfig conf)
        {
            _brain = new BattleshipBrain();
            
            _brain.SetClassicGame(conf);
            
            var result = HumanOrComputerMenu();
            
            return result;
        }

        private static string HumanOrComputerMenu()
        {
            var menu = new Menu("Human or computer", EMenuLevel.SecondOrMore);
            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "Human vs Human", () => SetGameMode(false) ),
                new MenuItem("2", "Computer vs Human", () => SetGameMode(true) )
            });

            var result = menu.Run();
            return result;
        }

        private static string SetGameMode(bool computerBool)
        {
            CustomRules.SetGameMode(computerBool, _brain!);

            var result = RunGameMenu();
            return result;
        }

        private static void UndoMove()
        {
            if (_brain!.BombMoveHistory.Count != 0)
            {
                _brain.UndoMove();
            }
        }

        private static void PlayGame()
        {
           
            if (_brain!.AllBoatsPlaced() == false)
            {
                BattleshipsUI.PlaceShip(_brain);
            }
            
            if(_brain.AllBoatsPlaced())
            {
                if (_brain.IsGameOver == false) Information.LoadingGame(_brain);  

                BattleshipsUI.PlaceBomb(_brain);
                
                if (_brain.IsGameOver) BattleshipsUI.ShowResult(_brain);
            }
        }
    }
}