using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.Persistence.Migrations;

/// <inheritdoc />
public partial class SeedingData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Role",
            columns: new[] { "Id", "Description", "IsActive", "Name" },
            values: new object[,]
            {
                { 1, "Administrator role with full permissions", true, "Admin" },
                { 2, "Client role for end users", true, "Client" },
                { 3, "Seller role for merchants", true, "Seller" }
            });

        migrationBuilder.InsertData(
            table: "User",
            columns: new[] { "Id", "Avatar", "Email", "Name", "Password", "PhoneNumber", "RoleId", "Status", "TotpSecret" },
            values: new object[] { 1, "", "admin@ecommerce.local", "System Administrator", "Admin@123", "0000000000", 1, 0, "" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Role",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "Role",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "User",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "Role",
            keyColumn: "Id",
            keyValue: 1);
    }
}
