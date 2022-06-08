using System.Collections.Generic;
using BattleShipGameBrain.Interfaces;
using Domain;
using Domain.Enums;

namespace BattleShipGameBrain
{
    public class TestConfiguration : IGameConfig
    {
        public int BoardSizeX { get; set; } = 10;
        public int BoardSizeY { get; set; } = 5;

        public List<Ship> Ships { get; set; } = new List<Ship>()
        {
            new Ship()
            {
                Name = "Cruiser",
                Length = 2,
                Height = 1
            },
            new Ship()
            {
                Name = "Cruiser",
                Length = 2,
                Height = 1
            }
        };

        public EShipTouchRule ShipRule { get; set; } = EShipTouchRule.NoTouch;
        public EMoveAfterHit MoveRule { get; set; } = EMoveAfterHit.SamePlayer;
        public Player Player1 { get; set; } = new Player() { Name = "Henry" };
        public Player Player2 { get; set; } = new Player() { Name = "Marko" };
    }
}