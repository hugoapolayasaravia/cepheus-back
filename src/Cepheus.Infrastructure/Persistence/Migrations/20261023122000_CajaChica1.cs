using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cepheus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CajaChica1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CajasChica",
                schema: "contabilidad",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MontoSol = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TotalSol = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    ProvisionAsientoCode = table.Column<string>(type: "char(13)", nullable: true),
                    ReversionAsientoCode = table.Column<string>(type: "char(13)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsPrinted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CajasChica", x => x.Code);
                    table.ForeignKey(
                        name: "FK_CajasChica_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CajasChica_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CajasChicaDetalle",
                schema: "contabilidad",
                columns: table => new
                {
                    CajaCode = table.Column<string>(type: "char(6)", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    Origen = table.Column<int>(type: "int", nullable: false),
                    ReferenceCode = table.Column<string>(type: "char(6)", nullable: true),
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    ComprobantePagoCode = table.Column<string>(type: "char(2)", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", nullable: false),
                    TipoCambio = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioUnidad = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Parcial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Igv = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    NoGravable = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Renta = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Fonavi = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Servicio = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    IgvExterior = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CuentaContableCode = table.Column<string>(type: "char(8)", nullable: false),
                    AnexoTipoCode = table.Column<string>(type: "char(3)", nullable: true),
                    AnexoCode = table.Column<string>(type: "char(5)", nullable: true),
                    Concepto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    DocumentoReferenciaComprobantePagoCode = table.Column<string>(type: "char(2)", nullable: true),
                    DocumentoReferenciaNumero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DocumentoReferenciaFecha = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CajasChicaDetalle", x => new { x.CajaCode, x.ItemNumber });
                    table.ForeignKey(
                        name: "FK_CajasChicaDetalle_Anexos_AnexoTipoCode_AnexoCode",
                        columns: x => new { x.AnexoTipoCode, x.AnexoCode },
                        principalSchema: "contabilidad",
                        principalTable: "Anexos",
                        principalColumns: new[] { "TipoAnexoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CajasChicaDetalle_CajasChica_CajaCode",
                        column: x => x.CajaCode,
                        principalSchema: "contabilidad",
                        principalTable: "CajasChica",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CajasChicaDetalle_ComprobantesPago_ComprobantePagoCode",
                        column: x => x.ComprobantePagoCode,
                        principalSchema: "comun",
                        principalTable: "ComprobantesPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CajasChicaDetalle_CuentasContables_CuentaContableCode",
                        column: x => x.CuentaContableCode,
                        principalSchema: "contabilidad",
                        principalTable: "CuentasContables",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CajasChicaDetalle_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CajasChicaDetalle_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CajasChicaDetalle_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CajasChica_IssueDate",
                schema: "contabilidad",
                table: "CajasChica",
                column: "IssueDate");

            migrationBuilder.CreateIndex(
                name: "IX_CajasChica_PlantaCode",
                schema: "contabilidad",
                table: "CajasChica",
                column: "PlantaCode");

            migrationBuilder.CreateIndex(
                name: "IX_CajasChica_Status",
                schema: "contabilidad",
                table: "CajasChica",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CajasChica_TrabajadorCode",
                schema: "contabilidad",
                table: "CajasChica",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_CajasChicaDetalle_AnexoTipoCode_AnexoCode",
                schema: "contabilidad",
                table: "CajasChicaDetalle",
                columns: new[] { "AnexoTipoCode", "AnexoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_CajasChicaDetalle_ComprobantePagoCode_NumeroDocumento_ProveedorCode",
                schema: "contabilidad",
                table: "CajasChicaDetalle",
                columns: new[] { "ComprobantePagoCode", "NumeroDocumento", "ProveedorCode" },
                unique: true,
                filter: "[ComprobantePagoCode] <> '00'");

            migrationBuilder.CreateIndex(
                name: "IX_CajasChicaDetalle_CuentaContableCode",
                schema: "contabilidad",
                table: "CajasChicaDetalle",
                column: "CuentaContableCode");

            migrationBuilder.CreateIndex(
                name: "IX_CajasChicaDetalle_MonedaCode",
                schema: "contabilidad",
                table: "CajasChicaDetalle",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_CajasChicaDetalle_Origen_PlantaCode_ReferenceCode",
                schema: "contabilidad",
                table: "CajasChicaDetalle",
                columns: new[] { "Origen", "PlantaCode", "ReferenceCode" });

            migrationBuilder.CreateIndex(
                name: "IX_CajasChicaDetalle_PlantaCode",
                schema: "contabilidad",
                table: "CajasChicaDetalle",
                column: "PlantaCode");

            migrationBuilder.CreateIndex(
                name: "IX_CajasChicaDetalle_ProveedorCode",
                schema: "contabilidad",
                table: "CajasChicaDetalle",
                column: "ProveedorCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CajasChicaDetalle",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "CajasChica",
                schema: "contabilidad");
        }
    }
}
