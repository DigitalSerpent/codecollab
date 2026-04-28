using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeCollabFrontend.Migrations
{
    /// <inheritdoc />
    public partial class AddSignalRFieldsToRoomParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "RoomParticipants",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cursor",
                table: "RoomParticipants",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSeen",
                table: "RoomParticipants",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "RoomParticipants",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "RoomParticipants");

            migrationBuilder.DropColumn(
                name: "Cursor",
                table: "RoomParticipants");

            migrationBuilder.DropColumn(
                name: "LastSeen",
                table: "RoomParticipants");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "RoomParticipants");
        }
    }
}
