using System;
using BattleShipGameBrain;
using Domain;
using Domain.Enums;

namespace BattleShipConsoleUI
{
    public static class CustomRules
    {
        public static Ship AskBoatInfo()
        {
            Console.Clear();
            
            var name = AskString("Add name for your ship: ");
            var length = AskSizeInt("Enter ship length");
            var height = AskSizeInt("Enter ship width");

            var boat = new Ship()
            {
                Name = name,
                Length = length,
                Height = height
            };
            return boat;
        }

        public static string AskString(string question)
        {
            string returnString;
            string? input = null;
            do
            {
                Console.Clear();
                
                if (input is not null)
                {
                    Console.WriteLine($"Invalid input! Your input: {input}");
                }

                Console.WriteLine(question);
                input = Console.ReadLine();
                if (int.TryParse(input, out var result) == false && 
                    string.IsNullOrEmpty(input) is false && 
                    string.IsNullOrWhiteSpace(input) is false)
                {
                    returnString = input;
                    break;
                }
                
                
            } while (true);

            return returnString;
        }

        private static int AskSizeInt(string question)
        {
            int size;
            string? input = null;
            do
            {
                Console.Clear();
                if (input is not null)
                {
                    Console.WriteLine($"Invalid input! Your input: {input}");
                }

                
                Console.WriteLine(question);
                input = Console.ReadLine();
                if (int.TryParse(input, out var result) && result > 0)
                {
                    size = result;
                    break;
                }
                
            } while (true);

            return size;

        }

        public static Tuple<int, int> BoardSize()
        {
            var width = AskSizeInt("Enter board width: ");
            var height = AskSizeInt("Enter board height");

            return new Tuple<int, int>(width, height);
        }

        public static string AskName()
        {
            return AskString("Enter player name: ");
        }
        
        
        public static string CheckGameSettings(BattleshipBrain brain)
        {
            var errors = "";
            
            if (brain.GameBoards[brain._currentPlayerNo].Ships.Count == 0) errors += "Please add boats.\n";
            
            if(brain.MoveRule is null) errors += "Please set who moves after hit rule.\n";
            if(brain.ShipRule is null) errors += "Please set ship touching rule.\n";
            if(brain.GameBoards[0].Board is null) errors += "Please set board size.\n";
            if (CheckPlayerName(brain,0)) errors += "Please set player 1 name.\n";
            if (CheckPlayerName(brain,1)) errors += "Please set player 2 name.\n";

            return errors.TrimEnd();
        }

        public static bool CheckPlayerName(BattleshipBrain brain, int playerNo)
        {
            return (brain.GameBoards[playerNo].Player is null); 
       
        }
        
        public static void SetGameMode(bool againstPcBool, BattleshipBrain brain)
        {
            brain.AgainstPc = againstPcBool;
            if (againstPcBool)
            {
                brain.GameBoards[1].Player = new Player() { Name = "Computer", PlayerType = EPlayerType.Pc };
            }
        }
    }
}