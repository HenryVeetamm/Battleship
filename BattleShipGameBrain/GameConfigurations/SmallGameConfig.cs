using System.Collections.Generic;
using BattleShipGameBrain.Interfaces;
using Domain;
using Domain.Enums;

namespace BattleShipGameBrain
{
    public class SmallGameConfig: IGameConfig
    {
        public int BoardSizeX { get; set; } = 5;
        public int BoardSizeY { get; set; } = 5;
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
                    Name = "Patrol",
                    Length = 1,
                    Height = 1
                },
                new Ship()
                {
                    Name = "Patrol",
                    Length = 1,
                    Height = 1
                },
                new Ship()
                {
                    Name = "Patrol",
                    Length = 1,
                    Height = 1
                },
                new Ship()
                {
                    Name = "Patrol",
                    Length = 1,
                    Height = 1
                }
            };

        public EShipTouchRule ShipRule { get; set; } = EShipTouchRule.Yes;
        public EMoveAfterHit MoveRule { get; set; } = EMoveAfterHit.SamePlayer;
        public Player Player1 { get; set; } = null!;
        public Player Player2 { get; set; } = null!;
    }
}