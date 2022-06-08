using System;
using System.Collections.Generic;
using Domain;

namespace BattleShipGameBrain
{
    public class GameBoard
    {
        public BoardSquareState[,]? Board { get; set; }
        
        public List<Ship> Ships { get; set; } = new List<Ship>();

        public Player? Player { get; set; }

        public override string ToString()
        {
            foreach (var ship in Ships)
            {
                Console.Write($"{ship.Name}");
                foreach (var coor in ship.Coordinates!)
                {
                    Console.Write(coor.ToString());
                }
            }

            return "";
        }
        
        
    }
}