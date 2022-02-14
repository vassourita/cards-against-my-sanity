using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Migrations
{
    public partial class RenameUserTypeIdForeignKeyName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_user_type_UserTypeId",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "UserTypeId",
                table: "user",
                newName: "user_type_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_UserTypeId",
                table: "user",
                newName: "IX_user_user_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_user_type_user_type_id",
                table: "user",
                column: "user_type_id",
                principalTable: "user_type",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_user_type_user_type_id",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "user_type_id",
                table: "user",
                newName: "UserTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_user_user_type_id",
                table: "user",
                newName: "IX_user_UserTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_user_type_UserTypeId",
                table: "user",
                column: "UserTypeId",
                principalTable: "user_type",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
