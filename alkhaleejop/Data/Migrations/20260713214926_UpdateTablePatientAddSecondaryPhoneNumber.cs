using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alkhaleejop.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablePatientAddSecondaryPhoneNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecondaryPhoneNumber",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryPhoneOwner",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecondaryPhoneNumber",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "SecondaryPhoneOwner",
                table: "Patients");
        }
    }
}
