using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Calculator;
using MenuSystem;

namespace ConsoleApp1
{
    class Program
    {
        /*private static double _calculatorCurrentDisplay = 0.0;*/

        private static CalculatorBrain? _calculator;

        static void Main(string[] args)
        {
            _calculator = new CalculatorBrain();
            var mainMenu = new Menu(_calculator.GetCurrent, "Calculator Main", EMenuLevel.Root);
            mainMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("A", "Binary operations", SubmenuBinary, true),
                new MenuItem("B", "Unary operations", SubmenuUnary),
                new MenuItem("S", "Set Value", _calculator.SetValue),
                new MenuItem("g", "Reset value", _calculator.ResetValue)
            });

            mainMenu.Run();
        }


        public static string SubmenuBinary()
        {
            var menu = new Menu(_calculator!.GetCurrent, "Binary", EMenuLevel.First);
            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("+", "+", _calculator.Add, true),
                new MenuItem("-", "-", _calculator.Subtract),
                new MenuItem("/", "/", _calculator.Divide),
                new MenuItem("*", "*", _calculator.Multiply),
                new MenuItem("P", "Power of", _calculator.PowerOf),
            });
            var res = menu.Run();
            return res;
        }

        private static string SubmenuUnary()
        {
            var menu = new Menu(_calculator!.GetCurrent, "Unary", EMenuLevel.First);
            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("N", "Negate", _calculator.Negate, true),
                new MenuItem("S", "Square", _calculator.Square),
                new MenuItem("P", "Root", _calculator.Root),
            });
            var res = menu.Run();
            return res;
        }
    }
}