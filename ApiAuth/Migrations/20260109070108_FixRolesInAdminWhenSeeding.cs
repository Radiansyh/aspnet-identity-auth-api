using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApiAuth.Migrations
{
    /// <inheritdoc />
    public partial class FixRolesInAdminWhenSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "331c7962-f56e-4a27-bf7e-10b4f237e157");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d07c8d20-ee5c-4a32-b23e-9e29f7e1f57f");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "44e7bc59-f962-4916-8550-9a10c9e8fd22", "44e7bc59-f962-4916-8550-9a10c9e8fd22", "Admin", "ADMIN" },
                    { "5be15348-a7f1-4b35-98eb-2c61e01a0b40", "5be15348-a7f1-4b35-98eb-2c61e01a0b40", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "44e7bc59-f962-4916-8550-9a10c9e8fd22");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5be15348-a7f1-4b35-98eb-2c61e01a0b40");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "331c7962-f56e-4a27-bf7e-10b4f237e157", "331c7962-f56e-4a27-bf7e-10b4f237e157", "User", "USER" },
                    { "d07c8d20-ee5c-4a32-b23e-9e29f7e1f57f", "d07c8d20-ee5c-4a32-b23e-9e29f7e1f57f", "Admin", "ADMIN" }
                });
        }
    }
}
