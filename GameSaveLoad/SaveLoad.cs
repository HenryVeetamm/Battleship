using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BattleShipGameBrain;
using DAL;
using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GameSaveLoad
{
    public static class SaveLoad
    {
        public static void SetJsonGame(Game game, BattleshipBrain brain)
        {
            brain.Game = game;

            var gameBoard1 = game.GameBoard1;

            brain.GameBoards[0].Board =
                brain.ConvertBoardToMulti(JsonSerializer.Deserialize<BoardSquareState[][]>(gameBoard1!.Board!)!);
            brain.GameBoards[0].Player = gameBoard1.Player;
            brain.GameBoards[0].Ships = gameBoard1.Ships!.ToList();

            var gameBoard2 = game.GameBoard2;

            brain.GameBoards[1].Board =
                brain.ConvertBoardToMulti(JsonSerializer.Deserialize<BoardSquareState[][]>(gameBoard2!.Board!)!);
            brain.GameBoards[1].Player = gameBoard2.Player;
            brain.GameBoards[1].Ships = gameBoard2.Ships!.ToList();

            brain._currentPlayerNo = game.CurrentPlayer;
            brain.MoveRule = game.EMoveAfterHit;
            brain.ShipRule = game.EShipTouchRule;

            brain.BombMoveHistory = JsonSerializer.Deserialize<List<Tuple<Coordinate, int>>>(brain.Game.BombMoveHistory!)!;

            brain.IsGameOver = game.IsGameOver;
        }
        
        public static void SaveGameJson(string gameName, BattleshipBrain brain)
        {
            var gameBoardState1 = new GameBoardState()
            {
                Board = JsonSerializer.Serialize(brain.ConvertBoardToJagged(brain.GameBoards[0].Board!)),
                Player = brain.GameBoards[0].Player!,
                Ships = brain.GameBoards[0].Ships
            };

            var gameBoardState2 = new GameBoardState()
            {
                Board = JsonSerializer.Serialize(brain.ConvertBoardToJagged(brain.GameBoards[1].Board!)),
                Player = brain.GameBoards[1].Player!,
                Ships = brain.GameBoards[1].Ships
            };

            var game = new Game()
            {
                EMoveAfterHit = brain.MoveRule,
                EShipTouchRule = brain.ShipRule,
                GameBoard1 = gameBoardState1,
                GameBoard2 = gameBoardState2,
                CurrentPlayer = brain._currentPlayerNo,
                BombMoveHistory = JsonSerializer.Serialize(brain.BombMoveHistory),
                IsGameOver = brain.IsGameOver,
                DateTime = DateTime.Now,
                GameName = gameName,
                SaveType = ESaveType.Json,
            };

            if (brain.Game is not null && brain.Game.SaveType == ESaveType.Database) game.InDatabase = true;

            brain.Game = game;

            var indented = new JsonSerializerOptions()
                { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve };

            var serializedGame = JsonSerializer.Serialize(game, indented);

            var separator = Path.DirectorySeparatorChar;

            var basePath = Directory.GetCurrentDirectory() + separator + "JSONSaves";

            var file = $"{basePath}{separator}{gameName}.json";

            File.WriteAllText(file, serializedGame);
        }
        
        public static async Task<Game> GetGameAsync(int gameId)
        {
            await using var ctx = new ApplicationDbContext();

            var game = await ctx.Games.Where(p => p.GameId == gameId)
                .Include(game => game.GameBoard1).ThenInclude(board => board!.Player)
                .Include(game => game.GameBoard2).ThenInclude(board => board!.Player)
                .Include(game => game.GameBoard1).ThenInclude(board => board!.Ships)
                .ThenInclude(ship => ship.Coordinates)
                .Include(game => game.GameBoard2).ThenInclude(board => board!.Ships)
                .ThenInclude(ship => ship.Coordinates).FirstOrDefaultAsync();

            return game!;
        }
        
        public static void SetDatabaseGame(Game game, BattleshipBrain brain)
        {
            brain.Game = game;
            brain._currentPlayerNo = game.CurrentPlayer;

            var board1 = 
                brain.ConvertBoardToMulti(JsonSerializer.Deserialize<BoardSquareState[][]>(game.GameBoard1!.Board!)!);

            brain.GameBoards[0].Board = board1;
            brain.GameBoards[0].Player = game.GameBoard1!.Player;
            brain.GameBoards[0].Ships = game.GameBoard1.Ships!;

            var board2 =
                brain.ConvertBoardToMulti(JsonSerializer.Deserialize<BoardSquareState[][]>(game.GameBoard2!.Board!)!);
            
            brain.GameBoards[1].Board = board2;
            brain.GameBoards[1].Player = game.GameBoard2.Player;
            brain.GameBoards[1].Ships = game.GameBoard2.Ships!;


            brain.ShipRule = game.EShipTouchRule;
            brain.MoveRule = game.EMoveAfterHit;
            brain.BombMoveHistory = JsonSerializer.Deserialize<List<Tuple<Coordinate, int>>>(game.BombMoveHistory!)!;
            brain.IsGameOver = game.IsGameOver;
        }
        
        public static void SetDatabaseGameWithId(int id, BattleshipBrain brain) => SetDatabaseGame(GetGame(id), brain);
        
        public static Game GetGame(int gameId)
        {
            using var ctx = new ApplicationDbContext();

            var game = ctx.Games.Where(p => p.GameId == gameId)
                .Include(game => game.GameBoard1).ThenInclude(board => board!.Player)
                .Include(game => game.GameBoard2).ThenInclude(board => board!.Player)
                .Include(game => game.GameBoard1).ThenInclude(board => board!.Ships)
                .ThenInclude(ship => ship.Coordinates)
                .Include(game => game.GameBoard2).ThenInclude(board => board!.Ships)
                .ThenInclude(ship => ship.Coordinates).FirstOrDefault();

            return game!;
        }
        
        public static void UpdateDatabaseGame(BattleshipBrain brain)
        {
            using var ctx = new ApplicationDbContext();

            var player1SerializedBoard = JsonSerializer.Serialize(brain.ConvertBoardToJagged(brain.GameBoards[0].Board!));
            brain.Game!.GameBoard1!.Board = player1SerializedBoard;
            brain.Game!.GameBoard1!.Ships = brain.GameBoards[0].Ships;
            var cs = brain.GetHitRate();

            var player2SerializedBoard = JsonSerializer.Serialize(brain.ConvertBoardToJagged(brain.GameBoards[1].Board!));
            brain.Game!.GameBoard2!.Board = player2SerializedBoard;
            brain.Game!.GameBoard2!.Ships = brain.GameBoards[1].Ships;

            brain.Game.CurrentPlayer = brain._currentPlayerNo;
            brain.Game.IsGameOver = brain.IsGameOver;
            brain.Game.BombMoveHistory = JsonSerializer.Serialize(brain.BombMoveHistory);

            ctx.Update(brain.Game);
            ctx.SaveChanges();
        }
        
        public static int SaveGameToDb(string gameName, BattleshipBrain brain)
        {
            if (brain.Game is not null 
                && brain.Game!.InDatabase 
                && brain.Game!.PredefinedGame == false)
            {
                UpdateDatabaseGame(brain);
                return brain.Game.GameId;
            }

            using var dbContext = new ApplicationDbContext();

            brain.GameBoards[0].Ships.ForEach(ship => ship.ShipId = 0);
            var gameBoard1 = new GameBoardState()
            {
                Board = JsonSerializer.Serialize(
                    brain.ConvertBoardToJagged(brain.GameBoards[0].Board!)),
                Player = brain.GameBoards[0].Player,
                Ships = brain.GameBoards[0].Ships
            };

            brain.GameBoards[1].Ships.ForEach(ship => ship.ShipId = 0);
            var gameBoard2 = new GameBoardState()
            {
                Board = JsonSerializer.Serialize(
                    brain.ConvertBoardToJagged(brain.GameBoards[1].Board!)),
                Player = brain.GameBoards[1].Player,
                Ships = brain.GameBoards[1].Ships
            };

            var game = new Game()
            {
                GameBoard1 = gameBoard1,
                GameBoard2 = gameBoard2,
                EMoveAfterHit = brain.MoveRule,
                EShipTouchRule = brain.ShipRule,
                CurrentPlayer = brain._currentPlayerNo,
                BombMoveHistory = JsonSerializer.Serialize(brain.BombMoveHistory),
                IsGameOver = brain.IsGameOver,
                DateTime = DateTime.Now,
                GameName = gameName,
                SaveType = ESaveType.Database,
                InDatabase = true,
                PredefinedGame = false
            };
            brain.Game = game;

            dbContext.Games.Add(brain.Game);
            dbContext.SaveChanges();
            dbContext.Dispose();
            return game.GameId;
        }
    }
}