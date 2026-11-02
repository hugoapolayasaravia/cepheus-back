using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cepheus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PRECIOPRODUCTO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PreciosProducto",
                schema: "facturacion",
                table: "PreciosProducto");

            migrationBuilder.DropColumn(
                name: "CurrencyTypeCode",
                schema: "facturacion",
                table: "PreciosProducto");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                schema: "facturacion",
                table: "PreciosProducto",
                type: "char(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(1)",
                oldDefaultValue: "S");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PreciosProducto",
                schema: "facturacion",
                table: "PreciosProducto",
                columns: new[] { "FleteCode", "ProductoTipoCode", "ProductoCode", "CurrencyCode" });

            migrationBuilder.CreateIndex(
                name: "IX_PreciosProducto_CurrencyCode",
                schema: "facturacion",
                table: "PreciosProducto",
                column: "CurrencyCode");

            migrationBuilder.AddForeignKey(
                name: "FK_PreciosProducto_Monedas_CurrencyCode",
                schema: "facturacion",
                table: "PreciosProducto",
                column: "CurrencyCode",
                principalSchema: "comun",
                principalTable: "Monedas",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreciosProducto_Monedas_CurrencyCode",
                schema: "facturacion",
                table: "PreciosProducto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PreciosProducto",
                schema: "facturacion",
                table: "PreciosProducto");

            migrationBuilder.DropIndex(
                name: "IX_PreciosProducto_CurrencyCode",
                schema: "facturacion",
                table: "PreciosProducto");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                schema: "facturacion",
                table: "PreciosProducto",
                type: "char(1)",
                nullable: false,
                defaultValue: "S",
                oldClrType: typeof(string),
                oldType: "char(3)",
                oldMaxLength: 3);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyTypeCode",
                schema: "facturacion",
                table: "PreciosProducto",
                type: "char(1)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PreciosProducto",
                schema: "facturacion",
                table: "PreciosProducto",
                columns: new[] { "FleteCode", "ProductoTipoCode", "ProductoCode", "CurrencyTypeCode", "CurrencyCode" });
        }
    }
}
