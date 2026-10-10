using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeCollabFrontend.Migrations
{
    /// <inheritdoc />
    public partial class FixModelPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomParticipants_Rooms_RoomId1",
                table: "RoomParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomParticipants_Users_UserId1",
                table: "RoomParticipants");

            migrationBuilder.DropIndex(
                name: "IX_RoomParticipants_RoomId1",
                table: "RoomParticipants");

            migrationBuilder.DropIndex(
                name: "IX_RoomParticipants_UserId1",
                table: "RoomParticipants");

            migrationBuilder.DropColumn(
                name: "RoomId1",
                table: "RoomParticipants");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "RoomParticipants");

            migrationBuilder.AlterColumn<string>(
                name: "TelegramChatId",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedAt",
                table: "RoomParticipants",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RoomFiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "ChatMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomParticipants_RoomId_UserId",
                table: "RoomParticipants",
                columns: new[] { "RoomId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_RoomId",
                table: "ChatMessages",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UserId",
                table: "ChatMessages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Rooms_RoomId",
                table: "ChatMessages",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_UserId",
                table: "ChatMessages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Rooms_RoomId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Users_UserId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_RoomParticipants_RoomId_UserId",
                table: "RoomParticipants");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_RoomId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_UserId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "RoomParticipants");

            migrationBuilder.AlterColumn<long>(
                name: "TelegramChatId",
                table: "Users",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomId1",
                table: "RoomParticipants",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "RoomParticipants",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RoomFiles",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "ChatMessages",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_RoomParticipants_RoomId1",
                table: "RoomParticipants",
                column: "RoomId1");

            migrationBuilder.CreateIndex(
                name: "IX_RoomParticipants_UserId1",
                table: "RoomParticipants",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomParticipants_Rooms_RoomId1",
                table: "RoomParticipants",
                column: "RoomId1",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomParticipants_Users_UserId1",
                table: "RoomParticipants",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
