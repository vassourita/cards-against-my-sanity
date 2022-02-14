using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Migrations
{
    public partial class AddUserTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserTypeId",
                table: "user",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserTypeDbModel",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypeDbModel", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "UserTypeDbModel",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "account" },
                    { 2, "guest" },
                    { 3, "admin" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_UserTypeId",
                table: "user",
                column: "UserTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_UserTypeDbModel_UserTypeId",
                table: "user",
                column: "UserTypeId",
                principalTable: "UserTypeDbModel",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_UserTypeDbModel_UserTypeId",
                table: "user");

            migrationBuilder.DropTable(
                name: "UserTypeDbModel");

            migrationBuilder.DropIndex(
                name: "IX_user_UserTypeId",
                table: "user");

            migrationBuilder.DropColumn(
                name: "UserTypeId",
                table: "user");
        }
    }
}
