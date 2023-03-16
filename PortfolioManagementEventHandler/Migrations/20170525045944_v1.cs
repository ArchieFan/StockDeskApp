using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StockDesk.PortfolioManagementEventHandler.Migrations
{
    public partial class v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    ClientId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TelephoneNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    Ticker = table.Column<string>(nullable: false),
                    CompanyName = table.Column<string>(nullable: true),
                    OwnerId = table.Column<string>(nullable: true),
                    Industry = table.Column<string>(nullable: true),
                    MarketCap = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.Ticker);
                });

            migrationBuilder.CreateTable(
                name: "Trading",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ActualEndTime = table.Column<DateTime>(nullable: true),
                    ActualStartTime = table.Column<DateTime>(nullable: true),
                    ClientId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    StockTicker = table.Column<string>(nullable: true),
                    PortfolioPlanningDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trading", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trading_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trading_Stock_StockTicker",
                        column: x => x.StockTicker,
                        principalTable: "Stock",
                        principalColumn: "Ticker",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trading_ClientId",
                table: "Trading",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Trading_StockTicker",
                table: "Trading",
                column: "StockTicker");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trading");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "Stock");
        }
    }
}
