using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cepheus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MONEDATIPOCAMBIO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentoCompraDetalles_UnidadesMedidaVenta_UnidadMedidaCode",
                schema: "contabilidad",
                table: "DocumentoCompraDetalles");

            migrationBuilder.DropIndex(
                name: "IX_TiposCambio_Date",
                schema: "comun",
                table: "TiposCambio");

            migrationBuilder.AddColumn<string>(
                name: "MonedaCode",
                schema: "comun",
                table: "TiposCambio",
                type: "char(3)",
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<string>(
                name: "PayableAccountForeignCode",
                schema: "logistica",
                table: "Proveedores",
                type: "char(8)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayableAccountLocalCode",
                schema: "logistica",
                table: "Proveedores",
                type: "char(8)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AsientosContables",
                schema: "contabilidad",
                columns: table => new
                {
                    Anio = table.Column<string>(type: "char(4)", nullable: false),
                    Mes = table.Column<string>(type: "char(2)", nullable: false),
                    TipoAsientoCode = table.Column<string>(type: "char(2)", nullable: false),
                    Numero = table.Column<string>(type: "char(5)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaProceso = table.Column<DateTime>(type: "date", nullable: false),
                    TipoCambio = table.Column<decimal>(type: "decimal(10,4)", nullable: false, defaultValue: 0m),
                    TotalCargosSol = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    TotalAbonosSol = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Origen = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsientosContables", x => new { x.Anio, x.Mes, x.TipoAsientoCode, x.Numero });
                    table.ForeignKey(
                        name: "FK_AsientosContables_TiposAsiento_TipoAsientoCode",
                        column: x => x.TipoAsientoCode,
                        principalSchema: "contabilidad",
                        principalTable: "TiposAsiento",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PeriodosContables",
                schema: "contabilidad",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ReopenedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReopenedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodosContables", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "PlantillasAsiento",
                schema: "contabilidad",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Process = table.Column<int>(type: "int", nullable: false),
                    TipoAsientoCode = table.Column<string>(type: "char(2)", nullable: false),
                    PurchaseAccountCode = table.Column<string>(type: "char(8)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantillasAsiento", x => x.Code);
                    table.ForeignKey(
                        name: "FK_PlantillasAsiento_CuentasContables_PurchaseAccountCode",
                        column: x => x.PurchaseAccountCode,
                        principalSchema: "contabilidad",
                        principalTable: "CuentasContables",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlantillasAsiento_TiposAsiento_TipoAsientoCode",
                        column: x => x.TipoAsientoCode,
                        principalSchema: "contabilidad",
                        principalTable: "TiposAsiento",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubFamiliaAnexos",
                schema: "logistica",
                columns: table => new
                {
                    SubCentroCostoCode = table.Column<string>(type: "char(6)", nullable: false),
                    SubFamiliaCode = table.Column<string>(type: "char(4)", nullable: false),
                    AccountingAccountCode = table.Column<string>(type: "char(8)", nullable: false),
                    AccountingAttachmentTypeCode = table.Column<string>(type: "char(3)", nullable: true),
                    AccountingAttachmentCode = table.Column<string>(type: "char(5)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubFamiliaAnexos", x => new { x.SubCentroCostoCode, x.SubFamiliaCode });
                    table.ForeignKey(
                        name: "FK_SubFamiliaAnexos_Anexos_AccountingAttachmentTypeCode_AccountingAttachmentCode",
                        columns: x => new { x.AccountingAttachmentTypeCode, x.AccountingAttachmentCode },
                        principalSchema: "contabilidad",
                        principalTable: "Anexos",
                        principalColumns: new[] { "TipoAnexoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubFamiliaAnexos_CuentasContables_AccountingAccountCode",
                        column: x => x.AccountingAccountCode,
                        principalSchema: "contabilidad",
                        principalTable: "CuentasContables",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubFamiliaAnexos_SubCentrosCosto_SubCentroCostoCode",
                        column: x => x.SubCentroCostoCode,
                        principalSchema: "logistica",
                        principalTable: "SubCentrosCosto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubFamiliaAnexos_SubFamilias_SubFamiliaCode",
                        column: x => x.SubFamiliaCode,
                        principalSchema: "logistica",
                        principalTable: "SubFamilias",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubFamiliaAnexos_TiposAnexo_AccountingAttachmentTypeCode",
                        column: x => x.AccountingAttachmentTypeCode,
                        principalSchema: "contabilidad",
                        principalTable: "TiposAnexo",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AsientoContableDetalles",
                schema: "contabilidad",
                columns: table => new
                {
                    Anio = table.Column<string>(type: "char(4)", nullable: false),
                    Mes = table.Column<string>(type: "char(2)", nullable: false),
                    TipoAsientoCode = table.Column<string>(type: "char(2)", nullable: false),
                    Numero = table.Column<string>(type: "char(5)", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    EsDestino = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CuentaContableCode = table.Column<string>(type: "char(8)", nullable: false),
                    AnexoTipoCode = table.Column<string>(type: "char(3)", nullable: true),
                    AnexoCode = table.Column<string>(type: "char(5)", nullable: true),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CargoSol = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    AbonoSol = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TipoContabilidad = table.Column<int>(type: "int", nullable: false),
                    PlantaEjecutoraCode = table.Column<string>(type: "char(2)", nullable: true),
                    ComprobantePagoCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsientoContableDetalles", x => new { x.Anio, x.Mes, x.TipoAsientoCode, x.Numero, x.ItemNumber });
                    table.ForeignKey(
                        name: "FK_AsientoContableDetalles_Anexos_AnexoTipoCode_AnexoCode",
                        columns: x => new { x.AnexoTipoCode, x.AnexoCode },
                        principalSchema: "contabilidad",
                        principalTable: "Anexos",
                        principalColumns: new[] { "TipoAnexoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsientoContableDetalles_AsientosContables_Anio_Mes_TipoAsientoCode_Numero",
                        columns: x => new { x.Anio, x.Mes, x.TipoAsientoCode, x.Numero },
                        principalSchema: "contabilidad",
                        principalTable: "AsientosContables",
                        principalColumns: new[] { "Anio", "Mes", "TipoAsientoCode", "Numero" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AsientoContableDetalles_ComprobantesPago_ComprobantePagoCode",
                        column: x => x.ComprobantePagoCode,
                        principalSchema: "comun",
                        principalTable: "ComprobantesPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsientoContableDetalles_CuentasContables_CuentaContableCode",
                        column: x => x.CuentaContableCode,
                        principalSchema: "contabilidad",
                        principalTable: "CuentasContables",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsientoContableDetalles_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsientoContableDetalles_Plantas_PlantaEjecutoraCode",
                        column: x => x.PlantaEjecutoraCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlantillaAsientoLineas",
                schema: "contabilidad",
                columns: table => new
                {
                    TemplateCode = table.Column<string>(type: "char(3)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Concept = table.Column<int>(type: "int", nullable: false),
                    Nature = table.Column<int>(type: "int", nullable: false),
                    AccountSource = table.Column<int>(type: "int", nullable: false),
                    AccountCode = table.Column<string>(type: "char(8)", nullable: true),
                    AnnexSource = table.Column<int>(type: "int", nullable: false),
                    DescriptionSource = table.Column<int>(type: "int", nullable: false),
                    InvertsOnCreditNote = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDestination = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AccountingType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantillaAsientoLineas", x => new { x.TemplateCode, x.Order });
                    table.ForeignKey(
                        name: "FK_PlantillaAsientoLineas_CuentasContables_AccountCode",
                        column: x => x.AccountCode,
                        principalSchema: "contabilidad",
                        principalTable: "CuentasContables",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlantillaAsientoLineas_PlantillasAsiento_TemplateCode",
                        column: x => x.TemplateCode,
                        principalSchema: "contabilidad",
                        principalTable: "PlantillasAsiento",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TiposCambio_MonedaCode_Date",
                schema: "comun",
                table: "TiposCambio",
                columns: new[] { "MonedaCode", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_PayableAccountForeignCode",
                schema: "logistica",
                table: "Proveedores",
                column: "PayableAccountForeignCode");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_PayableAccountLocalCode",
                schema: "logistica",
                table: "Proveedores",
                column: "PayableAccountLocalCode");

            migrationBuilder.CreateIndex(
                name: "IX_AsientoContableDetalles_AnexoTipoCode_AnexoCode",
                schema: "contabilidad",
                table: "AsientoContableDetalles",
                columns: new[] { "AnexoTipoCode", "AnexoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_AsientoContableDetalles_ComprobantePagoCode",
                schema: "contabilidad",
                table: "AsientoContableDetalles",
                column: "ComprobantePagoCode");

            migrationBuilder.CreateIndex(
                name: "IX_AsientoContableDetalles_CuentaContableCode",
                schema: "contabilidad",
                table: "AsientoContableDetalles",
                column: "CuentaContableCode");

            migrationBuilder.CreateIndex(
                name: "IX_AsientoContableDetalles_NumeroDocumento",
                schema: "contabilidad",
                table: "AsientoContableDetalles",
                column: "NumeroDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_AsientoContableDetalles_PlantaCode",
                schema: "contabilidad",
                table: "AsientoContableDetalles",
                column: "PlantaCode");

            migrationBuilder.CreateIndex(
                name: "IX_AsientoContableDetalles_PlantaEjecutoraCode",
                schema: "contabilidad",
                table: "AsientoContableDetalles",
                column: "PlantaEjecutoraCode");

            migrationBuilder.CreateIndex(
                name: "IX_AsientosContables_Anio_Mes",
                schema: "contabilidad",
                table: "AsientosContables",
                columns: new[] { "Anio", "Mes" });

            migrationBuilder.CreateIndex(
                name: "IX_AsientosContables_Estado",
                schema: "contabilidad",
                table: "AsientosContables",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_AsientosContables_FechaProceso",
                schema: "contabilidad",
                table: "AsientosContables",
                column: "FechaProceso");

            migrationBuilder.CreateIndex(
                name: "IX_AsientosContables_Origen",
                schema: "contabilidad",
                table: "AsientosContables",
                column: "Origen");

            migrationBuilder.CreateIndex(
                name: "IX_AsientosContables_TipoAsientoCode",
                schema: "contabilidad",
                table: "AsientosContables",
                column: "TipoAsientoCode");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosContables_Status",
                schema: "contabilidad",
                table: "PeriodosContables",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosContables_Year_Month",
                schema: "contabilidad",
                table: "PeriodosContables",
                columns: new[] { "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlantillaAsientoLineas_AccountCode",
                schema: "contabilidad",
                table: "PlantillaAsientoLineas",
                column: "AccountCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasAsiento_Process",
                schema: "contabilidad",
                table: "PlantillasAsiento",
                column: "Process");

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasAsiento_PurchaseAccountCode",
                schema: "contabilidad",
                table: "PlantillasAsiento",
                column: "PurchaseAccountCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasAsiento_TipoAsientoCode",
                schema: "contabilidad",
                table: "PlantillasAsiento",
                column: "TipoAsientoCode");

            migrationBuilder.CreateIndex(
                name: "IX_SubFamiliaAnexos_AccountingAccountCode",
                schema: "logistica",
                table: "SubFamiliaAnexos",
                column: "AccountingAccountCode");

            migrationBuilder.CreateIndex(
                name: "IX_SubFamiliaAnexos_AccountingAttachmentTypeCode_AccountingAttachmentCode",
                schema: "logistica",
                table: "SubFamiliaAnexos",
                columns: new[] { "AccountingAttachmentTypeCode", "AccountingAttachmentCode" });

            migrationBuilder.CreateIndex(
                name: "IX_SubFamiliaAnexos_SubFamiliaCode",
                schema: "logistica",
                table: "SubFamiliaAnexos",
                column: "SubFamiliaCode");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentoCompraDetalles_UnidadesMedida_UnidadMedidaCode",
                schema: "contabilidad",
                table: "DocumentoCompraDetalles",
                column: "UnidadMedidaCode",
                principalSchema: "logistica",
                principalTable: "UnidadesMedida",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proveedores_CuentasContables_PayableAccountForeignCode",
                schema: "logistica",
                table: "Proveedores",
                column: "PayableAccountForeignCode",
                principalSchema: "contabilidad",
                principalTable: "CuentasContables",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proveedores_CuentasContables_PayableAccountLocalCode",
                schema: "logistica",
                table: "Proveedores",
                column: "PayableAccountLocalCode",
                principalSchema: "contabilidad",
                principalTable: "CuentasContables",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TiposCambio_Monedas_MonedaCode",
                schema: "comun",
                table: "TiposCambio",
                column: "MonedaCode",
                principalSchema: "comun",
                principalTable: "Monedas",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentoCompraDetalles_UnidadesMedida_UnidadMedidaCode",
                schema: "contabilidad",
                table: "DocumentoCompraDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_Proveedores_CuentasContables_PayableAccountForeignCode",
                schema: "logistica",
                table: "Proveedores");

            migrationBuilder.DropForeignKey(
                name: "FK_Proveedores_CuentasContables_PayableAccountLocalCode",
                schema: "logistica",
                table: "Proveedores");

            migrationBuilder.DropForeignKey(
                name: "FK_TiposCambio_Monedas_MonedaCode",
                schema: "comun",
                table: "TiposCambio");

            migrationBuilder.DropTable(
                name: "AsientoContableDetalles",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "PeriodosContables",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "PlantillaAsientoLineas",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "SubFamiliaAnexos",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "AsientosContables",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "PlantillasAsiento",
                schema: "contabilidad");

            migrationBuilder.DropIndex(
                name: "IX_TiposCambio_MonedaCode_Date",
                schema: "comun",
                table: "TiposCambio");

            migrationBuilder.DropIndex(
                name: "IX_Proveedores_PayableAccountForeignCode",
                schema: "logistica",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Proveedores_PayableAccountLocalCode",
                schema: "logistica",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "MonedaCode",
                schema: "comun",
                table: "TiposCambio");

            migrationBuilder.DropColumn(
                name: "PayableAccountForeignCode",
                schema: "logistica",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "PayableAccountLocalCode",
                schema: "logistica",
                table: "Proveedores");

            migrationBuilder.CreateIndex(
                name: "IX_TiposCambio_Date",
                schema: "comun",
                table: "TiposCambio",
                column: "Date",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentoCompraDetalles_UnidadesMedidaVenta_UnidadMedidaCode",
                schema: "contabilidad",
                table: "DocumentoCompraDetalles",
                column: "UnidadMedidaCode",
                principalSchema: "facturacion",
                principalTable: "UnidadesMedidaVenta",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
