using System;
using System.Threading.Tasks;
using BattleShipGameBrain;
using Domain.Enums;
using GameSaveLoad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.WinPage
{
    public class Index : PageModel
    {
        [BindProperty(SupportsGet = true)] public int gameId { get; set; }
        public BoardSquareState[,] WinnerBoard { get; set; } = default!;
        public BoardSquareState[,] LoserBoard { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            if (gameId == 0)
            {
                return RedirectToPage("./Index");
            }
            var brain = new BattleshipBrain();
            var game = await SaveLoad.GetGameAsync(gameId);
            SaveLoad.SetDatabaseGame(game, brain);

            ViewData["WinnerName"] =
                brain.GameBoards[brain.GetWinnerNo()].Player!.Name;

            ViewData["LoserName"] = brain.GameBoards[brain.GetLoserNo()].Player!.Name;
            
            WinnerBoard = brain.GameBoards[brain.GetWinnerNo()].Board!;
            LoserBoard = brain.GameBoards[brain.GetLoserNo()].Board!;
            
            return Page();
        }
    }
}