using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonTask.Migrations
{
    /// <inheritdoc />
    public partial class removebloat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_CreatedBy",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_ModifiedBy",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_RevokedBy",
                table: "Users");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Users_Login",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CreatedBy",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ModifiedBy",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RevokedBy",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Users_Login",
                table: "Users",
                column: "Login");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedBy",
                table: "Users",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ModifiedBy",
                table: "Users",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RevokedBy",
                table: "Users",
                column: "RevokedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_CreatedBy",
                table: "Users",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Login",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_ModifiedBy",
                table: "Users",
                column: "ModifiedBy",
                principalTable: "Users",
                principalColumn: "Login",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_RevokedBy",
                table: "Users",
                column: "RevokedBy",
                principalTable: "Users",
                principalColumn: "Login",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
