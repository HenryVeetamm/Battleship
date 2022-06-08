using System;
using BattleShipGameBrain;
using ColorString;
using Domain;
using Domain.Enums;
using static System.Console;

namespace BattleShipConsoleUI
{
    public static class BattleshipsUI
    {
        private static void DrawBoard(BattleshipBrain brain,
            int? x, int? y,
            int left, int top,
            int playerNumber,
            EVisibility visibility)
        {
            var board = brain.GameBoards[playerNumber].Board;
            var width = board!.GetUpperBound(0) + 1; // x
            var height = board.GetUpperBound(1) + 1; // ye


            BoardHeader(width, left, top);

            for (int rowIndex = 0; rowIndex < height; rowIndex++)
            {
                if ((rowIndex + 1).ToString().Length == 2)
                {
                    SetCursorPosition(left, CursorTop);
                    Write(rowIndex + 1 + " ");
                }
                else
                {
                    SetCursorPosition(left, CursorTop);
                    Write(rowIndex + 1 + "  ");
                }

                for (int colIndex = 0; colIndex < width; colIndex++)
                {
                    if (visibility == EVisibility.Hidden && rowIndex == x && colIndex == y)
                    {
                        ShowCursor();
                    }
                    else
                    {
                        CellColor(board, colIndex, rowIndex, visibility);
                    }
                }

                BoardFooter(width, left);
            }
        }

        private static void ShowCursor()
        {
            ColoredString.WriteString("| ", ConsoleColor.Yellow);
            ColoredString.WriteString("C", ConsoleColor.Cyan);
            ColoredString.WriteString(" |", ConsoleColor.Yellow);
        }

        private static void CellColor(BoardSquareState[,] board, int colIndex, int rowIndex, EVisibility visibility)
        {
            ColoredString.WriteString("| ", ConsoleColor.Yellow);


            if (board[colIndex, rowIndex].IsBomb && board[colIndex, rowIndex].IsShip)
            {
                ColoredString.WriteString(board[colIndex, rowIndex].ToString(), ConsoleColor.Red);
            }
            else if (board[colIndex, rowIndex].IsShip && visibility == EVisibility.Visible)
            {
                ColoredString.WriteString(board[colIndex, rowIndex].ToString(), ConsoleColor.Green);
            }
            else if (board[colIndex, rowIndex].IsBomb)
            {
                ColoredString.WriteString(board[colIndex, rowIndex].ToString(), ConsoleColor.DarkRed);
            }
            else if (visibility == EVisibility.Hidden)
            {
                Write(new BoardSquareState() { IsBomb = false, IsShip = false });
            }
            else
            {
                Write(board[colIndex, rowIndex]);
            }

            ColoredString.WriteString(" |", ConsoleColor.Yellow);
        }

        public static void PlaceBomb(BattleshipBrain brain)
        {
            var xCoordinate = 0;
            var yCoordinate = 0;
            var run = true;
            EMoveResult? moveResult = null;
            ConsoleKeyInfo keyInfo = default;

            var message = "";
            ConsoleColor color = ConsoleColor.Red;
            do
            {
                Clear();

                if (moveResult is not null && moveResult == EMoveResult.Invalid || moveResult == EMoveResult.Hit)
                {
                    ColoredString.WriteLineString(message, color);
                    moveResult = null;
                }
                
                if (brain.CheckForWin())
                {
                    break;
                }
                
                if (brain.GameBoards[brain._currentPlayerNo].Player!.PlayerType == EPlayerType.Human)
                {
                    DrawBoards(brain, xCoordinate, yCoordinate);
                    keyInfo = ReadKey();
                }

                //COMPUTER
                if (brain.GameBoards[brain._currentPlayerNo].Player!.PlayerType == EPlayerType.Pc)
                {
                    var random = new Random();
                    xCoordinate = random.Next(0, brain.GameBoards[0].Board!.GetUpperBound(1) + 1);
                    yCoordinate = random.Next(0, brain.GameBoards[0].Board!.GetUpperBound(0) + 1);

                    keyInfo = new ConsoleKeyInfo('E', ConsoleKey.Enter, false, false, false);
                }

                switch (keyInfo.Key)
                {
                    case ConsoleKey.DownArrow:
                        xCoordinate = BattleShipUIBrain.MoveDownBomb(brain, xCoordinate);
                        break;
                    case ConsoleKey.UpArrow:
                        xCoordinate = BattleShipUIBrain.MoveUpBomb(brain, xCoordinate);
                        break;
                    case ConsoleKey.RightArrow:
                        yCoordinate = BattleShipUIBrain.MoveRightBomb(brain, yCoordinate);
                        break;
                    case ConsoleKey.LeftArrow:
                        yCoordinate = BattleShipUIBrain.MoveLeftBomb(brain, yCoordinate);
                        break;
                    case ConsoleKey.Enter:
                        moveResult = brain.PlaceBomb(yCoordinate, xCoordinate);
                        if (moveResult == EMoveResult.Invalid)
                        {
                            message = "You already bombed this square, try again";
                            color = ConsoleColor.Red;
                        }

                        if (moveResult == EMoveResult.Hit)
                        {
                            if (brain.CheckForWin())
                            {
                                run = false;
                                break;
                            }
                            message = "You hit a ship.";
                            color = ConsoleColor.Green;
                        }

                        if (moveResult == EMoveResult.Miss || brain.MoveRule == EMoveAfterHit.OtherPlayer &&
                            moveResult != EMoveResult.Invalid)
                        {
                            Clear();
                            PlayerChangeScreen(moveResult.ToString()!, brain);
                        }
                        xCoordinate = 0;
                        yCoordinate = 0;
                        break;
                    case ConsoleKey.Escape:
                        run = false;
                        break;
                    case ConsoleKey.U:
                        if (brain.BombMoveHistory.Count != 0) brain.UndoMove();
                        break;
                }
            } while (run);
        }

        private static void PlayerChangeScreen(string moveResult, BattleshipBrain brain)
        {
            var run = true;
            if (brain.GameBoards[brain._currentPlayerNo].Player!.PlayerType == EPlayerType.Pc) return;
            do
            {
                Information.PlayerChangeInfo(moveResult, brain);

                var keyInput = ReadKey();
                switch (keyInput.Key)
                {
                    case ConsoleKey.Enter:
                        run = false;
                        break;
                    case ConsoleKey.U:
                        brain.UndoMove();
                        run = false;
                        break;
                }
            } while (run);
        }

        public static void PlaceShip(BattleshipBrain brain)
        {
            var xCoordinate = 0;
            var yCoordinate = 0;
            var run = true;
            var errorBool = false;
            var errorMessage = "";
            do
            {
                if (brain.PlayerBoatsPlaced(brain._currentPlayerNo))
                {
                    brain.SwitchPlayer();

                    //AGAINST PC
                    if (brain.GameBoards[brain._currentPlayerNo].Player!.PlayerType == EPlayerType.Pc)
                    {
                        brain.PlacePlayerShipsRandomly(brain._currentPlayerNo);
                        brain.SwitchPlayer();
                    }

                    //END
                    if (brain.AllBoatsPlaced())
                    {
                        break;
                    }
                }

                var ship = brain.GetFirstNotPlacedShip();
                Clear();

                if (errorBool)
                {
                    ColoredString.WriteLineString(errorMessage, ConsoleColor.Red);
                    errorBool = false;
                }

                DrawPlaceShipBoard(brain, ship, xCoordinate, yCoordinate);
               
                Information.ShipPlacingMovement();
                
                Information.ShowCurrentPlayerName(brain.GameBoards[brain._currentPlayerNo].Player!.Name);

                var keyInfo = ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.DownArrow:
                        xCoordinate = BattleShipUIBrain.MoveDownShip(brain, xCoordinate, ship);
                        break;
                    case ConsoleKey.UpArrow:
                        xCoordinate = BattleShipUIBrain.MoveUpShip(brain, xCoordinate, ship);
                        break;
                    case ConsoleKey.RightArrow:
                        yCoordinate = BattleShipUIBrain.MoveRightShip(brain, yCoordinate, ship);
                        break;
                    case ConsoleKey.LeftArrow:
                        yCoordinate = BattleShipUIBrain.MoveLeftShip(brain, yCoordinate, ship);
                        break;
                    case ConsoleKey.Enter:
                        errorBool = brain.PlaceShip(yCoordinate, xCoordinate, ship);
                        if (errorBool)
                        {
                            errorMessage = "Cant place ship there!";
                        }
                        else
                        {
                            xCoordinate = 0;
                            yCoordinate = 0;
                        }

                        break;
                    case ConsoleKey.R:
                        if (BattleShipUIBrain.Rotate(brain, ship, yCoordinate, xCoordinate))
                        {
                            (ship.Height, ship.Length) = (ship.Length, ship.Height);
                        }

                        break;
                    case ConsoleKey.Escape:
                        run = false;
                        break;
                    case ConsoleKey.Q:
                        brain.PlacePlayerShipsRandomly(brain._currentPlayerNo);
                        break;
                }
            } while (run);
        }

        private static void BoardHeader(int width, int left, int top)
        {
            var leftCor = left;
            var topCor = top;
            SetCursorPosition(leftCor, topCor);
            Write("   ");
            leftCor += 3;
            for (var colIndex = 0; colIndex < width; colIndex++)
            {
                SetCursorPosition(leftCor, topCor);
                Write("  " + Convert.ToChar(colIndex + 'A') + "  ");
                leftCor += 5;
            }

            SetCursorPosition(leftCor, topCor);
            WriteLine();
            SetCursorPosition(leftCor, topCor);
            Write("   ");

            topCor += 1;
            leftCor = left + 3;
            for (var colIndex = 0; colIndex < width; colIndex++)
            {
                SetCursorPosition(leftCor, topCor);
                ColoredString.WriteString("+---+", ConsoleColor.Yellow);
                leftCor += 5;
            }

            WriteLine();
        }

        private static void BoardFooter(int width, int left)
        {
            WriteLine();
            SetCursorPosition(left, CursorTop);
            Write("   ");
            var leftCor = left + 3;
            for (int colIndex = 0; colIndex < width; colIndex++)
            {
                SetCursorPosition(leftCor, CursorTop);
                ColoredString.WriteString("+---+", ConsoleColor.Yellow);
                leftCor += 5;
            }

            WriteLine();
        }

        private static void DrawPlaceShipBoard(BattleshipBrain brain, Ship ship, int x, int y)
        {

            WriteLine($"You are placing a ship: {ship.Name} with length of {ship!.Length} " +
                      $"and width of {ship.Height}\n");

            var board = brain.GameBoards[brain._currentPlayerNo].Board;

            var width = board!.GetUpperBound(0) + 1; // x
            var height = board.GetUpperBound(1) + 1; // y

            BoardHeader(width, CursorLeft, CursorTop);

            var shipSize = ship.Length - 1;
            var shipSize2 = ship.Height - 1;
            var tempY = y;


            for (var rowIndex = 0; rowIndex < height; rowIndex++)
            {
                if ((rowIndex + 1).ToString().Length == 2)
                {
                    Write(rowIndex + 1 + " ");
                }
                else
                {
                    Write(rowIndex + 1 + "  ");
                }


                for (var colIndex = 0; colIndex < width; colIndex++)
                {
                    if (colIndex == tempY && rowIndex == x && shipSize2 != 0)
                    {
                        ColoredString.WriteString("| ", ConsoleColor.Yellow);
                        Write("S");
                        ColoredString.WriteString(" |", ConsoleColor.Yellow);
                        tempY += 1;
                        shipSize2 -= 1;
                    }
                    else if (rowIndex == x && colIndex == tempY)
                    {
                        ColoredString.WriteString("| ", ConsoleColor.Yellow);
                        Write("S");
                        ColoredString.WriteString(" |", ConsoleColor.Yellow);
                        if (shipSize != 0)
                        {
                            x += 1;
                            shipSize -= 1;
                        }
                    }
                    else
                    {
                        CellColor(board, colIndex, rowIndex, EVisibility.Visible);
                    }
                }

                BoardFooter(width, 0);
                shipSize2 = ship.Height - 1;
                tempY = y;
            }
        }
        

        public static void ShowResult(BattleshipBrain brain)
        {
            var cursorLeft1 = WindowWidth / 2 -
                              ((brain.GameBoards[brain._currentPlayerNo].Board!.GetUpperBound(0) + 1) * 3 + 25);
            var cursorLeft2 = WindowWidth / 2 + 15;
            var cursorTop = 0;

            var winnerInt = brain.GetWinnerNo();
            var loserInt = brain.GetLoserNo();

            Clear();
            Information.ShowGameOverKey();

            Information.GameOverInfo(cursorLeft1, cursorTop,
                $"Winner is: {brain.GameBoards[winnerInt].Player!.Name}", ConsoleColor.Green);

            DrawBoard(brain, null, null, cursorLeft1, cursorTop + 2, winnerInt, EVisibility.Visible);

            Information.GameOverInfo(cursorLeft2, cursorTop,
                $"Loser is: {brain.GameBoards[loserInt].Player!.Name}", ConsoleColor.Red);
            DrawBoard(brain, null, null, cursorLeft2, cursorTop + 2, loserInt, EVisibility.Visible);

            var run = true;
            do
            {
                var key = ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        run = false;
                        break;
                }
            } while (run);
        }

        private static void DrawBoards(BattleshipBrain brain, int x, int y)
        {
            var cursorLeft1 = WindowWidth / 2 -
                              ((brain.GameBoards[brain._currentPlayerNo].Board!.GetUpperBound(0) + 1) * 3 + 40);
            var cursorLeft2 = WindowWidth / 2 + 5;
            var cursorTop = 0;
            
            Information.Statistics(brain);
            Information.ShowGameControls();
            Information.SunkenShips(brain);

            SetCursorPosition(cursorLeft1 + 3, cursorTop);
            WriteLine("Opponents board");
            DrawBoard(brain, x, y, cursorLeft1, cursorTop + 1, brain.OtherPlayer(), EVisibility.Hidden);

            SetCursorPosition(cursorLeft2 + 3, cursorTop);
            WriteLine("Your board");
            DrawBoard(brain, null, null, cursorLeft2, cursorTop + 1, brain._currentPlayerNo, EVisibility.Visible);
        }
    }
}