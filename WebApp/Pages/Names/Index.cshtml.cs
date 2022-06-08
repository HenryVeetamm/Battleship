using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Names
{
    public class Index : PageModel
    {
        private readonly ApplicationDbContext _ctx;
        
        public Index(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [BindProperty(SupportsGet = true)] public int gameId { get; set;}
        [Display(Name = "game name")]
        [BindProperty] public string GameName { get; set; } = default!;
        [Display(Name = "player1 name")]
        [BindProperty] public string Player1Name { get; set; } = default!;
        [Display(Name = "player2 name")]
        [BindProperty] public string Player2Name { get; set; } = default!;

        public IActionResult OnGet()
        {
            if (gameId == 0)
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public  async Task<IActionResult> OnPost()
        {
            var game = await _ctx.Games.Where(g => g.GameId == gameId)
                .Include(b => b.GameBoard1).ThenInclude(p => p!.Player)
                .Include(b => b.GameBoard1).ThenInclude(p => p!.Ships)
                .Include(b => b.GameBoard2).ThenInclude(p => p!.Player)
                .Include(b => b.GameBoard2).ThenInclude(p => p!.Ships).FirstAsync();
            

            if (!ModelState.IsValid)
            {
                return Page();
            }

            game.GameBoard1!.Player = new Player() { Name = Player1Name };
            game.GameBoard2!.Player = new Player() { Name = Player2Name };
            game.GameName = GameName;
            _ctx.Update(game);
            await _ctx.SaveChangesAsync();
            
            return Redirect("/PlaceShip/Index?gameId="+gameId);
        }
        
    }
}