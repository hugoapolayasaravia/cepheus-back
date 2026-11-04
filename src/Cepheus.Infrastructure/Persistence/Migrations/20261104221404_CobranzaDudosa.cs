using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cepheus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CobranzaDudosa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DoubtfulExchangeRate",
                schema: "facturacion",
                table: "NotasDebitoVenta",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DoubtfulStatus",
                schema: "facturacion",
                table: "NotasDebitoVenta",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DoubtfulExchangeRate",
                schema: "facturacion",
                table: "NotasCreditoVenta",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DoubtfulStatus",
                schema: "facturacion",
                table: "NotasCreditoVenta",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DoubtfulExchangeRate",
                schema: "facturacion",
                table: "LetrasVenta",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DoubtfulStatus",
                schema: "facturacion",
                table: "LetrasVenta",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DoubtfulExchangeRate",
                schema: "facturacion",
                table: "ComprobantesVenta",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DoubtfulStatus",
                schema: "facturacion",
                table: "ComprobantesVenta",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DoubtfulExchangeRate",
                schema: "facturacion",
                table: "ChequesVenta",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DoubtfulStatus",
                schema: "facturacion",
                table: "ChequesVenta",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CobranzasDudosas",
                schema: "facturacion",
                columns: table => new
                {
                    TypeCode = table.Column<string>(type: "char(2)", nullable: false),
                    PlantaCode = table.Column<string>(type: "varchar(2)", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ClienteCode = table.Column<string>(type: "char(5)", nullable: false),
                    BancoCode = table.Column<string>(type: "varchar(3)", nullable: false),
                    ObraCode = table.Column<string>(type: "char(3)", nullable: true),
                    ProcessDate = table.Column<DateTime>(type: "date", nullable: false),
                    SystemDate = table.Column<DateTime>(type: "date", nullable: false),
                    TipoContabilidad = table.Column<int>(type: "int", nullable: false),
                    CurrencyCode = table.Column<string>(type: "char(3)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalSoles = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDollars = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProvisionEntry = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    CanjeEntry = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    WrittenOffAt = table.Column<DateTime>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CobranzasDudosas", x => new { x.TypeCode, x.PlantaCode, x.Number, x.ClienteCode, x.BancoCode });
                });

            migrationBuilder.CreateIndex(
                name: "IX_CobranzasDudosas_CanjeEntry",
                schema: "facturacion",
                table: "CobranzasDudosas",
                column: "CanjeEntry");

            migrationBuilder.CreateIndex(
                name: "IX_CobranzasDudosas_ClienteCode",
                schema: "facturacion",
                table: "CobranzasDudosas",
                column: "ClienteCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CobranzasDudosas",
                schema: "facturacion");

            migrationBuilder.DropColumn(
                name: "DoubtfulExchangeRate",
                schema: "facturacion",
                table: "NotasDebitoVenta");

            migrationBuilder.DropColumn(
                name: "DoubtfulStatus",
                schema: "facturacion",
                table: "NotasDebitoVenta");

            migrationBuilder.DropColumn(
                name: "DoubtfulExchangeRate",
                schema: "facturacion",
                table: "NotasCreditoVenta");

            migrationBuilder.DropColumn(
                name: "DoubtfulStatus",
                schema: "facturacion",
                table: "NotasCreditoVenta");

            migrationBuilder.DropColumn(
                name: "DoubtfulExchangeRate",
                schema: "facturacion",
                table: "LetrasVenta");

            migrationBuilder.DropColumn(
                name: "DoubtfulStatus",
                schema: "facturacion",
                table: "LetrasVenta");

            migrationBuilder.DropColumn(
                name: "DoubtfulExchangeRate",
                schema: "facturacion",
                table: "ComprobantesVenta");

            migrationBuilder.DropColumn(
                name: "DoubtfulStatus",
                schema: "facturacion",
                table: "ComprobantesVenta");

            migrationBuilder.DropColumn(
                name: "DoubtfulExchangeRate",
                schema: "facturacion",
                table: "ChequesVenta");

            migrationBuilder.DropColumn(
                name: "DoubtfulStatus",
                schema: "facturacion",
                table: "ChequesVenta");
        }
    }
}
