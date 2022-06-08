using System;

namespace ColorString
{
    public static class ColoredString
    {
        public static void WriteString(string stringToWrite, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(stringToWrite);
            Console.ResetColor();
        }
        
        public static void WriteLineString(string stringToWrite, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(stringToWrite);
            Console.ResetColor();
        }
    }
}