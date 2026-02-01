using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshNoodle.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVerificationCodeToPasswordResetRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "PasswordResetRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationCode",
                table: "PasswordResetRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "PasswordResetRequests");

            migrationBuilder.DropColumn(
                name: "VerificationCode",
                table: "PasswordResetRequests");
        }
    }
}
