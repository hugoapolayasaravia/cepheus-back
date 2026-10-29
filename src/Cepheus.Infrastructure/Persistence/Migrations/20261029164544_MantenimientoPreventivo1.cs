using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cepheus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MantenimientoPreventivo1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUnit",
                schema: "logistica",
                table: "SubCentrosCosto",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ActividadesMantenimiento",
                schema: "mantenimiento",
                columns: table => new
                {
                    ActividadCode = table.Column<string>(type: "char(6)", nullable: false),
                    ActividadCubiertaCode = table.Column<string>(type: "char(6)", nullable: false),
                    Prioridad = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActividadesMantenimiento", x => new { x.ActividadCode, x.ActividadCubiertaCode });
                    table.ForeignKey(
                        name: "FK_ActividadesMantenimiento_Actividades_ActividadCode",
                        column: x => x.ActividadCode,
                        principalSchema: "mantenimiento",
                        principalTable: "Actividades",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActividadesMantenimiento_Actividades_ActividadCubiertaCode",
                        column: x => x.ActividadCubiertaCode,
                        principalSchema: "mantenimiento",
                        principalTable: "Actividades",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fallas",
                schema: "mantenimiento",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(5)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fallas", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "PlanesMantenimientoPreventivo",
                schema: "mantenimiento",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EquipoCode = table.Column<string>(type: "char(8)", nullable: false),
                    EspecialidadCode = table.Column<string>(type: "char(1)", nullable: false),
                    ResponsableCode = table.Column<string>(type: "char(5)", nullable: false),
                    SubCentroCostoCode = table.Column<string>(type: "char(6)", nullable: false),
                    SubCentroEjecutorCode = table.Column<string>(type: "char(4)", nullable: false),
                    ActividadCode = table.Column<string>(type: "char(6)", nullable: false),
                    Frecuencia = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    UnidadFrecuencia = table.Column<int>(type: "int", nullable: false),
                    Factor = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    FechaInicio = table.Column<DateTime>(type: "date", nullable: true),
                    FechaTermino = table.Column<DateTime>(type: "date", nullable: true),
                    FrecuenciaGama = table.Column<decimal>(type: "decimal(18,0)", nullable: false, defaultValue: 0m),
                    TiempoRestante = table.Column<decimal>(type: "decimal(18,0)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanesMantenimientoPreventivo", x => new { x.PlantaCode, x.Code });
                    table.ForeignKey(
                        name: "FK_PlanesMantenimientoPreventivo_Actividades_ActividadCode",
                        column: x => x.ActividadCode,
                        principalSchema: "mantenimiento",
                        principalTable: "Actividades",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanesMantenimientoPreventivo_Equipos_EquipoCode",
                        column: x => x.EquipoCode,
                        principalSchema: "mantenimiento",
                        principalTable: "Equipos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanesMantenimientoPreventivo_Especialidades_EspecialidadCode",
                        column: x => x.EspecialidadCode,
                        principalSchema: "mantenimiento",
                        principalTable: "Especialidades",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanesMantenimientoPreventivo_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanesMantenimientoPreventivo_SubCentrosCosto_SubCentroCostoCode",
                        column: x => x.SubCentroCostoCode,
                        principalSchema: "logistica",
                        principalTable: "SubCentrosCosto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanesMantenimientoPreventivo_SubCentrosEjecutores_SubCentroEjecutorCode",
                        column: x => x.SubCentroEjecutorCode,
                        principalSchema: "mantenimiento",
                        principalTable: "SubCentrosEjecutores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanesMantenimientoPreventivo_Trabajadores_ResponsableCode",
                        column: x => x.ResponsableCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesHorometro",
                schema: "mantenimiento",
                columns: table => new
                {
                    SubCentroCostoCode = table.Column<string>(type: "char(6)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "date", nullable: false),
                    Horometro = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    PlantaAfectadaCode = table.Column<string>(type: "char(2)", nullable: false),
                    EquipoCode = table.Column<string>(type: "char(8)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesHorometro", x => new { x.SubCentroCostoCode, x.Fecha });
                    table.ForeignKey(
                        name: "FK_UnidadesHorometro_Equipos_EquipoCode",
                        column: x => x.EquipoCode,
                        principalSchema: "mantenimiento",
                        principalTable: "Equipos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnidadesHorometro_Plantas_PlantaAfectadaCode",
                        column: x => x.PlantaAfectadaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnidadesHorometro_SubCentrosCosto_SubCentroCostoCode",
                        column: x => x.SubCentroCostoCode,
                        principalSchema: "logistica",
                        principalTable: "SubCentrosCosto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesParada",
                schema: "mantenimiento",
                columns: table => new
                {
                    SubCentroCostoCode = table.Column<string>(type: "char(6)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "date", nullable: false),
                    Horas = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    PlantaAfectadaCode = table.Column<string>(type: "char(2)", nullable: false),
                    FallaCode = table.Column<string>(type: "char(5)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesParada", x => new { x.SubCentroCostoCode, x.Fecha });
                    table.ForeignKey(
                        name: "FK_UnidadesParada_Fallas_FallaCode",
                        column: x => x.FallaCode,
                        principalSchema: "mantenimiento",
                        principalTable: "Fallas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnidadesParada_Plantas_PlantaAfectadaCode",
                        column: x => x.PlantaAfectadaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnidadesParada_SubCentrosCosto_SubCentroCostoCode",
                        column: x => x.SubCentroCostoCode,
                        principalSchema: "logistica",
                        principalTable: "SubCentrosCosto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlanesMantenimientoPreventivoManoObra",
                schema: "mantenimiento",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    PlanCode = table.Column<string>(type: "char(6)", nullable: false),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    Tiempo = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    CostoTotal = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanesMantenimientoPreventivoManoObra", x => new { x.PlantaCode, x.PlanCode, x.TrabajadorCode });
                    table.ForeignKey(
                        name: "FK_PlanesMantenimientoPreventivoManoObra_PlanesMantenimientoPreventivo_PlantaCode_PlanCode",
                        columns: x => new { x.PlantaCode, x.PlanCode },
                        principalSchema: "mantenimiento",
                        principalTable: "PlanesMantenimientoPreventivo",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanesMantenimientoPreventivoManoObra_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlanesMantenimientoPreventivoMateriales",
                schema: "mantenimiento",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    PlanCode = table.Column<string>(type: "char(6)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    CostoUnitario = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    CostoTotal = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanesMantenimientoPreventivoMateriales", x => new { x.PlantaCode, x.PlanCode, x.ArticuloCode });
                    table.ForeignKey(
                        name: "FK_PlanesMantenimientoPreventivoMateriales_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanesMantenimientoPreventivoMateriales_PlanesMantenimientoPreventivo_PlantaCode_PlanCode",
                        columns: x => new { x.PlantaCode, x.PlanCode },
                        principalSchema: "mantenimiento",
                        principalTable: "PlanesMantenimientoPreventivo",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesMantenimiento_ActividadCubiertaCode",
                schema: "mantenimiento",
                table: "ActividadesMantenimiento",
                column: "ActividadCubiertaCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesMantenimientoPreventivo_ActividadCode",
                schema: "mantenimiento",
                table: "PlanesMantenimientoPreventivo",
                column: "ActividadCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesMantenimientoPreventivo_EquipoCode",
                schema: "mantenimiento",
                table: "PlanesMantenimientoPreventivo",
                column: "EquipoCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesMantenimientoPreventivo_EspecialidadCode",
                schema: "mantenimiento",
                table: "PlanesMantenimientoPreventivo",
                column: "EspecialidadCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesMantenimientoPreventivo_ResponsableCode",
                schema: "mantenimiento",
                table: "PlanesMantenimientoPreventivo",
                column: "ResponsableCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesMantenimientoPreventivo_SubCentroCostoCode",
                schema: "mantenimiento",
                table: "PlanesMantenimientoPreventivo",
                column: "SubCentroCostoCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesMantenimientoPreventivo_SubCentroEjecutorCode",
                schema: "mantenimiento",
                table: "PlanesMantenimientoPreventivo",
                column: "SubCentroEjecutorCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesMantenimientoPreventivoManoObra_TrabajadorCode",
                schema: "mantenimiento",
                table: "PlanesMantenimientoPreventivoManoObra",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesMantenimientoPreventivoMateriales_ArticuloCode",
                schema: "mantenimiento",
                table: "PlanesMantenimientoPreventivoMateriales",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesHorometro_EquipoCode",
                schema: "mantenimiento",
                table: "UnidadesHorometro",
                column: "EquipoCode");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesHorometro_PlantaAfectadaCode_Fecha",
                schema: "mantenimiento",
                table: "UnidadesHorometro",
                columns: new[] { "PlantaAfectadaCode", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesParada_FallaCode",
                schema: "mantenimiento",
                table: "UnidadesParada",
                column: "FallaCode");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesParada_PlantaAfectadaCode_Fecha",
                schema: "mantenimiento",
                table: "UnidadesParada",
                columns: new[] { "PlantaAfectadaCode", "Fecha" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActividadesMantenimiento",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "PlanesMantenimientoPreventivoManoObra",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "PlanesMantenimientoPreventivoMateriales",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "UnidadesHorometro",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "UnidadesParada",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "PlanesMantenimientoPreventivo",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "Fallas",
                schema: "mantenimiento");

            migrationBuilder.DropColumn(
                name: "IsUnit",
                schema: "logistica",
                table: "SubCentrosCosto");
        }
    }
}
