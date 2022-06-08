namespace BattleShipGameBrain
{
    public class BoardSquareState
    {
        public bool IsShip { get; set; }
        public bool IsBomb { get; set; }

        public override string ToString()
        {
            switch (IsEmpty: IsShip, IsBomb)
            {
                case (false, false):
                    return " "; // Empty
                case (false, true):
                    return "B"; // Bomb
                case (true, false):
                    return "S"; // Ship
                case (true, true):
                    return "H"; //Hit boat
            }
        }
    }
}