using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BattleShipGameBrain;
using Domain;
using GameSaveLoad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Custom
{
    public class AddShip : PageModel
    {

        [BindProperty(SupportsGet = true)] public int gameId { get; set; }

        [BindProperty] [Display(Name = "'ship name'")]
        public string ShipName { get; set; } = default!;
        
        [Range(1, 6)] [BindProperty][Display(Name = "'ship length'")]
        public int ShipLength { get; set; }
        
        [Range(1, 6)] [BindProperty] [Display(Name = "'ship height'")]
        public int ShipHeight { get; set; }
        

        [BindProperty]public List<Ship> Ships { get; set; } = new List<Ship>();
        
        
        public IActionResult OnGet()
        {
            if (gameId == 0)
            {
                return Redirect("/Custom/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPost(string button)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var brain = new BattleshipBrain();
            var game = await SaveLoad.GetGameAsync(gameId);
            SaveLoad.SetDatabaseGame(game, brain);
            Ships = brain.GetShips();

            if (button == "Continue")
            {
                if (Ships.Count == 0)
                {
                    ModelState.AddModelError("NoBoats", "Please add some ships to continue");
                    return Page();
                }

                return Redirect($"/Names/Index?gameId={gameId}");
            }
            
            var ship = new Ship()
            {
                Name = ShipName,
                Length = ShipLength,
                Height = ShipHeight
            };
            
            brain.AddBoat(ship);
            SaveLoad.UpdateDatabaseGame(brain);
            return Page();

        }
    }
}