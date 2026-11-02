using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cepheus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CAMBIOSCONTABILIDAD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SettledAt",
                schema: "facturacion",
                table: "NotasCreditoVenta",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PendingAmount",
                schema: "facturacion",
                table: "ComprobantesVenta",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "SettledAt",
                schema: "facturacion",
                table: "ComprobantesVenta",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChequesMixtosDocumentos",
                schema: "facturacion",
                columns: table => new
                {
                    ChequeCode = table.Column<string>(type: "char(10)", nullable: false),
                    ChequeType = table.Column<string>(type: "char(1)", nullable: false),
                    ChequeNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BancoCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TypeCode = table.Column<string>(type: "char(2)", nullable: false),
                    DocumentNumber = table.Column<string>(type: "char(15)", nullable: false),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    ClienteCode = table.Column<string>(type: "char(5)", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrencyCode = table.Column<string>(type: "char(3)", nullable: false),
                    DocumentTotalSoles = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    DocumentPendingSoles = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    PaidSoles = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    RealPaidSoles = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DocumentExchangeRate = table.Column<decimal>(type: "decimal(7,4)", precision: 7, scale: 4, nullable: false),
                    DifferenceSoles = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    ProvisionEntryNumber = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    IsDetraction = table.Column<bool>(type: "bit", nullable: false),
                    RetentionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RetentionBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChequesMixtosDocumentos", x => new { x.ChequeCode, x.ChequeType, x.ChequeNumber, x.BancoCode, x.TypeCode, x.DocumentNumber });
                });

            migrationBuilder.CreateTable(
                name: "ClientesResponsablesLetra",
                schema: "facturacion",
                columns: table => new
                {
                    ClienteCode = table.Column<string>(type: "char(5)", nullable: false),
                    Sequence = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PaternalSurname = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    MaternalSurname = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    FirstNames = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DocumentTypeCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientesResponsablesLetra", x => new { x.ClienteCode, x.Sequence });
                    table.ForeignKey(
                        name: "FK_ClientesResponsablesLetra_Clientes_ClienteCode",
                        column: x => x.ClienteCode,
                        principalSchema: "facturacion",
                        principalTable: "Clientes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormasPagoCancelacion",
                schema: "contabilidad",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(1)", nullable: false),
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
                    table.PrimaryKey("PK_FormasPagoCancelacion", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "NoAfectosRetencion",
                schema: "contabilidad",
                columns: table => new
                {
                    Ruc = table.Column<string>(type: "char(11)", nullable: false),
                    Type = table.Column<string>(type: "char(1)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    Resolution = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoAfectosRetencion", x => new { x.Ruc, x.Type });
                });

            migrationBuilder.CreateTable(
                name: "TiposPago",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ShowInApproval = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPago", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "PagosProveedor",
                schema: "contabilidad",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(10)", nullable: false),
                    NumeroCheque = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NumeroControl = table.Column<string>(type: "char(1)", nullable: false, defaultValue: "*"),
                    BancoCode = table.Column<string>(type: "char(3)", nullable: false),
                    CuentaBancariaNumero = table.Column<string>(type: "char(20)", nullable: false),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    NumeroComprobanteRetencion = table.Column<string>(type: "char(10)", nullable: true),
                    FechaComprobanteRetencion = table.Column<DateTime>(type: "date", nullable: true),
                    FechaEmision = table.Column<DateTime>(type: "date", nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    PagoSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    RetencionSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    TotalSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    PagoRealSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    PagoSolVenta = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    MontoReferencial = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    AsientoCancelacionCode = table.Column<string>(type: "char(13)", nullable: true),
                    AsientoAnulacionCode = table.Column<string>(type: "char(13)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaAnulacion = table.Column<DateTime>(type: "date", nullable: true),
                    ComprobanteCajaBancosImpreso = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ComprobanteRetencionImpreso = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Afecto = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Detraccion = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Canje = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DepositoElectronico = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Glosa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PagarA = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    CuentaContableCode = table.Column<string>(type: "char(8)", nullable: false),
                    ClienteCode = table.Column<string>(type: "char(5)", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    FormaPagoCancelacionCode = table.Column<string>(type: "char(1)", nullable: true),
                    ProveedorCuentaId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosProveedor", x => x.Code);
                    table.ForeignKey(
                        name: "FK_PagosProveedor_Bancos_BancoCode",
                        column: x => x.BancoCode,
                        principalSchema: "comun",
                        principalTable: "Bancos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagosProveedor_Clientes_ClienteCode",
                        column: x => x.ClienteCode,
                        principalSchema: "facturacion",
                        principalTable: "Clientes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagosProveedor_CuentasBancarias_BancoCode_CuentaBancariaNumero",
                        columns: x => new { x.BancoCode, x.CuentaBancariaNumero },
                        principalSchema: "contabilidad",
                        principalTable: "CuentasBancarias",
                        principalColumns: new[] { "BankCode", "AccountNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagosProveedor_CuentasContables_CuentaContableCode",
                        column: x => x.CuentaContableCode,
                        principalSchema: "contabilidad",
                        principalTable: "CuentasContables",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagosProveedor_FormasPagoCancelacion_FormaPagoCancelacionCode",
                        column: x => x.FormaPagoCancelacionCode,
                        principalSchema: "contabilidad",
                        principalTable: "FormasPagoCancelacion",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagosProveedor_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagosProveedor_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagosProveedor_ProveedorCuentas_ProveedorCuentaId",
                        column: x => x.ProveedorCuentaId,
                        principalSchema: "logistica",
                        principalTable: "ProveedorCuentas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagosProveedor_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CancelacionesVenta",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(10)", nullable: false),
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    ClienteCode = table.Column<string>(type: "char(5)", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    CobradorCode = table.Column<string>(type: "char(4)", nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TipoPagoCode = table.Column<string>(type: "char(2)", nullable: false),
                    PaymentNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    BancoDocCode = table.Column<string>(type: "char(3)", nullable: true),
                    BancoDepCode = table.Column<string>(type: "char(3)", nullable: true),
                    IsDeferred = table.Column<bool>(type: "bit", nullable: false),
                    DeferredDate = table.Column<DateTime>(type: "date", nullable: true),
                    OperationNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    OperationDate = table.Column<DateTime>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UsedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DifferenceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AvailableAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProvisionEntryNumber = table.Column<string>(type: "char(13)", nullable: true),
                    ReceptionDate = table.Column<DateTime>(type: "date", nullable: true),
                    InstallmentCount = table.Column<int>(type: "int", nullable: false),
                    InstallmentType = table.Column<string>(type: "char(1)", nullable: true),
                    FirstPaymentDate = table.Column<DateTime>(type: "date", nullable: true),
                    InitialAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Interest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CancellationEntryNumber = table.Column<string>(type: "char(13)", nullable: true),
                    CanceledBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CanceledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancelacionesVenta", x => x.Code);
                    table.ForeignKey(
                        name: "FK_CancelacionesVenta_Bancos_BancoDepCode",
                        column: x => x.BancoDepCode,
                        principalSchema: "comun",
                        principalTable: "Bancos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancelacionesVenta_Bancos_BancoDocCode",
                        column: x => x.BancoDocCode,
                        principalSchema: "comun",
                        principalTable: "Bancos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancelacionesVenta_Clientes_ClienteCode",
                        column: x => x.ClienteCode,
                        principalSchema: "facturacion",
                        principalTable: "Clientes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancelacionesVenta_Cobradores_CobradorCode",
                        column: x => x.CobradorCode,
                        principalSchema: "facturacion",
                        principalTable: "Cobradores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancelacionesVenta_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancelacionesVenta_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancelacionesVenta_TiposPago_TipoPagoCode",
                        column: x => x.TipoPagoCode,
                        principalSchema: "facturacion",
                        principalTable: "TiposPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TiposPagoCuentaContable",
                schema: "facturacion",
                columns: table => new
                {
                    TipoPagoCode = table.Column<string>(type: "char(2)", nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", nullable: false),
                    AccountCode = table.Column<string>(type: "char(8)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPagoCuentaContable", x => new { x.TipoPagoCode, x.MonedaCode });
                    table.ForeignKey(
                        name: "FK_TiposPagoCuentaContable_CuentasContables_AccountCode",
                        column: x => x.AccountCode,
                        principalSchema: "contabilidad",
                        principalTable: "CuentasContables",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TiposPagoCuentaContable_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TiposPagoCuentaContable_TiposPago_TipoPagoCode",
                        column: x => x.TipoPagoCode,
                        principalSchema: "facturacion",
                        principalTable: "TiposPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PagoProveedorDetalles",
                schema: "contabilidad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PagoCode = table.Column<string>(type: "char(10)", nullable: false),
                    ComprobantePagoCode = table.Column<string>(type: "char(2)", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "char(15)", nullable: false),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    TotalSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    PendienteSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    PagoSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    AsientoProvisionCode = table.Column<string>(type: "char(13)", nullable: true),
                    Detraccion = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    BaseRetencionSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    MontoRetenidoSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagoProveedorDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagoProveedorDetalles_DocumentosCompra_ComprobantePagoCode_NumeroDocumento_ProveedorCode",
                        columns: x => new { x.ComprobantePagoCode, x.NumeroDocumento, x.ProveedorCode },
                        principalSchema: "contabilidad",
                        principalTable: "DocumentosCompra",
                        principalColumns: new[] { "ComprobantePagoCode", "NumeroDocumento", "ProveedorCode" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagoProveedorDetalles_PagosProveedor_PagoCode",
                        column: x => x.PagoCode,
                        principalSchema: "contabilidad",
                        principalTable: "PagosProveedor",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CancelacionesVentaDetalle",
                schema: "facturacion",
                columns: table => new
                {
                    CancelacionCode = table.Column<string>(type: "char(10)", nullable: false),
                    Item = table.Column<string>(type: "char(3)", nullable: false),
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    ClienteCode = table.Column<string>(type: "char(5)", nullable: false),
                    CancelacionDate = table.Column<DateTime>(type: "date", nullable: false),
                    TypeCode = table.Column<string>(type: "char(2)", nullable: false),
                    DocumentNumber = table.Column<string>(type: "char(15)", nullable: false),
                    DocumentBancoCode = table.Column<string>(type: "char(3)", nullable: true),
                    DocumentDate = table.Column<DateTime>(type: "date", nullable: false),
                    DocumentCurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    DocumentExchangeRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidSoles = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DifferenceSoles = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProvisionEntryNumber = table.Column<string>(type: "char(13)", nullable: true),
                    ObraCode = table.Column<string>(type: "char(3)", nullable: true),
                    IsDoubtful = table.Column<bool>(type: "bit", nullable: false),
                    DoubtfulExchangeRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    IsDetraction = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancelacionesVentaDetalle", x => new { x.CancelacionCode, x.Item });
                    table.ForeignKey(
                        name: "FK_CancelacionesVentaDetalle_CancelacionesVenta_CancelacionCode",
                        column: x => x.CancelacionCode,
                        principalSchema: "facturacion",
                        principalTable: "CancelacionesVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LetrasVenta",
                schema: "facturacion",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Number = table.Column<string>(type: "char(9)", nullable: false),
                    ClienteCode = table.Column<string>(type: "char(5)", nullable: false),
                    BancoCode = table.Column<string>(type: "char(3)", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "date", nullable: false),
                    CobradorCode = table.Column<string>(type: "char(4)", nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PendingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Interest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProvisionEntryNumber = table.Column<string>(type: "char(13)", nullable: true),
                    CancellationEntryNumber = table.Column<string>(type: "char(13)", nullable: true),
                    CanceledAt = table.Column<DateTime>(type: "date", nullable: true),
                    SettledAt = table.Column<DateTime>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CancelacionCode = table.Column<string>(type: "char(10)", nullable: false),
                    IsPrinted = table.Column<bool>(type: "bit", nullable: false),
                    AcceptedByClient = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LetrasVenta", x => new { x.PlantaCode, x.Number });
                    table.ForeignKey(
                        name: "FK_LetrasVenta_Bancos_BancoCode",
                        column: x => x.BancoCode,
                        principalSchema: "comun",
                        principalTable: "Bancos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LetrasVenta_CancelacionesVenta_CancelacionCode",
                        column: x => x.CancelacionCode,
                        principalSchema: "facturacion",
                        principalTable: "CancelacionesVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LetrasVenta_Clientes_ClienteCode",
                        column: x => x.ClienteCode,
                        principalSchema: "facturacion",
                        principalTable: "Clientes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LetrasVenta_Cobradores_CobradorCode",
                        column: x => x.CobradorCode,
                        principalSchema: "facturacion",
                        principalTable: "Cobradores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LetrasVenta_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LetrasVenta_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CancelacionesVenta_BancoDepCode",
                schema: "facturacion",
                table: "CancelacionesVenta",
                column: "BancoDepCode");

            migrationBuilder.CreateIndex(
                name: "IX_CancelacionesVenta_BancoDocCode",
                schema: "facturacion",
                table: "CancelacionesVenta",
                column: "BancoDocCode");

            migrationBuilder.CreateIndex(
                name: "IX_CancelacionesVenta_ClienteCode",
                schema: "facturacion",
                table: "CancelacionesVenta",
                column: "ClienteCode");

            migrationBuilder.CreateIndex(
                name: "IX_CancelacionesVenta_CobradorCode",
                schema: "facturacion",
                table: "CancelacionesVenta",
                column: "CobradorCode");

            migrationBuilder.CreateIndex(
                name: "IX_CancelacionesVenta_Date",
                schema: "facturacion",
                table: "CancelacionesVenta",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_CancelacionesVenta_MonedaCode",
                schema: "facturacion",
                table: "CancelacionesVenta",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_CancelacionesVenta_PlantaCode",
                schema: "facturacion",
                table: "CancelacionesVenta",
                column: "PlantaCode");

            migrationBuilder.CreateIndex(
                name: "IX_CancelacionesVenta_Status",
                schema: "facturacion",
                table: "CancelacionesVenta",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CancelacionesVenta_TipoPagoCode",
                schema: "facturacion",
                table: "CancelacionesVenta",
                column: "TipoPagoCode");

            migrationBuilder.CreateIndex(
                name: "IX_CancelacionesVentaDetalle_ClienteCode",
                schema: "facturacion",
                table: "CancelacionesVentaDetalle",
                column: "ClienteCode");

            migrationBuilder.CreateIndex(
                name: "IX_CancelacionesVentaDetalle_PlantaCode_ClienteCode_CancelacionDate",
                schema: "facturacion",
                table: "CancelacionesVentaDetalle",
                columns: new[] { "PlantaCode", "ClienteCode", "CancelacionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CancelacionesVentaDetalle_TypeCode_DocumentNumber",
                schema: "facturacion",
                table: "CancelacionesVentaDetalle",
                columns: new[] { "TypeCode", "DocumentNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_ChequesMixtosDocumentos_ClienteCode_TypeCode_DocumentNumber_ChequeType",
                schema: "facturacion",
                table: "ChequesMixtosDocumentos",
                columns: new[] { "ClienteCode", "TypeCode", "DocumentNumber", "ChequeType" });

            migrationBuilder.CreateIndex(
                name: "IX_LetrasVenta_BancoCode",
                schema: "facturacion",
                table: "LetrasVenta",
                column: "BancoCode");

            migrationBuilder.CreateIndex(
                name: "IX_LetrasVenta_CancelacionCode",
                schema: "facturacion",
                table: "LetrasVenta",
                column: "CancelacionCode");

            migrationBuilder.CreateIndex(
                name: "IX_LetrasVenta_ClienteCode",
                schema: "facturacion",
                table: "LetrasVenta",
                column: "ClienteCode");

            migrationBuilder.CreateIndex(
                name: "IX_LetrasVenta_CobradorCode",
                schema: "facturacion",
                table: "LetrasVenta",
                column: "CobradorCode");

            migrationBuilder.CreateIndex(
                name: "IX_LetrasVenta_DueDate",
                schema: "facturacion",
                table: "LetrasVenta",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_LetrasVenta_MonedaCode",
                schema: "facturacion",
                table: "LetrasVenta",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_LetrasVenta_Status",
                schema: "facturacion",
                table: "LetrasVenta",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedorDetalles_ComprobantePagoCode_NumeroDocumento_ProveedorCode",
                schema: "contabilidad",
                table: "PagoProveedorDetalles",
                columns: new[] { "ComprobantePagoCode", "NumeroDocumento", "ProveedorCode" });

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedorDetalles_PagoCode_ComprobantePagoCode_NumeroDocumento_ProveedorCode",
                schema: "contabilidad",
                table: "PagoProveedorDetalles",
                columns: new[] { "PagoCode", "ComprobantePagoCode", "NumeroDocumento", "ProveedorCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedor_BancoCode_CuentaBancariaNumero",
                schema: "contabilidad",
                table: "PagosProveedor",
                columns: new[] { "BancoCode", "CuentaBancariaNumero" });

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedor_BancoCode_NumeroCheque",
                schema: "contabilidad",
                table: "PagosProveedor",
                columns: new[] { "BancoCode", "NumeroCheque" });

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedor_ClienteCode",
                schema: "contabilidad",
                table: "PagosProveedor",
                column: "ClienteCode");

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedor_CuentaContableCode",
                schema: "contabilidad",
                table: "PagosProveedor",
                column: "CuentaContableCode");

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedor_Estado",
                schema: "contabilidad",
                table: "PagosProveedor",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedor_FechaEmision",
                schema: "contabilidad",
                table: "PagosProveedor",
                column: "FechaEmision");

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedor_FormaPagoCancelacionCode",
                schema: "contabilidad",
                table: "PagosProveedor",
                column: "FormaPagoCancelacionCode");

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedor_MonedaCode",
                schema: "contabilidad",
                table: "PagosProveedor",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedor_PlantaCode",
                schema: "contabilidad",
                table: "PagosProveedor",
                column: "PlantaCode");

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedor_ProveedorCode",
                schema: "contabilidad",
                table: "PagosProveedor",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedor_ProveedorCuentaId",
                schema: "contabilidad",
                table: "PagosProveedor",
                column: "ProveedorCuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposPago_Name",
                schema: "facturacion",
                table: "TiposPago",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposPagoCuentaContable_AccountCode",
                schema: "facturacion",
                table: "TiposPagoCuentaContable",
                column: "AccountCode");

            migrationBuilder.CreateIndex(
                name: "IX_TiposPagoCuentaContable_MonedaCode",
                schema: "facturacion",
                table: "TiposPagoCuentaContable",
                column: "MonedaCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CancelacionesVentaDetalle",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "ChequesMixtosDocumentos",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "ClientesResponsablesLetra",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "LetrasVenta",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "NoAfectosRetencion",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "PagoProveedorDetalles",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "TiposPagoCuentaContable",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "CancelacionesVenta",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "PagosProveedor",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "TiposPago",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "FormasPagoCancelacion",
                schema: "contabilidad");

            migrationBuilder.DropColumn(
                name: "SettledAt",
                schema: "facturacion",
                table: "NotasCreditoVenta");

            migrationBuilder.DropColumn(
                name: "PendingAmount",
                schema: "facturacion",
                table: "ComprobantesVenta");

            migrationBuilder.DropColumn(
                name: "SettledAt",
                schema: "facturacion",
                table: "ComprobantesVenta");
        }
    }
}
