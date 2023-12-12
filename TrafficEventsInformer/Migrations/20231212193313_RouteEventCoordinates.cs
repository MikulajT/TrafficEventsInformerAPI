using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrafficEventsInformer.Migrations
{
    /// <inheritdoc />
    public partial class RouteEventCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteEvent_TrafficRoute_RouteId",
                table: "RouteEvent");

            migrationBuilder.DropIndex(
                name: "IX_RouteEvent_RouteId",
                table: "RouteEvent");

            migrationBuilder.AddColumn<double>(
                name: "EndPointX",
                table: "RouteEvent",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EndPointY",
                table: "RouteEvent",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StartPointX",
                table: "RouteEvent",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StartPointY",
                table: "RouteEvent",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_RouteEvent_Id",
                table: "RouteEvent",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RouteEvent_TrafficRoute_Id",
                table: "RouteEvent",
                column: "RouteId",
                principalTable: "TrafficRoute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteEvent_TrafficRoute_RouteId",
                table: "RouteEvent");

            migrationBuilder.DropIndex(
                name: "IX_RouteEvent_RouteId",
                table: "RouteEvent");

            migrationBuilder.DropColumn(
                name: "EndPointX",
                table: "RouteEvent");

            migrationBuilder.DropColumn(
                name: "EndPointY",
                table: "RouteEvent");

            migrationBuilder.DropColumn(
                name: "StartPointX",
                table: "RouteEvent");

            migrationBuilder.DropColumn(
                name: "StartPointY",
                table: "RouteEvent");

            migrationBuilder.CreateIndex(
                name: "IX_RouteEvent_Id",
                table: "RouteEvent",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RouteEvent_TrafficRoute_Id",
                table: "RouteEvent",
                column: "RouteId",
                principalTable: "TrafficRoute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
