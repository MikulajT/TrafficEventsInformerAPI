using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrafficEventsInformer.Migrations
{
    /// <inheritdoc />
    public partial class DropTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RouteEvent");

            migrationBuilder.DropTable(
                name: "TrafficRoute");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrafficRoute",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Coordinates = table.Column<string>(type: "xml", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrafficRoute", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteEvent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndPointX = table.Column<double>(type: "float", nullable: false),
                    EndPointY = table.Column<double>(type: "float", nullable: false),
                    Expired = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartPointX = table.Column<double>(type: "float", nullable: false),
                    StartPointY = table.Column<double>(type: "float", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteEvent", x => new { x.Id, x.RouteId });
                    table.ForeignKey(
                        name: "FK_RouteEvent_TrafficRoute_RouteId",
                        column: x => x.RouteId,
                        principalTable: "TrafficRoute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RouteEvent_RouteId",
                table: "RouteEvent",
                column: "RouteId");
        }
    }
}
