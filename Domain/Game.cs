using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain
{
    public class Game
    {
        public int GameId { get; set; }

        
        public int GameBoard1Id { get; set; }
        public GameBoardState? GameBoard1 { get; set; }
        
        public int GameBoard2Id { get; set; }
        public GameBoardState? GameBoard2 { get; set; } 
        
        public EMoveAfterHit? EMoveAfterHit { get; set; }
        public EShipTouchRule? EShipTouchRule { get; set; }

        public int CurrentPlayer { get; set; }

        public string? BombMoveHistory { get; set; }

        public bool IsGameOver { get; set; }

        public DateTime DateTime { get; set; }

        public string GameName { get; set; } = null!;

        public ESaveType SaveType { get; set; }

        public bool InDatabase { get; set; } = false;

        public bool PredefinedGame { get; set; } = false;
    }
}