using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cepheus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NotaCreditoVenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MotivosSolicitudNotaCredito",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ParentCode = table.Column<string>(type: "char(2)", nullable: true),
                    MotivoDevolucionCode = table.Column<string>(type: "char(2)", nullable: true),
                    AffectsStock = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivosSolicitudNotaCredito", x => x.Code);
                    table.ForeignKey(
                        name: "FK_MotivosSolicitudNotaCredito_MotivosDevolucionArticulo_MotivoDevolucionCode",
                        column: x => x.MotivoDevolucionCode,
                        principalSchema: "logistica",
                        principalTable: "MotivosDevolucionArticulo",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MotivosSolicitudNotaCredito_MotivosSolicitudNotaCredito_ParentCode",
                        column: x => x.ParentCode,
                        principalSchema: "facturacion",
                        principalTable: "MotivosSolicitudNotaCredito",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosSolicitudNotaCredito",
                schema: "facturacion",
                columns: table => new
                {
                    NegocioCode = table.Column<string>(type: "char(2)", nullable: false),
                    Stage = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsPrincipal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CanEnter = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CanManage = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CanApprove = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosSolicitudNotaCredito", x => new { x.NegocioCode, x.Stage, x.UserId });
                    table.ForeignKey(
                        name: "FK_UsuariosSolicitudNotaCredito_Negocios_NegocioCode",
                        column: x => x.NegocioCode,
                        principalSchema: "comun",
                        principalTable: "Negocios",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosSolicitudNotaCredito_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "admin",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesNotaCredito",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(8)", nullable: false),
                    Year = table.Column<string>(type: "char(4)", nullable: false),
                    Month = table.Column<string>(type: "char(2)", nullable: false),
                    NegocioCode = table.Column<string>(type: "char(2)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClienteCode = table.Column<string>(type: "char(5)", nullable: false),
                    DocumentTypeCode = table.Column<string>(type: "char(2)", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MotivoCode = table.Column<string>(type: "char(2)", nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    IgvAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Observation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsEditable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsProcedent = table.Column<bool>(type: "bit", nullable: true),
                    RequesterUserId = table.Column<int>(type: "int", nullable: true),
                    RequesterAccepts = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesNotaCredito", x => new { x.Year, x.Month, x.Code });
                    table.UniqueConstraint("AK_SolicitudesNotaCredito_Code_Year_Month", x => new { x.Code, x.Year, x.Month });
                    table.ForeignKey(
                        name: "FK_SolicitudesNotaCredito_Clientes_ClienteCode",
                        column: x => x.ClienteCode,
                        principalSchema: "facturacion",
                        principalTable: "Clientes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesNotaCredito_ComprobantesPago_DocumentTypeCode",
                        column: x => x.DocumentTypeCode,
                        principalSchema: "comun",
                        principalTable: "ComprobantesPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesNotaCredito_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesNotaCredito_MotivosSolicitudNotaCredito_MotivoCode",
                        column: x => x.MotivoCode,
                        principalSchema: "facturacion",
                        principalTable: "MotivosSolicitudNotaCredito",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesNotaCredito_Negocios_NegocioCode",
                        column: x => x.NegocioCode,
                        principalSchema: "comun",
                        principalTable: "Negocios",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesNotaCredito_Users_RequesterUserId",
                        column: x => x.RequesterUserId,
                        principalSchema: "admin",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotasCreditoVenta",
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
                    Type = table.Column<string>(type: "char(1)", nullable: false, defaultValue: "N"),
                    DocumentTypeCode = table.Column<string>(type: "char(2)", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MotivoDevolucionCode = table.Column<string>(type: "char(2)", nullable: false),
                    SolicitudCode = table.Column<string>(type: "char(8)", nullable: true),
                    SolicitudYear = table.Column<string>(type: "char(4)", nullable: true),
                    SolicitudMonth = table.Column<string>(type: "char(2)", nullable: true),
                    CashDocument = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasCreditoVenta", x => new { x.PlantaCode, x.Number });
                    table.ForeignKey(
                        name: "FK_NotasCreditoVenta_Clientes_ClienteCode",
                        column: x => x.ClienteCode,
                        principalSchema: "facturacion",
                        principalTable: "Clientes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasCreditoVenta_ComprobantesPago_DocumentTypeCode",
                        column: x => x.DocumentTypeCode,
                        principalSchema: "comun",
                        principalTable: "ComprobantesPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasCreditoVenta_FormasPagoVenta_FormaPagoVentaCode",
                        column: x => x.FormaPagoVentaCode,
                        principalSchema: "facturacion",
                        principalTable: "FormasPagoVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasCreditoVenta_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasCreditoVenta_MotivosDevolucionArticulo_MotivoDevolucionCode",
                        column: x => x.MotivoDevolucionCode,
                        principalSchema: "logistica",
                        principalTable: "MotivosDevolucionArticulo",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasCreditoVenta_Obras_ClienteCode_ObraCode",
                        columns: x => new { x.ClienteCode, x.ObraCode },
                        principalSchema: "facturacion",
                        principalTable: "Obras",
                        principalColumns: new[] { "ClienteCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasCreditoVenta_Plantas_ExecutingPlantaCode",
                        column: x => x.ExecutingPlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasCreditoVenta_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasCreditoVenta_SolicitudesNotaCredito_SolicitudCode_SolicitudYear_SolicitudMonth",
                        columns: x => new { x.SolicitudCode, x.SolicitudYear, x.SolicitudMonth },
                        principalSchema: "facturacion",
                        principalTable: "SolicitudesNotaCredito",
                        principalColumns: new[] { "Code", "Year", "Month" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasCreditoVenta_Ubigeos_DeliveryUbigeoCode",
                        column: x => x.DeliveryUbigeoCode,
                        principalSchema: "comun",
                        principalTable: "Ubigeos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasCreditoVenta_Vendedores_VendedorCode",
                        column: x => x.VendedorCode,
                        principalSchema: "facturacion",
                        principalTable: "Vendedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesNotaCreditoAprobaciones",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(8)", nullable: false),
                    Year = table.Column<string>(type: "char(4)", nullable: false),
                    Month = table.Column<string>(type: "char(2)", nullable: false),
                    Stage = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ActedByUserId = table.Column<int>(type: "int", nullable: true),
                    ActedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ActedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesNotaCreditoAprobaciones", x => new { x.Year, x.Month, x.Code, x.Stage });
                    table.ForeignKey(
                        name: "FK_SolicitudesNotaCreditoAprobaciones_SolicitudesNotaCredito_Code_Year_Month",
                        columns: x => new { x.Code, x.Year, x.Month },
                        principalSchema: "facturacion",
                        principalTable: "SolicitudesNotaCredito",
                        principalColumns: new[] { "Code", "Year", "Month" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesNotaCreditoArchivos",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(8)", nullable: false),
                    Year = table.Column<string>(type: "char(4)", nullable: false),
                    Month = table.Column<string>(type: "char(2)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesNotaCreditoArchivos", x => new { x.Year, x.Month, x.Code, x.FileName });
                    table.ForeignKey(
                        name: "FK_SolicitudesNotaCreditoArchivos_SolicitudesNotaCredito_Code_Year_Month",
                        columns: x => new { x.Code, x.Year, x.Month },
                        principalSchema: "facturacion",
                        principalTable: "SolicitudesNotaCredito",
                        principalColumns: new[] { "Code", "Year", "Month" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesNotaCreditoDetalle",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(8)", nullable: false),
                    Year = table.Column<string>(type: "char(4)", nullable: false),
                    Month = table.Column<string>(type: "char(2)", nullable: false),
                    TipoProductoCode = table.Column<string>(type: "char(2)", nullable: false),
                    ProductoCode = table.Column<string>(type: "char(4)", nullable: false),
                    LineKey = table.Column<int>(type: "int", nullable: false),
                    Line = table.Column<string>(type: "char(3)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderCode = table.Column<string>(type: "char(8)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesNotaCreditoDetalle", x => new { x.Year, x.Month, x.Code, x.TipoProductoCode, x.ProductoCode, x.LineKey });
                    table.ForeignKey(
                        name: "FK_SolicitudesNotaCreditoDetalle_Productos_TipoProductoCode_ProductoCode",
                        columns: x => new { x.TipoProductoCode, x.ProductoCode },
                        principalSchema: "facturacion",
                        principalTable: "Productos",
                        principalColumns: new[] { "TipoProductoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesNotaCreditoDetalle_SolicitudesNotaCredito_Code_Year_Month",
                        columns: x => new { x.Code, x.Year, x.Month },
                        principalSchema: "facturacion",
                        principalTable: "SolicitudesNotaCredito",
                        principalColumns: new[] { "Code", "Year", "Month" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesNotaCreditoGuias",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(8)", nullable: false),
                    Year = table.Column<string>(type: "char(4)", nullable: false),
                    Month = table.Column<string>(type: "char(2)", nullable: false),
                    GuideNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesNotaCreditoGuias", x => new { x.Year, x.Month, x.Code, x.GuideNumber });
                    table.ForeignKey(
                        name: "FK_SolicitudesNotaCreditoGuias_SolicitudesNotaCredito_Code_Year_Month",
                        columns: x => new { x.Code, x.Year, x.Month },
                        principalSchema: "facturacion",
                        principalTable: "SolicitudesNotaCredito",
                        principalColumns: new[] { "Code", "Year", "Month" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotasCreditoVentaDetalle",
                schema: "facturacion",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Line = table.Column<string>(type: "char(3)", nullable: false),
                    GuideNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TipoProductoCode = table.Column<string>(type: "char(2)", nullable: false),
                    ProductoCode = table.Column<string>(type: "char(4)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DispatchedQuantity = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineKey = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    OrderCode = table.Column<string>(type: "char(8)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasCreditoVentaDetalle", x => new { x.PlantaCode, x.Number, x.Line });
                    table.ForeignKey(
                        name: "FK_NotasCreditoVentaDetalle_NotasCreditoVenta_PlantaCode_Number",
                        columns: x => new { x.PlantaCode, x.Number },
                        principalSchema: "facturacion",
                        principalTable: "NotasCreditoVenta",
                        principalColumns: new[] { "PlantaCode", "Number" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotasCreditoVentaDetalle_Productos_TipoProductoCode_ProductoCode",
                        columns: x => new { x.TipoProductoCode, x.ProductoCode },
                        principalSchema: "facturacion",
                        principalTable: "Productos",
                        principalColumns: new[] { "TipoProductoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MotivosSolicitudNotaCredito_MotivoDevolucionCode",
                schema: "facturacion",
                table: "MotivosSolicitudNotaCredito",
                column: "MotivoDevolucionCode");

            migrationBuilder.CreateIndex(
                name: "IX_MotivosSolicitudNotaCredito_Name",
                schema: "facturacion",
                table: "MotivosSolicitudNotaCredito",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MotivosSolicitudNotaCredito_ParentCode",
                schema: "facturacion",
                table: "MotivosSolicitudNotaCredito",
                column: "ParentCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCreditoVenta_ClienteCode",
                schema: "facturacion",
                table: "NotasCreditoVenta",
                column: "ClienteCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCreditoVenta_ClienteCode_ObraCode",
                schema: "facturacion",
                table: "NotasCreditoVenta",
                columns: new[] { "ClienteCode", "ObraCode" });

            migrationBuilder.CreateIndex(
                name: "IX_NotasCreditoVenta_DeliveryUbigeoCode",
                schema: "facturacion",
                table: "NotasCreditoVenta",
                column: "DeliveryUbigeoCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCreditoVenta_DocumentTypeCode",
                schema: "facturacion",
                table: "NotasCreditoVenta",
                column: "DocumentTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCreditoVenta_ExecutingPlantaCode",
                schema: "facturacion",
                table: "NotasCreditoVenta",
                column: "ExecutingPlantaCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCreditoVenta_FormaPagoVentaCode",
                schema: "facturacion",
                table: "NotasCreditoVenta",
                column: "FormaPagoVentaCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCreditoVenta_IssueDate",
                schema: "facturacion",
                table: "NotasCreditoVenta",
                column: "IssueDate");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCreditoVenta_MonedaCode",
                schema: "facturacion",
                table: "NotasCreditoVenta",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCreditoVenta_MotivoDevolucionCode",
                schema: "facturacion",
                table: "NotasCreditoVenta",
                column: "MotivoDevolucionCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCreditoVenta_SolicitudCode_SolicitudYear_SolicitudMonth",
                schema: "facturacion",
                table: "NotasCreditoVenta",
                columns: new[] { "SolicitudCode", "SolicitudYear", "SolicitudMonth" });

            migrationBuilder.CreateIndex(
                name: "IX_NotasCreditoVenta_Status",
                schema: "facturacion",
                table: "NotasCreditoVenta",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCreditoVenta_VendedorCode",
                schema: "facturacion",
                table: "NotasCreditoVenta",
                column: "VendedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCreditoVentaDetalle_TipoProductoCode_ProductoCode",
                schema: "facturacion",
                table: "NotasCreditoVentaDetalle",
                columns: new[] { "TipoProductoCode", "ProductoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesNotaCredito_ClienteCode",
                schema: "facturacion",
                table: "SolicitudesNotaCredito",
                column: "ClienteCode");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesNotaCredito_DocumentTypeCode",
                schema: "facturacion",
                table: "SolicitudesNotaCredito",
                column: "DocumentTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesNotaCredito_IssueDate",
                schema: "facturacion",
                table: "SolicitudesNotaCredito",
                column: "IssueDate");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesNotaCredito_MonedaCode",
                schema: "facturacion",
                table: "SolicitudesNotaCredito",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesNotaCredito_MotivoCode",
                schema: "facturacion",
                table: "SolicitudesNotaCredito",
                column: "MotivoCode");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesNotaCredito_NegocioCode",
                schema: "facturacion",
                table: "SolicitudesNotaCredito",
                column: "NegocioCode");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesNotaCredito_RequesterUserId",
                schema: "facturacion",
                table: "SolicitudesNotaCredito",
                column: "RequesterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesNotaCredito_Status",
                schema: "facturacion",
                table: "SolicitudesNotaCredito",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesNotaCreditoAprobaciones_Code_Year_Month",
                schema: "facturacion",
                table: "SolicitudesNotaCreditoAprobaciones",
                columns: new[] { "Code", "Year", "Month" });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesNotaCreditoArchivos_Code_Year_Month",
                schema: "facturacion",
                table: "SolicitudesNotaCreditoArchivos",
                columns: new[] { "Code", "Year", "Month" });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesNotaCreditoDetalle_Code_Year_Month_Line",
                schema: "facturacion",
                table: "SolicitudesNotaCreditoDetalle",
                columns: new[] { "Code", "Year", "Month", "Line" });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesNotaCreditoDetalle_TipoProductoCode_ProductoCode",
                schema: "facturacion",
                table: "SolicitudesNotaCreditoDetalle",
                columns: new[] { "TipoProductoCode", "ProductoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesNotaCreditoGuias_Code_Year_Month",
                schema: "facturacion",
                table: "SolicitudesNotaCreditoGuias",
                columns: new[] { "Code", "Year", "Month" });

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosSolicitudNotaCredito_NegocioCode_Stage",
                schema: "facturacion",
                table: "UsuariosSolicitudNotaCredito",
                columns: new[] { "NegocioCode", "Stage" });

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosSolicitudNotaCredito_UserId",
                schema: "facturacion",
                table: "UsuariosSolicitudNotaCredito",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotasCreditoVentaDetalle",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "SolicitudesNotaCreditoAprobaciones",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "SolicitudesNotaCreditoArchivos",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "SolicitudesNotaCreditoDetalle",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "SolicitudesNotaCreditoGuias",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "UsuariosSolicitudNotaCredito",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "NotasCreditoVenta",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "SolicitudesNotaCredito",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "MotivosSolicitudNotaCredito",
                schema: "facturacion");
        }
    }
}
