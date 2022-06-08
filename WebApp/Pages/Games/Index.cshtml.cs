using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_Games
{
    public class IndexModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public IndexModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Game> Game { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Game = await _context.Games.Where(game => game.PredefinedGame == false && game.GameBoard1!.Player != null && 
                                                      game.GameBoard2!.Player != null )
                .Include(g => g.GameBoard1).ThenInclude(g => g!.Player)
                .Include(g => g.GameBoard2).ThenInclude(g => g!.Player).ToListAsync();
        }
    }
}
