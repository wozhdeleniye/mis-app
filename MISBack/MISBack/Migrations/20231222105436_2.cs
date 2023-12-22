using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MISBack.Migrations
{
    /// <inheritdoc />
    public partial class _2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diagnoses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    icd10Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnoses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Icd10s",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Icd10s", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diagnoses");

            migrationBuilder.DropTable(
                name: "Icd10s");
        }
    }
}
