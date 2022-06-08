using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class Ship
    {
        public int ShipId { get; set; }
        
        public string? Name { get; set; }

        public int Length { get; set; }
        public int Height { get; set; }

        public bool Placed { get; set; }

        public int GameBoardStateId { get; set; }
        public GameBoardState? GameBoardState { get; set; } = null!;
        
        public ICollection<Coordinate> Coordinates { get; set; } = new List<Coordinate>();
        
        public void SetShipCoordinates(Coordinate point)
        {
            for (var y = point.Y; y < point.Y + Length; y++)
            {
                for (var x = point.X; x < point.X + Height; x++)
                {
                    Coordinates!.Add(new Coordinate() { X = x, Y = y });
                }
            }
        }

        public override string ToString()
        {
            return $"{Name},  Length: {Length}, Height: {Height}";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            
            Ship? s = obj as Ship;
            
            if (s == null)
                return false;

            return (Name == s.Name) && (Length == s.Length) && (Height == s.Height);
        }

        public override int GetHashCode()
        {
            return Name!.GetHashCode() ^ Height.GetHashCode() ^ Length.GetHashCode();
        }
    }
}