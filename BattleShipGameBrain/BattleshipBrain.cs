using System;
using System.Collections.Generic;
using System.Linq;
using BattleShipGameBrain.Interfaces;
using Domain;
using Domain.Enums;
using MenuSystem;


namespace BattleShipGameBrain
{
    public class BattleshipBrain
    {
        public Game? Game;

        public int _currentPlayerNo = 0;

        public GameBoard[] GameBoards = new GameBoard[2] { new GameBoard(), new GameBoard() };
        public EShipTouchRule? ShipRule;
        public EMoveAfterHit? MoveRule;
        public List<Tuple<Coordinate, int>> BombMoveHistory = new List<Tuple<Coordinate, int>>();
        public bool IsGameOver = false;
        public bool AgainstPc = false;


        public void AddBoat(Ship ship)
        {
            GameBoards[0].Ships.Add(new Ship()
            {
                Name = ship.Name,
                Length = ship.Length,
                Height = ship.Height
            });
            GameBoards[1].Ships.Add(new Ship()
            {
                Name = ship.Name,
                Length = ship.Length,
                Height = ship.Height
            });
        }

        public List<Ship> GetShips() => GameBoards[1].Ships;

        public string RemoveShip(Ship ship)
        {
            GameBoards[0].Ships.Remove(ship);
            GameBoards[1].Ships.Remove(ship);
            return EReturn.R.ToString();
        }

        public void SetMoveRule(EMoveAfterHit rule) => MoveRule = rule;

        public void SetShipRule(EShipTouchRule rule) => ShipRule = rule;

        public void SetPlayer(int playerNo, string playerName) =>
            GameBoards[playerNo].Player = new Player() { Name = playerName };

        public void SetBoard(Tuple<int, int> boardSize)
        {
            var (item1, item2) = boardSize;

            GameBoards[0].Board = new BoardSquareState[item1, item2];
            GameBoards[1].Board = new BoardSquareState[item1, item2];

            for (var x = 0; x < item1; x++)
            {
                for (var y = 0; y < item2; y++)
                {
                    GameBoards[0].Board![x, y] = new BoardSquareState
                    {
                        IsBomb = false,
                        IsShip = false
                    };
                    GameBoards[1].Board![x, y] = new BoardSquareState()
                    {
                        IsBomb = false,
                        IsShip = false
                    };
                }
            }
        }

        public void SetClassicGame(IGameConfig conf)
        {
            GameBoards[0].Ships = CopyOfList(conf.Ships);
            GameBoards[1].Ships = CopyOfList(conf.Ships);

            ShipRule = conf.ShipRule;
            MoveRule = conf.MoveRule;

            GameBoards[0].Player = conf.Player1;
            GameBoards[1].Player = conf.Player2;

            SetBoard(new Tuple<int, int>(conf.BoardSizeX, conf.BoardSizeY));
        }

        private static List<Ship> CopyOfList(List<Ship> ships) =>
            ships.Select(ship => new Ship() { Name = ship.Name, Length = ship.Length, Height = ship.Height }).ToList();

        public EMoveResult PlaceBomb(int x, int y)
        {
            if (TryPlaceBomb(x, y))
            {
                GameBoards[OtherPlayer()].Board![x, y].IsBomb = true;
                if (GameBoards[OtherPlayer()].Board![x, y].IsShip)
                {
                    BombMoveHistory.Add(new Tuple<Coordinate, int>(new Coordinate() { X = x, Y = y }, OtherPlayer()));

                    PlayerChangeToRule();

                    return EMoveResult.Hit;
                }
                else
                {
                    BombMoveHistory.Add(new Tuple<Coordinate, int>(new Coordinate() { X = x, Y = y }, OtherPlayer()));
                    SwitchPlayer();
                    return EMoveResult.Miss;
                }
            }

            return EMoveResult.Invalid;
        }

        private void PlayerChangeToRule()
        {
            switch (MoveRule)
            {
                case EMoveAfterHit.SamePlayer:
                    break;
                case EMoveAfterHit.OtherPlayer:
                    SwitchPlayer();
                    break;
            }
        }

        public static BoardSquareState[,] HiddenBoatsBoard(BoardSquareState[,] visibleBoard)
        {
            var newBoard = new BoardSquareState[visibleBoard.GetUpperBound(0) + 1, visibleBoard.GetUpperBound(1) + 1];

            for (int x = 0; x < visibleBoard.GetUpperBound(1) + 1; x++)
            {
                for (int y = 0; y < visibleBoard.GetUpperBound(0) + 1; y++)
                {
                    if (visibleBoard[y, x].IsBomb && visibleBoard[y, x].IsShip)
                    {
                        newBoard[y, x] = visibleBoard[y, x];
                    }
                    else
                    {
                        newBoard[y, x] = new BoardSquareState() { IsBomb = visibleBoard[y, x].IsBomb, IsShip = false };
                    }
                }
            }

            return newBoard;
        }

        public string GetCurrentPlayerName() => GameBoards[_currentPlayerNo].Player!.Name;

        public string GetOtherPlayerName() => GameBoards[OtherPlayer()].Player!.Name;

        private bool TryPlaceBomb(int x, int y) => GameBoards[OtherPlayer()].Board![x, y].IsBomb == false;

        public bool PlaceShip(int x, int y, Ship ship)
        {
            if (TryPlaceShip(x, y, ship))
            {
                ship.SetShipCoordinates(new Coordinate() { X = x, Y = y });

                foreach (var coordinate in ship.Coordinates)
                {
                    GameBoards[_currentPlayerNo].Board![coordinate.X, coordinate.Y].IsShip = true;
                }

                ship.Placed = true;
                return false;
            }

            return true;
        }

        public bool TryPlaceShip(int X, int Y, Ship ship)
        {
            for (var y = Y; y < Y + ship.Length; y++)
            {
                for (var x = X; x < X + ship.Height; x++)
                {
                    if (GameBoards[_currentPlayerNo].Board![x, y].IsShip || CheckAccordingToRule(ship, X, Y))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void PlacePlayerShipsRandomly(int playerNo)
        {
            var random = new Random();
            var board = GameBoards[playerNo].Board;
            foreach (var ship in GameBoards[playerNo].Ships
                .OrderByDescending(ship => ship.Height * ship.Length))
            {
                if (ship.Placed) continue;
                while (true)
                {
                    var turn = random.Next(0, 2);

                    if (turn == 1)
                    {
                        (ship.Height, ship.Length) = (ship.Length, ship.Height);
                    }

                    var x = random.Next(0, board!.GetUpperBound(1) - ship.Length + 1);
                    var y = random.Next(0, board.GetUpperBound(0) - ship.Height + 1);

                    while (board[y, x].IsShip == true)
                    {
                        x = random.Next(0, board.GetLength(1) - ship.Length);
                        y = random.Next(1, board.GetLength(0) - ship.Height + 1);
                    }

                    if (PlaceShip(y, x, ship) == false) break;
                }
            }
        }

        private bool CheckAccordingToRule(Ship ship, int X, int Y)
        {
            return ShipRule switch
            {
                EShipTouchRule.NoTouch => CheckSide(ship, X, Y),
                EShipTouchRule.CornerTouch => DiagonalCheck(ship, X, Y),
                EShipTouchRule.Yes => false,
                _ => true //never
            };
        }

        private bool CheckSide(Ship ship, int X, int Y)
        {
            for (var y = Y - 1; y < Y + ship.Length + 1; y++)
            {
                for (var x = X; x < X + ship.Height; x++)
                {
                    if (x < 0 || x == GameBoards[0].Board!.GetUpperBound(0) + 1
                              || y == GameBoards[0].Board!.GetUpperBound(1) + 1 || y < 0)
                    {
                        continue;
                    }

                    if (GameBoards[_currentPlayerNo].Board![x, y].IsShip)
                    {
                        return true;
                    }
                }
            }

            for (var y = Y; y < Y + ship.Length; y++)
            {
                for (var x = X - 1; x < X + ship.Height + 1; x++)
                {
                    if (x < 0 || x == GameBoards[0].Board!.GetUpperBound(0) + 1
                              || y == GameBoards[0].Board!.GetUpperBound(1) + 1 || y < 0)
                    {
                        continue;
                    }

                    if (GameBoards[_currentPlayerNo].Board![x, y].IsShip)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public double GetHitRate()
        {
            var bombs = 0;
            var hit = 0;

            foreach (var square in GameBoards[OtherPlayer()].Board!)
            {
                if (square.IsShip && square.IsBomb) hit++;
                if (square.IsBomb) bombs++;
            }

            return bombs == 0 ? 0 : Math.Round(hit / (double)bombs * 100, 2);
        }

        private bool DiagonalCheck(Ship ship, int X, int Y)
        {
            var count = 0;
            if (CheckSide(ship, X, Y))
            {
                for (var y = Y - 1; y < Y + ship.Length + 1; y++)
                {
                    for (var x = X; x < X + ship.Height; x++)
                    {
                        if (x < 0 || x == GameBoards[0].Board!.GetUpperBound(1) + 1
                                  || y == GameBoards[0].Board!.GetUpperBound(0) + 1 || y < 0) continue;
                        if (GameBoards[_currentPlayerNo].Board![x, y].IsShip) count++;
                    }
                }

                for (var y = Y; y < Y + ship.Length; y++)
                {
                    for (var x = X - 1; x < X + ship.Height + 1; x++)
                    {
                        if (x < 0 || x == GameBoards[0].Board!.GetUpperBound(1) + 1
                                  || y == GameBoards[0].Board!.GetUpperBound(0) + 1)
                        {
                            continue;
                        }

                        if (GameBoards[_currentPlayerNo].Board![x, y].IsShip)
                        {
                            count++;
                        }
                    }
                }
            }

            return count > 1;
        }

        public bool AllBoatsPlaced() => PlayerBoatsPlaced(0) && PlayerBoatsPlaced(1);

        public void SwitchPlayer() => _currentPlayerNo = _currentPlayerNo == 1 ? 0 : 1;

        public int OtherPlayer() => _currentPlayerNo == 0 ? 1 : 0;

        public bool CheckForWin()
        {
            if (MoveRule == EMoveAfterHit.OtherPlayer)
            {
                IsGameOver = GameBoards[OtherPlayer()].Ships.Count == GetSunkenShipsCount(_currentPlayerNo);
            }
            else
            {
                IsGameOver = GameBoards[OtherPlayer()].Ships.Count == GetSunkenShipsCount(OtherPlayer());
            }

            return IsGameOver;
        }

        public int GetWinnerNo()
        {
            return GameBoards[OtherPlayer()].Ships.Count == GetSunkenShipsCount(_currentPlayerNo)
                ? OtherPlayer()
                : _currentPlayerNo;
        }

        public int GetLoserNo()
        {
            return GameBoards[OtherPlayer()].Ships.Count == GetSunkenShipsCount(_currentPlayerNo)
                ? _currentPlayerNo
                : OtherPlayer();
        }

        public int GetSunkenShipsCount(int playerNo) =>
            GameBoards[playerNo].Ships.Count(ship => IsShipSunk(GameBoards[playerNo].Board!, ship));

        private static bool IsShipSunk(BoardSquareState[,] board, Ship ship) =>
            ship.Coordinates.All(c => board[c.X, c.Y].IsBomb);

        public List<Ship> GetAllSunkenShips(int playerNo) =>
            GameBoards[playerNo].Ships.FindAll(ship => IsShipSunk(GameBoards[playerNo].Board!, ship));

        public void UndoMove()
        {
            var last = BombMoveHistory.Last();

            (Coordinate coordinate, var playerNo) = last;


            if (MoveRule == EMoveAfterHit.SamePlayer && GameBoards[playerNo].Board![coordinate.X, coordinate.Y].IsShip)
            {
                GameBoards[playerNo].Board![coordinate.X, coordinate.Y].IsBomb = false;
                if (IsGameOver) IsGameOver = false;
            }
            else
            {
                GameBoards[playerNo].Board![coordinate.X, coordinate.Y].IsBomb = false;
                SwitchPlayer();
            }

            BombMoveHistory.Remove(last);
        }

        public bool PlayerBoatsPlaced(int playerNo) => GameBoards[playerNo].Ships.All(ship => ship.Placed);

        public Ship GetFirstNotPlacedShip() => GameBoards[_currentPlayerNo].Ships.Find(ship => ship.Placed == false)!;

        public BoardSquareState[][] ConvertBoardToJagged(BoardSquareState[,] board)
        {
            var newBoard = new BoardSquareState[board.GetLength(1)][];

            for (int i = 0; i < newBoard.Length; i++)
            {
                newBoard[i] = new BoardSquareState[board.GetLength(0)];
            }

            for (var x = 0; x < board.GetLength(1); x++)
            {
                for (var y = 0; y < board.GetLength(0); y++)
                {
                    newBoard[x][y] = board[y, x];
                }
            }

            return newBoard;
        }

        public BoardSquareState[,] ConvertBoardToMulti(BoardSquareState[][] board)
        {
            var height = board.Length;
            var width = board[0].Length;

            var newBoard = new BoardSquareState[width, height];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    newBoard[j, i] = board[i][j];
                }
            }

            return newBoard;
        }
    }
}