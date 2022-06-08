using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BattleShipGameBrain;
using Domain;
using Domain.Enums;
using GameSaveLoad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Bombing
{
    public class Index : PageModel
    {
        [BindProperty(SupportsGet = true)] public int gameId { get; set; }
        public BoardSquareState[,] OpponentBoard { get; set; } = default!;
        public BoardSquareState[,] OwnBoard { get; set; } = default!;
        public List<Ship> DestroyedShips { get; set; } = default!;
        
        public async Task<IActionResult> OnGet(int? x, int? y)
        {
            if (gameId == 0)
            {
                return Redirect("./Index");
            }
            
            var brain = new BattleshipBrain();
            var game = await SaveLoad.GetGameAsync(gameId);
            SaveLoad.SetDatabaseGame(game, brain);

            
            if (x.HasValue && y.HasValue)
            {
                var moveRes = brain.PlaceBomb(x.Value, y.Value);
                SaveLoad.UpdateDatabaseGame(brain);
                if (brain.CheckForWin())
                {
                    SaveLoad.UpdateDatabaseGame(brain);
                    return Redirect($"/WinPage/Index?gameId={gameId}");
                }
            
                if (moveRes != EMoveResult.Invalid) 
                    return Redirect($"/PlayerSwitch/Index?gameId={gameId}&moveResult={moveRes.ToString()}");
                
                ModelState.AddModelError("Bomb", "You already bombed this square");
                
            }
            SetBoards(brain);
            SetData(brain);
            return Page();
        }

        //For Undo, but is currently disabled, no point
        public async Task<IActionResult> OnPost()
        {
            var brain = new BattleshipBrain();
            var game = await SaveLoad.GetGameAsync(gameId);
            SaveLoad.SetDatabaseGame(game, brain);

            if (brain.BombMoveHistory.Count == 0)
            {
                ModelState.AddModelError("NoBombs", "There is no bomb to remove");
                SetBoards(brain);
                SetData(brain);
                return Page();
            }

            brain.UndoMove();
            SaveLoad.UpdateDatabaseGame(brain);

            return Redirect($"/Bombing/Index?gameId={gameId}");
        }

        private void SetBoards(BattleshipBrain brain)
        {
            OpponentBoard = BattleshipBrain.HiddenBoatsBoard(brain.GameBoards[brain.OtherPlayer()].Board!);
            OwnBoard = brain.GameBoards[brain._currentPlayerNo].Board!;
        }

        private void SetData(BattleshipBrain brain)
        {
            DestroyedShips = brain.GetAllSunkenShips(brain.OtherPlayer());
            ViewData["CurrentPlayerName"] = brain.GetCurrentPlayerName();
            ViewData["CurrentPlayerDestroyedShips"] = brain.GetSunkenShipsCount(brain.OtherPlayer());
            ViewData["BombHitRate"] = brain.GetHitRate();
            ViewData["OpponentDestroyedShips"] = brain.GetSunkenShipsCount(brain._currentPlayerNo);

            var ships = brain.GetShips().
                Aggregate("", (current, ship) => current + $"{ship.Name} with size of {ship.Height * ship.Length} \\n");
           
            var builtString = $"'Ships:\\n{ships}\\n"+
                              $"Move rule: {MoveAfterHit.ToString(brain.MoveRule!.Value)} \\n" +
                              $"Touch rule: {ShipTouchRule.ToString(brain.ShipRule!.Value)}'";
            
            
            ViewData["Rules"] = builtString;
            
        }
    }
}