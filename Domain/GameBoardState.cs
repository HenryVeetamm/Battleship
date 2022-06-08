using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class GameBoardState
    {
        public int GameBoardStateId { get; set; }

        public string? Board { get; set; }

        
        public int? PlayerId { get; set; }
        public Player? Player { get; set; }
        

        
        public List<Ship>? Ships { get; set; }

        [InverseProperty("GameBoard1")]public ICollection<Game>? Games1 { get; set; }
        [InverseProperty("GameBoard2")]public ICollection<Game>? Games2 { get; set; }
    }
}