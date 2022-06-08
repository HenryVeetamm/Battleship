using System;
using System.Threading.Tasks;
using BattleShipGameBrain;
using GameSaveLoad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Games
{
    public class Continue : PageModel
    {
        
        public async Task<IActionResult> OnGet(int id)
        {
            var brain = new BattleshipBrain();
            var game = await SaveLoad.GetGameAsync(id);
            SaveLoad.SetDatabaseGame(game, brain);

            return brain.AllBoatsPlaced() == false ? Redirect($"/PlaceShip/Index?gameId={id}") : 
                Redirect(brain.CheckForWin() ? $"/WinPage/Index?gameId={id}" : 
                    $"/Bombing/Info?gameId={id}");
        }
    }
}