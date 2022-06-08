using System;
using System.Threading.Tasks;
using BattleShipGameBrain;
using Domain.Enums;
using GameSaveLoad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.PlayerSwitch
{
    public class Index : PageModel
    {
        [BindProperty(SupportsGet = true)] public int gameId { get; set; } = default!;
        [BindProperty(SupportsGet = true)] public string moveResult { get; set; } = default!;
        public string CurrentPlayerName { get; set; } = default!;
        public string NextPlayerName { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            if (gameId == 0)
            {
                return RedirectToPage("./Index");
            }
            
            var brain = new BattleshipBrain();
            var game = await SaveLoad.GetGameAsync(gameId);
            SaveLoad.SetDatabaseGame(game, brain);

            
            if (moveResult == "Hit" && brain.MoveRule == EMoveAfterHit.SamePlayer)
            {
                CurrentPlayerName = brain.GetCurrentPlayerName();
                NextPlayerName = brain.GetCurrentPlayerName();
            }
            else
            {
                CurrentPlayerName = brain.GetOtherPlayerName();
                NextPlayerName = brain.GetCurrentPlayerName();
            }

            return Page();
        }
        
        public async Task<IActionResult> OnPost(string button)
        {
            if (button == "Undo")
            {
                var brain = new BattleshipBrain();
                var game = await SaveLoad.GetGameAsync(gameId);
                SaveLoad.SetDatabaseGame(game, brain);
                brain.UndoMove();
                SaveLoad.UpdateDatabaseGame(brain);
            }

            return Redirect($"/Bombing/Index?gameId={gameId}");

        }
    }
}