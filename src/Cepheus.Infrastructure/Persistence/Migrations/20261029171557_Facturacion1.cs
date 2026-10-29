using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cepheus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Facturacion1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                schema: "contabilidad",
                table: "PlantillaAsientoLineas",
                type: "char(3)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MotivosChequeRebote",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivosChequeRebote", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "NotasDebitoVenta",
                schema: "facturacion",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClienteCode = table.Column<string>(type: "char(5)", nullable: false),
                    ObraCode = table.Column<string>(type: "char(3)", nullable: false),
                    VendedorCode = table.Column<string>(type: "char(4)", nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DeliveryUbigeoCode = table.Column<string>(type: "char(6)", nullable: false),
                    PrintedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPrinted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FormaPagoVentaCode = table.Column<string>(type: "char(2)", nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    OrderCode = table.Column<string>(type: "char(8)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExecutingPlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    IgvAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    PendingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    ProvisionEntryNumber = table.Column<string>(type: "char(13)", nullable: true),
                    CancellationEntryNumber = table.Column<string>(type: "char(13)", nullable: true),
                    CanceledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SettledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<string>(type: "char(1)", nullable: false, defaultValue: "N"),
                    DocumentTypeCode = table.Column<string>(type: "char(2)", nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasDebitoVenta", x => new { x.PlantaCode, x.Number });
                    table.ForeignKey(
                        name: "FK_NotasDebitoVenta_Clientes_ClienteCode",
                        column: x => x.ClienteCode,
                        principalSchema: "facturacion",
                        principalTable: "Clientes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasDebitoVenta_ComprobantesPago_DocumentTypeCode",
                        column: x => x.DocumentTypeCode,
                        principalSchema: "comun",
                        principalTable: "ComprobantesPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasDebitoVenta_FormasPagoVenta_FormaPagoVentaCode",
                        column: x => x.FormaPagoVentaCode,
                        principalSchema: "facturacion",
                        principalTable: "FormasPagoVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasDebitoVenta_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasDebitoVenta_Obras_ClienteCode_ObraCode",
                        columns: x => new { x.ClienteCode, x.ObraCode },
                        principalSchema: "facturacion",
                        principalTable: "Obras",
                        principalColumns: new[] { "ClienteCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasDebitoVenta_Plantas_ExecutingPlantaCode",
                        column: x => x.ExecutingPlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasDebitoVenta_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasDebitoVenta_Ubigeos_DeliveryUbigeoCode",
                        column: x => x.DeliveryUbigeoCode,
                        principalSchema: "comun",
                        principalTable: "Ubigeos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasDebitoVenta_Vendedores_VendedorCode",
                        column: x => x.VendedorCode,
                        principalSchema: "facturacion",
                        principalTable: "Vendedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChequesVenta",
                schema: "facturacion",
                columns: table => new
                {
                    Number = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ClienteCode = table.Column<string>(type: "char(5)", nullable: false),
                    BancoCode = table.Column<string>(type: "char(3)", nullable: false),
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CobradorCode = table.Column<string>(type: "char(4)", nullable: true),
                    MonedaCode = table.Column<string>(type: "char(3)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PendingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProvisionEntryNumber = table.Column<string>(type: "char(13)", nullable: true),
                    ReboteEntryNumber = table.Column<string>(type: "char(13)", nullable: true),
                    CancellationEntryNumber = table.Column<string>(type: "char(13)", nullable: true),
                    CanceledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CancelacionCode = table.Column<string>(type: "char(10)", nullable: false),
                    IsDeferred = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    OperationNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OperationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoReboteCode = table.Column<string>(type: "char(2)", nullable: true),
                    ReboteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SettledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChequesVenta", x => new { x.Number, x.ClienteCode, x.BancoCode });
                    table.ForeignKey(
                        name: "FK_ChequesVenta_Bancos_BancoCode",
                        column: x => x.BancoCode,
                        principalSchema: "comun",
                        principalTable: "Bancos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChequesVenta_Clientes_ClienteCode",
                        column: x => x.ClienteCode,
                        principalSchema: "facturacion",
                        principalTable: "Clientes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChequesVenta_Cobradores_CobradorCode",
                        column: x => x.CobradorCode,
                        principalSchema: "facturacion",
                        principalTable: "Cobradores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChequesVenta_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChequesVenta_MotivosChequeRebote_MotivoReboteCode",
                        column: x => x.MotivoReboteCode,
                        principalSchema: "facturacion",
                        principalTable: "MotivosChequeRebote",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChequesVenta_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotasDebitoVentaDetalle",
                schema: "facturacion",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Line = table.Column<string>(type: "char(3)", nullable: false),
                    TipoProductoCode = table.Column<string>(type: "char(2)", nullable: false),
                    ProductoCode = table.Column<string>(type: "char(4)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineKey = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasDebitoVentaDetalle", x => new { x.PlantaCode, x.Number, x.Line });
                    table.ForeignKey(
                        name: "FK_NotasDebitoVentaDetalle_NotasDebitoVenta_PlantaCode_Number",
                        columns: x => new { x.PlantaCode, x.Number },
                        principalSchema: "facturacion",
                        principalTable: "NotasDebitoVenta",
                        principalColumns: new[] { "PlantaCode", "Number" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotasDebitoVentaDetalle_Productos_TipoProductoCode_ProductoCode",
                        columns: x => new { x.TipoProductoCode, x.ProductoCode },
                        principalSchema: "facturacion",
                        principalTable: "Productos",
                        principalColumns: new[] { "TipoProductoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChequesVenta_BancoCode",
                schema: "facturacion",
                table: "ChequesVenta",
                column: "BancoCode");

            migrationBuilder.CreateIndex(
                name: "IX_ChequesVenta_CancelacionCode",
                schema: "facturacion",
                table: "ChequesVenta",
                column: "CancelacionCode");

            migrationBuilder.CreateIndex(
                name: "IX_ChequesVenta_ClienteCode",
                schema: "facturacion",
                table: "ChequesVenta",
                column: "ClienteCode");

            migrationBuilder.CreateIndex(
                name: "IX_ChequesVenta_CobradorCode",
                schema: "facturacion",
                table: "ChequesVenta",
                column: "CobradorCode");

            migrationBuilder.CreateIndex(
                name: "IX_ChequesVenta_IssueDate",
                schema: "facturacion",
                table: "ChequesVenta",
                column: "IssueDate");

            migrationBuilder.CreateIndex(
                name: "IX_ChequesVenta_MonedaCode",
                schema: "facturacion",
                table: "ChequesVenta",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_ChequesVenta_MotivoReboteCode",
                schema: "facturacion",
                table: "ChequesVenta",
                column: "MotivoReboteCode");

            migrationBuilder.CreateIndex(
                name: "IX_ChequesVenta_PlantaCode",
                schema: "facturacion",
                table: "ChequesVenta",
                column: "PlantaCode");

            migrationBuilder.CreateIndex(
                name: "IX_ChequesVenta_Status",
                schema: "facturacion",
                table: "ChequesVenta",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MotivosChequeRebote_Name",
                schema: "facturacion",
                table: "MotivosChequeRebote",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotasDebitoVenta_ClienteCode",
                schema: "facturacion",
                table: "NotasDebitoVenta",
                column: "ClienteCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotasDebitoVenta_ClienteCode_ObraCode",
                schema: "facturacion",
                table: "NotasDebitoVenta",
                columns: new[] { "ClienteCode", "ObraCode" });

            migrationBuilder.CreateIndex(
                name: "IX_NotasDebitoVenta_DeliveryUbigeoCode",
                schema: "facturacion",
                table: "NotasDebitoVenta",
                column: "DeliveryUbigeoCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotasDebitoVenta_DocumentTypeCode",
                schema: "facturacion",
                table: "NotasDebitoVenta",
                column: "DocumentTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotasDebitoVenta_ExecutingPlantaCode",
                schema: "facturacion",
                table: "NotasDebitoVenta",
                column: "ExecutingPlantaCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotasDebitoVenta_FormaPagoVentaCode",
                schema: "facturacion",
                table: "NotasDebitoVenta",
                column: "FormaPagoVentaCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotasDebitoVenta_IssueDate",
                schema: "facturacion",
                table: "NotasDebitoVenta",
                column: "IssueDate");

            migrationBuilder.CreateIndex(
                name: "IX_NotasDebitoVenta_MonedaCode",
                schema: "facturacion",
                table: "NotasDebitoVenta",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotasDebitoVenta_Status",
                schema: "facturacion",
                table: "NotasDebitoVenta",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_NotasDebitoVenta_VendedorCode",
                schema: "facturacion",
                table: "NotasDebitoVenta",
                column: "VendedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotasDebitoVentaDetalle_TipoProductoCode_ProductoCode",
                schema: "facturacion",
                table: "NotasDebitoVentaDetalle",
                columns: new[] { "TipoProductoCode", "ProductoCode" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChequesVenta",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "NotasDebitoVentaDetalle",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "MotivosChequeRebote",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "NotasDebitoVenta",
                schema: "facturacion");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                schema: "contabilidad",
                table: "PlantillaAsientoLineas");
        }
    }
}
