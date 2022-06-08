using System.Collections.Generic;
using Domain;
using Domain.Enums;

namespace BattleShipGameBrain.Interfaces
{
    public interface IGameConfig
    {
        public int BoardSizeX { get; set; }
        public int BoardSizeY { get; set; }
        public List<Ship> Ships { get; set; }
        public EShipTouchRule ShipRule { get; set; }
        public EMoveAfterHit MoveRule { get; set; }

        public Player Player1 { get; set; }
        
        public Player Player2 { get; set; }
    }
}