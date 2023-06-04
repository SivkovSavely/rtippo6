using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rtippo6Sivkov.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrgOrgTypeAndLocality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationsTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationsTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Administration_Organization_Id = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Inn = table.Column<string>(type: "TEXT", nullable: false),
                    Kpp = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    IsPhysical = table.Column<bool>(type: "INTEGER", nullable: false),
                    TypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocalityId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_Localities_LocalityId",
                        column: x => x.LocalityId,
                        principalTable: "Localities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Organizations_OrganizationsTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "OrganizationsTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Localities_Administration_Organization_Id",
                table: "Localities",
                column: "Administration_Organization_Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_LocalityId",
                table: "Organizations",
                column: "LocalityId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_TypeId",
                table: "Organizations",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Localities_Organizations_Administration_Organization_Id",
                table: "Localities",
                column: "Administration_Organization_Id",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Localities_Organizations_Administration_Organization_Id",
                table: "Localities");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "Localities");

            migrationBuilder.DropTable(
                name: "OrganizationsTypes");
        }
    }
}
