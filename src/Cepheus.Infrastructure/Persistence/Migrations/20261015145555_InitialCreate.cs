using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cepheus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "mantenimiento");

            migrationBuilder.EnsureSchema(
                name: "rrhh");

            migrationBuilder.EnsureSchema(
                name: "logistica");

            migrationBuilder.EnsureSchema(
                name: "facturacion");

            migrationBuilder.EnsureSchema(
                name: "contabilidad");

            migrationBuilder.EnsureSchema(
                name: "comun");

            migrationBuilder.EnsureSchema(
                name: "admin");

            migrationBuilder.CreateTable(
                name: "Afps",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Afps", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Alergias",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alergias", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "AtributosConcreto",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(4)", nullable: false),
                    AttributeType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtributosConcreto", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Bancos",
                schema: "comun",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bancos", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Cargos",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargos", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "CategoriasProducto",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirthCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasProducto", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "CategoriasTrabajador",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasTrabajador", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "CentrosEjecutores",
                schema: "mantenimiento",
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
                    table.PrimaryKey("PK_CentrosEjecutores", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "CierreBloqueos",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    UltimoUso = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CierreBloqueos", x => x.PlantaCode);
                });

            migrationBuilder.CreateTable(
                name: "CierreRevaluaciones",
                schema: "logistica",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    PeriodCode = table.Column<string>(type: "char(6)", nullable: false),
                    Origen = table.Column<int>(type: "int", nullable: false),
                    DocumentoCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: true),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    PrecioAnterior = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    PrecioNuevo = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    TotalAnterior = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalNuevo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CierreRevaluaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClasificacionesCliente",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(1)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClasificacionesCliente", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Compradores",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
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
                    table.PrimaryKey("PK_Compradores", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ComprobantesPago",
                schema: "comun",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", fixedLength: true, nullable: false),
                    SunatCode = table.Column<string>(type: "char(2)", fixedLength: true, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RequiresRuc = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RequiresAddress = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AffectsIgv = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsNonTaxable = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AffectsIncomeTax = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AffectsFonavi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsService = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AffectsForeignIgv = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AvailableForPurchaseOrder = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprobantesPago", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ControlesVentas",
                schema: "comun",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IgvPercentage = table.Column<decimal>(type: "numeric(24,2)", nullable: false),
                    WithholdingPercentage = table.Column<decimal>(type: "numeric(24,2)", nullable: false),
                    ClosingPeriod = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    SalesProcessDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchasesProcessDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SalesCancelDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchasesCancelDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WithholdingCap = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    DetractionPercentage = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    IncomeTaxPercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    FonaviPercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    ForeignIgvPercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    QuotaPercentage = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
                    BlocksGrouping = table.Column<bool>(type: "bit", nullable: false),
                    WithholdingCapInvoice = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    WorkOrderDaysLimit = table.Column<int>(type: "int", nullable: false),
                    WorkOrderMaxDays = table.Column<int>(type: "int", nullable: false),
                    SaleMaxDays = table.Column<int>(type: "int", nullable: false),
                    BalanceLimit = table.Column<int>(type: "int", nullable: true),
                    AdditionalActivationDays = table.Column<int>(type: "int", nullable: true),
                    IsServiceIndicator = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlesVentas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Eps",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eps", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Especialidades",
                schema: "mantenimiento",
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
                    table.PrimaryKey("PK_Especialidades", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Especialidades",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especialidades", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "EstadosCiviles",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
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
                    table.PrimaryKey("PK_EstadosCiviles", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "EstadosTrabajador",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosTrabajador", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Familias",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
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
                    table.PrimaryKey("PK_Familias", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Fletes",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fletes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "FormasPago",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Days = table.Column<int>(type: "int", nullable: false),
                    IsCredit = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormasPago", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "FormasPagoVenta",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Days = table.Column<int>(type: "int", nullable: false),
                    IsCredit = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormasPagoVenta", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "GradosInstruccion",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradosInstruccion", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Horarios",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horarios", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Inspecciones",
                schema: "mantenimiento",
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
                    table.PrimaryKey("PK_Inspecciones", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "LugaresEnvio",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LugaresEnvio", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Maquinas",
                schema: "mantenimiento",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(4)", nullable: false),
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
                    table.PrimaryKey("PK_Maquinas", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ModalidadesFormativas",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModalidadesFormativas", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ModosPago",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModosPago", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Modulos",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tooltip = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monedas",
                schema: "comun",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    NumericCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    DecimalPlaces = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monedas", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "MotivosDevolucion",
                schema: "comun",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    AffectsStock = table.Column<bool>(type: "bit", nullable: false),
                    EsVenta = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivosDevolucion", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "MotivosDevolucionArticulo",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    AffectsStock = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivosDevolucionArticulo", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Nacionalidades",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nacionalidades", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Negocios",
                schema: "comun",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Negocios", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Niveles",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Niveles", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "NivelesEducativos",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NivelesEducativos", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "NivelesTrabajador",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NivelesTrabajador", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "NotasCompra",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasCompra", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ObjetosActividad",
                schema: "mantenimiento",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
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
                    table.PrimaryKey("PK_ObjetosActividad", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Ocupaciones",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ocupaciones", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Oficinas",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oficinas", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Oportunidades",
                schema: "mantenimiento",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(4)", nullable: false),
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
                    table.PrimaryKey("PK_Oportunidades", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Parentescos",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parentescos", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "PeriodosIgv",
                schema: "comun",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFin = table.Column<DateOnly>(type: "date", nullable: true),
                    PorcentajeIgv = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodosIgv", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanesArticulo",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
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
                    table.PrimaryKey("PK_PlanesArticulo", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Plantas",
                schema: "comun",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AddressComplement = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UbigeoCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    ManagerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HasWarehouse = table.Column<bool>(type: "bit", nullable: false),
                    IsProductionPlant = table.Column<bool>(type: "bit", nullable: false),
                    IsProject = table.Column<bool>(type: "bit", nullable: false),
                    RequiresApprovals = table.Column<bool>(type: "bit", nullable: false),
                    AppliesDetraction = table.Column<bool>(type: "bit", nullable: false),
                    StatusCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    NumeroGve = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "001-000000"),
                    NumeroFve = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "001-000000"),
                    NumeroDve = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "001-000000"),
                    NumeroBve = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "001-000000"),
                    NumeroCve = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "001-000000"),
                    NumeroLet = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "001-000000"),
                    NumeroRet = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "001-000000"),
                    GuiaNum = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "001-000000"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plantas", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Prioridades",
                schema: "mantenimiento",
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
                    table.PrimaryKey("PK_Prioridades", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "RegimenesLaborales",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegimenesLaborales", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "RegimenesPensionarios",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegimenesPensionarios", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SctrPension",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SctrPension", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "SctrSalud",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SctrSalud", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "SegmentosVentas",
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
                    table.PrimaryKey("PK_SegmentosVentas", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Sexos",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
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
                    table.PrimaryKey("PK_Sexos", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "SituacionesEps",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SituacionesEps", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "SubOcupaciones",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubOcupaciones", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposAfiliacion",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAfiliacion", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposAnexo",
                schema: "contabilidad",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Flag = table.Column<string>(type: "char(1)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAnexo", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposArticulo",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
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
                    table.PrimaryKey("PK_TiposArticulo", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposAsiento",
                schema: "contabilidad",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExchangeRateType = table.Column<int>(type: "int", nullable: false),
                    Valuation = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAsiento", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposBien",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DetractionRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposBien", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposCambio",
                schema: "comun",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    SellRate = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    BuyRate = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposCambio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposCentroFormacion",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposCentroFormacion", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposCliente",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(1)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposCliente", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposCompra",
                schema: "logistica",
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
                    table.PrimaryKey("PK_TiposCompra", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposContrato",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposContrato", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposCuenta",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposCuenta", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposCuentaContable",
                schema: "contabilidad",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposCuentaContable", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposDocumento",
                schema: "comun",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    SunatCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDocumento", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposExtensionContrato",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposExtensionContrato", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposHojaCorte",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposHojaCorte", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposOperacion",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposOperacion", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposOrden",
                schema: "mantenimiento",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
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
                    table.PrimaryKey("PK_TiposOrden", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposPedido",
                schema: "logistica",
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
                    table.PrimaryKey("PK_TiposPedido", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposPension",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPension", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposProducto",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposProducto", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposSangre",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
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
                    table.PrimaryKey("PK_TiposSangre", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposSctr",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposSctr", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposTrabajador",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposTrabajador", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposTransaccion",
                schema: "logistica",
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
                    table.PrimaryKey("PK_TiposTransaccion", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposVale",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
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
                    table.PrimaryKey("PK_TiposVale", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposValorizacion",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(1)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Days = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposValorizacion", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposVia",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposVia", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TiposZona",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposZona", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Titulos",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Titulos", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Tramites",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(1)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tramites", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Ubigeos",
                schema: "comun",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Province = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    District = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ubigeos", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesMedida",
                schema: "logistica",
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
                    table.PrimaryKey("PK_UnidadesMedida", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesMedidaVenta",
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
                    table.PrimaryKey("PK_UnidadesMedidaVenta", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesNegocio",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ParentCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesNegocio", x => x.Code);
                    table.ForeignKey(
                        name: "FK_UnidadesNegocio_UnidadesNegocio_ParentCode",
                        column: x => x.ParentCode,
                        principalSchema: "logistica",
                        principalTable: "UnidadesNegocio",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VerbosActividad",
                schema: "mantenimiento",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
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
                    table.PrimaryKey("PK_VerbosActividad", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "SubCentrosEjecutores",
                schema: "mantenimiento",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(4)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CentroEjecutorCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCentrosEjecutores", x => x.Code);
                    table.ForeignKey(
                        name: "FK_SubCentrosEjecutores_CentrosEjecutores_CentroEjecutorCode",
                        column: x => x.CentroEjecutorCode,
                        principalSchema: "mantenimiento",
                        principalTable: "CentrosEjecutores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubFamilias",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(4)", nullable: false),
                    FamiliaCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
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
                    table.PrimaryKey("PK_SubFamilias", x => x.Code);
                    table.ForeignKey(
                        name: "FK_SubFamilias_Familias_FamiliaCode",
                        column: x => x.FamiliaCode,
                        principalSchema: "logistica",
                        principalTable: "Familias",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Submodulos",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuloId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tooltip = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submodulos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submodulos_Modulos_ModuloId",
                        column: x => x.ModuloId,
                        principalSchema: "admin",
                        principalTable: "Modulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotasCotizacionPlantilla",
                schema: "facturacion",
                columns: table => new
                {
                    NegocioCode = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Option = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasCotizacionPlantilla", x => new { x.NegocioCode, x.Code });
                    table.ForeignKey(
                        name: "FK_NotasCotizacionPlantilla_Negocios_NegocioCode",
                        column: x => x.NegocioCode,
                        principalSchema: "comun",
                        principalTable: "Negocios",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AjustesInventario",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Neto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Igv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    AsientoContable = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AjustesInventario", x => new { x.PlantaCode, x.Code });
                    table.ForeignKey(
                        name: "FK_AjustesInventario_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CentrosCosto",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlantaCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentrosCosto", x => x.Code);
                    table.ForeignKey(
                        name: "FK_CentrosCosto_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ControlCierres",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    PeriodCode = table.Column<string>(type: "char(6)", nullable: false),
                    ClosureDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DifferenceAmount = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlCierres", x => new { x.PlantaCode, x.PeriodCode });
                    table.ForeignKey(
                        name: "FK_ControlCierres_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cotizaciones",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    FechaLimite = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, defaultValue: ""),
                    OriginalCode = table.Column<string>(type: "char(6)", nullable: true),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizaciones", x => new { x.PlantaCode, x.Code });
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Cotizaciones_PlantaCode_OriginalCode",
                        columns: x => new { x.PlantaCode, x.OriginalCode },
                        principalSchema: "logistica",
                        principalTable: "Cotizaciones",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Importaciones",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    PesoNeto = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    PesoBruto = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    FechaPoliza = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TipoCambio = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    TotalFob = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    TotalFlete = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    TotalSeguro = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    TotalAduana = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    Advalorem = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    Sobretasa = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    Igv = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    OtrosGastos = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Importaciones", x => new { x.PlantaCode, x.Code });
                    table.ForeignKey(
                        name: "FK_Importaciones_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnalisisVentas",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    SegmentoVentasCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalisisVentas", x => x.Code);
                    table.ForeignKey(
                        name: "FK_AnalisisVentas_SegmentosVentas_SegmentoVentasCode",
                        column: x => x.SegmentoVentasCode,
                        principalSchema: "facturacion",
                        principalTable: "SegmentosVentas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Anexos",
                schema: "contabilidad",
                columns: table => new
                {
                    TipoAnexoCode = table.Column<string>(type: "char(3)", nullable: false),
                    Code = table.Column<string>(type: "char(5)", nullable: false),
                    Name = table.Column<string>(type: "varchar(150)", nullable: true),
                    LinkAccount1Code = table.Column<string>(type: "char(8)", nullable: false),
                    LinkAccount2Code = table.Column<string>(type: "char(8)", nullable: false),
                    Flag = table.Column<string>(type: "char(1)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anexos", x => new { x.TipoAnexoCode, x.Code });
                    table.ForeignKey(
                        name: "FK_Anexos_TiposAnexo_TipoAnexoCode",
                        column: x => x.TipoAnexoCode,
                        principalSchema: "contabilidad",
                        principalTable: "TiposAnexo",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CuentasContables",
                schema: "contabilidad",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(8)", nullable: false),
                    Name = table.Column<string>(type: "varchar(70)", nullable: false),
                    TipoCuentaCode = table.Column<string>(type: "char(2)", nullable: false),
                    TipoAnexoCode = table.Column<string>(type: "char(3)", nullable: true),
                    LinkAccount1Code = table.Column<string>(type: "char(8)", nullable: false),
                    LinkAccount2Code = table.Column<string>(type: "char(8)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "char(3)", nullable: false),
                    AcmFlag = table.Column<string>(type: "char(1)", nullable: true),
                    PilotFlag = table.Column<string>(type: "char(1)", nullable: true),
                    GainsLossesFlag = table.Column<string>(type: "char(1)", nullable: true),
                    ClosingAccountCode = table.Column<string>(type: "char(8)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasContables", x => x.Code);
                    table.ForeignKey(
                        name: "FK_CuentasContables_Monedas_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasContables_TiposAnexo_TipoAnexoCode",
                        column: x => x.TipoAnexoCode,
                        principalSchema: "contabilidad",
                        principalTable: "TiposAnexo",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasContables_TiposCuentaContable_TipoCuentaCode",
                        column: x => x.TipoCuentaCode,
                        principalSchema: "contabilidad",
                        principalTable: "TiposCuentaContable",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Conductores",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(5)", nullable: false),
                    DocumentTypeCode = table.Column<string>(type: "char(2)", maxLength: 3, nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DriverLicenseNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LicenseCategory = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conductores", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Conductores_TiposDocumento_DocumentTypeCode",
                        column: x => x.DocumentTypeCode,
                        principalSchema: "comun",
                        principalTable: "TiposDocumento",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(5)", nullable: false),
                    DocumentTypeCode = table.Column<string>(type: "char(2)", maxLength: 3, nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TradeName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ProviderType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SunatCondition = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SunatStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DeactivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeactivatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Proveedores_TiposDocumento_DocumentTypeCode",
                        column: x => x.DocumentTypeCode,
                        principalSchema: "comun",
                        principalTable: "TiposDocumento",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(5)", nullable: false),
                    PersonType = table.Column<string>(type: "char(1)", nullable: false),
                    DocumentTypeCode = table.Column<string>(type: "char(2)", maxLength: 3, nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UbigeoCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ParentClientCode = table.Column<string>(type: "char(5)", maxLength: 5, nullable: true),
                    TipoClienteCode = table.Column<string>(type: "char(1)", maxLength: 1, nullable: false),
                    ClasificacionClienteCode = table.Column<string>(type: "char(1)", maxLength: 1, nullable: false),
                    LegalRepresentativeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LegalRepresentativePhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LegalRepresentativeDni = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsVip = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RequiresCashOnly = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HasGlobalCreditLine = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    GlobalCreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RequiresPurchaseOrderApproval = table.Column<bool>(type: "bit", nullable: true),
                    RequiresWorkOrderApproval = table.Column<bool>(type: "bit", nullable: true),
                    FormaPagoVentaCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: true),
                    RequiresManagementApproval = table.Column<bool>(type: "bit", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Clientes_ClasificacionesCliente_ClasificacionClienteCode",
                        column: x => x.ClasificacionClienteCode,
                        principalSchema: "facturacion",
                        principalTable: "ClasificacionesCliente",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_Clientes_ParentClientCode",
                        column: x => x.ParentClientCode,
                        principalSchema: "facturacion",
                        principalTable: "Clientes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_FormasPagoVenta_FormaPagoVentaCode",
                        column: x => x.FormaPagoVentaCode,
                        principalSchema: "facturacion",
                        principalTable: "FormasPagoVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_Monedas_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_TiposCliente_TipoClienteCode",
                        column: x => x.TipoClienteCode,
                        principalSchema: "facturacion",
                        principalTable: "TiposCliente",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_TiposDocumento_DocumentTypeCode",
                        column: x => x.DocumentTypeCode,
                        principalSchema: "comun",
                        principalTable: "TiposDocumento",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_Ubigeos_UbigeoCode",
                        column: x => x.UbigeoCode,
                        principalSchema: "comun",
                        principalTable: "Ubigeos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Trabajadores",
                schema: "rrhh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(5)", nullable: false),
                    FirstNames = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    PaternalSurname = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    MaternalSurname = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    SexoCode = table.Column<string>(type: "char(3)", maxLength: 10, nullable: true),
                    EstadoCivilCode = table.Column<string>(type: "char(3)", maxLength: 10, nullable: true),
                    NacionalidadCode = table.Column<string>(type: "char(3)", maxLength: 10, nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    BirthUbigeoCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    MobilePhone = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    HasDisability = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trabajadores", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Trabajadores_EstadosCiviles_EstadoCivilCode",
                        column: x => x.EstadoCivilCode,
                        principalSchema: "rrhh",
                        principalTable: "EstadosCiviles",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trabajadores_Nacionalidades_NacionalidadCode",
                        column: x => x.NacionalidadCode,
                        principalSchema: "rrhh",
                        principalTable: "Nacionalidades",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trabajadores_Sexos_SexoCode",
                        column: x => x.SexoCode,
                        principalSchema: "rrhh",
                        principalTable: "Sexos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trabajadores_Ubigeos_BirthUbigeoCode",
                        column: x => x.BirthUbigeoCode,
                        principalSchema: "comun",
                        principalTable: "Ubigeos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transportistas",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(5)", nullable: false),
                    DocumentTypeCode = table.Column<string>(type: "char(2)", maxLength: 3, nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TradeName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UbigeoCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    MtcRegistrationNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CertificationNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    IsOwnFleet = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transportistas", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Transportistas_TiposDocumento_DocumentTypeCode",
                        column: x => x.DocumentTypeCode,
                        principalSchema: "comun",
                        principalTable: "TiposDocumento",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transportistas_Ubigeos_UbigeoCode",
                        column: x => x.UbigeoCode,
                        principalSchema: "comun",
                        principalTable: "Ubigeos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransportistasVentas",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(4)", nullable: false),
                    DocumentTypeCode = table.Column<string>(type: "char(2)", maxLength: 3, nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UbigeoCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    MtcInternalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportistasVentas", x => x.Code);
                    table.ForeignKey(
                        name: "FK_TransportistasVentas_TiposDocumento_DocumentTypeCode",
                        column: x => x.DocumentTypeCode,
                        principalSchema: "comun",
                        principalTable: "TiposDocumento",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransportistasVentas_Ubigeos_UbigeoCode",
                        column: x => x.UbigeoCode,
                        principalSchema: "comun",
                        principalTable: "Ubigeos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RangosAprobacion",
                schema: "logistica",
                columns: table => new
                {
                    NivelCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    TipoTransaccionCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    UnidadNegocioCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    ImporteMinimo = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    ImporteMaximo = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    ImporteAcumuladoDiario = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    ImporteAcumuladoMensual = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    PorcentajeTotal = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RangosAprobacion", x => new { x.NivelCode, x.TipoTransaccionCode, x.UnidadNegocioCode, x.MonedaCode });
                    table.ForeignKey(
                        name: "FK_RangosAprobacion_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RangosAprobacion_Niveles_NivelCode",
                        column: x => x.NivelCode,
                        principalSchema: "logistica",
                        principalTable: "Niveles",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RangosAprobacion_TiposTransaccion_TipoTransaccionCode",
                        column: x => x.TipoTransaccionCode,
                        principalSchema: "logistica",
                        principalTable: "TiposTransaccion",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RangosAprobacion_UnidadesNegocio_UnidadNegocioCode",
                        column: x => x.UnidadNegocioCode,
                        principalSchema: "logistica",
                        principalTable: "UnidadesNegocio",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Actividades",
                schema: "mantenimiento",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    VerboActividadCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    ObjetoActividadCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actividades", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Actividades_ObjetosActividad_ObjetoActividadCode",
                        column: x => x.ObjetoActividadCode,
                        principalSchema: "mantenimiento",
                        principalTable: "ObjetosActividad",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Actividades_VerbosActividad_VerboActividadCode",
                        column: x => x.VerboActividadCode,
                        principalSchema: "mantenimiento",
                        principalTable: "VerbosActividad",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Programas",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmoduloId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tooltip = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Route = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Programas_Submodulos_SubmoduloId",
                        column: x => x.SubmoduloId,
                        principalSchema: "admin",
                        principalTable: "Submodulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Articulos",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(7)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    UnidadMedidaCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    SubFamiliaCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: false),
                    MinStock = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    MaxStock = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    IncomingStock = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    LeadTimeDays = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    AbcClass = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    TipoArticuloCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    PlanCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    ManufacturerCode = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Observations = table.Column<string>(type: "varchar(max)", nullable: false, defaultValue: ""),
                    SalesTypeCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    SalesProductCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    IsAgreement = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RequiresHorometro = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AccountingAccountCode = table.Column<string>(type: "char(8)", maxLength: 8, nullable: true),
                    AccountingAttachmentTypeCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: true),
                    PlantOriginCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articulos", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Articulos_CuentasContables_AccountingAccountCode",
                        column: x => x.AccountingAccountCode,
                        principalSchema: "contabilidad",
                        principalTable: "CuentasContables",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articulos_PlanesArticulo_PlanCode",
                        column: x => x.PlanCode,
                        principalSchema: "logistica",
                        principalTable: "PlanesArticulo",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articulos_Plantas_PlantOriginCode",
                        column: x => x.PlantOriginCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articulos_SubFamilias_SubFamiliaCode",
                        column: x => x.SubFamiliaCode,
                        principalSchema: "logistica",
                        principalTable: "SubFamilias",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articulos_TiposAnexo_AccountingAttachmentTypeCode",
                        column: x => x.AccountingAttachmentTypeCode,
                        principalSchema: "contabilidad",
                        principalTable: "TiposAnexo",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articulos_TiposArticulo_TipoArticuloCode",
                        column: x => x.TipoArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "TiposArticulo",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articulos_UnidadesMedida_UnidadMedidaCode",
                        column: x => x.UnidadMedidaCode,
                        principalSchema: "logistica",
                        principalTable: "UnidadesMedida",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CuentasBancarias",
                schema: "contabilidad",
                columns: table => new
                {
                    BankCode = table.Column<string>(type: "char(3)", nullable: false),
                    AccountNumber = table.Column<string>(type: "char(20)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "char(3)", nullable: false),
                    CheckStart = table.Column<string>(type: "char(10)", nullable: false),
                    CheckEnd = table.Column<string>(type: "char(10)", nullable: false),
                    CheckCurrent = table.Column<string>(type: "char(10)", nullable: false),
                    ControlFlag = table.Column<string>(type: "char(1)", nullable: false),
                    AccountCode = table.Column<string>(type: "char(8)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasBancarias", x => new { x.BankCode, x.AccountNumber });
                    table.ForeignKey(
                        name: "FK_CuentasBancarias_Bancos_BankCode",
                        column: x => x.BankCode,
                        principalSchema: "comun",
                        principalTable: "Bancos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasBancarias_CuentasContables_AccountCode",
                        column: x => x.AccountCode,
                        principalSchema: "contabilidad",
                        principalTable: "CuentasContables",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasBancarias_Monedas_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                schema: "facturacion",
                columns: table => new
                {
                    TipoProductoCode = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(4)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitCode = table.Column<string>(type: "char(2)", maxLength: 5, nullable: false),
                    AccountingAccountCode = table.Column<string>(type: "char(8)", maxLength: 8, nullable: true),
                    TransportAccountCode = table.Column<string>(type: "char(8)", maxLength: 8, nullable: true),
                    CreditNoteAccountCode = table.Column<string>(type: "char(8)", maxLength: 8, nullable: true),
                    LengthLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TransportTipoProductoCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: true),
                    TransportCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: true),
                    CategoryCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: true),
                    StrengthCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: true),
                    CementTypeCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: true),
                    StoneSizeCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: true),
                    SlumpCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: true),
                    WaterCementRatioCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: true),
                    AgeCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: true),
                    SpecialConditionCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: true),
                    MixProportionCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: true),
                    IsPumpable = table.Column<bool>(type: "bit", nullable: false),
                    IsSubjectToDetraction = table.Column<bool>(type: "bit", nullable: false),
                    GoodsTypeCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: true),
                    OperationTypeCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: true),
                    CementValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => new { x.TipoProductoCode, x.Code });
                    table.ForeignKey(
                        name: "FK_Productos_AtributosConcreto_AgeCode",
                        column: x => x.AgeCode,
                        principalSchema: "facturacion",
                        principalTable: "AtributosConcreto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_AtributosConcreto_CementTypeCode",
                        column: x => x.CementTypeCode,
                        principalSchema: "facturacion",
                        principalTable: "AtributosConcreto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_AtributosConcreto_MixProportionCode",
                        column: x => x.MixProportionCode,
                        principalSchema: "facturacion",
                        principalTable: "AtributosConcreto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_AtributosConcreto_SlumpCode",
                        column: x => x.SlumpCode,
                        principalSchema: "facturacion",
                        principalTable: "AtributosConcreto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_AtributosConcreto_SpecialConditionCode",
                        column: x => x.SpecialConditionCode,
                        principalSchema: "facturacion",
                        principalTable: "AtributosConcreto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_AtributosConcreto_StoneSizeCode",
                        column: x => x.StoneSizeCode,
                        principalSchema: "facturacion",
                        principalTable: "AtributosConcreto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_AtributosConcreto_StrengthCode",
                        column: x => x.StrengthCode,
                        principalSchema: "facturacion",
                        principalTable: "AtributosConcreto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_AtributosConcreto_WaterCementRatioCode",
                        column: x => x.WaterCementRatioCode,
                        principalSchema: "facturacion",
                        principalTable: "AtributosConcreto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_CategoriasProducto_CategoryCode",
                        column: x => x.CategoryCode,
                        principalSchema: "facturacion",
                        principalTable: "CategoriasProducto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_CuentasContables_AccountingAccountCode",
                        column: x => x.AccountingAccountCode,
                        principalSchema: "contabilidad",
                        principalTable: "CuentasContables",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_CuentasContables_CreditNoteAccountCode",
                        column: x => x.CreditNoteAccountCode,
                        principalSchema: "contabilidad",
                        principalTable: "CuentasContables",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_CuentasContables_TransportAccountCode",
                        column: x => x.TransportAccountCode,
                        principalSchema: "contabilidad",
                        principalTable: "CuentasContables",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_Productos_TransportTipoProductoCode_TransportCode",
                        columns: x => new { x.TransportTipoProductoCode, x.TransportCode },
                        principalSchema: "facturacion",
                        principalTable: "Productos",
                        principalColumns: new[] { "TipoProductoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_TiposBien_GoodsTypeCode",
                        column: x => x.GoodsTypeCode,
                        principalSchema: "facturacion",
                        principalTable: "TiposBien",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_TiposOperacion_OperationTypeCode",
                        column: x => x.OperationTypeCode,
                        principalSchema: "facturacion",
                        principalTable: "TiposOperacion",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_TiposProducto_TipoProductoCode",
                        column: x => x.TipoProductoCode,
                        principalSchema: "facturacion",
                        principalTable: "TiposProducto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_UnidadesMedidaVenta_UnitCode",
                        column: x => x.UnitCode,
                        principalSchema: "facturacion",
                        principalTable: "UnidadesMedidaVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubCentrosCosto",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    CentroCostoCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountingAccountCode = table.Column<string>(type: "char(8)", maxLength: 8, nullable: true),
                    AccountingAttachmentTypeCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: true),
                    PlantaCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    ParentCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCentrosCosto", x => x.Code);
                    table.ForeignKey(
                        name: "FK_SubCentrosCosto_CentrosCosto_CentroCostoCode",
                        column: x => x.CentroCostoCode,
                        principalSchema: "logistica",
                        principalTable: "CentrosCosto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubCentrosCosto_CuentasContables_AccountingAccountCode",
                        column: x => x.AccountingAccountCode,
                        principalSchema: "contabilidad",
                        principalTable: "CuentasContables",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubCentrosCosto_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubCentrosCosto_SubCentrosCosto_ParentCode",
                        column: x => x.ParentCode,
                        principalSchema: "logistica",
                        principalTable: "SubCentrosCosto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubCentrosCosto_TiposAnexo_AccountingAttachmentTypeCode",
                        column: x => x.AccountingAttachmentTypeCode,
                        principalSchema: "contabilidad",
                        principalTable: "TiposAnexo",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CotizacionProveedores",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    CotizacionCode = table.Column<string>(type: "char(6)", nullable: false),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    NetoCotizacion = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    IgvCotizacion = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TotalCotizacion = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Observaciones = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, defaultValue: ""),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotizacionProveedores", x => new { x.PlantaCode, x.CotizacionCode, x.ProveedorCode });
                    table.ForeignKey(
                        name: "FK_CotizacionProveedores_Cotizaciones_PlantaCode_CotizacionCode",
                        columns: x => new { x.PlantaCode, x.CotizacionCode },
                        principalSchema: "logistica",
                        principalTable: "Cotizaciones",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CotizacionProveedores_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CotizacionProveedores_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImportacionGastos",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    ImportacionCode = table.Column<string>(type: "char(6)", nullable: false),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ComprobantePagoCode = table.Column<string>(type: "char(2)", fixedLength: true, nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    Afecto = table.Column<bool>(type: "bit", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoCambio = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    NetoGasto = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    NetoGastoInafecto = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    Igv = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    IgvExterior = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    Total = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportacionGastos", x => new { x.PlantaCode, x.ImportacionCode, x.ProveedorCode, x.NumeroDocumento });
                    table.ForeignKey(
                        name: "FK_ImportacionGastos_ComprobantesPago_ComprobantePagoCode",
                        column: x => x.ComprobantePagoCode,
                        principalSchema: "comun",
                        principalTable: "ComprobantesPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportacionGastos_Importaciones_PlantaCode_ImportacionCode",
                        columns: x => new { x.PlantaCode, x.ImportacionCode },
                        principalSchema: "logistica",
                        principalTable: "Importaciones",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImportacionGastos_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportacionGastos_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesCompra",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    TipoCompraCode = table.Column<string>(type: "char(1)", maxLength: 1, nullable: false),
                    ComprobantePagoCode = table.Column<string>(type: "char(2)", fixedLength: true, nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    CompradorCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    LugarEnvioCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    FormaPagoCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    TramiteCode = table.Column<string>(type: "char(1)", maxLength: 1, nullable: false),
                    Observaciones1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Observaciones2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NotaCompraCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: true),
                    UnidadNegocioCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: false, defaultValue: "000000"),
                    EnviarCorreoProveedor = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    AprobadoPor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoRetraso = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NetoCompra = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    IgvCompra = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    TotalCompra = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    NoGravableCompra = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    RentaCompra = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    FonaviCompra = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    ServicioCompra = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    IgvExteriorCompra = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesCompra", x => new { x.PlantaCode, x.Code });
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_Compradores_CompradorCode",
                        column: x => x.CompradorCode,
                        principalSchema: "logistica",
                        principalTable: "Compradores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_ComprobantesPago_ComprobantePagoCode",
                        column: x => x.ComprobantePagoCode,
                        principalSchema: "comun",
                        principalTable: "ComprobantesPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_FormasPago_FormaPagoCode",
                        column: x => x.FormaPagoCode,
                        principalSchema: "logistica",
                        principalTable: "FormasPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_LugaresEnvio_LugarEnvioCode",
                        column: x => x.LugarEnvioCode,
                        principalSchema: "logistica",
                        principalTable: "LugaresEnvio",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_NotasCompra_NotaCompraCode",
                        column: x => x.NotaCompraCode,
                        principalSchema: "logistica",
                        principalTable: "NotasCompra",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_TiposCompra_TipoCompraCode",
                        column: x => x.TipoCompraCode,
                        principalSchema: "logistica",
                        principalTable: "TiposCompra",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_Tramites_TramiteCode",
                        column: x => x.TramiteCode,
                        principalSchema: "logistica",
                        principalTable: "Tramites",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_UnidadesNegocio_UnidadNegocioCode",
                        column: x => x.UnidadNegocioCode,
                        principalSchema: "logistica",
                        principalTable: "UnidadesNegocio",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesCompraAdministracion",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    ComprobantePagoCode = table.Column<string>(type: "char(2)", fixedLength: true, nullable: false),
                    EmitirDocumento = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    CompradorCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    LugarEnvioCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    FormaPagoCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    TramiteCode = table.Column<string>(type: "char(1)", maxLength: 1, nullable: false),
                    Observaciones1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Observaciones2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NotaCompraCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: true),
                    UnidadNegocioCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: false, defaultValue: "000000"),
                    Motivo = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    AprobadoPor = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NetoCompra = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    IgvCompra = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    TotalCompra = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    NoGravableCompra = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    RentaCompra = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    FonaviCompra = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    ServicioCompra = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    IgvExteriorCompra = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesCompraAdministracion", x => x.Code);
                    table.ForeignKey(
                        name: "FK_OrdenesCompraAdministracion_Compradores_CompradorCode",
                        column: x => x.CompradorCode,
                        principalSchema: "logistica",
                        principalTable: "Compradores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompraAdministracion_ComprobantesPago_ComprobantePagoCode",
                        column: x => x.ComprobantePagoCode,
                        principalSchema: "comun",
                        principalTable: "ComprobantesPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompraAdministracion_FormasPago_FormaPagoCode",
                        column: x => x.FormaPagoCode,
                        principalSchema: "logistica",
                        principalTable: "FormasPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompraAdministracion_LugaresEnvio_LugarEnvioCode",
                        column: x => x.LugarEnvioCode,
                        principalSchema: "logistica",
                        principalTable: "LugaresEnvio",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompraAdministracion_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompraAdministracion_NotasCompra_NotaCompraCode",
                        column: x => x.NotaCompraCode,
                        principalSchema: "logistica",
                        principalTable: "NotasCompra",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompraAdministracion_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompraAdministracion_Tramites_TramiteCode",
                        column: x => x.TramiteCode,
                        principalSchema: "logistica",
                        principalTable: "Tramites",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompraAdministracion_UnidadesNegocio_UnidadNegocioCode",
                        column: x => x.UnidadNegocioCode,
                        principalSchema: "logistica",
                        principalTable: "UnidadesNegocio",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProveedorCondiciones",
                schema: "logistica",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    FormaPagoCode = table.Column<string>(type: "char(2)", nullable: false),
                    PaymentTermDays = table.Column<int>(type: "int", nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", nullable: false),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedorCondiciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProveedorCondiciones_FormasPago_FormaPagoCode",
                        column: x => x.FormaPagoCode,
                        principalSchema: "logistica",
                        principalTable: "FormasPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProveedorCondiciones_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProveedorCondiciones_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProveedorContactos",
                schema: "logistica",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    MobilePhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedorContactos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProveedorContactos_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProveedorCuentas",
                schema: "logistica",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    BancoCode = table.Column<string>(type: "char(3)", nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    InterbankCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MonedaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedorCuentas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProveedorCuentas_Bancos_BancoCode",
                        column: x => x.BancoCode,
                        principalSchema: "comun",
                        principalTable: "Bancos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProveedorCuentas_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProveedorCuentas_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProveedorDirecciones",
                schema: "logistica",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    AddressType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UbigeoCode = table.Column<string>(type: "char(6)", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedorDirecciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProveedorDirecciones_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProveedorDirecciones_Ubigeos_UbigeoCode",
                        column: x => x.UbigeoCode,
                        principalSchema: "comun",
                        principalTable: "Ubigeos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Obras",
                schema: "facturacion",
                columns: table => new
                {
                    ClienteCode = table.Column<string>(type: "char(5)", nullable: false),
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UbigeoCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeliveryUbigeoCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: false),
                    BillingAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BillingUbigeoCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: false),
                    ResponsibleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ResponsiblePhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ResponsibleEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FormaPagoVentaCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: true),
                    CobradorCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    VendedorCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    AnalisisVentaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: true),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditCurrencyCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HasSurcharge = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ShortName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TipoValorizacionCode = table.Column<string>(type: "char(1)", maxLength: 1, nullable: true),
                    RequiresValorizacion = table.Column<bool>(type: "bit", nullable: true),
                    ScheduledWeekday = table.Column<int>(type: "int", nullable: true),
                    IsProject = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RequiresPrinting = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RequiresManagementApproval = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obras", x => new { x.ClienteCode, x.Code });
                    table.ForeignKey(
                        name: "FK_Obras_AnalisisVentas_AnalisisVentaCode",
                        column: x => x.AnalisisVentaCode,
                        principalSchema: "facturacion",
                        principalTable: "AnalisisVentas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Obras_Clientes_ClienteCode",
                        column: x => x.ClienteCode,
                        principalSchema: "facturacion",
                        principalTable: "Clientes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Obras_FormasPagoVenta_FormaPagoVentaCode",
                        column: x => x.FormaPagoVentaCode,
                        principalSchema: "facturacion",
                        principalTable: "FormasPagoVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Obras_Monedas_CreditCurrencyCode",
                        column: x => x.CreditCurrencyCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Obras_TiposValorizacion_TipoValorizacionCode",
                        column: x => x.TipoValorizacionCode,
                        principalSchema: "facturacion",
                        principalTable: "TiposValorizacion",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Obras_Ubigeos_BillingUbigeoCode",
                        column: x => x.BillingUbigeoCode,
                        principalSchema: "comun",
                        principalTable: "Ubigeos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Obras_Ubigeos_DeliveryUbigeoCode",
                        column: x => x.DeliveryUbigeoCode,
                        principalSchema: "comun",
                        principalTable: "Ubigeos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Obras_Ubigeos_UbigeoCode",
                        column: x => x.UbigeoCode,
                        principalSchema: "comun",
                        principalTable: "Ubigeos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesServicioSalida",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    Neto = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Igv = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    AsientoContable = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    AprobadoPor = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcesadoPor = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FechaProcesado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesServicioSalida", x => new { x.PlantaCode, x.Code });
                    table.ForeignKey(
                        name: "FK_OrdenesServicioSalida_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicioSalida_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tecnicos",
                schema: "facturacion",
                columns: table => new
                {
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnicos", x => x.TrabajadorCode);
                    table.ForeignKey(
                        name: "FK_Tecnicos_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "trabajador_antecedente",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    TieneAntecedentes = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajador_antecedente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajador_antecedente_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trabajador_beneficio",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    Cts = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Gratificacion = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Vacaciones = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MovilidadAntesEntrada = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MovilidadDespuesSalida = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Refrigerio = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Cena = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Vale = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajador_beneficio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajador_beneficio_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trabajador_contable",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    NumeroItem = table.Column<int>(type: "int", nullable: false),
                    CuentaContable = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Porcentaje = table.Column<decimal>(type: "decimal(7,4)", nullable: false, defaultValue: 0m),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajador_contable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajador_contable_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trabajador_contrato",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    TipoContratoCode = table.Column<string>(type: "char(3)", nullable: true),
                    TipoExtensionCode = table.Column<string>(type: "char(3)", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaTermino = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Renovado = table.Column<bool>(type: "bit", nullable: true),
                    TipoDuracion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CantidadDuracion = table.Column<int>(type: "int", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajador_contrato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajador_contrato_TiposContrato_TipoContratoCode",
                        column: x => x.TipoContratoCode,
                        principalSchema: "rrhh",
                        principalTable: "TiposContrato",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_contrato_TiposExtensionContrato_TipoExtensionCode",
                        column: x => x.TipoExtensionCode,
                        principalSchema: "rrhh",
                        principalTable: "TiposExtensionContrato",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_contrato_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trabajador_cuenta_bancaria",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    TipoCuentaCode = table.Column<string>(type: "char(3)", nullable: true),
                    BancoCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: true),
                    MonedaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: true),
                    NumeroCuenta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TipoOperacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Principal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajador_cuenta_bancaria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajador_cuenta_bancaria_Bancos_BancoCode",
                        column: x => x.BancoCode,
                        principalSchema: "comun",
                        principalTable: "Bancos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_cuenta_bancaria_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_cuenta_bancaria_TiposCuenta_TipoCuentaCode",
                        column: x => x.TipoCuentaCode,
                        principalSchema: "rrhh",
                        principalTable: "TiposCuenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_cuenta_bancaria_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trabajador_dependiente",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ParentescoCode = table.Column<string>(type: "char(3)", nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Documento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Asegurado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajador_dependiente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajador_dependiente_Parentescos_ParentescoCode",
                        column: x => x.ParentescoCode,
                        principalSchema: "rrhh",
                        principalTable: "Parentescos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_dependiente_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trabajador_fiscal",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    ConInmTrabajador = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Domiciliado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    OtrosIngresosQuinta = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RentaQuintaExonerada = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MadreResFamiliar = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajador_fiscal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajador_fiscal_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trabajador_formacion",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    NivelEducativoCode = table.Column<string>(type: "char(3)", nullable: true),
                    GradoInstruccionCode = table.Column<string>(type: "char(3)", nullable: true),
                    TituloCode = table.Column<string>(type: "char(3)", nullable: true),
                    EspecialidadCode = table.Column<string>(type: "char(3)", nullable: true),
                    TipoCentroFormacionCode = table.Column<string>(type: "char(3)", nullable: true),
                    ModalidadFormativaCode = table.Column<string>(type: "char(3)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajador_formacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajador_formacion_Especialidades_EspecialidadCode",
                        column: x => x.EspecialidadCode,
                        principalSchema: "rrhh",
                        principalTable: "Especialidades",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_formacion_GradosInstruccion_GradoInstruccionCode",
                        column: x => x.GradoInstruccionCode,
                        principalSchema: "rrhh",
                        principalTable: "GradosInstruccion",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_formacion_ModalidadesFormativas_ModalidadFormativaCode",
                        column: x => x.ModalidadFormativaCode,
                        principalSchema: "rrhh",
                        principalTable: "ModalidadesFormativas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_formacion_NivelesEducativos_NivelEducativoCode",
                        column: x => x.NivelEducativoCode,
                        principalSchema: "rrhh",
                        principalTable: "NivelesEducativos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_formacion_TiposCentroFormacion_TipoCentroFormacionCode",
                        column: x => x.TipoCentroFormacionCode,
                        principalSchema: "rrhh",
                        principalTable: "TiposCentroFormacion",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_formacion_Titulos_TituloCode",
                        column: x => x.TituloCode,
                        principalSchema: "rrhh",
                        principalTable: "Titulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_formacion_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trabajador_jornada",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    HorarioCode = table.Column<string>(type: "char(3)", nullable: true),
                    HorasExtras = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HorasExt40 = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HorasExtCon = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HorasExtCon125 = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    HorasExtCon135 = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    ControlHorario = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HorarioOrdinario = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HorarioNocturno = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    JornadaMaxima = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RegimenAlternativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajador_jornada", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajador_jornada_Horarios_HorarioCode",
                        column: x => x.HorarioCode,
                        principalSchema: "rrhh",
                        principalTable: "Horarios",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_jornada_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trabajador_laboral",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCese = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TipoTrabajadorCode = table.Column<string>(type: "char(3)", nullable: true),
                    CategoriaTrabajadorCode = table.Column<string>(type: "char(3)", nullable: true),
                    EstadoTrabajadorCode = table.Column<string>(type: "char(3)", nullable: true),
                    AreaCode = table.Column<string>(type: "char(3)", nullable: true),
                    OcupacionCode = table.Column<string>(type: "char(3)", nullable: true),
                    SubOcupacionCode = table.Column<string>(type: "char(3)", nullable: true),
                    OficinaCode = table.Column<string>(type: "char(3)", nullable: true),
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: true),
                    CargoCode = table.Column<string>(type: "char(3)", nullable: true),
                    NivelCode = table.Column<string>(type: "char(3)", nullable: true),
                    RegimenLaboralCode = table.Column<string>(type: "char(3)", nullable: true),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: true),
                    Permanente = table.Column<bool>(type: "bit", nullable: true),
                    Pensionista = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajador_laboral", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajador_laboral_Areas_AreaCode",
                        column: x => x.AreaCode,
                        principalSchema: "rrhh",
                        principalTable: "Areas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_laboral_Cargos_CargoCode",
                        column: x => x.CargoCode,
                        principalSchema: "rrhh",
                        principalTable: "Cargos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_laboral_CategoriasTrabajador_CategoriaTrabajadorCode",
                        column: x => x.CategoriaTrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "CategoriasTrabajador",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_laboral_EstadosTrabajador_EstadoTrabajadorCode",
                        column: x => x.EstadoTrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "EstadosTrabajador",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_laboral_NivelesTrabajador_NivelCode",
                        column: x => x.NivelCode,
                        principalSchema: "rrhh",
                        principalTable: "NivelesTrabajador",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_laboral_Ocupaciones_OcupacionCode",
                        column: x => x.OcupacionCode,
                        principalSchema: "rrhh",
                        principalTable: "Ocupaciones",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_laboral_Oficinas_OficinaCode",
                        column: x => x.OficinaCode,
                        principalSchema: "rrhh",
                        principalTable: "Oficinas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_laboral_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_laboral_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_laboral_RegimenesLaborales_RegimenLaboralCode",
                        column: x => x.RegimenLaboralCode,
                        principalSchema: "rrhh",
                        principalTable: "RegimenesLaborales",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_laboral_SubOcupaciones_SubOcupacionCode",
                        column: x => x.SubOcupacionCode,
                        principalSchema: "rrhh",
                        principalTable: "SubOcupaciones",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_laboral_TiposTrabajador_TipoTrabajadorCode",
                        column: x => x.TipoTrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "TiposTrabajador",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_laboral_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trabajador_pension",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    TipoAfiliacionCode = table.Column<string>(type: "char(3)", nullable: true),
                    AfpCode = table.Column<string>(type: "char(3)", nullable: true),
                    FechaAfiliacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NumeroAfp = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    RegimenPensionarioCode = table.Column<string>(type: "char(3)", nullable: true),
                    TipoPensionCode = table.Column<string>(type: "char(3)", nullable: true),
                    NumeroCarnetSsp = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajador_pension", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajador_pension_Afps_AfpCode",
                        column: x => x.AfpCode,
                        principalSchema: "rrhh",
                        principalTable: "Afps",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_pension_RegimenesPensionarios_RegimenPensionarioCode",
                        column: x => x.RegimenPensionarioCode,
                        principalSchema: "rrhh",
                        principalTable: "RegimenesPensionarios",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_pension_TiposAfiliacion_TipoAfiliacionCode",
                        column: x => x.TipoAfiliacionCode,
                        principalSchema: "rrhh",
                        principalTable: "TiposAfiliacion",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_pension_TiposPension_TipoPensionCode",
                        column: x => x.TipoPensionCode,
                        principalSchema: "rrhh",
                        principalTable: "TiposPension",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_pension_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trabajador_remuneracion",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    SueldoBasico = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    MonedaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: true),
                    ModoPagoCode = table.Column<string>(type: "char(3)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajador_remuneracion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajador_remuneracion_ModosPago_ModoPagoCode",
                        column: x => x.ModoPagoCode,
                        principalSchema: "rrhh",
                        principalTable: "ModosPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_remuneracion_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_remuneracion_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trabajador_salud",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    TipoSangreCode = table.Column<string>(type: "char(3)", nullable: true),
                    AlergiaCode = table.Column<string>(type: "char(3)", nullable: true),
                    Otros = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaEvaluacionMedica = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajador_salud", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajador_salud_Alergias_AlergiaCode",
                        column: x => x.AlergiaCode,
                        principalSchema: "rrhh",
                        principalTable: "Alergias",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_salud_TiposSangre_TipoSangreCode",
                        column: x => x.TipoSangreCode,
                        principalSchema: "rrhh",
                        principalTable: "TiposSangre",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_salud_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trabajador_seguro",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    EpsCode = table.Column<string>(type: "char(3)", nullable: true),
                    SituacionEpsCode = table.Column<string>(type: "char(3)", nullable: true),
                    NumeroSeguro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SctrTipoCode = table.Column<string>(type: "char(3)", nullable: true),
                    SctrSaludCode = table.Column<string>(type: "char(3)", nullable: true),
                    SctrPensionCode = table.Column<string>(type: "char(3)", nullable: true),
                    EpsActivo = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SeguroMedico = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EssaludVida = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajador_seguro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajador_seguro_Eps_EpsCode",
                        column: x => x.EpsCode,
                        principalSchema: "rrhh",
                        principalTable: "Eps",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_seguro_SctrPension_SctrPensionCode",
                        column: x => x.SctrPensionCode,
                        principalSchema: "rrhh",
                        principalTable: "SctrPension",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_seguro_SctrSalud_SctrSaludCode",
                        column: x => x.SctrSaludCode,
                        principalSchema: "rrhh",
                        principalTable: "SctrSalud",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_seguro_SituacionesEps_SituacionEpsCode",
                        column: x => x.SituacionEpsCode,
                        principalSchema: "rrhh",
                        principalTable: "SituacionesEps",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_seguro_TiposSctr_SctrTipoCode",
                        column: x => x.SctrTipoCode,
                        principalSchema: "rrhh",
                        principalTable: "TiposSctr",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajador_seguro_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trabajador_sindicato",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    Afiliado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajador_sindicato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajador_sindicato_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trabajador_vacacion",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    FechaVacaciones = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajador_vacacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajador_vacacion_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrabajadorContactos",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", maxLength: 5, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ParentescoCode = table.Column<string>(type: "char(3)", maxLength: 10, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrabajadorContactos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrabajadorContactos_Parentescos_ParentescoCode",
                        column: x => x.ParentescoCode,
                        principalSchema: "rrhh",
                        principalTable: "Parentescos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrabajadorContactos_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrabajadorDocumentos",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", maxLength: 5, nullable: false),
                    TipoDocumentoCode = table.Column<string>(type: "char(2)", maxLength: 20, nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrabajadorDocumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrabajadorDocumentos_TiposDocumento_TipoDocumentoCode",
                        column: x => x.TipoDocumentoCode,
                        principalSchema: "comun",
                        principalTable: "TiposDocumento",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrabajadorDocumentos_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrabajadorDomicilios",
                schema: "rrhh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrabajadorCode = table.Column<string>(type: "char(5)", maxLength: 5, nullable: false),
                    RoadTypeCode = table.Column<string>(type: "char(3)", maxLength: 10, nullable: true),
                    StreetName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    StreetNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    InteriorNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ZoneTypeCode = table.Column<string>(type: "char(3)", maxLength: 10, nullable: true),
                    ZoneName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UbigeoCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrabajadorDomicilios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrabajadorDomicilios_TiposVia_RoadTypeCode",
                        column: x => x.RoadTypeCode,
                        principalSchema: "rrhh",
                        principalTable: "TiposVia",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrabajadorDomicilios_TiposZona_ZoneTypeCode",
                        column: x => x.ZoneTypeCode,
                        principalSchema: "rrhh",
                        principalTable: "TiposZona",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrabajadorDomicilios_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrabajadorDomicilios_Ubigeos_UbigeoCode",
                        column: x => x.UbigeoCode,
                        principalSchema: "comun",
                        principalTable: "Ubigeos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vehiculos",
                schema: "logistica",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(5)", nullable: false),
                    TransportistaCode = table.Column<string>(type: "char(5)", maxLength: 5, nullable: false),
                    LicensePlate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    VehicleCategory = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    VehicleType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ManufactureYear = table.Column<short>(type: "smallint", nullable: true),
                    EngineNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChassisNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CargoCapacityKg = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    LengthM = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    WidthM = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    HeightM = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    VehicularCertificateNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CirculationCardNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VehicularConfiguration = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehiculos", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Vehiculos_Transportistas_TransportistaCode",
                        column: x => x.TransportistaCode,
                        principalSchema: "logistica",
                        principalTable: "Transportistas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChoferesVentas",
                schema: "facturacion",
                columns: table => new
                {
                    TransportistaCode = table.Column<string>(type: "char(4)", nullable: false),
                    Code = table.Column<string>(type: "char(4)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DriverLicenseNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChoferesVentas", x => new { x.TransportistaCode, x.Code });
                    table.ForeignKey(
                        name: "FK_ChoferesVentas_TransportistasVentas_TransportistaCode",
                        column: x => x.TransportistaCode,
                        principalSchema: "facturacion",
                        principalTable: "TransportistasVentas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AprobadoresAsignados",
                schema: "logistica",
                columns: table => new
                {
                    NivelCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    TipoTransaccionCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    UnidadNegocioCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    SuplenteTrabajadorCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    SuperiorTrabajadorCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AprobadoresAsignados", x => new { x.NivelCode, x.TipoTransaccionCode, x.UnidadNegocioCode, x.MonedaCode, x.TrabajadorCode });
                    table.ForeignKey(
                        name: "FK_AprobadoresAsignados_RangosAprobacion_NivelCode_TipoTransaccionCode_UnidadNegocioCode_MonedaCode",
                        columns: x => new { x.NivelCode, x.TipoTransaccionCode, x.UnidadNegocioCode, x.MonedaCode },
                        principalSchema: "logistica",
                        principalTable: "RangosAprobacion",
                        principalColumns: new[] { "NivelCode", "TipoTransaccionCode", "UnidadNegocioCode", "MonedaCode" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AprobadoresAsignados_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramaId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Programas_ProgramaId",
                        column: x => x.ProgramaId,
                        principalSchema: "admin",
                        principalTable: "Programas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AjusteInventarioDetalles",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    AjusteCode = table.Column<string>(type: "char(6)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(12,4)", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    AsientoContable = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AjusteInventarioDetalles", x => new { x.PlantaCode, x.AjusteCode, x.ArticuloCode });
                    table.ForeignKey(
                        name: "FK_AjusteInventarioDetalles_AjustesInventario_PlantaCode_AjusteCode",
                        columns: x => new { x.PlantaCode, x.AjusteCode },
                        principalSchema: "logistica",
                        principalTable: "AjustesInventario",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AjusteInventarioDetalles_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArticuloProveedor",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    IsAgreement = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AgreementPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticuloProveedor", x => new { x.PlantaCode, x.ArticuloCode, x.ProveedorCode });
                    table.ForeignKey(
                        name: "FK_ArticuloProveedor_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticuloProveedor_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticuloProveedor_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CierreIncidencias",
                schema: "logistica",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    PeriodCode = table.Column<string>(type: "char(6)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    DocumentoCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Mensaje = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CierreIncidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CierreIncidencias_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CierreIncidencias_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CotizacionDetalles",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    CotizacionCode = table.Column<string>(type: "char(6)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    CantidadArticulo = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotizacionDetalles", x => new { x.PlantaCode, x.CotizacionCode, x.ArticuloCode });
                    table.ForeignKey(
                        name: "FK_CotizacionDetalles_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CotizacionDetalles_Cotizaciones_PlantaCode_CotizacionCode",
                        columns: x => new { x.PlantaCode, x.CotizacionCode },
                        principalSchema: "logistica",
                        principalTable: "Cotizaciones",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImportacionDetalles",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    ImportacionCode = table.Column<string>(type: "char(6)", nullable: false),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    ComprobantePagoCode = table.Column<string>(type: "char(2)", fixedLength: true, nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoCambio = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    Cantidad = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    ValorFob = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    Flete = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    Seguro = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    ValorAduana = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    PorcentajeDet = table.Column<decimal>(type: "decimal(18,10)", nullable: false, defaultValue: 0m),
                    ValorDet = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    AdvaloremDet = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    SobretasaDet = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    IgvDet = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    OtrosGastosDet = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportacionDetalles", x => new { x.PlantaCode, x.ImportacionCode, x.ProveedorCode, x.ArticuloCode });
                    table.ForeignKey(
                        name: "FK_ImportacionDetalles_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportacionDetalles_ComprobantesPago_ComprobantePagoCode",
                        column: x => x.ComprobantePagoCode,
                        principalSchema: "comun",
                        principalTable: "ComprobantesPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportacionDetalles_Importaciones_PlantaCode_ImportacionCode",
                        columns: x => new { x.PlantaCode, x.ImportacionCode },
                        principalSchema: "logistica",
                        principalTable: "Importaciones",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImportacionDetalles_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosArticulo",
                schema: "logistica",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "date", nullable: false),
                    PeriodCode = table.Column<string>(type: "char(6)", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Origen = table.Column<int>(type: "int", nullable: false),
                    DocumentoCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: true),
                    Cantidad = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    CostoUnitarioUsd = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    StockPosterior = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    CostoPromedioPosterior = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    EsReversion = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReversaDeMovimientoId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosArticulo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosArticulo_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosArticulo_MovimientosArticulo_ReversaDeMovimientoId",
                        column: x => x.ReversaDeMovimientoId,
                        principalSchema: "logistica",
                        principalTable: "MovimientosArticulo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosArticulo_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaldosMensualesArticulo",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    PeriodCode = table.Column<string>(type: "char(6)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    AverageCost = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    IncomingQuantity = table.Column<decimal>(type: "decimal(14,2)", nullable: false, defaultValue: 0m),
                    OutgoingQuantity = table.Column<decimal>(type: "decimal(14,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaldosMensualesArticulo", x => new { x.PlantaCode, x.PeriodCode, x.ArticuloCode });
                    table.ForeignKey(
                        name: "FK_SaldosMensualesArticulo_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaldosMensualesArticulo_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockArticulos",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    UnitCost = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    UnitCostUsd = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    AverageCost = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    MinStock = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    MaxStock = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockArticulos", x => new { x.PlantaCode, x.ArticuloCode });
                    table.CheckConstraint("CK_StockArticulos_Quantity_NonNegative", "[Quantity] >= 0");
                    table.ForeignKey(
                        name: "FK_StockArticulos_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockArticulos_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TiposValeArticulo",
                schema: "logistica",
                columns: table => new
                {
                    TipoValeCode = table.Column<string>(type: "char(3)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposValeArticulo", x => new { x.TipoValeCode, x.ArticuloCode });
                    table.ForeignKey(
                        name: "FK_TiposValeArticulo_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TiposValeArticulo_TiposVale_TipoValeCode",
                        column: x => x.TipoValeCode,
                        principalSchema: "logistica",
                        principalTable: "TiposVale",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlturasLosa",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Width = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    ProductoTipoCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    ProductoCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: false),
                    PolystyreneProductoTipoCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: true),
                    PolystyreneProductoCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: true),
                    PolystyreneValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    PolystyreneWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlturasLosa", x => x.Code);
                    table.ForeignKey(
                        name: "FK_AlturasLosa_Productos_PolystyreneProductoTipoCode_PolystyreneProductoCode",
                        columns: x => new { x.PolystyreneProductoTipoCode, x.PolystyreneProductoCode },
                        principalSchema: "facturacion",
                        principalTable: "Productos",
                        principalColumns: new[] { "TipoProductoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AlturasLosa_Productos_ProductoTipoCode_ProductoCode",
                        columns: x => new { x.ProductoTipoCode, x.ProductoCode },
                        principalSchema: "facturacion",
                        principalTable: "Productos",
                        principalColumns: new[] { "TipoProductoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ListasPrecios",
                schema: "facturacion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoProductoCode = table.Column<string>(type: "char(2)", nullable: false),
                    ProductoCode = table.Column<string>(type: "char(4)", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasPrecios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListasPrecios_Productos_TipoProductoCode_ProductoCode",
                        columns: x => new { x.TipoProductoCode, x.ProductoCode },
                        principalSchema: "facturacion",
                        principalTable: "Productos",
                        principalColumns: new[] { "TipoProductoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreciosProducto",
                schema: "facturacion",
                columns: table => new
                {
                    FleteCode = table.Column<string>(type: "char(2)", nullable: false),
                    ProductoTipoCode = table.Column<string>(type: "char(2)", nullable: false),
                    ProductoCode = table.Column<string>(type: "char(4)", nullable: false),
                    CurrencyTypeCode = table.Column<string>(type: "char(1)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "char(1)", nullable: false, defaultValue: "S"),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    TransportAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    FreightAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreciosProducto", x => new { x.FleteCode, x.ProductoTipoCode, x.ProductoCode, x.CurrencyTypeCode, x.CurrencyCode });
                    table.ForeignKey(
                        name: "FK_PreciosProducto_Fletes_FleteCode",
                        column: x => x.FleteCode,
                        principalSchema: "facturacion",
                        principalTable: "Fletes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreciosProducto_Productos_ProductoTipoCode_ProductoCode",
                        columns: x => new { x.ProductoTipoCode, x.ProductoCode },
                        principalSchema: "facturacion",
                        principalTable: "Productos",
                        principalColumns: new[] { "TipoProductoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockProductos",
                schema: "facturacion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    TipoProductoCode = table.Column<string>(type: "char(2)", nullable: false),
                    ProductoCode = table.Column<string>(type: "char(4)", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockProductos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockProductos_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockProductos_Productos_TipoProductoCode_ProductoCode",
                        columns: x => new { x.TipoProductoCode, x.ProductoCode },
                        principalSchema: "facturacion",
                        principalTable: "Productos",
                        principalColumns: new[] { "TipoProductoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Equipos",
                schema: "mantenimiento",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(8)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    SubCentroCostoCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipos", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Equipos_SubCentrosCosto_SubCentroCostoCode",
                        column: x => x.SubCentroCostoCode,
                        principalSchema: "logistica",
                        principalTable: "SubCentrosCosto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CotizacionProveedorDetalles",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    CotizacionCode = table.Column<string>(type: "char(6)", nullable: false),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    CantidadArticulo = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    PrecioArticulo = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    DescuentoArticulo = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TotalLinea = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotizacionProveedorDetalles", x => new { x.PlantaCode, x.CotizacionCode, x.ProveedorCode, x.ArticuloCode });
                    table.ForeignKey(
                        name: "FK_CotizacionProveedorDetalles_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CotizacionProveedorDetalles_CotizacionProveedores_PlantaCode_CotizacionCode_ProveedorCode",
                        columns: x => new { x.PlantaCode, x.CotizacionCode, x.ProveedorCode },
                        principalSchema: "logistica",
                        principalTable: "CotizacionProveedores",
                        principalColumns: new[] { "PlantaCode", "CotizacionCode", "ProveedorCode" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImportacionGastoArticulos",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    ImportacionCode = table.Column<string>(type: "char(6)", nullable: false),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    ValorGasto = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    IgvGasto = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m),
                    IgvExtGasto = table.Column<decimal>(type: "decimal(12,4)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportacionGastoArticulos", x => new { x.PlantaCode, x.ImportacionCode, x.ProveedorCode, x.NumeroDocumento, x.ArticuloCode });
                    table.ForeignKey(
                        name: "FK_ImportacionGastoArticulos_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportacionGastoArticulos_ImportacionGastos_PlantaCode_ImportacionCode_ProveedorCode_NumeroDocumento",
                        columns: x => new { x.PlantaCode, x.ImportacionCode, x.ProveedorCode, x.NumeroDocumento },
                        principalSchema: "logistica",
                        principalTable: "ImportacionGastos",
                        principalColumns: new[] { "PlantaCode", "ImportacionCode", "ProveedorCode", "NumeroDocumento" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenCompraDetalles",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    OrdenCompraCode = table.Column<string>(type: "char(6)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    CantidadArticulo = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    PrecioArticulo = table.Column<decimal>(type: "decimal(12,6)", nullable: false, defaultValue: 0m),
                    DescuentoArticulo = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    TotalArticulo = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    CantidadEntregada = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    SubCentroCostoCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenCompraDetalles", x => new { x.PlantaCode, x.OrdenCompraCode, x.ArticuloCode });
                    table.ForeignKey(
                        name: "FK_OrdenCompraDetalles_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenCompraDetalles_OrdenesCompra_PlantaCode_OrdenCompraCode",
                        columns: x => new { x.PlantaCode, x.OrdenCompraCode },
                        principalSchema: "logistica",
                        principalTable: "OrdenesCompra",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenCompraDetalles_SubCentrosCosto_SubCentroCostoCode",
                        column: x => x.SubCentroCostoCode,
                        principalSchema: "logistica",
                        principalTable: "SubCentrosCosto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosCompra",
                schema: "contabilidad",
                columns: table => new
                {
                    ComprobantePagoCode = table.Column<string>(type: "char(2)", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "char(15)", nullable: false),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "date", nullable: false),
                    FechaRecepcion = table.Column<DateTime>(type: "date", nullable: false),
                    FormaPagoCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    TipoCambioFecha = table.Column<DateOnly>(type: "date", nullable: true),
                    ParcialSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    IgvSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    NoGravableSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    RentaSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    FonaviSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    ServicioSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    IgvExteriorSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    TotalSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    Glosa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Origen = table.Column<int>(type: "int", nullable: false),
                    Procedencia = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    TipoBienCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: true),
                    TipoOperacionCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: true),
                    OperacionDetraccion = table.Column<string>(type: "char(10)", nullable: true),
                    ConstanciaDetraccion = table.Column<string>(type: "char(10)", nullable: true),
                    FechaConstanciaDetraccion = table.Column<DateTime>(type: "date", nullable: true),
                    NumeroDetraccion = table.Column<string>(type: "char(10)", nullable: true),
                    FechaEmisionDetraccion = table.Column<DateTime>(type: "date", nullable: true),
                    MontoDetraccionSol = table.Column<decimal>(type: "decimal(16,2)", nullable: false, defaultValue: 0m),
                    DocumentoOrigenComprobantePagoCode = table.Column<string>(type: "char(2)", nullable: true),
                    DocumentoOrigenNumero = table.Column<string>(type: "char(15)", nullable: true),
                    OrdenCompraAdministracionCode = table.Column<string>(type: "char(6)", nullable: true),
                    FechaVencimiento = table.Column<DateTime>(type: "date", nullable: false),
                    PagadoSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    FechaCancelacion = table.Column<DateTime>(type: "date", nullable: true),
                    PendienteSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, computedColumnSql: "[TotalSol] - [PagadoSol]"),
                    AsientoProvisionCode = table.Column<string>(type: "char(13)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosCompra", x => new { x.ComprobantePagoCode, x.NumeroDocumento, x.ProveedorCode });
                    table.ForeignKey(
                        name: "FK_DocumentosCompra_ComprobantesPago_ComprobantePagoCode",
                        column: x => x.ComprobantePagoCode,
                        principalSchema: "comun",
                        principalTable: "ComprobantesPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentosCompra_DocumentosCompra_DocumentoOrigenComprobantePagoCode_DocumentoOrigenNumero_ProveedorCode",
                        columns: x => new { x.DocumentoOrigenComprobantePagoCode, x.DocumentoOrigenNumero, x.ProveedorCode },
                        principalSchema: "contabilidad",
                        principalTable: "DocumentosCompra",
                        principalColumns: new[] { "ComprobantePagoCode", "NumeroDocumento", "ProveedorCode" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentosCompra_FormasPago_FormaPagoCode",
                        column: x => x.FormaPagoCode,
                        principalSchema: "logistica",
                        principalTable: "FormasPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentosCompra_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentosCompra_OrdenesCompraAdministracion_OrdenCompraAdministracionCode",
                        column: x => x.OrdenCompraAdministracionCode,
                        principalSchema: "logistica",
                        principalTable: "OrdenesCompraAdministracion",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentosCompra_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentosCompra_TiposBien_TipoBienCode",
                        column: x => x.TipoBienCode,
                        principalSchema: "facturacion",
                        principalTable: "TiposBien",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentosCompra_TiposOperacion_TipoOperacionCode",
                        column: x => x.TipoOperacionCode,
                        principalSchema: "facturacion",
                        principalTable: "TiposOperacion",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenCompraAdministracionDetalles",
                schema: "logistica",
                columns: table => new
                {
                    OrdenCompraAdministracionCode = table.Column<string>(type: "char(6)", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    Glosa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UnidadMedidaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    Precio = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    Descuento = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    SubCentroCostoCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: false),
                    CantidadEntregada = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenCompraAdministracionDetalles", x => new { x.OrdenCompraAdministracionCode, x.ItemNumber });
                    table.ForeignKey(
                        name: "FK_OrdenCompraAdministracionDetalles_OrdenesCompraAdministracion_OrdenCompraAdministracionCode",
                        column: x => x.OrdenCompraAdministracionCode,
                        principalSchema: "logistica",
                        principalTable: "OrdenesCompraAdministracion",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenCompraAdministracionDetalles_SubCentrosCosto_SubCentroCostoCode",
                        column: x => x.SubCentroCostoCode,
                        principalSchema: "logistica",
                        principalTable: "SubCentrosCosto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenCompraAdministracionDetalles_UnidadesMedida_UnidadMedidaCode",
                        column: x => x.UnidadMedidaCode,
                        principalSchema: "logistica",
                        principalTable: "UnidadesMedida",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResponsablesObra",
                schema: "facturacion",
                columns: table => new
                {
                    ClienteCode = table.Column<string>(type: "char(5)", nullable: false),
                    ObraCode = table.Column<string>(type: "char(3)", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsablesObra", x => new { x.ClienteCode, x.ObraCode, x.Code });
                    table.ForeignKey(
                        name: "FK_ResponsablesObra_Obras_ClienteCode_ObraCode",
                        columns: x => new { x.ClienteCode, x.ObraCode },
                        principalSchema: "facturacion",
                        principalTable: "Obras",
                        principalColumns: new[] { "ClienteCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesServicio",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    ComprobantePagoCode = table.Column<string>(type: "char(2)", fixedLength: true, nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    FormaPagoCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    FechaRecepcion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoCambio = table.Column<decimal>(type: "decimal(12,4)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Igv = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    NoGravable = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Renta = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Fonavi = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Servicio = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    IgvExterior = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    AsientoContable = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    AprobadoPor = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcesadoPor = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CompradorCode = table.Column<string>(type: "char(3)", nullable: false),
                    LugarEnvioCode = table.Column<string>(type: "char(3)", nullable: false),
                    TramiteCode = table.Column<string>(type: "char(1)", nullable: false),
                    NotaCompraCode = table.Column<string>(type: "char(3)", nullable: true),
                    UnidadNegocioCode = table.Column<string>(type: "char(6)", nullable: false),
                    ValeSalidaCode = table.Column<string>(type: "char(6)", nullable: false),
                    Observaciones1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Observaciones2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesServicio", x => new { x.PlantaCode, x.Code });
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Compradores_CompradorCode",
                        column: x => x.CompradorCode,
                        principalSchema: "logistica",
                        principalTable: "Compradores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_ComprobantesPago_ComprobantePagoCode",
                        column: x => x.ComprobantePagoCode,
                        principalSchema: "comun",
                        principalTable: "ComprobantesPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_FormasPago_FormaPagoCode",
                        column: x => x.FormaPagoCode,
                        principalSchema: "logistica",
                        principalTable: "FormasPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_LugaresEnvio_LugarEnvioCode",
                        column: x => x.LugarEnvioCode,
                        principalSchema: "logistica",
                        principalTable: "LugaresEnvio",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_NotasCompra_NotaCompraCode",
                        column: x => x.NotaCompraCode,
                        principalSchema: "logistica",
                        principalTable: "NotasCompra",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_OrdenesServicioSalida_PlantaCode_ValeSalidaCode",
                        columns: x => new { x.PlantaCode, x.ValeSalidaCode },
                        principalSchema: "logistica",
                        principalTable: "OrdenesServicioSalida",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Tramites_TramiteCode",
                        column: x => x.TramiteCode,
                        principalSchema: "logistica",
                        principalTable: "Tramites",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_UnidadesNegocio_UnidadNegocioCode",
                        column: x => x.UnidadNegocioCode,
                        principalSchema: "logistica",
                        principalTable: "UnidadesNegocio",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cobradores",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(4)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cobradores", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Cobradores_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "admin",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TokenHash = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "admin",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "admin",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleUsers",
                schema: "admin",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUsers", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_RoleUsers_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "admin",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUsers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "admin",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vendedores",
                schema: "facturacion",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(4)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendedores", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Vendedores_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "admin",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Guias",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(10)", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "date", nullable: false),
                    Hora = table.Column<string>(type: "char(5)", nullable: false),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    MotivoCode = table.Column<string>(type: "char(2)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    PuntoPartida = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    TransportistaCode = table.Column<string>(type: "char(5)", nullable: false),
                    ConductorCode = table.Column<string>(type: "char(5)", nullable: false),
                    VehiculoCode = table.Column<string>(type: "char(5)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: ""),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guias", x => new { x.PlantaCode, x.Code });
                    table.ForeignKey(
                        name: "FK_Guias_Conductores_ConductorCode",
                        column: x => x.ConductorCode,
                        principalSchema: "logistica",
                        principalTable: "Conductores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guias_MotivosDevolucion_MotivoCode",
                        column: x => x.MotivoCode,
                        principalSchema: "comun",
                        principalTable: "MotivosDevolucion",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guias_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guias_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guias_Transportistas_TransportistaCode",
                        column: x => x.TransportistaCode,
                        principalSchema: "logistica",
                        principalTable: "Transportistas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guias_Vehiculos_VehiculoCode",
                        column: x => x.VehiculoCode,
                        principalSchema: "logistica",
                        principalTable: "Vehiculos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vehiculos",
                schema: "facturacion",
                columns: table => new
                {
                    TransportistaCode = table.Column<string>(type: "char(4)", nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "char(4)", nullable: false),
                    LicensePlate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChoferCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: true),
                    Capacity = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    Suple = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    LengthM = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    WidthM = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    HeightM = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    Telescopic = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    WithoutSuple = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    WithSuple = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    CubicWithoutSuple = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    CubicWithSuple = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    MtcInternalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    VehicularConfiguration = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PlanillaCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehiculos", x => new { x.TransportistaCode, x.VehicleType, x.Code });
                    table.ForeignKey(
                        name: "FK_Vehiculos_ChoferesVentas_TransportistaCode_ChoferCode",
                        columns: x => new { x.TransportistaCode, x.ChoferCode },
                        principalSchema: "facturacion",
                        principalTable: "ChoferesVentas",
                        principalColumns: new[] { "TransportistaCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehiculos_TransportistasVentas_TransportistaCode",
                        column: x => x.TransportistaCode,
                        principalSchema: "facturacion",
                        principalTable: "TransportistasVentas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PermissionRoles",
                schema: "admin",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionRoles", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_PermissionRoles_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "admin",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "admin",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesTrabajo",
                schema: "mantenimiento",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    FechaProceso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaTermino = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponsableCode = table.Column<string>(type: "char(5)", nullable: false),
                    EspecialidadCode = table.Column<string>(type: "char(1)", maxLength: 1, nullable: false),
                    OportunidadCode = table.Column<string>(type: "char(4)", maxLength: 2, nullable: false),
                    EquipoCode = table.Column<string>(type: "char(8)", maxLength: 8, nullable: false),
                    PrioridadCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    InspeccionCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    TipoOrdenCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    ActividadCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: false),
                    SubCentroCostoCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: true),
                    SubCentroEjecutorCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: true),
                    PlanMantenimientoPreventivoCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    DowntimeHours = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Horometro = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    Turno = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesTrabajo", x => new { x.PlantaCode, x.Code });
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajo_Actividades_ActividadCode",
                        column: x => x.ActividadCode,
                        principalSchema: "mantenimiento",
                        principalTable: "Actividades",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajo_Equipos_EquipoCode",
                        column: x => x.EquipoCode,
                        principalSchema: "mantenimiento",
                        principalTable: "Equipos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajo_Especialidades_EspecialidadCode",
                        column: x => x.EspecialidadCode,
                        principalSchema: "mantenimiento",
                        principalTable: "Especialidades",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajo_Inspecciones_InspeccionCode",
                        column: x => x.InspeccionCode,
                        principalSchema: "mantenimiento",
                        principalTable: "Inspecciones",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajo_Oportunidades_OportunidadCode",
                        column: x => x.OportunidadCode,
                        principalSchema: "mantenimiento",
                        principalTable: "Oportunidades",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajo_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajo_Prioridades_PrioridadCode",
                        column: x => x.PrioridadCode,
                        principalSchema: "mantenimiento",
                        principalTable: "Prioridades",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajo_SubCentrosCosto_SubCentroCostoCode",
                        column: x => x.SubCentroCostoCode,
                        principalSchema: "logistica",
                        principalTable: "SubCentrosCosto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajo_SubCentrosEjecutores_SubCentroEjecutorCode",
                        column: x => x.SubCentroEjecutorCode,
                        principalSchema: "mantenimiento",
                        principalTable: "SubCentrosEjecutores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajo_TiposOrden_TipoOrdenCode",
                        column: x => x.TipoOrdenCode,
                        principalSchema: "mantenimiento",
                        principalTable: "TiposOrden",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajo_Trabajadores_ResponsableCode",
                        column: x => x.ResponsableCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoCompraDetalles",
                schema: "contabilidad",
                columns: table => new
                {
                    ComprobantePagoCode = table.Column<string>(type: "char(2)", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "char(15)", nullable: false),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    CuentaContableCode = table.Column<string>(type: "char(8)", nullable: false),
                    AnexoTipoCode = table.Column<string>(type: "char(3)", nullable: true),
                    AnexoCode = table.Column<string>(type: "char(5)", nullable: true),
                    Detalle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(24,4)", nullable: false, defaultValue: 0m),
                    UnidadMedidaCode = table.Column<string>(type: "char(2)", maxLength: 5, nullable: false),
                    PrecioSol = table.Column<decimal>(type: "decimal(24,5)", nullable: false, defaultValue: 0m),
                    ParcialSol = table.Column<decimal>(type: "decimal(24,2)", nullable: false, defaultValue: 0m),
                    NumeroGuiaVenta = table.Column<string>(type: "char(10)", nullable: true),
                    ProductoTipoCode = table.Column<string>(type: "char(2)", nullable: true),
                    ProductoCode = table.Column<string>(type: "char(4)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoCompraDetalles", x => new { x.ComprobantePagoCode, x.NumeroDocumento, x.ProveedorCode, x.ItemNumber });
                    table.ForeignKey(
                        name: "FK_DocumentoCompraDetalles_Anexos_AnexoTipoCode_AnexoCode",
                        columns: x => new { x.AnexoTipoCode, x.AnexoCode },
                        principalSchema: "contabilidad",
                        principalTable: "Anexos",
                        principalColumns: new[] { "TipoAnexoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentoCompraDetalles_CuentasContables_CuentaContableCode",
                        column: x => x.CuentaContableCode,
                        principalSchema: "contabilidad",
                        principalTable: "CuentasContables",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentoCompraDetalles_DocumentosCompra_ComprobantePagoCode_NumeroDocumento_ProveedorCode",
                        columns: x => new { x.ComprobantePagoCode, x.NumeroDocumento, x.ProveedorCode },
                        principalSchema: "contabilidad",
                        principalTable: "DocumentosCompra",
                        principalColumns: new[] { "ComprobantePagoCode", "NumeroDocumento", "ProveedorCode" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentoCompraDetalles_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentoCompraDetalles_Productos_ProductoTipoCode_ProductoCode",
                        columns: x => new { x.ProductoTipoCode, x.ProductoCode },
                        principalSchema: "facturacion",
                        principalTable: "Productos",
                        principalColumns: new[] { "TipoProductoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentoCompraDetalles_UnidadesMedidaVenta_UnidadMedidaCode",
                        column: x => x.UnidadMedidaCode,
                        principalSchema: "facturacion",
                        principalTable: "UnidadesMedidaVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cotizaciones",
                schema: "facturacion",
                columns: table => new
                {
                    NegocioCode = table.Column<string>(type: "char(2)", nullable: false),
                    Year = table.Column<string>(type: "char(4)", nullable: false),
                    Month = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(8)", nullable: false),
                    VendedorCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrencyCode = table.Column<string>(type: "char(1)", nullable: false),
                    FormaPagoVentaCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    TecnicoCode = table.Column<string>(type: "char(5)", maxLength: 5, nullable: true),
                    AppliesIgv = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    GlobalVolume = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    IsEditable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Type = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    MetradoCalculationSystem = table.Column<int>(type: "int", nullable: true),
                    WorkDurationMonths = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ClienteCode = table.Column<string>(type: "char(5)", maxLength: 5, nullable: true),
                    Ruc = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ClientAddress = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    ClientAddressUbigeoCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: false, defaultValue: "150101"),
                    ObraCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: true),
                    WorkName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ProjectStatus = table.Column<int>(type: "int", nullable: false),
                    WorkAddressUbigeoCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: false, defaultValue: "150101"),
                    WorkAddress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DispatchDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FleteCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false, defaultValue: "01"),
                    IgvRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    ProcessDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IgvAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    OriginNegocioCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: true),
                    OriginYear = table.Column<string>(type: "char(4)", maxLength: 4, nullable: true),
                    OriginMonth = table.Column<string>(type: "char(2)", maxLength: 2, nullable: true),
                    OriginCode = table.Column<string>(type: "char(8)", maxLength: 8, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CanceledBy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CanceledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPrinted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizaciones", x => new { x.NegocioCode, x.Year, x.Month, x.Code });
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Clientes_ClienteCode",
                        column: x => x.ClienteCode,
                        principalSchema: "facturacion",
                        principalTable: "Clientes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Cotizaciones_OriginNegocioCode_OriginYear_OriginMonth_OriginCode",
                        columns: x => new { x.OriginNegocioCode, x.OriginYear, x.OriginMonth, x.OriginCode },
                        principalSchema: "facturacion",
                        principalTable: "Cotizaciones",
                        principalColumns: new[] { "NegocioCode", "Year", "Month", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Fletes_FleteCode",
                        column: x => x.FleteCode,
                        principalSchema: "facturacion",
                        principalTable: "Fletes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_FormasPagoVenta_FormaPagoVentaCode",
                        column: x => x.FormaPagoVentaCode,
                        principalSchema: "facturacion",
                        principalTable: "FormasPagoVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Negocios_NegocioCode",
                        column: x => x.NegocioCode,
                        principalSchema: "comun",
                        principalTable: "Negocios",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Obras_ClienteCode_ObraCode",
                        columns: x => new { x.ClienteCode, x.ObraCode },
                        principalSchema: "facturacion",
                        principalTable: "Obras",
                        principalColumns: new[] { "ClienteCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Tecnicos_TecnicoCode",
                        column: x => x.TecnicoCode,
                        principalSchema: "facturacion",
                        principalTable: "Tecnicos",
                        principalColumn: "TrabajadorCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Ubigeos_ClientAddressUbigeoCode",
                        column: x => x.ClientAddressUbigeoCode,
                        principalSchema: "comun",
                        principalTable: "Ubigeos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Ubigeos_WorkAddressUbigeoCode",
                        column: x => x.WorkAddressUbigeoCode,
                        principalSchema: "comun",
                        principalTable: "Ubigeos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Vendedores_VendedorCode",
                        column: x => x.VendedorCode,
                        principalSchema: "facturacion",
                        principalTable: "Vendedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HojasCorte",
                schema: "facturacion",
                columns: table => new
                {
                    Year = table.Column<string>(type: "char(4)", nullable: false),
                    Code = table.Column<string>(type: "char(8)", nullable: false),
                    TipoHojaCorteCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    ClienteCode = table.Column<string>(type: "char(5)", maxLength: 5, nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ObraCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: true),
                    WorkName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    WorkAddress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    WorkAddressUbigeoCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: false),
                    TecnicoCode = table.Column<string>(type: "char(5)", maxLength: 5, nullable: false),
                    VendedorCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: false),
                    Sector = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Floor = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    ViguetaDispatchDate = table.Column<DateTime>(type: "date", nullable: true),
                    BovedillaDispatchDate = table.Column<DateTime>(type: "date", nullable: true),
                    ClientViguetaDispatchDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClientBovedillaDispatchDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StorageDate = table.Column<DateTime>(type: "date", nullable: true),
                    OrdenTrabajoCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsScheduled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ParentYear = table.Column<string>(type: "char(4)", nullable: true),
                    ParentCode = table.Column<string>(type: "char(8)", nullable: true),
                    Turn = table.Column<int>(type: "int", nullable: false),
                    IsModifiable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Motivo = table.Column<int>(type: "int", nullable: true),
                    ProductType = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HojasCorte", x => new { x.Year, x.Code });
                    table.ForeignKey(
                        name: "FK_HojasCorte_Clientes_ClienteCode",
                        column: x => x.ClienteCode,
                        principalSchema: "facturacion",
                        principalTable: "Clientes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HojasCorte_HojasCorte_ParentYear_ParentCode",
                        columns: x => new { x.ParentYear, x.ParentCode },
                        principalSchema: "facturacion",
                        principalTable: "HojasCorte",
                        principalColumns: new[] { "Year", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HojasCorte_Obras_ClienteCode_ObraCode",
                        columns: x => new { x.ClienteCode, x.ObraCode },
                        principalSchema: "facturacion",
                        principalTable: "Obras",
                        principalColumns: new[] { "ClienteCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HojasCorte_Tecnicos_TecnicoCode",
                        column: x => x.TecnicoCode,
                        principalSchema: "facturacion",
                        principalTable: "Tecnicos",
                        principalColumn: "TrabajadorCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HojasCorte_TiposHojaCorte_TipoHojaCorteCode",
                        column: x => x.TipoHojaCorteCode,
                        principalSchema: "facturacion",
                        principalTable: "TiposHojaCorte",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HojasCorte_Ubigeos_WorkAddressUbigeoCode",
                        column: x => x.WorkAddressUbigeoCode,
                        principalSchema: "comun",
                        principalTable: "Ubigeos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HojasCorte_Vendedores_VendedorCode",
                        column: x => x.VendedorCode,
                        principalSchema: "facturacion",
                        principalTable: "Vendedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GuiaDetalles",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    GuiaCode = table.Column<string>(type: "char(10)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuiaDetalles", x => new { x.PlantaCode, x.GuiaCode, x.ArticuloCode });
                    table.ForeignKey(
                        name: "FK_GuiaDetalles_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuiaDetalles_Guias_PlantaCode_GuiaCode",
                        columns: x => new { x.PlantaCode, x.GuiaCode },
                        principalSchema: "logistica",
                        principalTable: "Guias",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenServicioDetalles",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    OrdenServicioCode = table.Column<string>(type: "char(6)", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    Glosa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cantidad = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    TipoValeCode = table.Column<string>(type: "char(3)", nullable: false),
                    SubCentroCostoCode = table.Column<string>(type: "char(6)", nullable: false),
                    SubCentroEjecutorCode = table.Column<string>(type: "char(4)", nullable: true),
                    OrdenTrabajoCode = table.Column<string>(type: "char(6)", nullable: true),
                    PlantaAfectadaCode = table.Column<string>(type: "char(2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenServicioDetalles", x => new { x.PlantaCode, x.OrdenServicioCode, x.ItemNumber });
                    table.ForeignKey(
                        name: "FK_OrdenServicioDetalles_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenServicioDetalles_OrdenesServicio_PlantaCode_OrdenServicioCode",
                        columns: x => new { x.PlantaCode, x.OrdenServicioCode },
                        principalSchema: "logistica",
                        principalTable: "OrdenesServicio",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenServicioDetalles_OrdenesTrabajo_PlantaCode_OrdenTrabajoCode",
                        columns: x => new { x.PlantaCode, x.OrdenTrabajoCode },
                        principalSchema: "mantenimiento",
                        principalTable: "OrdenesTrabajo",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenServicioDetalles_Plantas_PlantaAfectadaCode",
                        column: x => x.PlantaAfectadaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenServicioDetalles_SubCentrosCosto_SubCentroCostoCode",
                        column: x => x.SubCentroCostoCode,
                        principalSchema: "logistica",
                        principalTable: "SubCentrosCosto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenServicioDetalles_SubCentrosEjecutores_SubCentroEjecutorCode",
                        column: x => x.SubCentroEjecutorCode,
                        principalSchema: "mantenimiento",
                        principalTable: "SubCentrosEjecutores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenServicioDetalles_TiposVale_TipoValeCode",
                        column: x => x.TipoValeCode,
                        principalSchema: "logistica",
                        principalTable: "TiposVale",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenServicioSalidaDetalles",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    SalidaCode = table.Column<string>(type: "char(6)", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    Glosa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cantidad = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    AsientoContable = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    Propiedad01 = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    TipoValeCode = table.Column<string>(type: "char(3)", nullable: false),
                    SubCentroCostoCode = table.Column<string>(type: "char(6)", nullable: false),
                    SubCentroEjecutorCode = table.Column<string>(type: "char(4)", nullable: true),
                    OrdenTrabajoCode = table.Column<string>(type: "char(6)", nullable: true),
                    PlantaAfectadaCode = table.Column<string>(type: "char(2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenServicioSalidaDetalles", x => new { x.PlantaCode, x.SalidaCode, x.ItemNumber });
                    table.ForeignKey(
                        name: "FK_OrdenServicioSalidaDetalles_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenServicioSalidaDetalles_OrdenesServicioSalida_PlantaCode_SalidaCode",
                        columns: x => new { x.PlantaCode, x.SalidaCode },
                        principalSchema: "logistica",
                        principalTable: "OrdenesServicioSalida",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenServicioSalidaDetalles_OrdenesTrabajo_PlantaCode_OrdenTrabajoCode",
                        columns: x => new { x.PlantaCode, x.OrdenTrabajoCode },
                        principalSchema: "mantenimiento",
                        principalTable: "OrdenesTrabajo",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenServicioSalidaDetalles_Plantas_PlantaAfectadaCode",
                        column: x => x.PlantaAfectadaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenServicioSalidaDetalles_SubCentrosCosto_SubCentroCostoCode",
                        column: x => x.SubCentroCostoCode,
                        principalSchema: "logistica",
                        principalTable: "SubCentrosCosto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenServicioSalidaDetalles_SubCentrosEjecutores_SubCentroEjecutorCode",
                        column: x => x.SubCentroEjecutorCode,
                        principalSchema: "mantenimiento",
                        principalTable: "SubCentrosEjecutores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenServicioSalidaDetalles_TiposVale_TipoValeCode",
                        column: x => x.TipoValeCode,
                        principalSchema: "logistica",
                        principalTable: "TiposVale",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OTResponsables",
                schema: "mantenimiento",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    OrdenTrabajoCode = table.Column<string>(type: "char(6)", nullable: false),
                    FechaProceso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    TiempoProceso = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    Basico = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CostoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTResponsables", x => new { x.PlantaCode, x.OrdenTrabajoCode, x.FechaProceso, x.TrabajadorCode });
                    table.ForeignKey(
                        name: "FK_OTResponsables_OrdenesTrabajo_PlantaCode_OrdenTrabajoCode",
                        columns: x => new { x.PlantaCode, x.OrdenTrabajoCode },
                        principalSchema: "mantenimiento",
                        principalTable: "OrdenesTrabajo",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OTResponsables_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OTRMaquinas",
                schema: "mantenimiento",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    OrdenTrabajoCode = table.Column<string>(type: "char(6)", nullable: false),
                    MaquinaCode = table.Column<string>(type: "char(4)", maxLength: 4, nullable: false),
                    FechaProceso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    Horas = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTRMaquinas", x => new { x.PlantaCode, x.OrdenTrabajoCode, x.MaquinaCode });
                    table.ForeignKey(
                        name: "FK_OTRMaquinas_Maquinas_MaquinaCode",
                        column: x => x.MaquinaCode,
                        principalSchema: "mantenimiento",
                        principalTable: "Maquinas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OTRMaquinas_OrdenesTrabajo_PlantaCode_OrdenTrabajoCode",
                        columns: x => new { x.PlantaCode, x.OrdenTrabajoCode },
                        principalSchema: "mantenimiento",
                        principalTable: "OrdenesTrabajo",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OTRMateriales",
                schema: "mantenimiento",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    OrdenTrabajoCode = table.Column<string>(type: "char(6)", nullable: false),
                    FechaProceso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", maxLength: 7, nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    CostoUnitario = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    CostoTotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    EstadoCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTRMateriales", x => new { x.PlantaCode, x.OrdenTrabajoCode, x.FechaProceso, x.ArticuloCode });
                    table.ForeignKey(
                        name: "FK_OTRMateriales_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OTRMateriales_OrdenesTrabajo_PlantaCode_OrdenTrabajoCode",
                        columns: x => new { x.PlantaCode, x.OrdenTrabajoCode },
                        principalSchema: "mantenimiento",
                        principalTable: "OrdenesTrabajo",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    CodPlanta = table.Column<string>(type: "char(2)", nullable: false),
                    TipoPedidoCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    TipoValeCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    TramiteCode = table.Column<string>(type: "char(1)", maxLength: 1, nullable: false),
                    SubCentroCostoCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: false),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    OrdenTrabajoCode = table.Column<string>(type: "char(6)", nullable: true),
                    UnidadNegocioCode = table.Column<string>(type: "char(6)", maxLength: 6, nullable: false, defaultValue: "000000"),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NetoPedido = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    IgvPedido = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TotalPedido = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    EstadoPedido = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, defaultValue: ""),
                    AprobadoPor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompradoPor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => new { x.PlantaCode, x.Code });
                    table.ForeignKey(
                        name: "FK_Pedidos_OrdenesTrabajo_PlantaCode_OrdenTrabajoCode",
                        columns: x => new { x.PlantaCode, x.OrdenTrabajoCode },
                        principalSchema: "mantenimiento",
                        principalTable: "OrdenesTrabajo",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_SubCentrosCosto_SubCentroCostoCode",
                        column: x => x.SubCentroCostoCode,
                        principalSchema: "logistica",
                        principalTable: "SubCentrosCosto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_TiposPedido_TipoPedidoCode",
                        column: x => x.TipoPedidoCode,
                        principalSchema: "logistica",
                        principalTable: "TiposPedido",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_TiposVale_TipoValeCode",
                        column: x => x.TipoValeCode,
                        principalSchema: "logistica",
                        principalTable: "TiposVale",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_Tramites_TramiteCode",
                        column: x => x.TramiteCode,
                        principalSchema: "logistica",
                        principalTable: "Tramites",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_UnidadesNegocio_UnidadNegocioCode",
                        column: x => x.UnidadNegocioCode,
                        principalSchema: "logistica",
                        principalTable: "UnidadesNegocio",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vales",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(7)", nullable: false),
                    TipoValeCode = table.Column<string>(type: "char(3)", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubCentroCostoCode = table.Column<string>(type: "char(6)", nullable: false),
                    SubCentroEjecutorCode = table.Column<string>(type: "char(4)", nullable: true),
                    TrabajadorCode = table.Column<string>(type: "char(5)", nullable: false),
                    OrdenTrabajoCode = table.Column<string>(type: "char(6)", nullable: true),
                    UnidadNegocioCode = table.Column<string>(type: "char(6)", nullable: false),
                    PlantaAfectadaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Neto = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Igv = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    AsientoContable = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    AprobadoPor = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcesadoPor = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FechaProcesado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnuladoPor = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FechaAnulacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Preparado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vales", x => new { x.PlantaCode, x.Code });
                    table.ForeignKey(
                        name: "FK_Vales_OrdenesTrabajo_PlantaCode_OrdenTrabajoCode",
                        columns: x => new { x.PlantaCode, x.OrdenTrabajoCode },
                        principalSchema: "mantenimiento",
                        principalTable: "OrdenesTrabajo",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vales_Plantas_PlantaAfectadaCode",
                        column: x => x.PlantaAfectadaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vales_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vales_SubCentrosCosto_SubCentroCostoCode",
                        column: x => x.SubCentroCostoCode,
                        principalSchema: "logistica",
                        principalTable: "SubCentrosCosto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vales_SubCentrosEjecutores_SubCentroEjecutorCode",
                        column: x => x.SubCentroEjecutorCode,
                        principalSchema: "mantenimiento",
                        principalTable: "SubCentrosEjecutores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vales_TiposVale_TipoValeCode",
                        column: x => x.TipoValeCode,
                        principalSchema: "logistica",
                        principalTable: "TiposVale",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vales_Trabajadores_TrabajadorCode",
                        column: x => x.TrabajadorCode,
                        principalSchema: "rrhh",
                        principalTable: "Trabajadores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vales_UnidadesNegocio_UnidadNegocioCode",
                        column: x => x.UnidadNegocioCode,
                        principalSchema: "logistica",
                        principalTable: "UnidadesNegocio",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CotizacionesDetalle",
                schema: "facturacion",
                columns: table => new
                {
                    NegocioCode = table.Column<string>(type: "char(2)", nullable: false),
                    Year = table.Column<string>(type: "char(4)", nullable: false),
                    Month = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(8)", nullable: false),
                    Item = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoTipoCode = table.Column<string>(type: "char(2)", nullable: false),
                    ProductoCode = table.Column<string>(type: "char(4)", nullable: false),
                    UnitCode = table.Column<string>(type: "char(2)", maxLength: 5, nullable: false, defaultValue: ""),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    Observations = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    DeliveredQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotizacionesDetalle", x => new { x.NegocioCode, x.Year, x.Month, x.Code, x.Item });
                    table.ForeignKey(
                        name: "FK_CotizacionesDetalle_Cotizaciones_NegocioCode_Year_Month_Code",
                        columns: x => new { x.NegocioCode, x.Year, x.Month, x.Code },
                        principalSchema: "facturacion",
                        principalTable: "Cotizaciones",
                        principalColumns: new[] { "NegocioCode", "Year", "Month", "Code" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CotizacionesDetalle_Productos_ProductoTipoCode_ProductoCode",
                        columns: x => new { x.ProductoTipoCode, x.ProductoCode },
                        principalSchema: "facturacion",
                        principalTable: "Productos",
                        principalColumns: new[] { "TipoProductoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CotizacionesDetalle_UnidadesMedidaVenta_UnitCode",
                        column: x => x.UnitCode,
                        principalSchema: "facturacion",
                        principalTable: "UnidadesMedidaVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CotizacionesMetradoResumen",
                schema: "facturacion",
                columns: table => new
                {
                    NegocioCode = table.Column<string>(type: "char(2)", nullable: false),
                    Year = table.Column<string>(type: "char(4)", nullable: false),
                    Month = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(8)", nullable: false),
                    LevelNumber = table.Column<int>(type: "int", nullable: false),
                    LevelName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AlturaLosaCode = table.Column<string>(type: "char(2)", nullable: false),
                    OverloadOrShortage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LinealMeters = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TotalVaults = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TotalMeters = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    PricePerM2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Quantity = table.Column<decimal>(type: "decimal(18,0)", nullable: false, defaultValue: 0m),
                    BuildingLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HasTransport = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TotalVaultsAlt = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TotalMetersAlt = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TotalPriceAlt = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    PricePerM2Alt = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    HasMinPrice = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MinTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    MinTotalAlt = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    MinTotalB = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    MinTotalAltB = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    HasTransportB = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    HasMinPriceB = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotizacionesMetradoResumen", x => new { x.NegocioCode, x.Year, x.Month, x.Code, x.LevelNumber });
                    table.ForeignKey(
                        name: "FK_CotizacionesMetradoResumen_AlturasLosa_AlturaLosaCode",
                        column: x => x.AlturaLosaCode,
                        principalSchema: "facturacion",
                        principalTable: "AlturasLosa",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CotizacionesMetradoResumen_Cotizaciones_NegocioCode_Year_Month_Code",
                        columns: x => new { x.NegocioCode, x.Year, x.Month, x.Code },
                        principalSchema: "facturacion",
                        principalTable: "Cotizaciones",
                        principalColumns: new[] { "NegocioCode", "Year", "Month", "Code" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CotizacionesNotas",
                schema: "facturacion",
                columns: table => new
                {
                    NegocioCode = table.Column<string>(type: "char(2)", nullable: false),
                    Year = table.Column<string>(type: "char(4)", nullable: false),
                    Month = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(8)", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Option = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotizacionesNotas", x => new { x.NegocioCode, x.Year, x.Month, x.Code, x.Sequence });
                    table.ForeignKey(
                        name: "FK_CotizacionesNotas_Cotizaciones_NegocioCode_Year_Month_Code",
                        columns: x => new { x.NegocioCode, x.Year, x.Month, x.Code },
                        principalSchema: "facturacion",
                        principalTable: "Cotizaciones",
                        principalColumns: new[] { "NegocioCode", "Year", "Month", "Code" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HojaCorteDetalles",
                schema: "facturacion",
                columns: table => new
                {
                    Year = table.Column<string>(type: "char(4)", nullable: false),
                    HojaCorteCode = table.Column<string>(type: "char(8)", nullable: false),
                    ProductoTipoCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    ProductoCode = table.Column<string>(type: "char(4)", maxLength: 8, nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    UnitCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Panel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CablesSide1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CablesSide2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Lot1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Lot2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Lot3 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HojaCorteDetalles", x => new { x.Year, x.HojaCorteCode, x.ProductoTipoCode, x.ProductoCode, x.ItemNumber });
                    table.ForeignKey(
                        name: "FK_HojaCorteDetalles_HojasCorte_Year_HojaCorteCode",
                        columns: x => new { x.Year, x.HojaCorteCode },
                        principalSchema: "facturacion",
                        principalTable: "HojasCorte",
                        principalColumns: new[] { "Year", "Code" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HojaCorteDetalles_Productos_ProductoTipoCode_ProductoCode",
                        columns: x => new { x.ProductoTipoCode, x.ProductoCode },
                        principalSchema: "facturacion",
                        principalTable: "Productos",
                        principalColumns: new[] { "TipoProductoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HojaCorteDetalles_UnidadesMedidaVenta_UnitCode",
                        column: x => x.UnitCode,
                        principalSchema: "facturacion",
                        principalTable: "UnidadesMedidaVenta",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PedidoDetalles",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    PedidoCode = table.Column<string>(type: "char(6)", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    ArticuloCode = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    DescripcionArticulo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    UnidadMedidaCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false, defaultValue: "UN"),
                    PrecioArticulo = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    CantidadArticulo = table.Column<decimal>(type: "decimal(12,5)", nullable: false, defaultValue: 0m),
                    TotalArticulo = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    EstadoPedidoDetalle = table.Column<int>(type: "int", nullable: false),
                    OrdenCompraCode = table.Column<string>(type: "char(6)", nullable: true),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: true),
                    CantidadCotizada = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CantidadEnCompra = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoDetalles", x => new { x.PlantaCode, x.PedidoCode, x.ItemNumber });
                    table.ForeignKey(
                        name: "FK_PedidoDetalles_Pedidos_PlantaCode_PedidoCode",
                        columns: x => new { x.PlantaCode, x.PedidoCode },
                        principalSchema: "logistica",
                        principalTable: "Pedidos",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoDetalles_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidoDetalles_UnidadesMedida_UnidadMedidaCode",
                        column: x => x.UnidadMedidaCode,
                        principalSchema: "logistica",
                        principalTable: "UnidadesMedida",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotaIngresos",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(6)", nullable: false),
                    Condicion = table.Column<int>(type: "int", nullable: false),
                    Origen = table.Column<int>(type: "int", nullable: false),
                    OrdenCompraCode = table.Column<string>(type: "char(6)", nullable: true),
                    ImportacionCode = table.Column<string>(type: "char(6)", nullable: true),
                    ComprobantePagoCode = table.Column<string>(type: "char(2)", nullable: false),
                    MotivoDevolucionCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: true),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    NumeroGuia = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    NumeroReferencia = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ComprobantePagoReferenciaCode = table.Column<string>(type: "char(2)", fixedLength: true, nullable: true),
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    FormaPagoCode = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    FechaRecepcion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaProceso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaDocumento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TipoCambio = table.Column<decimal>(type: "decimal(12,4)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Igv = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    NoGravable = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Renta = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Fonavi = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Servicio = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    IgvExterior = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    AsientoContable = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    CodigoVale = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    PlantaOrigenCode = table.Column<string>(type: "char(2)", nullable: true),
                    ValeTransferenciaCode = table.Column<string>(type: "char(7)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotaIngresos", x => new { x.PlantaCode, x.Code });
                    table.ForeignKey(
                        name: "FK_NotaIngresos_ComprobantesPago_ComprobantePagoCode",
                        column: x => x.ComprobantePagoCode,
                        principalSchema: "comun",
                        principalTable: "ComprobantesPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotaIngresos_ComprobantesPago_ComprobantePagoReferenciaCode",
                        column: x => x.ComprobantePagoReferenciaCode,
                        principalSchema: "comun",
                        principalTable: "ComprobantesPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotaIngresos_FormasPago_FormaPagoCode",
                        column: x => x.FormaPagoCode,
                        principalSchema: "logistica",
                        principalTable: "FormasPago",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotaIngresos_Importaciones_PlantaCode_ImportacionCode",
                        columns: x => new { x.PlantaCode, x.ImportacionCode },
                        principalSchema: "logistica",
                        principalTable: "Importaciones",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotaIngresos_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotaIngresos_MotivosDevolucionArticulo_MotivoDevolucionCode",
                        column: x => x.MotivoDevolucionCode,
                        principalSchema: "logistica",
                        principalTable: "MotivosDevolucionArticulo",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotaIngresos_OrdenesCompra_PlantaCode_OrdenCompraCode",
                        columns: x => new { x.PlantaCode, x.OrdenCompraCode },
                        principalSchema: "logistica",
                        principalTable: "OrdenesCompra",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotaIngresos_Plantas_PlantaCode",
                        column: x => x.PlantaCode,
                        principalSchema: "comun",
                        principalTable: "Plantas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotaIngresos_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotaIngresos_Vales_PlantaOrigenCode_ValeTransferenciaCode",
                        columns: x => new { x.PlantaOrigenCode, x.ValeTransferenciaCode },
                        principalSchema: "logistica",
                        principalTable: "Vales",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ValeDetalles",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    ValeCode = table.Column<string>(type: "char(7)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(12,4)", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    AsientoContable = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    Propiedad01 = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    MaterialOtFechaProceso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValeDetalles", x => new { x.PlantaCode, x.ValeCode, x.ArticuloCode });
                    table.ForeignKey(
                        name: "FK_ValeDetalles_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ValeDetalles_Vales_PlantaCode_ValeCode",
                        columns: x => new { x.PlantaCode, x.ValeCode },
                        principalSchema: "logistica",
                        principalTable: "Vales",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CotizacionesMetradoDetalle",
                schema: "facturacion",
                columns: table => new
                {
                    NegocioCode = table.Column<string>(type: "char(2)", nullable: false),
                    Year = table.Column<string>(type: "char(4)", nullable: false),
                    Month = table.Column<string>(type: "char(2)", nullable: false),
                    Code = table.Column<string>(type: "char(8)", nullable: false),
                    LevelNumber = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<string>(type: "char(3)", nullable: false),
                    ProductoTipoCode = table.Column<string>(type: "char(2)", nullable: false),
                    ProductoCode = table.Column<string>(type: "char(4)", nullable: false),
                    PanelCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false, defaultValue: ""),
                    Times = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    InnerLength = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    OuterLength = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    Support = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    TotalMaterial = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    VaultCount = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    WastePercentage = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    Row = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    QuantityB = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    Support2 = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    Width = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    Area = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    MaterialPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    MaterialIgv = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TransportPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    VaultPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    VaultTotalPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    MaterialPriceAlt = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    TransportPriceAlt = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    VaultPriceAlt = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    VaultTotalPriceAlt = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    SortOrder = table.Column<string>(type: "char(3)", nullable: false, defaultValue: "00"),
                    DeliveredTotalMaterial = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    DeliveredQuantityB = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    PolystyrenePrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    PolystyreneTotalPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    PolystyrenePriceAlt = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    PolystyreneTotalPriceAlt = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    Widening = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    SupportP = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    WastePercentageP = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    QuantityP = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    Anchorage = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Spacing = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotizacionesMetradoDetalle", x => new { x.NegocioCode, x.Year, x.Month, x.Code, x.LevelNumber, x.Order, x.ProductoTipoCode, x.ProductoCode });
                    table.ForeignKey(
                        name: "FK_CotizacionesMetradoDetalle_CotizacionesMetradoResumen_NegocioCode_Year_Month_Code_LevelNumber",
                        columns: x => new { x.NegocioCode, x.Year, x.Month, x.Code, x.LevelNumber },
                        principalSchema: "facturacion",
                        principalTable: "CotizacionesMetradoResumen",
                        principalColumns: new[] { "NegocioCode", "Year", "Month", "Code", "LevelNumber" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CotizacionesMetradoDetalle_Productos_ProductoTipoCode_ProductoCode",
                        columns: x => new { x.ProductoTipoCode, x.ProductoCode },
                        principalSchema: "facturacion",
                        principalTable: "Productos",
                        principalColumns: new[] { "TipoProductoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CotizacionPedidoOrigenes",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    CotizacionCode = table.Column<string>(type: "char(6)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    PedidoCode = table.Column<string>(type: "char(6)", nullable: false),
                    PedidoItemNumber = table.Column<int>(type: "int", nullable: false),
                    CantidadTomada = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotizacionPedidoOrigenes", x => new { x.PlantaCode, x.CotizacionCode, x.ArticuloCode, x.PedidoCode, x.PedidoItemNumber });
                    table.ForeignKey(
                        name: "FK_CotizacionPedidoOrigenes_CotizacionDetalles_PlantaCode_CotizacionCode_ArticuloCode",
                        columns: x => new { x.PlantaCode, x.CotizacionCode, x.ArticuloCode },
                        principalSchema: "logistica",
                        principalTable: "CotizacionDetalles",
                        principalColumns: new[] { "PlantaCode", "CotizacionCode", "ArticuloCode" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CotizacionPedidoOrigenes_PedidoDetalles_PlantaCode_PedidoCode_PedidoItemNumber",
                        columns: x => new { x.PlantaCode, x.PedidoCode, x.PedidoItemNumber },
                        principalSchema: "logistica",
                        principalTable: "PedidoDetalles",
                        principalColumns: new[] { "PlantaCode", "PedidoCode", "ItemNumber" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenCompraPedidoOrigenes",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    OrdenCompraCode = table.Column<string>(type: "char(6)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    PedidoCode = table.Column<string>(type: "char(6)", nullable: false),
                    PedidoItemNumber = table.Column<int>(type: "int", nullable: false),
                    CantidadTomada = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CantidadEntregada = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenCompraPedidoOrigenes", x => new { x.PlantaCode, x.OrdenCompraCode, x.ArticuloCode, x.PedidoCode, x.PedidoItemNumber });
                    table.ForeignKey(
                        name: "FK_OrdenCompraPedidoOrigenes_OrdenCompraDetalles_PlantaCode_OrdenCompraCode_ArticuloCode",
                        columns: x => new { x.PlantaCode, x.OrdenCompraCode, x.ArticuloCode },
                        principalSchema: "logistica",
                        principalTable: "OrdenCompraDetalles",
                        principalColumns: new[] { "PlantaCode", "OrdenCompraCode", "ArticuloCode" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenCompraPedidoOrigenes_PedidoDetalles_PlantaCode_PedidoCode_PedidoItemNumber",
                        columns: x => new { x.PlantaCode, x.PedidoCode, x.PedidoItemNumber },
                        principalSchema: "logistica",
                        principalTable: "PedidoDetalles",
                        principalColumns: new[] { "PlantaCode", "PedidoCode", "ItemNumber" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotaIngresoDetalles",
                schema: "logistica",
                columns: table => new
                {
                    PlantaCode = table.Column<string>(type: "char(2)", nullable: false),
                    NotaIngresoCode = table.Column<string>(type: "char(6)", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
                    PedidoCode = table.Column<string>(type: "char(6)", nullable: true),
                    GuiaCode = table.Column<string>(type: "char(6)", nullable: true),
                    Cantidad = table.Column<decimal>(type: "decimal(12,4)", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(12,6)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotaIngresoDetalles", x => new { x.PlantaCode, x.NotaIngresoCode, x.ItemNumber });
                    table.ForeignKey(
                        name: "FK_NotaIngresoDetalles_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotaIngresoDetalles_NotaIngresos_PlantaCode_NotaIngresoCode",
                        columns: x => new { x.PlantaCode, x.NotaIngresoCode },
                        principalSchema: "logistica",
                        principalTable: "NotaIngresos",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotaIngresoDetalles_Pedidos_PlantaCode_PedidoCode",
                        columns: x => new { x.PlantaCode, x.PedidoCode },
                        principalSchema: "logistica",
                        principalTable: "Pedidos",
                        principalColumns: new[] { "PlantaCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actividades_ObjetoActividadCode",
                schema: "mantenimiento",
                table: "Actividades",
                column: "ObjetoActividadCode");

            migrationBuilder.CreateIndex(
                name: "IX_Actividades_VerboActividadCode",
                schema: "mantenimiento",
                table: "Actividades",
                column: "VerboActividadCode");

            migrationBuilder.CreateIndex(
                name: "IX_AjusteInventarioDetalles_ArticuloCode",
                schema: "logistica",
                table: "AjusteInventarioDetalles",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_AjusteInventarioDetalles_PlantaCode_AjusteCode_ItemNumber",
                schema: "logistica",
                table: "AjusteInventarioDetalles",
                columns: new[] { "PlantaCode", "AjusteCode", "ItemNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_AjusteInventarioDetalles_PlantaCode_ArticuloCode_Estado_Tipo",
                schema: "logistica",
                table: "AjusteInventarioDetalles",
                columns: new[] { "PlantaCode", "ArticuloCode", "Estado", "Tipo" });

            migrationBuilder.CreateIndex(
                name: "IX_AjustesInventario_PlantaCode_CreatedAt",
                schema: "logistica",
                table: "AjustesInventario",
                columns: new[] { "PlantaCode", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AjustesInventario_PlantaCode_Estado",
                schema: "logistica",
                table: "AjustesInventario",
                columns: new[] { "PlantaCode", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_AlturasLosa_Name",
                schema: "facturacion",
                table: "AlturasLosa",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlturasLosa_PolystyreneProductoTipoCode_PolystyreneProductoCode",
                schema: "facturacion",
                table: "AlturasLosa",
                columns: new[] { "PolystyreneProductoTipoCode", "PolystyreneProductoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_AlturasLosa_ProductoTipoCode_ProductoCode",
                schema: "facturacion",
                table: "AlturasLosa",
                columns: new[] { "ProductoTipoCode", "ProductoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalisisVentas_Name",
                schema: "facturacion",
                table: "AnalisisVentas",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnalisisVentas_SegmentoVentasCode",
                schema: "facturacion",
                table: "AnalisisVentas",
                column: "SegmentoVentasCode");

            migrationBuilder.CreateIndex(
                name: "IX_AprobadoresAsignados_TrabajadorCode",
                schema: "logistica",
                table: "AprobadoresAsignados",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloProveedor_ArticuloCode",
                schema: "logistica",
                table: "ArticuloProveedor",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloProveedor_ProveedorCode",
                schema: "logistica",
                table: "ArticuloProveedor",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_Articulos_AccountingAccountCode",
                schema: "logistica",
                table: "Articulos",
                column: "AccountingAccountCode");

            migrationBuilder.CreateIndex(
                name: "IX_Articulos_AccountingAttachmentTypeCode",
                schema: "logistica",
                table: "Articulos",
                column: "AccountingAttachmentTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Articulos_PlanCode",
                schema: "logistica",
                table: "Articulos",
                column: "PlanCode");

            migrationBuilder.CreateIndex(
                name: "IX_Articulos_PlantOriginCode",
                schema: "logistica",
                table: "Articulos",
                column: "PlantOriginCode");

            migrationBuilder.CreateIndex(
                name: "IX_Articulos_SubFamiliaCode",
                schema: "logistica",
                table: "Articulos",
                column: "SubFamiliaCode");

            migrationBuilder.CreateIndex(
                name: "IX_Articulos_TipoArticuloCode",
                schema: "logistica",
                table: "Articulos",
                column: "TipoArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_Articulos_UnidadMedidaCode",
                schema: "logistica",
                table: "Articulos",
                column: "UnidadMedidaCode");

            migrationBuilder.CreateIndex(
                name: "IX_AtributosConcreto_AttributeType",
                schema: "facturacion",
                table: "AtributosConcreto",
                column: "AttributeType");

            migrationBuilder.CreateIndex(
                name: "IX_CentrosCosto_PlantaCode",
                schema: "logistica",
                table: "CentrosCosto",
                column: "PlantaCode");

            migrationBuilder.CreateIndex(
                name: "IX_CierreIncidencias_ArticuloCode",
                schema: "logistica",
                table: "CierreIncidencias",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_CierreIncidencias_Periodo",
                schema: "logistica",
                table: "CierreIncidencias",
                columns: new[] { "PlantaCode", "PeriodCode", "Tipo" });

            migrationBuilder.CreateIndex(
                name: "IX_CierreRevaluaciones_Periodo",
                schema: "logistica",
                table: "CierreRevaluaciones",
                columns: new[] { "PlantaCode", "PeriodCode" });

            migrationBuilder.CreateIndex(
                name: "IX_ClasificacionesCliente_Name",
                schema: "facturacion",
                table: "ClasificacionesCliente",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_ClasificacionClienteCode",
                schema: "facturacion",
                table: "Clientes",
                column: "ClasificacionClienteCode");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CurrencyCode",
                schema: "facturacion",
                table: "Clientes",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_DocumentNumber",
                schema: "facturacion",
                table: "Clientes",
                column: "DocumentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_DocumentTypeCode",
                schema: "facturacion",
                table: "Clientes",
                column: "DocumentTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Estado",
                schema: "facturacion",
                table: "Clientes",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_FormaPagoVentaCode",
                schema: "facturacion",
                table: "Clientes",
                column: "FormaPagoVentaCode");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Name",
                schema: "facturacion",
                table: "Clientes",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_ParentClientCode",
                schema: "facturacion",
                table: "Clientes",
                column: "ParentClientCode");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_TipoClienteCode",
                schema: "facturacion",
                table: "Clientes",
                column: "TipoClienteCode");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_UbigeoCode",
                schema: "facturacion",
                table: "Clientes",
                column: "UbigeoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Cobradores_UserId",
                schema: "facturacion",
                table: "Cobradores",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesPago_Code",
                schema: "comun",
                table: "ComprobantesPago",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesPago_SunatCode",
                schema: "comun",
                table: "ComprobantesPago",
                column: "SunatCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conductores_DocumentTypeCode_DocumentNumber",
                schema: "logistica",
                table: "Conductores",
                columns: new[] { "DocumentTypeCode", "DocumentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conductores_DriverLicenseNumber",
                schema: "logistica",
                table: "Conductores",
                column: "DriverLicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionDetalles_ArticuloCode",
                schema: "logistica",
                table: "CotizacionDetalles",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_ClientAddressUbigeoCode",
                schema: "facturacion",
                table: "Cotizaciones",
                column: "ClientAddressUbigeoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_ClienteCode",
                schema: "facturacion",
                table: "Cotizaciones",
                column: "ClienteCode");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_ClienteCode_ObraCode",
                schema: "facturacion",
                table: "Cotizaciones",
                columns: new[] { "ClienteCode", "ObraCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_Date",
                schema: "facturacion",
                table: "Cotizaciones",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_FleteCode",
                schema: "facturacion",
                table: "Cotizaciones",
                column: "FleteCode");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_FormaPagoVentaCode",
                schema: "facturacion",
                table: "Cotizaciones",
                column: "FormaPagoVentaCode");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_OriginNegocioCode_OriginYear_OriginMonth_OriginCode",
                schema: "facturacion",
                table: "Cotizaciones",
                columns: new[] { "OriginNegocioCode", "OriginYear", "OriginMonth", "OriginCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_Status",
                schema: "facturacion",
                table: "Cotizaciones",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_TecnicoCode",
                schema: "facturacion",
                table: "Cotizaciones",
                column: "TecnicoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_VendedorCode",
                schema: "facturacion",
                table: "Cotizaciones",
                column: "VendedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_WorkAddressUbigeoCode",
                schema: "facturacion",
                table: "Cotizaciones",
                column: "WorkAddressUbigeoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_PlantaCode_OriginalCode",
                schema: "logistica",
                table: "Cotizaciones",
                columns: new[] { "PlantaCode", "OriginalCode" });

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionesDetalle_ProductoTipoCode_ProductoCode",
                schema: "facturacion",
                table: "CotizacionesDetalle",
                columns: new[] { "ProductoTipoCode", "ProductoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionesDetalle_UnitCode",
                schema: "facturacion",
                table: "CotizacionesDetalle",
                column: "UnitCode");

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionesMetradoDetalle_ProductoTipoCode_ProductoCode",
                schema: "facturacion",
                table: "CotizacionesMetradoDetalle",
                columns: new[] { "ProductoTipoCode", "ProductoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionesMetradoResumen_AlturaLosaCode",
                schema: "facturacion",
                table: "CotizacionesMetradoResumen",
                column: "AlturaLosaCode");

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionPedidoOrigenes_PlantaCode_PedidoCode_PedidoItemNumber",
                schema: "logistica",
                table: "CotizacionPedidoOrigenes",
                columns: new[] { "PlantaCode", "PedidoCode", "PedidoItemNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionProveedorDetalles_ArticuloCode",
                schema: "logistica",
                table: "CotizacionProveedorDetalles",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionProveedores_MonedaCode",
                schema: "logistica",
                table: "CotizacionProveedores",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionProveedores_ProveedorCode",
                schema: "logistica",
                table: "CotizacionProveedores",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasBancarias_AccountCode",
                schema: "contabilidad",
                table: "CuentasBancarias",
                column: "AccountCode");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasBancarias_CurrencyCode",
                schema: "contabilidad",
                table: "CuentasBancarias",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasContables_CurrencyCode",
                schema: "contabilidad",
                table: "CuentasContables",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasContables_TipoAnexoCode",
                schema: "contabilidad",
                table: "CuentasContables",
                column: "TipoAnexoCode");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasContables_TipoCuentaCode",
                schema: "contabilidad",
                table: "CuentasContables",
                column: "TipoCuentaCode");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoCompraDetalles_AnexoTipoCode_AnexoCode",
                schema: "contabilidad",
                table: "DocumentoCompraDetalles",
                columns: new[] { "AnexoTipoCode", "AnexoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoCompraDetalles_CuentaContableCode",
                schema: "contabilidad",
                table: "DocumentoCompraDetalles",
                column: "CuentaContableCode");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoCompraDetalles_PlantaCode",
                schema: "contabilidad",
                table: "DocumentoCompraDetalles",
                column: "PlantaCode");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoCompraDetalles_ProductoTipoCode_ProductoCode",
                schema: "contabilidad",
                table: "DocumentoCompraDetalles",
                columns: new[] { "ProductoTipoCode", "ProductoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoCompraDetalles_UnidadMedidaCode",
                schema: "contabilidad",
                table: "DocumentoCompraDetalles",
                column: "UnidadMedidaCode");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosCompra_DocumentoOrigenComprobantePagoCode_DocumentoOrigenNumero_ProveedorCode",
                schema: "contabilidad",
                table: "DocumentosCompra",
                columns: new[] { "DocumentoOrigenComprobantePagoCode", "DocumentoOrigenNumero", "ProveedorCode" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosCompra_Estado",
                schema: "contabilidad",
                table: "DocumentosCompra",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosCompra_FechaEmision",
                schema: "contabilidad",
                table: "DocumentosCompra",
                column: "FechaEmision");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosCompra_FormaPagoCode",
                schema: "contabilidad",
                table: "DocumentosCompra",
                column: "FormaPagoCode");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosCompra_MonedaCode",
                schema: "contabilidad",
                table: "DocumentosCompra",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosCompra_OrdenCompraAdministracionCode",
                schema: "contabilidad",
                table: "DocumentosCompra",
                column: "OrdenCompraAdministracionCode");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosCompra_ProveedorCode",
                schema: "contabilidad",
                table: "DocumentosCompra",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosCompra_TipoBienCode",
                schema: "contabilidad",
                table: "DocumentosCompra",
                column: "TipoBienCode");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosCompra_TipoOperacionCode",
                schema: "contabilidad",
                table: "DocumentosCompra",
                column: "TipoOperacionCode");

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_SubCentroCostoCode",
                schema: "mantenimiento",
                table: "Equipos",
                column: "SubCentroCostoCode");

            migrationBuilder.CreateIndex(
                name: "IX_EstadosCiviles_Name",
                schema: "rrhh",
                table: "EstadosCiviles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fletes_Name",
                schema: "facturacion",
                table: "Fletes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormasPagoVenta_Name",
                schema: "facturacion",
                table: "FormasPagoVenta",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuiaDetalles_ArticuloCode",
                schema: "logistica",
                table: "GuiaDetalles",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_Guias_ConductorCode",
                schema: "logistica",
                table: "Guias",
                column: "ConductorCode");

            migrationBuilder.CreateIndex(
                name: "IX_Guias_MotivoCode",
                schema: "logistica",
                table: "Guias",
                column: "MotivoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Guias_PlantaCode_FechaEmision",
                schema: "logistica",
                table: "Guias",
                columns: new[] { "PlantaCode", "FechaEmision" });

            migrationBuilder.CreateIndex(
                name: "IX_Guias_ProveedorCode",
                schema: "logistica",
                table: "Guias",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_Guias_TransportistaCode",
                schema: "logistica",
                table: "Guias",
                column: "TransportistaCode");

            migrationBuilder.CreateIndex(
                name: "IX_Guias_VehiculoCode",
                schema: "logistica",
                table: "Guias",
                column: "VehiculoCode");

            migrationBuilder.CreateIndex(
                name: "IX_HojaCorteDetalles_ProductoTipoCode_ProductoCode",
                schema: "facturacion",
                table: "HojaCorteDetalles",
                columns: new[] { "ProductoTipoCode", "ProductoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_HojaCorteDetalles_UnitCode",
                schema: "facturacion",
                table: "HojaCorteDetalles",
                column: "UnitCode");

            migrationBuilder.CreateIndex(
                name: "IX_HojasCorte_ClienteCode",
                schema: "facturacion",
                table: "HojasCorte",
                column: "ClienteCode");

            migrationBuilder.CreateIndex(
                name: "IX_HojasCorte_ClienteCode_ObraCode",
                schema: "facturacion",
                table: "HojasCorte",
                columns: new[] { "ClienteCode", "ObraCode" });

            migrationBuilder.CreateIndex(
                name: "IX_HojasCorte_Date",
                schema: "facturacion",
                table: "HojasCorte",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_HojasCorte_OrdenTrabajoCode",
                schema: "facturacion",
                table: "HojasCorte",
                column: "OrdenTrabajoCode");

            migrationBuilder.CreateIndex(
                name: "IX_HojasCorte_ParentYear_ParentCode",
                schema: "facturacion",
                table: "HojasCorte",
                columns: new[] { "ParentYear", "ParentCode" });

            migrationBuilder.CreateIndex(
                name: "IX_HojasCorte_Status",
                schema: "facturacion",
                table: "HojasCorte",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_HojasCorte_TecnicoCode",
                schema: "facturacion",
                table: "HojasCorte",
                column: "TecnicoCode");

            migrationBuilder.CreateIndex(
                name: "IX_HojasCorte_TipoHojaCorteCode",
                schema: "facturacion",
                table: "HojasCorte",
                column: "TipoHojaCorteCode");

            migrationBuilder.CreateIndex(
                name: "IX_HojasCorte_VendedorCode",
                schema: "facturacion",
                table: "HojasCorte",
                column: "VendedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_HojasCorte_WorkAddressUbigeoCode",
                schema: "facturacion",
                table: "HojasCorte",
                column: "WorkAddressUbigeoCode");

            migrationBuilder.CreateIndex(
                name: "IX_ImportacionDetalles_ArticuloCode",
                schema: "logistica",
                table: "ImportacionDetalles",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_ImportacionDetalles_ComprobantePagoCode",
                schema: "logistica",
                table: "ImportacionDetalles",
                column: "ComprobantePagoCode");

            migrationBuilder.CreateIndex(
                name: "IX_ImportacionDetalles_PlantaCode_ImportacionCode_Estado",
                schema: "logistica",
                table: "ImportacionDetalles",
                columns: new[] { "PlantaCode", "ImportacionCode", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_ImportacionDetalles_ProveedorCode",
                schema: "logistica",
                table: "ImportacionDetalles",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_Importaciones_PlantaCode_CreatedAt",
                schema: "logistica",
                table: "Importaciones",
                columns: new[] { "PlantaCode", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Importaciones_PlantaCode_Estado",
                schema: "logistica",
                table: "Importaciones",
                columns: new[] { "PlantaCode", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_ImportacionGastoArticulos_ArticuloCode",
                schema: "logistica",
                table: "ImportacionGastoArticulos",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_ImportacionGastos_ComprobantePagoCode",
                schema: "logistica",
                table: "ImportacionGastos",
                column: "ComprobantePagoCode");

            migrationBuilder.CreateIndex(
                name: "IX_ImportacionGastos_MonedaCode",
                schema: "logistica",
                table: "ImportacionGastos",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_ImportacionGastos_ProveedorCode",
                schema: "logistica",
                table: "ImportacionGastos",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_ListasPrecios_TipoProductoCode_ProductoCode",
                schema: "facturacion",
                table: "ListasPrecios",
                columns: new[] { "TipoProductoCode", "ProductoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Modulos_Code",
                schema: "admin",
                table: "Modulos",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modulos_Name",
                schema: "admin",
                table: "Modulos",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Monedas_Code",
                schema: "comun",
                table: "Monedas",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Monedas_NumericCode",
                schema: "comun",
                table: "Monedas",
                column: "NumericCode",
                unique: true,
                filter: "[NumericCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MotivosDevolucion_Code",
                schema: "comun",
                table: "MotivosDevolucion",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MotivosDevolucionArticulo_Code",
                schema: "logistica",
                table: "MotivosDevolucionArticulo",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosArticulo_ArticuloCode",
                schema: "logistica",
                table: "MovimientosArticulo",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosArticulo_Documento",
                schema: "logistica",
                table: "MovimientosArticulo",
                columns: new[] { "PlantaCode", "Origen", "DocumentoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosArticulo_Kardex",
                schema: "logistica",
                table: "MovimientosArticulo",
                columns: new[] { "PlantaCode", "ArticuloCode", "FechaMovimiento", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosArticulo_Periodo",
                schema: "logistica",
                table: "MovimientosArticulo",
                columns: new[] { "PlantaCode", "PeriodCode", "ArticuloCode", "FechaMovimiento", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosArticulo_ReversaDeMovimientoId",
                schema: "logistica",
                table: "MovimientosArticulo",
                column: "ReversaDeMovimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Nacionalidades_Name",
                schema: "rrhh",
                table: "Nacionalidades",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotaIngresoDetalles_ArticuloCode",
                schema: "logistica",
                table: "NotaIngresoDetalles",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotaIngresoDetalles_PlantaCode_NotaIngresoCode_ArticuloCode_PedidoCode_GuiaCode",
                schema: "logistica",
                table: "NotaIngresoDetalles",
                columns: new[] { "PlantaCode", "NotaIngresoCode", "ArticuloCode", "PedidoCode", "GuiaCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotaIngresoDetalles_PlantaCode_PedidoCode",
                schema: "logistica",
                table: "NotaIngresoDetalles",
                columns: new[] { "PlantaCode", "PedidoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_NotaIngresos_ComprobantePagoCode_ProveedorCode_NumeroDocumento",
                schema: "logistica",
                table: "NotaIngresos",
                columns: new[] { "ComprobantePagoCode", "ProveedorCode", "NumeroDocumento" });

            migrationBuilder.CreateIndex(
                name: "IX_NotaIngresos_ComprobantePagoReferenciaCode",
                schema: "logistica",
                table: "NotaIngresos",
                column: "ComprobantePagoReferenciaCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotaIngresos_FormaPagoCode",
                schema: "logistica",
                table: "NotaIngresos",
                column: "FormaPagoCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotaIngresos_MonedaCode",
                schema: "logistica",
                table: "NotaIngresos",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotaIngresos_MotivoDevolucionCode",
                schema: "logistica",
                table: "NotaIngresos",
                column: "MotivoDevolucionCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotaIngresos_PlantaCode_FechaProceso",
                schema: "logistica",
                table: "NotaIngresos",
                columns: new[] { "PlantaCode", "FechaProceso" });

            migrationBuilder.CreateIndex(
                name: "IX_NotaIngresos_PlantaCode_ImportacionCode",
                schema: "logistica",
                table: "NotaIngresos",
                columns: new[] { "PlantaCode", "ImportacionCode" });

            migrationBuilder.CreateIndex(
                name: "IX_NotaIngresos_PlantaCode_OrdenCompraCode",
                schema: "logistica",
                table: "NotaIngresos",
                columns: new[] { "PlantaCode", "OrdenCompraCode" });

            migrationBuilder.CreateIndex(
                name: "IX_NotaIngresos_ProveedorCode",
                schema: "logistica",
                table: "NotaIngresos",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotaIngresos_ValeTransferencia",
                schema: "logistica",
                table: "NotaIngresos",
                columns: new[] { "PlantaOrigenCode", "ValeTransferenciaCode" });

            migrationBuilder.CreateIndex(
                name: "IX_NotasCotizacionPlantilla_Option",
                schema: "facturacion",
                table: "NotasCotizacionPlantilla",
                column: "Option");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_AnalisisVentaCode",
                schema: "facturacion",
                table: "Obras",
                column: "AnalisisVentaCode");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_BillingUbigeoCode",
                schema: "facturacion",
                table: "Obras",
                column: "BillingUbigeoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_CobradorCode",
                schema: "facturacion",
                table: "Obras",
                column: "CobradorCode");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_CreditCurrencyCode",
                schema: "facturacion",
                table: "Obras",
                column: "CreditCurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_DeliveryUbigeoCode",
                schema: "facturacion",
                table: "Obras",
                column: "DeliveryUbigeoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_Estado",
                schema: "facturacion",
                table: "Obras",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_FormaPagoVentaCode",
                schema: "facturacion",
                table: "Obras",
                column: "FormaPagoVentaCode");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_TipoValorizacionCode",
                schema: "facturacion",
                table: "Obras",
                column: "TipoValorizacionCode");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_UbigeoCode",
                schema: "facturacion",
                table: "Obras",
                column: "UbigeoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_VendedorCode",
                schema: "facturacion",
                table: "Obras",
                column: "VendedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompraAdministracionDetalles_SubCentroCostoCode",
                schema: "logistica",
                table: "OrdenCompraAdministracionDetalles",
                column: "SubCentroCostoCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompraAdministracionDetalles_UnidadMedidaCode",
                schema: "logistica",
                table: "OrdenCompraAdministracionDetalles",
                column: "UnidadMedidaCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompraDetalles_ArticuloCode",
                schema: "logistica",
                table: "OrdenCompraDetalles",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompraDetalles_SubCentroCostoCode",
                schema: "logistica",
                table: "OrdenCompraDetalles",
                column: "SubCentroCostoCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompraPedidoOrigenes_PlantaCode_PedidoCode_PedidoItemNumber",
                schema: "logistica",
                table: "OrdenCompraPedidoOrigenes",
                columns: new[] { "PlantaCode", "PedidoCode", "PedidoItemNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_CompradorCode",
                schema: "logistica",
                table: "OrdenesCompra",
                column: "CompradorCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_ComprobantePagoCode",
                schema: "logistica",
                table: "OrdenesCompra",
                column: "ComprobantePagoCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_FormaPagoCode",
                schema: "logistica",
                table: "OrdenesCompra",
                column: "FormaPagoCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_LugarEnvioCode",
                schema: "logistica",
                table: "OrdenesCompra",
                column: "LugarEnvioCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_MonedaCode",
                schema: "logistica",
                table: "OrdenesCompra",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_NotaCompraCode",
                schema: "logistica",
                table: "OrdenesCompra",
                column: "NotaCompraCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_ProveedorCode",
                schema: "logistica",
                table: "OrdenesCompra",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_TipoCompraCode",
                schema: "logistica",
                table: "OrdenesCompra",
                column: "TipoCompraCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_TramiteCode",
                schema: "logistica",
                table: "OrdenesCompra",
                column: "TramiteCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_UnidadNegocioCode",
                schema: "logistica",
                table: "OrdenesCompra",
                column: "UnidadNegocioCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraAdministracion_CompradorCode",
                schema: "logistica",
                table: "OrdenesCompraAdministracion",
                column: "CompradorCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraAdministracion_ComprobantePagoCode",
                schema: "logistica",
                table: "OrdenesCompraAdministracion",
                column: "ComprobantePagoCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraAdministracion_CreatedAt",
                schema: "logistica",
                table: "OrdenesCompraAdministracion",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraAdministracion_Estado",
                schema: "logistica",
                table: "OrdenesCompraAdministracion",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraAdministracion_FormaPagoCode",
                schema: "logistica",
                table: "OrdenesCompraAdministracion",
                column: "FormaPagoCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraAdministracion_LugarEnvioCode",
                schema: "logistica",
                table: "OrdenesCompraAdministracion",
                column: "LugarEnvioCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraAdministracion_MonedaCode",
                schema: "logistica",
                table: "OrdenesCompraAdministracion",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraAdministracion_NotaCompraCode",
                schema: "logistica",
                table: "OrdenesCompraAdministracion",
                column: "NotaCompraCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraAdministracion_ProveedorCode",
                schema: "logistica",
                table: "OrdenesCompraAdministracion",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraAdministracion_TramiteCode",
                schema: "logistica",
                table: "OrdenesCompraAdministracion",
                column: "TramiteCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraAdministracion_UnidadNegocioCode",
                schema: "logistica",
                table: "OrdenesCompraAdministracion",
                column: "UnidadNegocioCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_CompradorCode",
                schema: "logistica",
                table: "OrdenesServicio",
                column: "CompradorCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_ComprobantePagoCode_ProveedorCode_NumeroDocumento",
                schema: "logistica",
                table: "OrdenesServicio",
                columns: new[] { "ComprobantePagoCode", "ProveedorCode", "NumeroDocumento" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_FormaPagoCode",
                schema: "logistica",
                table: "OrdenesServicio",
                column: "FormaPagoCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_LugarEnvioCode",
                schema: "logistica",
                table: "OrdenesServicio",
                column: "LugarEnvioCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_MonedaCode",
                schema: "logistica",
                table: "OrdenesServicio",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_NotaCompraCode",
                schema: "logistica",
                table: "OrdenesServicio",
                column: "NotaCompraCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_PlantaCode_CreatedAt",
                schema: "logistica",
                table: "OrdenesServicio",
                columns: new[] { "PlantaCode", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_PlantaCode_ValeSalidaCode",
                schema: "logistica",
                table: "OrdenesServicio",
                columns: new[] { "PlantaCode", "ValeSalidaCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_ProveedorCode",
                schema: "logistica",
                table: "OrdenesServicio",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_TramiteCode",
                schema: "logistica",
                table: "OrdenesServicio",
                column: "TramiteCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_UnidadNegocioCode",
                schema: "logistica",
                table: "OrdenesServicio",
                column: "UnidadNegocioCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicioSalida_PlantaCode_CreatedAt",
                schema: "logistica",
                table: "OrdenesServicioSalida",
                columns: new[] { "PlantaCode", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicioSalida_TrabajadorCode",
                schema: "logistica",
                table: "OrdenesServicioSalida",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_ActividadCode",
                schema: "mantenimiento",
                table: "OrdenesTrabajo",
                column: "ActividadCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_EquipoCode",
                schema: "mantenimiento",
                table: "OrdenesTrabajo",
                column: "EquipoCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_EspecialidadCode",
                schema: "mantenimiento",
                table: "OrdenesTrabajo",
                column: "EspecialidadCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_InspeccionCode",
                schema: "mantenimiento",
                table: "OrdenesTrabajo",
                column: "InspeccionCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_OportunidadCode",
                schema: "mantenimiento",
                table: "OrdenesTrabajo",
                column: "OportunidadCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_PrioridadCode",
                schema: "mantenimiento",
                table: "OrdenesTrabajo",
                column: "PrioridadCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_ResponsableCode",
                schema: "mantenimiento",
                table: "OrdenesTrabajo",
                column: "ResponsableCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_SubCentroCostoCode",
                schema: "mantenimiento",
                table: "OrdenesTrabajo",
                column: "SubCentroCostoCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_SubCentroEjecutorCode",
                schema: "mantenimiento",
                table: "OrdenesTrabajo",
                column: "SubCentroEjecutorCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_TipoOrdenCode",
                schema: "mantenimiento",
                table: "OrdenesTrabajo",
                column: "TipoOrdenCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioDetalles_ArticuloCode",
                schema: "logistica",
                table: "OrdenServicioDetalles",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioDetalles_PlantaAfectadaCode",
                schema: "logistica",
                table: "OrdenServicioDetalles",
                column: "PlantaAfectadaCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioDetalles_PlantaCode_OrdenTrabajoCode",
                schema: "logistica",
                table: "OrdenServicioDetalles",
                columns: new[] { "PlantaCode", "OrdenTrabajoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioDetalles_SubCentroCostoCode",
                schema: "logistica",
                table: "OrdenServicioDetalles",
                column: "SubCentroCostoCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioDetalles_SubCentroEjecutorCode",
                schema: "logistica",
                table: "OrdenServicioDetalles",
                column: "SubCentroEjecutorCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioDetalles_TipoValeCode",
                schema: "logistica",
                table: "OrdenServicioDetalles",
                column: "TipoValeCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioSalidaDetalles_ArticuloCode",
                schema: "logistica",
                table: "OrdenServicioSalidaDetalles",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioSalidaDetalles_PlantaAfectadaCode",
                schema: "logistica",
                table: "OrdenServicioSalidaDetalles",
                column: "PlantaAfectadaCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioSalidaDetalles_PlantaCode_OrdenTrabajoCode",
                schema: "logistica",
                table: "OrdenServicioSalidaDetalles",
                columns: new[] { "PlantaCode", "OrdenTrabajoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioSalidaDetalles_SubCentroCostoCode",
                schema: "logistica",
                table: "OrdenServicioSalidaDetalles",
                column: "SubCentroCostoCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioSalidaDetalles_SubCentroEjecutorCode",
                schema: "logistica",
                table: "OrdenServicioSalidaDetalles",
                column: "SubCentroEjecutorCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioSalidaDetalles_TipoValeCode",
                schema: "logistica",
                table: "OrdenServicioSalidaDetalles",
                column: "TipoValeCode");

            migrationBuilder.CreateIndex(
                name: "IX_OTResponsables_TrabajadorCode",
                schema: "mantenimiento",
                table: "OTResponsables",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_OTRMaquinas_MaquinaCode",
                schema: "mantenimiento",
                table: "OTRMaquinas",
                column: "MaquinaCode");

            migrationBuilder.CreateIndex(
                name: "IX_OTRMateriales_ArticuloCode",
                schema: "mantenimiento",
                table: "OTRMateriales",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_TokenHash",
                schema: "admin",
                table: "PasswordResetTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                schema: "admin",
                table: "PasswordResetTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalles_ProveedorCode",
                schema: "logistica",
                table: "PedidoDetalles",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalles_UnidadMedidaCode",
                schema: "logistica",
                table: "PedidoDetalles",
                column: "UnidadMedidaCode");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_PlantaCode_OrdenTrabajoCode",
                schema: "logistica",
                table: "Pedidos",
                columns: new[] { "PlantaCode", "OrdenTrabajoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_SubCentroCostoCode",
                schema: "logistica",
                table: "Pedidos",
                column: "SubCentroCostoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_TipoPedidoCode",
                schema: "logistica",
                table: "Pedidos",
                column: "TipoPedidoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_TipoValeCode",
                schema: "logistica",
                table: "Pedidos",
                column: "TipoValeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_TrabajadorCode",
                schema: "logistica",
                table: "Pedidos",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_TramiteCode",
                schema: "logistica",
                table: "Pedidos",
                column: "TramiteCode");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_UnidadNegocioCode",
                schema: "logistica",
                table: "Pedidos",
                column: "UnidadNegocioCode");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosIgv_FechaInicio",
                schema: "comun",
                table: "PeriodosIgv",
                column: "FechaInicio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRoles_PermissionId",
                schema: "admin",
                table: "PermissionRoles",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ProgramaId_Code",
                schema: "admin",
                table: "Permissions",
                columns: new[] { "ProgramaId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plantas_Code",
                schema: "comun",
                table: "Plantas",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreciosProducto_ProductoTipoCode_ProductoCode",
                schema: "facturacion",
                table: "PreciosProducto",
                columns: new[] { "ProductoTipoCode", "ProductoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_AccountingAccountCode",
                schema: "facturacion",
                table: "Productos",
                column: "AccountingAccountCode");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_AgeCode",
                schema: "facturacion",
                table: "Productos",
                column: "AgeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoryCode",
                schema: "facturacion",
                table: "Productos",
                column: "CategoryCode");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CementTypeCode",
                schema: "facturacion",
                table: "Productos",
                column: "CementTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CreditNoteAccountCode",
                schema: "facturacion",
                table: "Productos",
                column: "CreditNoteAccountCode");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_GoodsTypeCode",
                schema: "facturacion",
                table: "Productos",
                column: "GoodsTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_MixProportionCode",
                schema: "facturacion",
                table: "Productos",
                column: "MixProportionCode");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_OperationTypeCode",
                schema: "facturacion",
                table: "Productos",
                column: "OperationTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_SlumpCode",
                schema: "facturacion",
                table: "Productos",
                column: "SlumpCode");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_SpecialConditionCode",
                schema: "facturacion",
                table: "Productos",
                column: "SpecialConditionCode");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_StoneSizeCode",
                schema: "facturacion",
                table: "Productos",
                column: "StoneSizeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_StrengthCode",
                schema: "facturacion",
                table: "Productos",
                column: "StrengthCode");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_TransportAccountCode",
                schema: "facturacion",
                table: "Productos",
                column: "TransportAccountCode");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_TransportTipoProductoCode_TransportCode",
                schema: "facturacion",
                table: "Productos",
                columns: new[] { "TransportTipoProductoCode", "TransportCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_UnitCode",
                schema: "facturacion",
                table: "Productos",
                column: "UnitCode");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_WaterCementRatioCode",
                schema: "facturacion",
                table: "Productos",
                column: "WaterCementRatioCode");

            migrationBuilder.CreateIndex(
                name: "IX_Programas_SubmoduloId_Code",
                schema: "admin",
                table: "Programas",
                columns: new[] { "SubmoduloId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Programas_SubmoduloId_Name",
                schema: "admin",
                table: "Programas",
                columns: new[] { "SubmoduloId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorCondiciones_FormaPagoCode",
                schema: "logistica",
                table: "ProveedorCondiciones",
                column: "FormaPagoCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorCondiciones_MonedaCode",
                schema: "logistica",
                table: "ProveedorCondiciones",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorCondiciones_ProveedorCode",
                schema: "logistica",
                table: "ProveedorCondiciones",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorContactos_ProveedorCode",
                schema: "logistica",
                table: "ProveedorContactos",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorCuentas_BancoCode",
                schema: "logistica",
                table: "ProveedorCuentas",
                column: "BancoCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorCuentas_MonedaCode",
                schema: "logistica",
                table: "ProveedorCuentas",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorCuentas_ProveedorCode",
                schema: "logistica",
                table: "ProveedorCuentas",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorDirecciones_ProveedorCode",
                schema: "logistica",
                table: "ProveedorDirecciones",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorDirecciones_UbigeoCode",
                schema: "logistica",
                table: "ProveedorDirecciones",
                column: "UbigeoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_DocumentNumber",
                schema: "logistica",
                table: "Proveedores",
                column: "DocumentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_DocumentTypeCode",
                schema: "logistica",
                table: "Proveedores",
                column: "DocumentTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_LegalName",
                schema: "logistica",
                table: "Proveedores",
                column: "LegalName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RangosAprobacion_MonedaCode",
                schema: "logistica",
                table: "RangosAprobacion",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_RangosAprobacion_TipoTransaccionCode",
                schema: "logistica",
                table: "RangosAprobacion",
                column: "TipoTransaccionCode");

            migrationBuilder.CreateIndex(
                name: "IX_RangosAprobacion_UnidadNegocioCode",
                schema: "logistica",
                table: "RangosAprobacion",
                column: "UnidadNegocioCode");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                schema: "admin",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "admin",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                schema: "admin",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleUsers_RoleId",
                schema: "admin",
                table: "RoleUsers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SaldosMensualesArticulo_Articulo",
                schema: "logistica",
                table: "SaldosMensualesArticulo",
                columns: new[] { "PlantaCode", "ArticuloCode", "PeriodCode" });

            migrationBuilder.CreateIndex(
                name: "IX_SaldosMensualesArticulo_ArticuloCode",
                schema: "logistica",
                table: "SaldosMensualesArticulo",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_SegmentosVentas_Name",
                schema: "facturacion",
                table: "SegmentosVentas",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sexos_Name",
                schema: "rrhh",
                table: "Sexos",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockArticulos_ArticuloCode",
                schema: "logistica",
                table: "StockArticulos",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_StockProductos_PlantaCode_TipoProductoCode_ProductoCode",
                schema: "facturacion",
                table: "StockProductos",
                columns: new[] { "PlantaCode", "TipoProductoCode", "ProductoCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockProductos_TipoProductoCode_ProductoCode",
                schema: "facturacion",
                table: "StockProductos",
                columns: new[] { "TipoProductoCode", "ProductoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_SubCentrosCosto_AccountingAccountCode",
                schema: "logistica",
                table: "SubCentrosCosto",
                column: "AccountingAccountCode");

            migrationBuilder.CreateIndex(
                name: "IX_SubCentrosCosto_AccountingAttachmentTypeCode",
                schema: "logistica",
                table: "SubCentrosCosto",
                column: "AccountingAttachmentTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_SubCentrosCosto_CentroCostoCode",
                schema: "logistica",
                table: "SubCentrosCosto",
                column: "CentroCostoCode");

            migrationBuilder.CreateIndex(
                name: "IX_SubCentrosCosto_ParentCode",
                schema: "logistica",
                table: "SubCentrosCosto",
                column: "ParentCode");

            migrationBuilder.CreateIndex(
                name: "IX_SubCentrosCosto_PlantaCode",
                schema: "logistica",
                table: "SubCentrosCosto",
                column: "PlantaCode");

            migrationBuilder.CreateIndex(
                name: "IX_SubCentrosEjecutores_CentroEjecutorCode",
                schema: "mantenimiento",
                table: "SubCentrosEjecutores",
                column: "CentroEjecutorCode");

            migrationBuilder.CreateIndex(
                name: "IX_SubFamilias_FamiliaCode",
                schema: "logistica",
                table: "SubFamilias",
                column: "FamiliaCode");

            migrationBuilder.CreateIndex(
                name: "IX_Submodulos_ModuloId_Code",
                schema: "admin",
                table: "Submodulos",
                columns: new[] { "ModuloId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submodulos_ModuloId_Name",
                schema: "admin",
                table: "Submodulos",
                columns: new[] { "ModuloId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposCambio_Date",
                schema: "comun",
                table: "TiposCambio",
                column: "Date",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposCliente_Name",
                schema: "facturacion",
                table: "TiposCliente",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposDocumento_Code",
                schema: "comun",
                table: "TiposDocumento",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposHojaCorte_Name",
                schema: "facturacion",
                table: "TiposHojaCorte",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposValeArticulo_ArticuloCode",
                schema: "logistica",
                table: "TiposValeArticulo",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_TiposValorizacion_Name",
                schema: "facturacion",
                table: "TiposValorizacion",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_antecedente_TrabajadorCode",
                schema: "rrhh",
                table: "trabajador_antecedente",
                column: "TrabajadorCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_beneficio_TrabajadorCode",
                schema: "rrhh",
                table: "trabajador_beneficio",
                column: "TrabajadorCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_contable_TrabajadorCode",
                schema: "rrhh",
                table: "trabajador_contable",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_contrato_TipoContratoCode",
                schema: "rrhh",
                table: "trabajador_contrato",
                column: "TipoContratoCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_contrato_TipoExtensionCode",
                schema: "rrhh",
                table: "trabajador_contrato",
                column: "TipoExtensionCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_contrato_TrabajadorCode",
                schema: "rrhh",
                table: "trabajador_contrato",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_cuenta_bancaria_BancoCode",
                schema: "rrhh",
                table: "trabajador_cuenta_bancaria",
                column: "BancoCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_cuenta_bancaria_MonedaCode",
                schema: "rrhh",
                table: "trabajador_cuenta_bancaria",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_cuenta_bancaria_TipoCuentaCode",
                schema: "rrhh",
                table: "trabajador_cuenta_bancaria",
                column: "TipoCuentaCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_cuenta_bancaria_TrabajadorCode",
                schema: "rrhh",
                table: "trabajador_cuenta_bancaria",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_dependiente_ParentescoCode",
                schema: "rrhh",
                table: "trabajador_dependiente",
                column: "ParentescoCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_dependiente_TrabajadorCode",
                schema: "rrhh",
                table: "trabajador_dependiente",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_fiscal_TrabajadorCode",
                schema: "rrhh",
                table: "trabajador_fiscal",
                column: "TrabajadorCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_formacion_EspecialidadCode",
                schema: "rrhh",
                table: "trabajador_formacion",
                column: "EspecialidadCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_formacion_GradoInstruccionCode",
                schema: "rrhh",
                table: "trabajador_formacion",
                column: "GradoInstruccionCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_formacion_ModalidadFormativaCode",
                schema: "rrhh",
                table: "trabajador_formacion",
                column: "ModalidadFormativaCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_formacion_NivelEducativoCode",
                schema: "rrhh",
                table: "trabajador_formacion",
                column: "NivelEducativoCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_formacion_TipoCentroFormacionCode",
                schema: "rrhh",
                table: "trabajador_formacion",
                column: "TipoCentroFormacionCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_formacion_TituloCode",
                schema: "rrhh",
                table: "trabajador_formacion",
                column: "TituloCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_formacion_TrabajadorCode",
                schema: "rrhh",
                table: "trabajador_formacion",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_jornada_HorarioCode",
                schema: "rrhh",
                table: "trabajador_jornada",
                column: "HorarioCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_jornada_TrabajadorCode",
                schema: "rrhh",
                table: "trabajador_jornada",
                column: "TrabajadorCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_laboral_AreaCode",
                schema: "rrhh",
                table: "trabajador_laboral",
                column: "AreaCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_laboral_CargoCode",
                schema: "rrhh",
                table: "trabajador_laboral",
                column: "CargoCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_laboral_CategoriaTrabajadorCode",
                schema: "rrhh",
                table: "trabajador_laboral",
                column: "CategoriaTrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_laboral_EstadoTrabajadorCode",
                schema: "rrhh",
                table: "trabajador_laboral",
                column: "EstadoTrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_laboral_NivelCode",
                schema: "rrhh",
                table: "trabajador_laboral",
                column: "NivelCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_laboral_OcupacionCode",
                schema: "rrhh",
                table: "trabajador_laboral",
                column: "OcupacionCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_laboral_OficinaCode",
                schema: "rrhh",
                table: "trabajador_laboral",
                column: "OficinaCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_laboral_PlantaCode",
                schema: "rrhh",
                table: "trabajador_laboral",
                column: "PlantaCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_laboral_ProveedorCode",
                schema: "rrhh",
                table: "trabajador_laboral",
                column: "ProveedorCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_laboral_RegimenLaboralCode",
                schema: "rrhh",
                table: "trabajador_laboral",
                column: "RegimenLaboralCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_laboral_SubOcupacionCode",
                schema: "rrhh",
                table: "trabajador_laboral",
                column: "SubOcupacionCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_laboral_TipoTrabajadorCode",
                schema: "rrhh",
                table: "trabajador_laboral",
                column: "TipoTrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_laboral_TrabajadorCode",
                schema: "rrhh",
                table: "trabajador_laboral",
                column: "TrabajadorCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_pension_AfpCode",
                schema: "rrhh",
                table: "trabajador_pension",
                column: "AfpCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_pension_RegimenPensionarioCode",
                schema: "rrhh",
                table: "trabajador_pension",
                column: "RegimenPensionarioCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_pension_TipoAfiliacionCode",
                schema: "rrhh",
                table: "trabajador_pension",
                column: "TipoAfiliacionCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_pension_TipoPensionCode",
                schema: "rrhh",
                table: "trabajador_pension",
                column: "TipoPensionCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_pension_TrabajadorCode",
                schema: "rrhh",
                table: "trabajador_pension",
                column: "TrabajadorCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_remuneracion_ModoPagoCode",
                schema: "rrhh",
                table: "trabajador_remuneracion",
                column: "ModoPagoCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_remuneracion_MonedaCode",
                schema: "rrhh",
                table: "trabajador_remuneracion",
                column: "MonedaCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_remuneracion_TrabajadorCode",
                schema: "rrhh",
                table: "trabajador_remuneracion",
                column: "TrabajadorCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_salud_AlergiaCode",
                schema: "rrhh",
                table: "trabajador_salud",
                column: "AlergiaCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_salud_TipoSangreCode",
                schema: "rrhh",
                table: "trabajador_salud",
                column: "TipoSangreCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_salud_TrabajadorCode",
                schema: "rrhh",
                table: "trabajador_salud",
                column: "TrabajadorCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_seguro_EpsCode",
                schema: "rrhh",
                table: "trabajador_seguro",
                column: "EpsCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_seguro_SctrPensionCode",
                schema: "rrhh",
                table: "trabajador_seguro",
                column: "SctrPensionCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_seguro_SctrSaludCode",
                schema: "rrhh",
                table: "trabajador_seguro",
                column: "SctrSaludCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_seguro_SctrTipoCode",
                schema: "rrhh",
                table: "trabajador_seguro",
                column: "SctrTipoCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_seguro_SituacionEpsCode",
                schema: "rrhh",
                table: "trabajador_seguro",
                column: "SituacionEpsCode");

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_seguro_TrabajadorCode",
                schema: "rrhh",
                table: "trabajador_seguro",
                column: "TrabajadorCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_sindicato_TrabajadorCode",
                schema: "rrhh",
                table: "trabajador_sindicato",
                column: "TrabajadorCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trabajador_vacacion_TrabajadorCode",
                schema: "rrhh",
                table: "trabajador_vacacion",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_TrabajadorContactos_ParentescoCode",
                schema: "rrhh",
                table: "TrabajadorContactos",
                column: "ParentescoCode");

            migrationBuilder.CreateIndex(
                name: "IX_TrabajadorContactos_TrabajadorCode",
                schema: "rrhh",
                table: "TrabajadorContactos",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_TrabajadorDocumentos_TipoDocumentoCode_DocumentNumber",
                schema: "rrhh",
                table: "TrabajadorDocumentos",
                columns: new[] { "TipoDocumentoCode", "DocumentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrabajadorDocumentos_TrabajadorCode",
                schema: "rrhh",
                table: "TrabajadorDocumentos",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_TrabajadorDomicilios_RoadTypeCode",
                schema: "rrhh",
                table: "TrabajadorDomicilios",
                column: "RoadTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_TrabajadorDomicilios_TrabajadorCode",
                schema: "rrhh",
                table: "TrabajadorDomicilios",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_TrabajadorDomicilios_UbigeoCode",
                schema: "rrhh",
                table: "TrabajadorDomicilios",
                column: "UbigeoCode");

            migrationBuilder.CreateIndex(
                name: "IX_TrabajadorDomicilios_ZoneTypeCode",
                schema: "rrhh",
                table: "TrabajadorDomicilios",
                column: "ZoneTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Trabajadores_BirthUbigeoCode",
                schema: "rrhh",
                table: "Trabajadores",
                column: "BirthUbigeoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Trabajadores_EstadoCivilCode",
                schema: "rrhh",
                table: "Trabajadores",
                column: "EstadoCivilCode");

            migrationBuilder.CreateIndex(
                name: "IX_Trabajadores_NacionalidadCode",
                schema: "rrhh",
                table: "Trabajadores",
                column: "NacionalidadCode");

            migrationBuilder.CreateIndex(
                name: "IX_Trabajadores_SexoCode",
                schema: "rrhh",
                table: "Trabajadores",
                column: "SexoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Transportistas_DocumentTypeCode_DocumentNumber",
                schema: "logistica",
                table: "Transportistas",
                columns: new[] { "DocumentTypeCode", "DocumentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transportistas_LegalName",
                schema: "logistica",
                table: "Transportistas",
                column: "LegalName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transportistas_UbigeoCode",
                schema: "logistica",
                table: "Transportistas",
                column: "UbigeoCode");

            migrationBuilder.CreateIndex(
                name: "IX_TransportistasVentas_DocumentTypeCode_DocumentNumber",
                schema: "facturacion",
                table: "TransportistasVentas",
                columns: new[] { "DocumentTypeCode", "DocumentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransportistasVentas_UbigeoCode",
                schema: "facturacion",
                table: "TransportistasVentas",
                column: "UbigeoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Ubigeos_Code",
                schema: "comun",
                table: "Ubigeos",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ubigeos_Department_Province_District",
                schema: "comun",
                table: "Ubigeos",
                columns: new[] { "Department", "Province", "District" });

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesNegocio_ParentCode",
                schema: "logistica",
                table: "UnidadesNegocio",
                column: "ParentCode");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "admin",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TrabajadorCode",
                schema: "admin",
                table: "Users",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_ValeDetalles_ArticuloCode",
                schema: "logistica",
                table: "ValeDetalles",
                column: "ArticuloCode");

            migrationBuilder.CreateIndex(
                name: "IX_ValeDetalles_PlantaCode_ArticuloCode_Estado",
                schema: "logistica",
                table: "ValeDetalles",
                columns: new[] { "PlantaCode", "ArticuloCode", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_ValeDetalles_PlantaCode_ValeCode_ItemNumber",
                schema: "logistica",
                table: "ValeDetalles",
                columns: new[] { "PlantaCode", "ValeCode", "ItemNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Vales_PlantaAfectadaCode",
                schema: "logistica",
                table: "Vales",
                column: "PlantaAfectadaCode");

            migrationBuilder.CreateIndex(
                name: "IX_Vales_PlantaCode_CreatedAt",
                schema: "logistica",
                table: "Vales",
                columns: new[] { "PlantaCode", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Vales_PlantaCode_Estado",
                schema: "logistica",
                table: "Vales",
                columns: new[] { "PlantaCode", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_Vales_PlantaCode_OrdenTrabajoCode",
                schema: "logistica",
                table: "Vales",
                columns: new[] { "PlantaCode", "OrdenTrabajoCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Vales_SubCentroCostoCode",
                schema: "logistica",
                table: "Vales",
                column: "SubCentroCostoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Vales_SubCentroEjecutorCode",
                schema: "logistica",
                table: "Vales",
                column: "SubCentroEjecutorCode");

            migrationBuilder.CreateIndex(
                name: "IX_Vales_TipoValeCode",
                schema: "logistica",
                table: "Vales",
                column: "TipoValeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Vales_TrabajadorCode",
                schema: "logistica",
                table: "Vales",
                column: "TrabajadorCode");

            migrationBuilder.CreateIndex(
                name: "IX_Vales_UnidadNegocioCode",
                schema: "logistica",
                table: "Vales",
                column: "UnidadNegocioCode");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_TransportistaCode_ChoferCode",
                schema: "facturacion",
                table: "Vehiculos",
                columns: new[] { "TransportistaCode", "ChoferCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_TransportistaCode_LicensePlate",
                schema: "facturacion",
                table: "Vehiculos",
                columns: new[] { "TransportistaCode", "LicensePlate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_LicensePlate",
                schema: "logistica",
                table: "Vehiculos",
                column: "LicensePlate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_TransportistaCode",
                schema: "logistica",
                table: "Vehiculos",
                column: "TransportistaCode");

            migrationBuilder.CreateIndex(
                name: "IX_Vendedores_UserId",
                schema: "facturacion",
                table: "Vendedores",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AjusteInventarioDetalles",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "AprobadoresAsignados",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "ArticuloProveedor",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "CierreBloqueos",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "CierreIncidencias",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "CierreRevaluaciones",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "Cobradores",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "ControlCierres",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "ControlesVentas",
                schema: "comun");

            migrationBuilder.DropTable(
                name: "CotizacionesDetalle",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "CotizacionesMetradoDetalle",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "CotizacionesNotas",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "CotizacionPedidoOrigenes",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "CotizacionProveedorDetalles",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "CuentasBancarias",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "DocumentoCompraDetalles",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "GuiaDetalles",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "HojaCorteDetalles",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "ImportacionDetalles",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "ImportacionGastoArticulos",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "ListasPrecios",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "MovimientosArticulo",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "NotaIngresoDetalles",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "NotasCotizacionPlantilla",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "OrdenCompraAdministracionDetalles",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "OrdenCompraPedidoOrigenes",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "OrdenServicioDetalles",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "OrdenServicioSalidaDetalles",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "OTResponsables",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "OTRMaquinas",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "OTRMateriales",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "PeriodosIgv",
                schema: "comun");

            migrationBuilder.DropTable(
                name: "PermissionRoles",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "PreciosProducto",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "ProveedorCondiciones",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "ProveedorContactos",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "ProveedorCuentas",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "ProveedorDirecciones",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "ResponsablesObra",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "RoleUsers",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "SaldosMensualesArticulo",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "StockArticulos",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "StockProductos",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "TiposAsiento",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "TiposCambio",
                schema: "comun");

            migrationBuilder.DropTable(
                name: "TiposValeArticulo",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "trabajador_antecedente",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "trabajador_beneficio",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "trabajador_contable",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "trabajador_contrato",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "trabajador_cuenta_bancaria",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "trabajador_dependiente",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "trabajador_fiscal",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "trabajador_formacion",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "trabajador_jornada",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "trabajador_laboral",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "trabajador_pension",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "trabajador_remuneracion",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "trabajador_salud",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "trabajador_seguro",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "trabajador_sindicato",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "trabajador_vacacion",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "TrabajadorContactos",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "TrabajadorDocumentos",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "TrabajadorDomicilios",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "ValeDetalles",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "Vehiculos",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "AjustesInventario",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "RangosAprobacion",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "CotizacionesMetradoResumen",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "CotizacionDetalles",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "CotizacionProveedores",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "Anexos",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "DocumentosCompra",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "Guias",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "HojasCorte",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "ImportacionGastos",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "NotaIngresos",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "OrdenCompraDetalles",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "PedidoDetalles",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "OrdenesServicio",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "Maquinas",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "TiposContrato",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "TiposExtensionContrato",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "Bancos",
                schema: "comun");

            migrationBuilder.DropTable(
                name: "TiposCuenta",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "Especialidades",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "GradosInstruccion",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "ModalidadesFormativas",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "NivelesEducativos",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "TiposCentroFormacion",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "Titulos",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "Horarios",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "Areas",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "Cargos",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "CategoriasTrabajador",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "EstadosTrabajador",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "NivelesTrabajador",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "Ocupaciones",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "Oficinas",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "RegimenesLaborales",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "SubOcupaciones",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "TiposTrabajador",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "Afps",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "RegimenesPensionarios",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "TiposAfiliacion",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "TiposPension",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "ModosPago",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "Alergias",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "TiposSangre",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "Eps",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "SctrPension",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "SctrSalud",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "SituacionesEps",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "TiposSctr",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "Parentescos",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "TiposVia",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "TiposZona",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "ChoferesVentas",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "Niveles",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "TiposTransaccion",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "AlturasLosa",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "Cotizaciones",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "Cotizaciones",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "OrdenesCompraAdministracion",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "Conductores",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "MotivosDevolucion",
                schema: "comun");

            migrationBuilder.DropTable(
                name: "Vehiculos",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "TiposHojaCorte",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "Importaciones",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "MotivosDevolucionArticulo",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "Vales",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "Articulos",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "OrdenesCompra",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "Pedidos",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "OrdenesServicioSalida",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "Programas",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "TransportistasVentas",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "Productos",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "Fletes",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "Negocios",
                schema: "comun");

            migrationBuilder.DropTable(
                name: "Obras",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "Tecnicos",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "Vendedores",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "Transportistas",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "PlanesArticulo",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "SubFamilias",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "TiposArticulo",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "UnidadesMedida",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "Compradores",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "ComprobantesPago",
                schema: "comun");

            migrationBuilder.DropTable(
                name: "FormasPago",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "LugaresEnvio",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "NotasCompra",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "Proveedores",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "TiposCompra",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "OrdenesTrabajo",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "TiposPedido",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "TiposVale",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "Tramites",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "UnidadesNegocio",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "Submodulos",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "AtributosConcreto",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "CategoriasProducto",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "TiposBien",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "TiposOperacion",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "TiposProducto",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "UnidadesMedidaVenta",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "AnalisisVentas",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "Clientes",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "TiposValorizacion",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "Familias",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "Actividades",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "Equipos",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "Especialidades",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "Inspecciones",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "Oportunidades",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "Prioridades",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "SubCentrosEjecutores",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "TiposOrden",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "Modulos",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "SegmentosVentas",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "ClasificacionesCliente",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "FormasPagoVenta",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "TiposCliente",
                schema: "facturacion");

            migrationBuilder.DropTable(
                name: "TiposDocumento",
                schema: "comun");

            migrationBuilder.DropTable(
                name: "Trabajadores",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "ObjetosActividad",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "VerbosActividad",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "SubCentrosCosto",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "CentrosEjecutores",
                schema: "mantenimiento");

            migrationBuilder.DropTable(
                name: "EstadosCiviles",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "Nacionalidades",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "Sexos",
                schema: "rrhh");

            migrationBuilder.DropTable(
                name: "Ubigeos",
                schema: "comun");

            migrationBuilder.DropTable(
                name: "CentrosCosto",
                schema: "logistica");

            migrationBuilder.DropTable(
                name: "CuentasContables",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "Plantas",
                schema: "comun");

            migrationBuilder.DropTable(
                name: "Monedas",
                schema: "comun");

            migrationBuilder.DropTable(
                name: "TiposAnexo",
                schema: "contabilidad");

            migrationBuilder.DropTable(
                name: "TiposCuentaContable",
                schema: "contabilidad");
        }
    }
}
