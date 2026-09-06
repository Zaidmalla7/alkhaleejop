using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alkhaleejop.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableExaminationAddImeg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GlassImagePath",
                table: "Examinations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GlassImagePath",
                table: "Examinations");
        }
    }
}
