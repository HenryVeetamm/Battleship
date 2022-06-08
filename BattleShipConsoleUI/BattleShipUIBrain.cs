
using BattleShipGameBrain;
using Domain;

namespace BattleShipConsoleUI
{
    public static class BattleShipUIBrain
    {
        public static int MoveDownBomb(BattleshipBrain brain, int x)
        {
            if (brain.GameBoards[brain._currentPlayerNo].Board!.GetUpperBound(1) == x)
            {
                return 0;
            }

            return x + 1;
        }
        
        
        
        public static int MoveUpBomb(BattleshipBrain brain, int x)
        {
            if (0 == x)
            {
                return brain.GameBoards[brain._currentPlayerNo].Board!.GetUpperBound(1);
            }

            return x - 1;
        }
        
        public static int MoveRightBomb(BattleshipBrain brain, int y)
        {
            if (brain.GameBoards[brain._currentPlayerNo].Board!.GetUpperBound(0) == y)
            {
                return 0;
            }

            return y + 1;
        }

        public static int MoveLeftBomb(BattleshipBrain brain, int y)
        {
            if (0 == y)
            {
                return brain.GameBoards[brain._currentPlayerNo].Board!.GetUpperBound(0);
            }

            return y - 1;
        }
        
        public static int MoveDownShip(BattleshipBrain brain, int x, Ship ship)
        {
            if (brain.GameBoards[brain._currentPlayerNo].Board!.GetUpperBound(1) == x + ship.Length - 1)
            {
                return 0;
            }

            return x + 1;
        }

        public static int MoveUpShip(BattleshipBrain brain, int x, Ship ship)
        {
            if (0 == x)
            {
                
                return brain.GameBoards[brain._currentPlayerNo].Board!.GetUpperBound(1) - ship.Length + 1;
            }

            return x - 1;
        }

        public static int MoveRightShip(BattleshipBrain brain, int y, Ship ship)
        {
            if (brain.GameBoards[brain._currentPlayerNo].Board!.GetUpperBound(0) == y + ship.Height - 1)
            {
                return 0;
            }

            return y + 1;
        }

        public static int MoveLeftShip(BattleshipBrain brain, int y, Ship ship)
        {
            if (0 == y)
            {
                return brain.GameBoards[brain._currentPlayerNo].Board!.GetUpperBound(0) - ship.Height + 1;
            }

            return y - 1;
        }


        public static bool Rotate(BattleshipBrain brain, Ship ship, int y, int x)
        {
            if (y + ship.Length - 1 <= brain.GameBoards[brain._currentPlayerNo].Board!.GetUpperBound(0) &&
                x + ship.Height - 1 <= brain.GameBoards[brain._currentPlayerNo].Board!.GetUpperBound(1))
            {
                return true;
            }

            return false;
            
            
        }
    }
}