using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrafficEventsInformer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEntitiesRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RouteEvent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartPointX = table.Column<double>(type: "float", nullable: false),
                    StartPointY = table.Column<double>(type: "float", nullable: false),
                    EndPointX = table.Column<double>(type: "float", nullable: false),
                    EndPointY = table.Column<double>(type: "float", nullable: false),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    Expired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrafficRoute",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Coordinates = table.Column<string>(type: "xml", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrafficRoute", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteEventTrafficRoute",
                columns: table => new
                {
                    EventsId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    TrafficRouteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteEventTrafficRoute", x => new { x.EventsId, x.TrafficRouteId });
                    table.ForeignKey(
                        name: "FK_RouteEventTrafficRoute_RouteEvent_EventsId",
                        column: x => x.EventsId,
                        principalTable: "RouteEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteEventTrafficRoute_TrafficRoute_TrafficRouteId",
                        column: x => x.TrafficRouteId,
                        principalTable: "TrafficRoute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RouteEventTrafficRoute_TrafficRouteId",
                table: "RouteEventTrafficRoute",
                column: "TrafficRouteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RouteEventTrafficRoute");

            migrationBuilder.DropTable(
                name: "RouteEvent");

            migrationBuilder.DropTable(
                name: "TrafficRoute");
        }
    }
}
