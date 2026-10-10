using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeCollabFrontend.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    IsReadme = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomFiles_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomFiles_RoomId",
                table: "RoomFiles",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomFiles");
        }
    }
}
