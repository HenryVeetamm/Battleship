using System.Collections.Generic;
using System.Text.Json;
using BattleShipGameBrain.Interfaces;
using Domain;
using Domain.Enums;

namespace BattleShipGameBrain
{
    public class StandardGameConfig : IGameConfig
    {
        public int BoardSizeX { get; set; } = 10;
        public int BoardSizeY { get; set; } = 10;

        public List<Ship> Ships { get; set; } = new List<Ship>()
        {
            new Ship()
            {
                Name = "Patrol",
                Length = 1,
                Height = 1
            },
            new Ship()
            {
                Name = "Cruiser",
                Length = 2,
                Height = 1
            },
            new Ship()
            {
                Name = "Submarine",
                Length = 3,
                Height = 1
            },
            new Ship()
            {
                Name = "Battleship",
                Length = 4,
                Height = 1
            },
            new Ship()
            {
                Name = "Carrier",
                Length = 5,
                Height = 1
            }
        };

        public EShipTouchRule ShipRule { get; set; } = EShipTouchRule.NoTouch;
        
        public EMoveAfterHit MoveRule { get; set; } = EMoveAfterHit.SamePlayer;

        public Player Player1 { get; set; } = null!;

        public Player Player2 { get; set; } = null!;
    }
}