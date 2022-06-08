
using System.Linq;
using BattleShipGameBrain;
using DAL;
using Domain;
using GameSaveLoad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace WebApp.Pages.NewGame
{
    public class Index : PageModel
    {
        public void OnGet()
        {
            
        }

        public  IActionResult OnPost(int choice)
        {
            var brain = new BattleshipBrain();

            using var ctx = new ApplicationDbContext();

            int gameId;
            Game? game;
            
            switch (choice)
            {
                case 1:
                    
                    game = ctx.Games.FirstOrDefault(g => g.GameName == "Standard game");
                    SaveLoad.SetDatabaseGameWithId(game!.GameId, brain);
                    gameId = SaveLoad.SaveGameToDb("Classic", brain);
                    return Redirect($"./Names/Index?gameId={gameId}");
                    
                case 2:
                    
                    game = ctx.Games.FirstOrDefault(g => g.GameName == "Small game");
                    SaveLoad.SetDatabaseGameWithId(game!.GameId, brain);
                    gameId = SaveLoad.SaveGameToDb("Small", brain);

                    return Redirect("/Names/Index?gameId="+gameId);
                default:
                    return Redirect("/Custom/Index");
                    
            }
        }
    }
}