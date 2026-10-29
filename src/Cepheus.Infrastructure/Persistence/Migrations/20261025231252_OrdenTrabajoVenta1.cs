using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cepheus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OrdenTrabajoVenta1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrdenTrabajoCode",
                schema: "facturacion",
                table: "HojasCorte",
                type: "char(8)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ElementosConcreto",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UsesBalance = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementosConcreto", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesTrabajoVenta",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(8)", nullable: false),
                    Type = table.Column<string>(type: "char(1)", nullable: false),
                    Stage = table.Column<string>(type: "char(1)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ClienteCode = table.Column<string>(type: "char(5)", nullable: false),
                    ObraCode = table.Column<string>(type: "char(3)", nullable: false),
                    DispatchDate = table.Column<DateTime>(type: "date", nullable: false),
                    DispatchTime = table.Column<string>(type: "varchar(5)", nullable: false),
                    VendedorCode = table.Column<string>(type: "char(4)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "char(1)", nullable: false),
                    MotivoCode = table.Column<string>(type: "char(2)", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreditObservations = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, defaultValue: ""),
                    CutSheetObservations = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PurchaseOrderNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    OriginCode = table.Column<string>(type: "char(8)", nullable: true),
                    CotizacionNegocioCode = table.Column<string>(type: "char(2)", nullable: true),
                    CotizacionYear = table.Column<string>(type: "char(4)", nullable: true),
                    CotizacionMonth = table.Column<string>(type: "char(2)", nullable: true),
                    CotizacionCode = table.Column<string>(type: "char(8)", nullable: true),
                    IsModifiable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    SentToApprovalAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalObservations = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovalDelayReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PassedToProduction = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ProductionPassDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductionPassReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SpecialApprovalGranted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SpecialApprovalBy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SpecialApprovalAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SpecialApprovalObservations = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CanceledBy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CanceledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InactivatedBy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    InactivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InactivationReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesTrabajoVenta", x => x.Code);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajoVenta_Clientes_ClienteCode",
                        column: x => x.ClienteCode,
                        principalSchema: "facturacion",
                        principalTable: "Clientes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajoVenta_Cotizaciones_CotizacionNegocioCode_CotizacionYear_CotizacionMonth_CotizacionCode",
                        columns: x => new { x.CotizacionNegocioCode, x.CotizacionYear, x.CotizacionMonth, x.CotizacionCode },
                        principalSchema: "facturacion",
                        principalTable: "Cotizaciones",
                        principalColumns: new[] { "NegocioCode", "Year", "Month", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajoVenta_MotivosDevolucion_MotivoCode",
                        column: x => x.MotivoCode,
                        principalSchema: "comun",
                        principalTable: "MotivosDevolucion",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajoVenta_Obras_ClienteCode_ObraCode",
                        columns: x => new { x.ClienteCode, x.ObraCode },
                        principalSchema: "facturacion",
                        principalTable: "Obras",
                        principalColumns: new[] { "ClienteCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajoVenta_OrdenesTrabajoVenta_OriginCode",
                        column: x => x.OriginCode,
                        principalSchema: "facturacion",
                        principalTable: "OrdenesTrabajoVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajoVenta_Vendedores_VendedorCode",
                        column: x => x.VendedorCode,
                        principalSchema: "facturacion",
                        principalTable: "Vendedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComprobantesVenta",
                schema: "facturacion",
                columns: table => new
                {
                    TypeCode = table.Column<string>(type: "char(2)", nullable: false),
                    Number = table.Column<string>(type: "char(10)", nullable: false),
                    PlantCode = table.Column<string>(type: "char(2)", nullable: false),
                    ExecutingPlantCode = table.Column<string>(type: "char(2)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "date", nullable: false),
                    ClienteCode = table.Column<string>(type: "char(5)", nullable: false),
                    ObraCode = table.Column<string>(type: "char(3)", nullable: false),
                    VendedorCode = table.Column<string>(type: "char(4)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UbigeoCode = table.Column<string>(type: "char(6)", nullable: false),
                    PaymentFormCode = table.Column<string>(type: "char(2)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    OrdenTrabajoCode = table.Column<string>(type: "char(8)", nullable: true),
                    Origin = table.Column<int>(type: "int", nullable: false),
                    DocumentFlow = table.Column<int>(type: "int", nullable: true),
                    IsFreeTransfer = table.Column<bool>(type: "bit", nullable: false),
                    Glosa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IgvPercentage = table.Column<decimal>(type: "decimal(9,2)", nullable: false),
                    SubtotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IgvAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalSoles = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsPrinted = table.Column<bool>(type: "bit", nullable: false),
                    PrintedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProvisionEntry = table.Column<string>(type: "char(13)", nullable: true),
                    DetractionDocument = table.Column<string>(type: "char(10)", nullable: true),
                    DetractionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CashDocument = table.Column<string>(type: "char(10)", nullable: true),
                    CanceledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CanceledBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CancelReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ReplacedByNumber = table.Column<string>(type: "char(10)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprobantesVenta", x => new { x.TypeCode, x.Number });
                    table.ForeignKey(
                        name: "FK_ComprobantesVenta_Clientes_ClienteCode",
                        column: x => x.ClienteCode,
                        principalSchema: "facturacion",
                        principalTable: "Clientes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprobantesVenta_FormasPagoVenta_PaymentFormCode",
                        column: x => x.PaymentFormCode,
                        principalSchema: "facturacion",
                        principalTable: "FormasPagoVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprobantesVenta_Obras_ClienteCode_ObraCode",
                        columns: x => new { x.ClienteCode, x.ObraCode },
                        principalSchema: "facturacion",
                        principalTable: "Obras",
                        principalColumns: new[] { "ClienteCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprobantesVenta_OrdenesTrabajoVenta_OrdenTrabajoCode",
                        column: x => x.OrdenTrabajoCode,
                        principalSchema: "facturacion",
                        principalTable: "OrdenesTrabajoVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprobantesVenta_Plantas_PlantCode",
                        column: x => x.PlantCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprobantesVenta_Ubigeos_UbigeoCode",
                        column: x => x.UbigeoCode,
                        principalSchema: "comun",
                        principalTable: "Ubigeos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprobantesVenta_Vendedores_VendedorCode",
                        column: x => x.VendedorCode,
                        principalSchema: "facturacion",
                        principalTable: "Vendedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GuiasRemisionVenta",
                schema: "facturacion",
                columns: table => new
                {
                    Number = table.Column<string>(type: "char(10)", nullable: false),
                    PlantCode = table.Column<string>(type: "char(2)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "date", nullable: false),
                    DispatchTime = table.Column<string>(type: "char(5)", nullable: true),
                    Motive = table.Column<int>(type: "int", nullable: false),
                    ClienteCode = table.Column<string>(type: "char(5)", nullable: false),
                    ObraCode = table.Column<string>(type: "char(3)", nullable: false),
                    VendedorCode = table.Column<string>(type: "char(4)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UbigeoCode = table.Column<string>(type: "char(6)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TransportistaCode = table.Column<string>(type: "char(4)", nullable: false),
                    ChoferCode = table.Column<string>(type: "char(4)", nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: true),
                    VehicleCode = table.Column<string>(type: "char(4)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DocumentFlow = table.Column<int>(type: "int", nullable: true),
                    DocumentTypeCode = table.Column<string>(type: "char(2)", nullable: true),
                    Document1Number = table.Column<string>(type: "char(10)", nullable: true),
                    Document2Number = table.Column<string>(type: "char(10)", nullable: true),
                    OrdenTrabajoCode = table.Column<string>(type: "char(8)", nullable: true),
                    ProgramacionType = table.Column<int>(type: "int", nullable: true),
                    ProgramacionCode = table.Column<string>(type: "char(8)", nullable: true),
                    FleteCode = table.Column<string>(type: "char(2)", nullable: false),
                    FleteAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReplacesNumber = table.Column<string>(type: "char(10)", nullable: true),
                    DevolutionMotiveCode = table.Column<string>(type: "char(2)", nullable: true),
                    DevolutionActionCode = table.Column<string>(type: "char(2)", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TicketNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PumpCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    IsPrinted = table.Column<bool>(type: "bit", nullable: false),
                    PrintedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsBalance = table.Column<bool>(type: "bit", nullable: false),
                    CanceledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CanceledBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CancelReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuiasRemisionVenta", x => x.Number);
                    table.ForeignKey(
                        name: "FK_GuiasRemisionVenta_ChoferesVentas_TransportistaCode_ChoferCode",
                        columns: x => new { x.TransportistaCode, x.ChoferCode },
                        principalSchema: "facturacion",
                        principalTable: "ChoferesVentas",
                        principalColumns: new[] { "TransportistaCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuiasRemisionVenta_Clientes_ClienteCode",
                        column: x => x.ClienteCode,
                        principalSchema: "facturacion",
                        principalTable: "Clientes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuiasRemisionVenta_Fletes_FleteCode",
                        column: x => x.FleteCode,
                        principalSchema: "facturacion",
                        principalTable: "Fletes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuiasRemisionVenta_MotivosDevolucion_DevolutionMotiveCode",
                        column: x => x.DevolutionMotiveCode,
                        principalSchema: "comun",
                        principalTable: "MotivosDevolucion",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuiasRemisionVenta_Obras_ClienteCode_ObraCode",
                        columns: x => new { x.ClienteCode, x.ObraCode },
                        principalSchema: "facturacion",
                        principalTable: "Obras",
                        principalColumns: new[] { "ClienteCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuiasRemisionVenta_OrdenesTrabajoVenta_OrdenTrabajoCode",
                        column: x => x.OrdenTrabajoCode,
                        principalSchema: "facturacion",
                        principalTable: "OrdenesTrabajoVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuiasRemisionVenta_Plantas_PlantCode",
                        column: x => x.PlantCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuiasRemisionVenta_TransportistasVentas_TransportistaCode",
                        column: x => x.TransportistaCode,
                        principalSchema: "facturacion",
                        principalTable: "TransportistasVentas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuiasRemisionVenta_Ubigeos_UbigeoCode",
                        column: x => x.UbigeoCode,
                        principalSchema: "comun",
                        principalTable: "Ubigeos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuiasRemisionVenta_Vehiculos_TransportistaCode_VehicleType_VehicleCode",
                        columns: x => new { x.TransportistaCode, x.VehicleType, x.VehicleCode },
                        principalSchema: "facturacion",
                        principalTable: "Vehiculos",
                        principalColumns: new[] { "TransportistaCode", "VehicleType", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuiasRemisionVenta_Vendedores_VendedorCode",
                        column: x => x.VendedorCode,
                        principalSchema: "facturacion",
                        principalTable: "Vendedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesTrabajoVentaAprobaciones",
                schema: "facturacion",
                columns: table => new
                {
                    OrdenTrabajoCode = table.Column<string>(type: "char(8)", nullable: false),
                    CreditsObservations = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Document1Number = table.Column<string>(type: "char(10)", nullable: true),
                    DocumentType = table.Column<string>(type: "char(1)", nullable: false),
                    ApprovedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentType = table.Column<string>(type: "char(1)", nullable: false),
                    CreditStatus = table.Column<string>(type: "char(1)", nullable: false),
                    DocumentFlow = table.Column<string>(type: "char(1)", nullable: false),
                    ValorizationCode = table.Column<string>(type: "char(1)", nullable: false, defaultValue: "N"),
                    ValorizationWeekday = table.Column<int>(type: "int", nullable: true),
                    ValorizationDays = table.Column<int>(type: "int", nullable: true),
                    Document2Number = table.Column<string>(type: "char(10)", nullable: true),
                    HasTwoDocuments = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Plant1Code = table.Column<string>(type: "char(2)", nullable: true),
                    Plant2Code = table.Column<string>(type: "char(2)", nullable: true),
                    GuideMotivoCode = table.Column<string>(type: "char(2)", nullable: true),
                    PaymentMethodCode = table.Column<string>(type: "char(2)", nullable: true),
                    BankCode = table.Column<string>(type: "char(3)", nullable: true),
                    PaymentDocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FormaPagoVentaCode = table.Column<string>(type: "char(2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesTrabajoVentaAprobaciones", x => x.OrdenTrabajoCode);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajoVentaAprobaciones_Bancos_BankCode",
                        column: x => x.BankCode,
                        principalSchema: "comun",
                        principalTable: "Bancos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajoVentaAprobaciones_FormasPagoVenta_FormaPagoVentaCode",
                        column: x => x.FormaPagoVentaCode,
                        principalSchema: "facturacion",
                        principalTable: "FormasPagoVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajoVentaAprobaciones_OrdenesTrabajoVenta_OrdenTrabajoCode",
                        column: x => x.OrdenTrabajoCode,
                        principalSchema: "facturacion",
                        principalTable: "OrdenesTrabajoVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajoVentaAprobaciones_Plantas_Plant1Code",
                        column: x => x.Plant1Code,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajoVentaAprobaciones_Plantas_Plant2Code",
                        column: x => x.Plant2Code,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesTrabajoVentaAprobacionesAnuladas",
                schema: "facturacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenTrabajoCode = table.Column<string>(type: "char(8)", nullable: false),
                    Reason = table.Column<string>(type: "char(2)", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DocumentType = table.Column<string>(type: "char(1)", nullable: false),
                    Document1Number = table.Column<string>(type: "char(10)", nullable: true),
                    Document2Number = table.Column<string>(type: "char(10)", nullable: true),
                    Plant1Code = table.Column<string>(type: "char(2)", nullable: true),
                    Plant2Code = table.Column<string>(type: "char(2)", nullable: true),
                    ApprovedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesTrabajoVentaAprobacionesAnuladas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajoVentaAprobacionesAnuladas_OrdenesTrabajoVenta_OrdenTrabajoCode",
                        column: x => x.OrdenTrabajoCode,
                        principalSchema: "facturacion",
                        principalTable: "OrdenesTrabajoVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesTrabajoVentaDetalle",
                schema: "facturacion",
                columns: table => new
                {
                    OrdenTrabajoCode = table.Column<string>(type: "char(8)", nullable: false),
                    Item = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoTipoCode = table.Column<string>(type: "char(2)", nullable: false),
                    ProductoCode = table.Column<string>(type: "char(4)", nullable: false),
                    NegocioCode = table.Column<string>(type: "char(2)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApprovedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    ProgrammedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    DeliveredQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "char(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesTrabajoVentaDetalle", x => new { x.OrdenTrabajoCode, x.Item });
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajoVentaDetalle_Negocios_NegocioCode",
                        column: x => x.NegocioCode,
                        principalSchema: "comun",
                        principalTable: "Negocios",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajoVentaDetalle_OrdenesTrabajoVenta_OrdenTrabajoCode",
                        column: x => x.OrdenTrabajoCode,
                        principalSchema: "facturacion",
                        principalTable: "OrdenesTrabajoVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajoVentaDetalle_Productos_ProductoTipoCode_ProductoCode",
                        columns: x => new { x.ProductoTipoCode, x.ProductoCode },
                        principalSchema: "facturacion",
                        principalTable: "Productos",
                        principalColumns: new[] { "TipoProductoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProgramacionesConcreto",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(8)", nullable: false),
                    OrdenTrabajoCode = table.Column<string>(type: "char(8)", nullable: false),
                    ProgramDate = table.Column<DateTime>(type: "date", nullable: false),
                    ProgramTime = table.Column<string>(type: "char(5)", nullable: false),
                    PlantCode = table.Column<string>(type: "char(2)", nullable: false),
                    ElementoCode = table.Column<string>(type: "char(2)", nullable: false),
                    TransportObservations = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RequiresLabService = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramacionesConcreto", x => x.Code);
                    table.ForeignKey(
                        name: "FK_ProgramacionesConcreto_ElementosConcreto_ElementoCode",
                        column: x => x.ElementoCode,
                        principalSchema: "facturacion",
                        principalTable: "ElementosConcreto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramacionesConcreto_OrdenesTrabajoVenta_OrdenTrabajoCode",
                        column: x => x.OrdenTrabajoCode,
                        principalSchema: "facturacion",
                        principalTable: "OrdenesTrabajoVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramacionesConcreto_Plantas_PlantCode",
                        column: x => x.PlantCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProgramacionesOtros",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(8)", nullable: false),
                    OrdenTrabajoCode = table.Column<string>(type: "char(8)", nullable: false),
                    ProgramDate = table.Column<DateTime>(type: "date", nullable: false),
                    ProgramTime = table.Column<string>(type: "char(5)", nullable: false),
                    PlantCode = table.Column<string>(type: "char(2)", nullable: false),
                    TransportObservations = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramacionesOtros", x => x.Code);
                    table.ForeignKey(
                        name: "FK_ProgramacionesOtros_OrdenesTrabajoVenta_OrdenTrabajoCode",
                        column: x => x.OrdenTrabajoCode,
                        principalSchema: "facturacion",
                        principalTable: "OrdenesTrabajoVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramacionesOtros_Plantas_PlantCode",
                        column: x => x.PlantCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GuiasRemisionVentaDetalle",
                schema: "facturacion",
                columns: table => new
                {
                    GuiaNumber = table.Column<string>(type: "char(10)", nullable: false),
                    Line = table.Column<int>(type: "int", nullable: false),
                    OrdenTrabajoCode = table.Column<string>(type: "char(8)", nullable: true),
                    OrdenTrabajoItem = table.Column<int>(type: "int", nullable: true),
                    ProductoTipoCode = table.Column<string>(type: "char(2)", nullable: false),
                    ProductoCode = table.Column<string>(type: "char(4)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TopeQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuiasRemisionVentaDetalle", x => new { x.GuiaNumber, x.Line });
                    table.ForeignKey(
                        name: "FK_GuiasRemisionVentaDetalle_GuiasRemisionVenta_GuiaNumber",
                        column: x => x.GuiaNumber,
                        principalSchema: "facturacion",
                        principalTable: "GuiasRemisionVenta",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuiasRemisionVentaDetalle_OrdenesTrabajoVentaDetalle_OrdenTrabajoCode_OrdenTrabajoItem",
                        columns: x => new { x.OrdenTrabajoCode, x.OrdenTrabajoItem },
                        principalSchema: "facturacion",
                        principalTable: "OrdenesTrabajoVentaDetalle",
                        principalColumns: new[] { "OrdenTrabajoCode", "Item" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuiasRemisionVentaDetalle_Productos_ProductoTipoCode_ProductoCode",
                        columns: x => new { x.ProductoTipoCode, x.ProductoCode },
                        principalSchema: "facturacion",
                        principalTable: "Productos",
                        principalColumns: new[] { "TipoProductoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProgramacionesConcretoDetalle",
                schema: "facturacion",
                columns: table => new
                {
                    ProgramacionCode = table.Column<string>(type: "char(8)", nullable: false),
                    Item = table.Column<int>(type: "int", nullable: false),
                    OrdenTrabajoCode = table.Column<string>(type: "char(8)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeliveredQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    PumpCode = table.Column<string>(type: "char(6)", nullable: true),
                    Support = table.Column<byte>(type: "tinyint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramacionesConcretoDetalle", x => new { x.ProgramacionCode, x.Item });
                    table.ForeignKey(
                        name: "FK_ProgramacionesConcretoDetalle_OrdenesTrabajoVentaDetalle_OrdenTrabajoCode_Item",
                        columns: x => new { x.OrdenTrabajoCode, x.Item },
                        principalSchema: "facturacion",
                        principalTable: "OrdenesTrabajoVentaDetalle",
                        principalColumns: new[] { "OrdenTrabajoCode", "Item" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramacionesConcretoDetalle_ProgramacionesConcreto_ProgramacionCode",
                        column: x => x.ProgramacionCode,
                        principalSchema: "facturacion",
                        principalTable: "ProgramacionesConcreto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramacionesOtrosDetalle",
                schema: "facturacion",
                columns: table => new
                {
                    ProgramacionCode = table.Column<string>(type: "char(8)", nullable: false),
                    Item = table.Column<int>(type: "int", nullable: false),
                    OrdenTrabajoCode = table.Column<string>(type: "char(8)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeliveredQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramacionesOtrosDetalle", x => new { x.ProgramacionCode, x.Item });
                    table.ForeignKey(
                        name: "FK_ProgramacionesOtrosDetalle_OrdenesTrabajoVentaDetalle_OrdenTrabajoCode_Item",
                        columns: x => new { x.OrdenTrabajoCode, x.Item },
                        principalSchema: "facturacion",
                        principalTable: "OrdenesTrabajoVentaDetalle",
                        principalColumns: new[] { "OrdenTrabajoCode", "Item" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramacionesOtrosDetalle_ProgramacionesOtros_ProgramacionCode",
                        column: x => x.ProgramacionCode,
                        principalSchema: "facturacion",
                        principalTable: "ProgramacionesOtros",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComprobantesVentaDetalle",
                schema: "facturacion",
                columns: table => new
                {
                    TypeCode = table.Column<string>(type: "char(2)", nullable: false),
                    Number = table.Column<string>(type: "char(10)", nullable: false),
                    Line = table.Column<int>(type: "int", nullable: false),
                    GuiaNumber = table.Column<string>(type: "char(10)", nullable: true),
                    GuiaLine = table.Column<int>(type: "int", nullable: true),
                    OrdenTrabajoCode = table.Column<string>(type: "char(8)", nullable: true),
                    OrdenTrabajoItem = table.Column<int>(type: "int", nullable: true),
                    ProductoTipoCode = table.Column<string>(type: "char(2)", nullable: false),
                    ProductoCode = table.Column<string>(type: "char(4)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprobantesVentaDetalle", x => new { x.TypeCode, x.Number, x.Line });
                    table.ForeignKey(
                        name: "FK_ComprobantesVentaDetalle_ComprobantesVenta_TypeCode_Number",
                        columns: x => new { x.TypeCode, x.Number },
                        principalSchema: "facturacion",
                        principalTable: "ComprobantesVenta",
                        principalColumns: new[] { "TypeCode", "Number" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComprobantesVentaDetalle_GuiasRemisionVentaDetalle_GuiaNumber_GuiaLine",
                        columns: x => new { x.GuiaNumber, x.GuiaLine },
                        principalSchema: "facturacion",
                        principalTable: "GuiasRemisionVentaDetalle",
                        principalColumns: new[] { "GuiaNumber", "Line" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprobantesVentaDetalle_Productos_ProductoTipoCode_ProductoCode",
                        columns: x => new { x.ProductoTipoCode, x.ProductoCode },
                        principalSchema: "facturacion",
                        principalTable: "Productos",
                        principalColumns: new[] { "TipoProductoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesVenta_ClienteCode_ObraCode",
                schema: "facturacion",
                table: "ComprobantesVenta",
                columns: new[] { "ClienteCode", "ObraCode" });

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesVenta_IssueDate",
                schema: "facturacion",
                table: "ComprobantesVenta",
                column: "IssueDate");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesVenta_OrdenTrabajoCode",
                schema: "facturacion",
                table: "ComprobantesVenta",
                column: "OrdenTrabajoCode");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesVenta_PaymentFormCode",
                schema: "facturacion",
                table: "ComprobantesVenta",
                column: "PaymentFormCode");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesVenta_PlantCode",
                schema: "facturacion",
                table: "ComprobantesVenta",
                column: "PlantCode");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesVenta_Status",
                schema: "facturacion",
                table: "ComprobantesVenta",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesVenta_UbigeoCode",
                schema: "facturacion",
                table: "ComprobantesVenta",
                column: "UbigeoCode");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesVenta_VendedorCode",
                schema: "facturacion",
                table: "ComprobantesVenta",
                column: "VendedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesVentaDetalle_GuiaNumber_GuiaLine",
                schema: "facturacion",
                table: "ComprobantesVentaDetalle",
                columns: new[] { "GuiaNumber", "GuiaLine" });

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesVentaDetalle_ProductoTipoCode_ProductoCode",
                schema: "facturacion",
                table: "ComprobantesVentaDetalle",
                columns: new[] { "ProductoTipoCode", "ProductoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_ElementosConcreto_Name",
                schema: "facturacion",
                table: "ElementosConcreto",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuiasRemisionVenta_ClienteCode_ObraCode",
                schema: "facturacion",
                table: "GuiasRemisionVenta",
                columns: new[] { "ClienteCode", "ObraCode" });

            migrationBuilder.CreateIndex(
                name: "IX_GuiasRemisionVenta_DevolutionMotiveCode",
                schema: "facturacion",
                table: "GuiasRemisionVenta",
                column: "DevolutionMotiveCode");

            migrationBuilder.CreateIndex(
                name: "IX_GuiasRemisionVenta_DocumentTypeCode_Document1Number",
                schema: "facturacion",
                table: "GuiasRemisionVenta",
                columns: new[] { "DocumentTypeCode", "Document1Number" });

            migrationBuilder.CreateIndex(
                name: "IX_GuiasRemisionVenta_FleteCode",
                schema: "facturacion",
                table: "GuiasRemisionVenta",
                column: "FleteCode");

            migrationBuilder.CreateIndex(
                name: "IX_GuiasRemisionVenta_IssueDate",
                schema: "facturacion",
                table: "GuiasRemisionVenta",
                column: "IssueDate");

            migrationBuilder.CreateIndex(
                name: "IX_GuiasRemisionVenta_OrdenTrabajoCode",
                schema: "facturacion",
                table: "GuiasRemisionVenta",
                column: "OrdenTrabajoCode");

            migrationBuilder.CreateIndex(
                name: "IX_GuiasRemisionVenta_PlantCode",
                schema: "facturacion",
                table: "GuiasRemisionVenta",
                column: "PlantCode");

            migrationBuilder.CreateIndex(
                name: "IX_GuiasRemisionVenta_ProgramacionType_ProgramacionCode",
                schema: "facturacion",
                table: "GuiasRemisionVenta",
                columns: new[] { "ProgramacionType", "ProgramacionCode" });

            migrationBuilder.CreateIndex(
                name: "IX_GuiasRemisionVenta_Status",
                schema: "facturacion",
                table: "GuiasRemisionVenta",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_GuiasRemisionVenta_TransportistaCode_ChoferCode",
                schema: "facturacion",
                table: "GuiasRemisionVenta",
                columns: new[] { "TransportistaCode", "ChoferCode" });

            migrationBuilder.CreateIndex(
                name: "IX_GuiasRemisionVenta_TransportistaCode_VehicleType_VehicleCode",
                schema: "facturacion",
                table: "GuiasRemisionVenta",
                columns: new[] { "TransportistaCode", "VehicleType", "VehicleCode" });

            migrationBuilder.CreateIndex(
                name: "IX_GuiasRemisionVenta_UbigeoCode",
                schema: "facturacion",
                table: "GuiasRemisionVenta",
                column: "UbigeoCode");

            migrationBuilder.CreateIndex(
                name: "IX_GuiasRemisionVenta_VendedorCode",
                schema: "facturacion",
                table: "GuiasRemisionVenta",
                column: "VendedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_GuiasRemisionVentaDetalle_OrdenTrabajoCode_OrdenTrabajoItem",
                schema: "facturacion",
                table: "GuiasRemisionVentaDetalle",
                columns: new[] { "OrdenTrabajoCode", "OrdenTrabajoItem" });

            migrationBuilder.CreateIndex(
                name: "IX_GuiasRemisionVentaDetalle_ProductoTipoCode_ProductoCode",
                schema: "facturacion",
                table: "GuiasRemisionVentaDetalle",
                columns: new[] { "ProductoTipoCode", "ProductoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVenta_ClienteCode_ObraCode",
                schema: "facturacion",
                table: "OrdenesTrabajoVenta",
                columns: new[] { "ClienteCode", "ObraCode" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVenta_CotizacionNegocioCode_CotizacionYear_CotizacionMonth_CotizacionCode",
                schema: "facturacion",
                table: "OrdenesTrabajoVenta",
                columns: new[] { "CotizacionNegocioCode", "CotizacionYear", "CotizacionMonth", "CotizacionCode" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVenta_CreatedAt",
                schema: "facturacion",
                table: "OrdenesTrabajoVenta",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVenta_MotivoCode",
                schema: "facturacion",
                table: "OrdenesTrabajoVenta",
                column: "MotivoCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVenta_OriginCode",
                schema: "facturacion",
                table: "OrdenesTrabajoVenta",
                column: "OriginCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVenta_Stage",
                schema: "facturacion",
                table: "OrdenesTrabajoVenta",
                column: "Stage");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVenta_Status",
                schema: "facturacion",
                table: "OrdenesTrabajoVenta",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVenta_VendedorCode",
                schema: "facturacion",
                table: "OrdenesTrabajoVenta",
                column: "VendedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVentaAprobaciones_BankCode",
                schema: "facturacion",
                table: "OrdenesTrabajoVentaAprobaciones",
                column: "BankCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVentaAprobaciones_DocumentType_Document1Number",
                schema: "facturacion",
                table: "OrdenesTrabajoVentaAprobaciones",
                columns: new[] { "DocumentType", "Document1Number" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVentaAprobaciones_DocumentType_Document2Number",
                schema: "facturacion",
                table: "OrdenesTrabajoVentaAprobaciones",
                columns: new[] { "DocumentType", "Document2Number" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVentaAprobaciones_FormaPagoVentaCode",
                schema: "facturacion",
                table: "OrdenesTrabajoVentaAprobaciones",
                column: "FormaPagoVentaCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVentaAprobaciones_Plant1Code",
                schema: "facturacion",
                table: "OrdenesTrabajoVentaAprobaciones",
                column: "Plant1Code");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVentaAprobaciones_Plant2Code",
                schema: "facturacion",
                table: "OrdenesTrabajoVentaAprobaciones",
                column: "Plant2Code");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVentaAprobacionesAnuladas_OrdenTrabajoCode",
                schema: "facturacion",
                table: "OrdenesTrabajoVentaAprobacionesAnuladas",
                column: "OrdenTrabajoCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVentaDetalle_Item",
                schema: "facturacion",
                table: "OrdenesTrabajoVentaDetalle",
                column: "Item",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVentaDetalle_NegocioCode",
                schema: "facturacion",
                table: "OrdenesTrabajoVentaDetalle",
                column: "NegocioCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajoVentaDetalle_ProductoTipoCode_ProductoCode",
                schema: "facturacion",
                table: "OrdenesTrabajoVentaDetalle",
                columns: new[] { "ProductoTipoCode", "ProductoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesConcreto_ElementoCode",
                schema: "facturacion",
                table: "ProgramacionesConcreto",
                column: "ElementoCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesConcreto_OrdenTrabajoCode",
                schema: "facturacion",
                table: "ProgramacionesConcreto",
                column: "OrdenTrabajoCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesConcreto_PlantCode",
                schema: "facturacion",
                table: "ProgramacionesConcreto",
                column: "PlantCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesConcreto_ProgramDate",
                schema: "facturacion",
                table: "ProgramacionesConcreto",
                column: "ProgramDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesConcreto_Status",
                schema: "facturacion",
                table: "ProgramacionesConcreto",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesConcretoDetalle_OrdenTrabajoCode_Item",
                schema: "facturacion",
                table: "ProgramacionesConcretoDetalle",
                columns: new[] { "OrdenTrabajoCode", "Item" });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesOtros_OrdenTrabajoCode",
                schema: "facturacion",
                table: "ProgramacionesOtros",
                column: "OrdenTrabajoCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesOtros_PlantCode",
                schema: "facturacion",
                table: "ProgramacionesOtros",
                column: "PlantCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesOtros_ProgramDate",
                schema: "facturacion",
                table: "ProgramacionesOtros",
                column: "ProgramDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesOtros_Status",
                schema: "facturacion",
                table: "ProgramacionesOtros",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesOtrosDetalle_OrdenTrabajoCode_Item",
                schema: "facturacion",
                table: "ProgramacionesOtrosDetalle",
                columns: new[] { "OrdenTrabajoCode", "Item" });

            migrationBuilder.AddForeignKey(
                name: "FK_HojasCorte_OrdenesTrabajoVenta_OrdenTrabajoCode",
                schema: "facturacion",
                table: "HojasCorte",
                column: "OrdenTrabajoCode",
                principalSchema: "facturacion",
                principalTable: "OrdenesTrabajoVenta",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HojasCorte_OrdenesTrabajoVenta_OrdenTrabajoCode",
                schema: "facturacion",
                table: "HojasCorte");

            migrationBuilder.DropTable(
                name: "ComprobantesVentaDetalle",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "OrdenesTrabajoVentaAprobaciones",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "OrdenesTrabajoVentaAprobacionesAnuladas",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "ProgramacionesConcretoDetalle",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "ProgramacionesOtrosDetalle",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "ComprobantesVenta",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "GuiasRemisionVentaDetalle",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "ProgramacionesConcreto",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "ProgramacionesOtros",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "GuiasRemisionVenta",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "OrdenesTrabajoVentaDetalle",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "ElementosConcreto",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "OrdenesTrabajoVenta",
                schema: "facturacion");

            migrationBuilder.AlterColumn<string>(
                name: "OrdenTrabajoCode",
                schema: "facturacion",
                table: "HojasCorte",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(8)",
                oldNullable: true);
        }
    }
}
