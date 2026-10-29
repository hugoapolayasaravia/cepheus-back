using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cepheus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProveedorCuentaContable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proveedores_CuentasContables_PayableAccountForeignCode",
                schema: "logistica",
                table: "Proveedores");

            migrationBuilder.DropForeignKey(
                name: "FK_Proveedores_CuentasContables_PayableAccountLocalCode",
                schema: "logistica",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Proveedores_PayableAccountForeignCode",
                schema: "logistica",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Proveedores_PayableAccountLocalCode",
                schema: "logistica",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "PayableAccountForeignCode",
                schema: "logistica",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "PayableAccountLocalCode",
                schema: "logistica",
                table: "Proveedores");

            migrationBuilder.CreateTable(
                name: "ProveedorCuentasContables",
                schema: "logistica",
                columns: table => new
                {
                    ProveedorCode = table.Column<string>(type: "char(5)", nullable: false),
                    MonedaCode = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    AccountingAccountCode = table.Column<string>(type: "char(8)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedorCuentasContables", x => new { x.ProveedorCode, x.MonedaCode });
                    table.ForeignKey(
                        name: "FK_ProveedorCuentasContables_CuentasContables_AccountingAccountCode",
                        column: x => x.AccountingAccountCode,
                        principalSchema: "contabilidad",
                        principalTable: "CuentasContables",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProveedorCuentasContables_Monedas_MonedaCode",
                        column: x => x.MonedaCode,
                        principalSchema: "comun",
                        principalTable: "Monedas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProveedorCuentasContables_Proveedores_ProveedorCode",
                        column: x => x.ProveedorCode,
                        principalSchema: "logistica",
                        principalTable: "Proveedores",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorCuentasContables_AccountingAccountCode",
                schema: "logistica",
                table: "ProveedorCuentasContables",
                column: "AccountingAccountCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorCuentasContables_MonedaCode",
                schema: "logistica",
                table: "ProveedorCuentasContables",
                column: "MonedaCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProveedorCuentasContables",
                schema: "logistica");

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
        }
    }
}
