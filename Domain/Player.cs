using System.Collections.Generic;
using Domain.Enums;

namespace Domain
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = default!;

        public List<GameBoardState>? GameBoardStates { get; set; }
        
        public EPlayerType PlayerType { get; set; } = EPlayerType.Human;

    }
}