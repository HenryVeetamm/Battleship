using System.Collections.Generic;
using BattleShipConsoleUI;
using BattleShipGameBrain;
using Domain;
using Domain.Enums;
using MenuSystem;

namespace BattleShipConsoleApp
{
    public static class CustomGameMenu
    {

        public static string RunPlayerMenu(BattleshipBrain brain)
        {
            var menu = new Menu(() => MenuHeaders.PlayerHeader(brain),"Player menu", EMenuLevel.CustomReturnOnly);
            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", brain.AgainstPc ? "Enter your name" : "Player one",() => brain.SetPlayer(0, CustomRules.AskName())),
                });
            if(brain.AgainstPc == false) menu.AddMenuItem(new MenuItem("2", "Player two", () => brain.SetPlayer(1, CustomRules.AskName())), 1);
            menu.InitializeMenu();
            var result = menu.Run();
            return result; 
        }
        
        public static string RunBoatsMenu(BattleshipBrain brain)
        {
            var boatMenu = new Menu(() => MenuHeaders.BoatHeader(brain),"Boats", EMenuLevel.CustomReturnOnly);
            boatMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "Add Boat", () => AddBoat(brain)),
                new MenuItem("2", "Remove Boat", () => RemoveBoatMenu(brain)),
            });
            var result = boatMenu.Run();
            return result;
        }
        
        public static string RunOpponentMenu(BattleshipBrain brain)
        {
            var gameModeMenu = new Menu(() => 
                MenuHeaders.OpponentSelectHeader(brain),"PC or Human?", EMenuLevel.CustomReturnOnly);
           
            gameModeMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "Against human", () => CustomRules.SetGameMode(false, brain)),
                new MenuItem("2", "Against PC", () => CustomRules.SetGameMode(true, brain)),
            });
            var result = gameModeMenu.Run();
            return result;
        }

        

        private static void AddBoat(BattleshipBrain brain)
        {
            var boat = CustomRules.AskBoatInfo();
            brain.AddBoat(boat);
        }
        
        private static string RemoveBoatMenu(BattleshipBrain brain)
        {
            if (brain.GetShips().Count == 0)
            {
                return "No boats";
            }
            var boatMenu = new Menu("Boats", EMenuLevel.CustomReturnOnly);
            
            var index = 1;
            foreach (var ship in (brain.GetShips()))
            {
                boatMenu.AddMenuItem(new MenuItem(index.ToString(), 
                    $"{ship}",
                    () => brain.RemoveShip(ship)));
                
                index++;
            }
            boatMenu.InitializeMenu();
            
            var result = boatMenu.Run();
            return result;
        }

        public static string RunMoveAfterHitMenu(BattleshipBrain brain)
        {
            var ruleMenu = new Menu(() => MenuHeaders.AfterHitHeader(brain),"Who Moves After Hit?", EMenuLevel.CustomReturnOnly);
            
            ruleMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "Same player",() => brain.SetMoveRule(EMoveAfterHit.SamePlayer)),
                new MenuItem("2", "Other player", () => brain.SetMoveRule(EMoveAfterHit.OtherPlayer)),
            });
            var result= ruleMenu.Run();
            return result;
        }
        
        public static string RunBoatTouchMenu(BattleshipBrain brain)
        {
            var ruleMenu = new Menu(() => MenuHeaders.ShipTouchHeader(brain),"Boat placement", EMenuLevel.CustomReturnOnly);
            
            ruleMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "Ships can touch",() => brain.SetShipRule(EShipTouchRule.Yes)),
                new MenuItem("2", "Ships can't touch", () => brain.SetShipRule(EShipTouchRule.NoTouch)),
                new MenuItem("3", "Only ships one corner can touch other ship.", () => brain.SetShipRule(EShipTouchRule.CornerTouch)),
            });
            var result = ruleMenu.Run();
            return result;
        }
        
        public static string RunBoardSizeMenu(BattleshipBrain brain)
        {
            var menu = new Menu(() => MenuHeaders.BoardSizeHeader(brain),"Board sizing", EMenuLevel.CustomReturnOnly);
            
            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "Set board size", () => BoardSize(brain)  ),
               
            });
            var result = menu.Run();
            return result;
        }

        private static void BoardSize(BattleshipBrain brain)
        {
            var boardSize = CustomRules.BoardSize();
            brain.SetBoard(boardSize);
        }
    }
}