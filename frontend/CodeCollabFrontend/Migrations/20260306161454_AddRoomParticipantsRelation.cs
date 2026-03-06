using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeCollabFrontend.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomParticipantsRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
