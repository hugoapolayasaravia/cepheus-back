using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cepheus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarUbicacionesLogistica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticuloUbicaciones",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(4)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PlantaCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    Zone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Rack = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Shelf = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Locker = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticuloUbicaciones", x => x.Code);
                    table.ForeignKey(
                        name: "FK_ArticuloUbicaciones_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArticuloPlantaUbicaciones",
                schema: "logistica",
                columns: table => new
                {
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    PlantaCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    ArticuloUbicacionCode = table.Column<string>(type: "char(4)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticuloPlantaUbicaciones", x => new { x.ArticuloCode, x.PlantaCode });
                    table.ForeignKey(
                        name: "FK_ArticuloPlantaUbicaciones_ArticuloUbicaciones_ArticuloUbicacionCode",
                        column: x => x.ArticuloUbicacionCode,
                        principalSchema: "logistica",
                        principalTable: "ArticuloUbicaciones",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticuloPlantaUbicaciones_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticuloPlantaUbicaciones_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloPlantaUbicaciones_ArticuloUbicacionCode",
                schema: "logistica",
                table: "ArticuloPlantaUbicaciones",
                column: "ArticuloUbicacionCode");

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloPlantaUbicaciones_PlantaCode",
                schema: "logistica",
                table: "ArticuloPlantaUbicaciones",
                column: "PlantaCode");

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloUbicaciones_PlantaCode",
                schema: "logistica",
                table: "ArticuloUbicaciones",
                column: "PlantaCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticuloPlantaUbicaciones",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "ArticuloUbicaciones",
                schema: "logistica");
        }
    }
}
