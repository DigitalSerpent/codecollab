using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeCollabFrontend.Migrations
{
    /// <inheritdoc />
    public partial class AddConfirmationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConfirmationCode",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmationCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Users");
        }
    }
}
