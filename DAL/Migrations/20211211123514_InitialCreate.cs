using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "GameBoardStates",
                columns: table => new
                {
                    GameBoardStateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Board = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameBoardStates", x => x.GameBoardStateId);
                    table.ForeignKey(
                        name: "FK_GameBoardStates_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameBoard1Id = table.Column<int>(type: "int", nullable: false),
                    GameBoard2Id = table.Column<int>(type: "int", nullable: false),
                    EMoveAfterHit = table.Column<int>(type: "int", nullable: true),
                    EShipTouchRule = table.Column<int>(type: "int", nullable: true),
                    CurrentPlayer = table.Column<int>(type: "int", nullable: false),
                    BombMoveHistory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsGameOver = table.Column<bool>(type: "bit", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GameName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SaveType = table.Column<int>(type: "int", nullable: false),
                    InDatabase = table.Column<bool>(type: "bit", nullable: false),
                    PredefinedGame = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_Games_GameBoardStates_GameBoard1Id",
                        column: x => x.GameBoard1Id,
                        principalTable: "GameBoardStates",
                        principalColumn: "GameBoardStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_GameBoardStates_GameBoard2Id",
                        column: x => x.GameBoard2Id,
                        principalTable: "GameBoardStates",
                        principalColumn: "GameBoardStateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ships",
                columns: table => new
                {
                    ShipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Length = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Placed = table.Column<bool>(type: "bit", nullable: false),
                    GameBoardStateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ships", x => x.ShipId);
                    table.ForeignKey(
                        name: "FK_Ships_GameBoardStates_GameBoardStateId",
                        column: x => x.GameBoardStateId,
                        principalTable: "GameBoardStates",
                        principalColumn: "GameBoardStateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Coordinates",
                columns: table => new
                {
                    CoordinateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipId = table.Column<int>(type: "int", nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coordinates", x => x.CoordinateId);
                    table.ForeignKey(
                        name: "FK_Coordinates_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "ShipId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coordinates_ShipId",
                table: "Coordinates",
                column: "ShipId");

            migrationBuilder.CreateIndex(
                name: "IX_GameBoardStates_PlayerId",
                table: "GameBoardStates",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameBoard1Id",
                table: "Games",
                column: "GameBoard1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameBoard2Id",
                table: "Games",
                column: "GameBoard2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_GameBoardStateId",
                table: "Ships",
                column: "GameBoardStateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coordinates");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Ships");

            migrationBuilder.DropTable(
                name: "GameBoardStates");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
