using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BattleShipGameBrain;
using Domain;
using GameSaveLoad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.PlaceShip
{
    public class Index : PageModel
    {

        public BoardSquareState[,] Board { get; set; } = default!;
        public Ship ship { get; set; } = default!;

        [BindProperty(SupportsGet = true)] public int gameId { get; set; }
        private int XCor { get; set; }
        private int YCor { get; set; }
        [BindProperty] public int Rotate { get; set; }
        public string CurrentPlayerName { get; set; } = default!;

        [RegularExpression(@"^[A-Za-z]{1}([0-9]{1,2})", ErrorMessage = "Invalid start coordinate")]
        [StringLength(3)]
        [BindProperty]
        [Display(Name = "ship start coordinate")]
        public string Start { get; set; } = default!;


        public async Task<IActionResult> OnGet()
        {
            if (gameId == 0)
            {
                return RedirectToPage("./Index");
            }
            await SetUp();
            return Page();
        }

        public async Task<IActionResult> OnPost(bool? randomPlacement)
        {
            var brain = new BattleshipBrain();
            var game = await SaveLoad.GetGameAsync(gameId);
            SaveLoad.SetDatabaseGame(game, brain);
            
            if (randomPlacement.HasValue)
            {
                brain.PlacePlayerShipsRandomly(brain._currentPlayerNo);
                brain.SwitchPlayer();
                SaveLoad.UpdateDatabaseGame(brain);
                return Redirect(brain.AllBoatsPlaced() ? $"/Bombing/Info?gameId={gameId}" : 
                    $"/PlaceShip/Index?gameId={gameId}");
            }

            var boat = brain.GetFirstNotPlacedShip();

            if (Rotate == 2)
            {
                (boat.Height, boat.Length) = (boat.Length, boat.Height);
            }
            

            CheckBoatPlacement(brain, boat);
            
            if (ModelState.ErrorCount != 0)
            {
                await SetUp();
                return Page();
            }
            
            
            brain.PlaceShip(XCor,YCor,boat);
            
            if (brain.PlayerBoatsPlaced(brain._currentPlayerNo))brain.SwitchPlayer();
            
            SaveLoad.UpdateDatabaseGame(brain);

            if (brain.AllBoatsPlaced()) return Redirect($"/Bombing/Info?gameId={gameId}");
            return Redirect($"/PlaceShip/Index?gameId={gameId}");
        }

        private async Task SetUp()
        {
            var brain = new BattleshipBrain();
            var game = await SaveLoad.GetGameAsync(gameId);
            SaveLoad.SetDatabaseGame(game, brain);
            
            Board = brain.GameBoards[brain._currentPlayerNo].Board!;

            CurrentPlayerName = brain.GameBoards[brain._currentPlayerNo].Player!.Name;
            
            if (brain.PlayerBoatsPlaced(brain._currentPlayerNo) == false)
            {
                ship = brain.GetFirstNotPlacedShip();
            }
        }

        private void CheckBoatPlacement(BattleshipBrain brain, Ship ship)
        {
            //If coordinate is invalid
            if (ModelState.ErrorCount != 0)
            {
                return;
            }
            XCor = char.ToUpper(Start[0]) - 65;
            YCor = int.Parse(Start[1..]) - 1;

            if (XCor > brain.GameBoards[brain._currentPlayerNo].Board!.GetUpperBound(0) 
            || YCor > brain.GameBoards[brain._currentPlayerNo].Board!.GetUpperBound(1))
            {
                ModelState.AddModelError("XCor", $"Invalid coordinate ({Start}), out of bound");
                return;
            }

            if (XCor + ship.Height - 1> brain.GameBoards[brain._currentPlayerNo].Board!.GetUpperBound(0)
                || YCor + ship.Length - 1 > brain.GameBoards[brain._currentPlayerNo].Board!.GetUpperBound(1))
            {
                ModelState.AddModelError("Ship Out", $"Invalid coordinate ({Start}),ship would be out of board");
                return;
            }
            
            if (brain.TryPlaceShip(XCor, YCor, ship) == false)
            {
                ModelState.AddModelError("Ship", $"Can't place ship there because of the rule. Can ships touch? {brain.ShipRule} ");
            }
        }
    }
}