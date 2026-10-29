using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cepheus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Detracciones1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Detracciones",
                schema: "contabilidad",
                columns: table => new
                {
                    Year = table.Column<string>(type: "char(4)", nullable: false),
                    Period = table.Column<string>(type: "char(2)", nullable: false),
                    LotNumber = table.Column<string>(type: "char(4)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    DepositDate = table.Column<DateTime>(type: "date", nullable: true),
                    PaymentReceiptNumber = table.Column<string>(type: "char(25)", nullable: true),
                    CancellationDate = table.Column<DateTime>(type: "date", nullable: true),
                    TotalSol = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    AsientoCode = table.Column<string>(type: "char(13)", nullable: true),
                    ChequeCode = table.Column<string>(type: "char(10)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detracciones", x => new { x.Year, x.Period, x.LotNumber });
                });

            migrationBuilder.CreateTable(
                name: "DetraccionItems",
                schema: "contabilidad",
                columns: table => new
                {
                    Year = table.Column<string>(type: "char(4)", nullable: false),
                    Period = table.Column<string>(type: "char(2)", nullable: false),
                    LotNumber = table.Column<string>(type: "char(4)", nullable: false),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    TipoOperacionCode = table.Column<string>(type: "char(2)", nullable: false),
                    TipoBienCode = table.Column<string>(type: "char(3)", nullable: false),
                    ProveedorRuc = table.Column<string>(type: "char(11)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ProveedorCuentaId = table.Column<int>(type: "int", nullable: true),
                    DepositSol = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetraccionItems", x => new { x.Year, x.Period, x.LotNumber, x.ProveedorCode, x.ItemNumber });
                    table.ForeignKey(
                        name: "FK_DetraccionItems_Detracciones_Year_Period_LotNumber",
                        columns: x => new { x.Year, x.Period, x.LotNumber },
                        principalSchema: "contabilidad",
                        principalTable: "Detracciones",
                        principalColumns: new[] { "Year", "Period", "LotNumber" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetraccionItems_ProveedorCuentas_ProveedorCuentaId",
                        column: x => x.ProveedorCuentaId,
                        principalSchema: "logistica",
                        principalTable: "ProveedorCuentas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetraccionItems_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetraccionItems_TiposBien_TipoBienCode",
                        column: x => x.TipoBienCode,
                        principalSchema: "facturacion",
                        principalTable: "TiposBien",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetraccionItems_TiposOperacion_TipoOperacionCode",
                        column: x => x.TipoOperacionCode,
                        principalSchema: "facturacion",
                        principalTable: "TiposOperacion",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetraccionDocumentos",
                schema: "contabilidad",
                columns: table => new
                {
                    Year = table.Column<string>(type: "char(4)", nullable: false),
                    Period = table.Column<string>(type: "char(2)", nullable: false),
                    LotNumber = table.Column<string>(type: "char(4)", nullable: false),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    ComprobantePagoCode = table.Column<string>(type: "char(2)", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "char(15)", nullable: false),
                    DetraccionSol = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    IncludeInExport = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetraccionDocumentos", x => new { x.Year, x.Period, x.LotNumber, x.ProveedorCode, x.ItemNumber, x.ComprobantePagoCode, x.NumeroDocumento });
                    table.ForeignKey(
                        name: "FK_DetraccionDocumentos_DetraccionItems_Year_Period_LotNumber_ProveedorCode_ItemNumber",
                        columns: x => new { x.Year, x.Period, x.LotNumber, x.ProveedorCode, x.ItemNumber },
                        principalSchema: "contabilidad",
                        principalTable: "DetraccionItems",
                        principalColumns: new[] { "Year", "Period", "LotNumber", "ProveedorCode", "ItemNumber" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetraccionDocumentos_DocumentosCompra_ComprobantePagoCode_NumeroDocumento_ProveedorCode",
                        columns: x => new { x.ComprobantePagoCode, x.NumeroDocumento, x.ProveedorCode },
                        principalSchema: "contabilidad",
                        principalTable: "DocumentosCompra",
                        principalColumns: new[] { "ComprobantePagoCode", "NumeroDocumento", "ProveedorCode" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetraccionDocumentos_ComprobantePagoCode_NumeroDocumento_ProveedorCode",
                schema: "contabilidad",
                table: "DetraccionDocumentos",
                columns: new[] { "ComprobantePagoCode", "NumeroDocumento", "ProveedorCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Detracciones_CreatedAt",
                schema: "contabilidad",
                table: "Detracciones",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Detracciones_Status",
                schema: "contabilidad",
                table: "Detracciones",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_DetraccionItems_ProveedorCode",
                schema: "contabilidad",
                table: "DetraccionItems",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_DetraccionItems_ProveedorCuentaId",
                schema: "contabilidad",
                table: "DetraccionItems",
                column: "ProveedorCuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetraccionItems_TipoBienCode",
                schema: "contabilidad",
                table: "DetraccionItems",
                column: "TipoBienCode");

            migrationBuilder.CreateIndex(
                name: "IX_DetraccionItems_TipoOperacionCode",
                schema: "contabilidad",
                table: "DetraccionItems",
                column: "TipoOperacionCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetraccionDocumentos",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "DetraccionItems",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "Detracciones",
                schema: "contabilidad");
        }
    }
}
