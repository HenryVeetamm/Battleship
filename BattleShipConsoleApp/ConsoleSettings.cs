using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BattleShipConsoleApp
{
    public static class ConsoleSettings
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ShowWindow(IntPtr ThisWindow, int nCmdShow);
      
    }
}