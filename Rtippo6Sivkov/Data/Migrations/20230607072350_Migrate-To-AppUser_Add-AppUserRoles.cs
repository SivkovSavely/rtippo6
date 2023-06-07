using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rtippo6Sivkov.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigrateToAppUser_AddAppUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AppUserRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserRole", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AppUserRole",
                columns: new[] { "Id", "Name" },
                values: new object[] { 0, "Гость" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RoleId",
                table: "AspNetUsers",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AppUserRole_RoleId",
                table: "AspNetUsers",
                column: "RoleId",
                principalTable: "AppUserRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AppUserRole_RoleId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "AppUserRole");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RoleId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "AspNetUsers");
        }
    }
}
