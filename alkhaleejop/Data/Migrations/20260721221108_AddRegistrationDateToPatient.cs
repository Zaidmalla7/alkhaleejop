using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alkhaleejop.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationDateToPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "Patients",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "Patients");
        }
    }
}
