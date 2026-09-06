using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alkhaleejop.Data.Migrations
{
    /// <inheritdoc />
    public partial class addruls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9A2E45B8-6D7A-4D99-8A4F-B1E2A3F7E9D1", "1", "admin", "ADMIN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9A2E45B8-6D7A-4D99-8A4F-B1E2A3F7E9D1");
        }
    }
}
