using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrafficEventsInformer.Migrations
{
    /// <inheritdoc />
    public partial class MoveNameToJoiningTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "RouteEvents");

            migrationBuilder.CreateTable(
                name: "TrafficRouteRouteEvents",
                columns: table => new
                {
                    TrafficRouteId = table.Column<int>(type: "int", nullable: false),
                    RouteEventId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrafficRouteRouteEvents", x => new { x.TrafficRouteId, x.RouteEventId });
                    table.ForeignKey(
                        name: "FK_TrafficRouteRouteEvents_RouteEvents_RouteEventId",
                        column: x => x.RouteEventId,
                        principalTable: "RouteEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrafficRouteRouteEvents_TrafficRoutes_TrafficRouteId",
                        column: x => x.TrafficRouteId,
                        principalTable: "TrafficRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrafficRouteRouteEvents_RouteEventId",
                table: "TrafficRouteRouteEvents",
                column: "RouteEventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrafficRouteRouteEvents");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "RouteEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
