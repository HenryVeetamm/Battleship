using System;
using System.Threading;
using BattleShipGameBrain;
using ColorString;
using Domain.Enums;

namespace BattleShipConsoleUI
{
    public static class Information
    {
        public static void Statistics(BattleshipBrain brain)
        {
            
            Console.Write("Player: ");
            ColoredString.WriteString(brain.GameBoards[brain._currentPlayerNo].Player!.Name + "\n", ConsoleColor.Green);
           
            Console.Write("Ships destroyed: ");
            ColoredString.WriteString(brain.GetSunkenShipsCount(brain.OtherPlayer()) + "\n", ConsoleColor.Red);
     
            Console.Write("Bomb hit rate: ");
            ColoredString.WriteString($"{brain.GetHitRate()}% \n", ConsoleColor.Red);
           
            Console.Write("Your opponent has destroyed: ");
            ColoredString.WriteString($"{brain.GetSunkenShipsCount(brain._currentPlayerNo)}\n", ConsoleColor.Red);
        }

        public static void SunkenShips(BattleshipBrain brain)
        {   
            Console.WriteLine();
            ColoredString.WriteLineString("You have destroyed:", ConsoleColor.Green);
            foreach (var ship in brain.GetAllSunkenShips(brain.OtherPlayer()))
            {
                Console.WriteLine(ship.ToString());
            }
        }

        public static void ShowGameControls()
        {
            Console.WriteLine();
            Console.WriteLine("Keyboard settings:");
            Console.WriteLine("Use arrow keys to move around");
            Console.WriteLine("ESC: opens game menu");
            Console.WriteLine("U: Undo your move");
            Console.WriteLine("ENTER: Places bomb");
        }
        
        public static void GameOverInfo(int cursorLeft, int cursorTop, string playerInfo, ConsoleColor color)
        {
            Console.SetCursorPosition(cursorLeft, cursorTop);
            ColoredString.WriteLineString(playerInfo, color);
        }

        public static void ShowGameOverKey() =>
            ColoredString.WriteLineString("Press ESC to return to menu", ConsoleColor.Red);
        

        public static void ShipPlacingMovement()
        {
            Console.WriteLine();
            Console.WriteLine("Keyboard settings:");
            Console.WriteLine("Use arrow keys to move around");
            Console.WriteLine("ESC: opens game menu");
            Console.WriteLine("R: Rotate ship");
            Console.WriteLine("ENTER: Place ship");
            Console.WriteLine("Q: Place all your boats randomly");
        }

        public static void LoadingGame(BattleshipBrain brain)
        {
            Console.Clear();
            ColoredString.WriteLineString("Loading game... ", ConsoleColor.Magenta);
            Console.Write("\nPlayer ");
            ColoredString.WriteString(brain.GameBoards[brain._currentPlayerNo].Player!.Name, ConsoleColor.Green );
            Console.WriteLine(" starts placing bombs");
            Console.WriteLine();
            
            ColoredString.WriteLineString("Press any key to continue", ConsoleColor.Red);
            Console.ReadKey();
        }

        public static void PlayerChangeInfo(string moveResult, BattleshipBrain brain)
        {
            Console.Clear();
            Console.Write("Player ");
            ColoredString.WriteString(brain.GetOtherPlayerName(), ConsoleColor.Green);
            Console.Write(" placed a bomb and it was a ");
            ColoredString.WriteString(moveResult + "\n", ConsoleColor.Cyan);
                
            Console.WriteLine();
                
            Console.Write("Player ");
            ColoredString.WriteString(brain.GetCurrentPlayerName(), ConsoleColor.Green);
            Console.Write(" press enter to place a bomb");
        }

        public static void ShowGameRules(BattleshipBrain brain)
        {
            Console.Clear();
            Console.Write("Player1 is: ");
            ColoredString.WriteLineString(brain.GameBoards[0].Player!.Name, ConsoleColor.Green);
            Console.WriteLine();
            
            Console.Write("Player2 is: ");
            ColoredString.WriteLineString(brain.GameBoards[1].Player!.Name, ConsoleColor.Green);
            Console.WriteLine();
            
            ColoredString.WriteLineString("Ships in game:", ConsoleColor.Cyan);
            foreach (var ship in brain.GetShips())
            {
                Console.WriteLine($"{ship.Name} with size of {ship.Height * ship.Length}");
            }
            Console.WriteLine();
            
            Console.Write("Can ships touch? ");
            ColoredString.WriteLineString(ShipTouchRule.ToString(brain.ShipRule!.Value), ConsoleColor.Red);
            Console.WriteLine();
            
            Console.Write("Who makes next move after hit? ");
            ColoredString.WriteLineString(MoveAfterHit.ToString(brain.MoveRule!.Value), ConsoleColor.Red);
            Console.WriteLine();

            ColoredString.WriteLineString("Press any key to continue", ConsoleColor.Red);
            Console.ReadKey();
        }
        
        public static void ShowCurrentPlayerName(string name)
        {
            Console.WriteLine();
            ColoredString.WriteLineString("Current player is", ConsoleColor.Red);
            ColoredString.WriteString("+", ConsoleColor.Green);
            for (int i = 0; i < name.Length; i++)
            {
                ColoredString.WriteString("-", ConsoleColor.Green);
            }
            
            
            ColoredString.WriteLineString("+", ConsoleColor.Green);
            
            ColoredString.WriteString("|", ConsoleColor.Green);
            ColoredString.WriteString($"{name}", ConsoleColor.Red);
            ColoredString.WriteLineString("|", ConsoleColor.Green);
            
            ColoredString.WriteString("+", ConsoleColor.Green);
            for (int i = 0; i < name.Length; i++)
            {
                ColoredString.WriteString("-", ConsoleColor.Green);
            }
            ColoredString.WriteString("+", ConsoleColor.Green);
        }
    }
}