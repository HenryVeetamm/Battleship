using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using BattleShipGameBrain;
using Domain;
using Domain.Enums;
using GameSaveLoad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Custom
{
    public class Index : PageModel
    {

        public List<Ship> Ships { get; set; } = default!;
        [BindProperty] public EMoveAfterHit MoveAfterHit { get; set; }
        [BindProperty] public EShipTouchRule ShipTouch { get; set; }
        [Range(5, 15)][Required] [BindProperty] public int Width { get; set; }
        [Range(5, 15)][Required] [BindProperty] public int Height { get; set; }
        
        
        public void OnGet()
        {
          
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var brain = new BattleshipBrain();
            brain.SetMoveRule(MoveAfterHit);
            brain.SetShipRule(ShipTouch);
            brain.SetBoard(new Tuple<int, int>(Width, Height));
            var gameId = SaveLoad.SaveGameToDb("initial custom", brain);
           
            return Redirect($"/Custom/AddShip?gameId={gameId}");

        }
    }
}