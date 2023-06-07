using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rtippo6Sivkov.Data.Migrations
{
    /// <inheritdoc />
    public partial class AppUser_AddLocality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocalityId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LocalityId",
                table: "AspNetUsers",
                column: "LocalityId");

            migrationBuilder.Sql("UPDATE AspNetUsers SET LocalityId=1");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Localities_LocalityId",
                table: "AspNetUsers",
                column: "LocalityId",
                principalTable: "Localities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Localities_LocalityId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LocalityId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LocalityId",
                table: "AspNetUsers");
        }
    }
}
