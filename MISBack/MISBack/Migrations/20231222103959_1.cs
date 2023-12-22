using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MISBack.Migrations
{
    /// <inheritdoc />
    public partial class _1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "speciality",
                table: "Doctors",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    authorId = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    consultationId = table.Column<Guid>(type: "uuid", nullable: false),
                    parentId = table.Column<Guid>(type: "uuid", nullable: true),
                    content = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Consultations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    inspectionId = table.Column<Guid>(type: "uuid", nullable: true),
                    specialityId = table.Column<Guid>(type: "uuid", nullable: false),
                    authorId = table.Column<Guid>(type: "uuid", nullable: false),
                    comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    docId = table.Column<Guid>(type: "uuid", nullable: false),
                    patientId = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    anamnesis = table.Column<string>(type: "text", nullable: false),
                    complaints = table.Column<string>(type: "text", nullable: false),
                    treatment = table.Column<string>(type: "text", nullable: false),
                    conclusion = table.Column<int>(type: "integer", nullable: false),
                    nextVisitDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deathDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    baseInspectionId = table.Column<Guid>(type: "uuid", nullable: true),
                    previousInspectionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    birthday = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    gender = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Specialities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialities", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Consultations");

            migrationBuilder.DropTable(
                name: "Inspections");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Specialities");

            migrationBuilder.DropColumn(
                name: "speciality",
                table: "Doctors");
        }
    }
}
