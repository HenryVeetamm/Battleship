using System.Linq;
using BattleShipGameBrain;
using Domain.Enums;

namespace BattleShipConsoleUI
{
    public static class MenuHeaders
    {
        public static string BoardSizeHeader(BattleshipBrain brain)
        {
            if (brain.GameBoards[0].Board is null) return "Board is not set";
            return $"Board is {brain.GameBoards[0].Board!.GetUpperBound(1) + 1 } x " +
                   $"{brain.GameBoards[0].Board!.GetUpperBound(0) + 1}";
        }
        
        public static string ShipTouchHeader(BattleshipBrain brain)
        {
            return brain.ShipRule is null ? "Rule is not set" : $"Can ships touch? {ShipTouchRule.ToString(brain.ShipRule.Value)}";
        }
        
        public static string AfterHitHeader(BattleshipBrain brain)
        {
           
            if (brain.MoveRule is null)
            {
                return "After hit rule is not set";
            }
            var afterHit = $"Player who makes move after hit is: {MoveAfterHit.ToString(brain.MoveRule.Value)}";
            return afterHit;
        }
        
        public static string OpponentSelectHeader(BattleshipBrain brain)
        {
            
            var opponent = "Current game mode is: ";
            opponent += brain.AgainstPc == false ? "Human vs Human" : "Computer vs Human";
            return opponent;
        }
        
        public static string BoatHeader(BattleshipBrain brain)
        {
           
            var ships = brain.GameBoards[0].Ships;
            if (ships.Count == 0)
            {
                return "No ships";
            }
            var boats = ships.Aggregate("", (current, ship) => current + $"Name: {ship}");
            return boats.TrimEnd();
        }
        
        public static string PlayerHeader(BattleshipBrain brain)
        {
            string returnString = brain.GameBoards[0].Player is null ? "Player 1 name is not set \n" :
                $"Player 1 name is {brain.GameBoards[0].Player!.Name} \n";
            returnString += brain.GameBoards[1].Player is null ? "Player 2 name is not set" :
                $"Player 2 name is {brain.GameBoards[1].Player!.Name} ";

            return returnString;
        }

    }
}