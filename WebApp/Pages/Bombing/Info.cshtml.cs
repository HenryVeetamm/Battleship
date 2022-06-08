using System.Threading.Tasks;
using BattleShipGameBrain;
using GameSaveLoad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Bombing
{
    public class Info : PageModel
    {
        [BindProperty(SupportsGet = true)] public int gameId { get; set; }


        public async Task<IActionResult> OnGet()
        {
            if (gameId == 0)
            {
                return RedirectToPage("./Index");
            }
            
            var brain = new BattleshipBrain();
            var game = await SaveLoad.GetGameAsync(gameId);
            SaveLoad.SetDatabaseGame(game, brain);
            ViewData["StartPlayer"] = brain.GetCurrentPlayerName();
            return Page();
        }

        
        public IActionResult OnPost()
        {
            return Redirect($"/Bombing/Index?gameId={gameId}");
        }
    }
}