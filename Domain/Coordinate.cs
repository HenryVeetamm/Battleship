﻿namespace Domain
{
    public class Coordinate
    {
        public int CoordinateId { get; set; }

        public int ShipId { get; set; }
        public Ship? Ship { get; set; }
        
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString() => $"X: {X}, Y: {Y}";
    }
}