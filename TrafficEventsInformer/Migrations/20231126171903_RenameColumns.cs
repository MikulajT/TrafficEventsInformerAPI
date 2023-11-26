using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrafficEventsInformer.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "TrafficRoute",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "RouteCoordinates",
                table: "TrafficRoute",
                newName: "Coordinates");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TrafficRoute",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "RouteEvent",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "RouteEvent",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "RouteEvent",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "RouteEvent");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "TrafficRoute",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Coordinates",
                table: "TrafficRoute",
                newName: "RouteCoordinates");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TrafficRoute",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "RouteEvent",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "RouteEvent",
                newName: "Id");
        }
    }
}
