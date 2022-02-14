using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Migrations
{
    public partial class RenameUserTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_UserTypeDbModel_UserTypeId",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTypeDbModel",
                table: "UserTypeDbModel");

            migrationBuilder.RenameTable(
                name: "UserTypeDbModel",
                newName: "user_type");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_type",
                table: "user_type",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_user_type_UserTypeId",
                table: "user",
                column: "UserTypeId",
                principalTable: "user_type",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_user_type_UserTypeId",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_type",
                table: "user_type");

            migrationBuilder.RenameTable(
                name: "user_type",
                newName: "UserTypeDbModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTypeDbModel",
                table: "UserTypeDbModel",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_UserTypeDbModel_UserTypeId",
                table: "user",
                column: "UserTypeId",
                principalTable: "UserTypeDbModel",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
