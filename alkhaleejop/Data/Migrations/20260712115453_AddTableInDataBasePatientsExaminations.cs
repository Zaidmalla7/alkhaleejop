using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alkhaleejop.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTableInDataBasePatientsExaminations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FatherName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FamilyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Job = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Examinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    ExamDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextExamDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ODSph = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ODCyl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ODAxis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ODAdd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ODVA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OSSph = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OSCyl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OSAxis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OSAdd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OSVA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IPD = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExaminerName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Examinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Examinations_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Examinations_NextExamDate",
                table: "Examinations",
                column: "NextExamDate");

            migrationBuilder.CreateIndex(
                name: "IX_Examinations_PatientId",
                table: "Examinations",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PhoneNumber",
                table: "Patients",
                column: "PhoneNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Examinations");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
