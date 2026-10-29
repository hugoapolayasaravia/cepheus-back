using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cepheus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AsientoContables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComprobantePagoCode",
                schema: "contabilidad",
                table: "PlantillasAsiento",
                type: "char(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionText",
                schema: "contabilidad",
                table: "PlantillasAsiento",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValeTypeCode",
                schema: "contabilidad",
                table: "PlantillaAsientoLineas",
                type: "char(3)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaDocumento",
                schema: "contabilidad",
                table: "DocumentosCompra",
                type: "date",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ArticuloAnexos",
                schema: "logistica",
                columns: table => new
                {
                    SubCentroCostoCode = table.Column<string>(type: "char(6)", nullable: false),
                    ArticuloCode = table.Column<string>(type: "char(7)", nullable: false),
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
                    table.PrimaryKey("PK_ArticuloAnexos", x => new { x.SubCentroCostoCode, x.ArticuloCode });
                    table.ForeignKey(
                        name: "FK_ArticuloAnexos_Anexos_AccountingAttachmentTypeCode_AccountingAttachmentCode",
                        columns: x => new { x.AccountingAttachmentTypeCode, x.AccountingAttachmentCode },
                        principalSchema: "contabilidad",
                        principalTable: "Anexos",
                        principalColumns: new[] { "TipoAnexoCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticuloAnexos_Articulos_ArticuloCode",
                        column: x => x.ArticuloCode,
                        principalSchema: "logistica",
                        principalTable: "Articulos",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticuloAnexos_CuentasContables_AccountingAccountCode",
                        column: x => x.AccountingAccountCode,
                        principalSchema: "contabilidad",
                        principalTable: "CuentasContables",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticuloAnexos_SubCentrosCosto_SubCentroCostoCode",
                        column: x => x.SubCentroCostoCode,
                        principalSchema: "logistica",
                        principalTable: "SubCentrosCosto",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticuloAnexos_TiposAnexo_AccountingAttachmentTypeCode",
                        column: x => x.AccountingAttachmentTypeCode,
                        principalSchema: "contabilidad",
                        principalTable: "TiposAnexo",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasAsiento_ComprobantePagoCode",
                schema: "contabilidad",
                table: "PlantillasAsiento",
                column: "ComprobantePagoCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlantillaAsientoLineas_ValeTypeCode",
                schema: "contabilidad",
                table: "PlantillaAsientoLineas",
                column: "ValeTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloAnexos_AccountingAccountCode",
                schema: "logistica",
                table: "ArticuloAnexos",
                column: "AccountingAccountCode");

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloAnexos_AccountingAttachmentTypeCode_AccountingAttachmentCode",
                schema: "logistica",
                table: "ArticuloAnexos",
                columns: new[] { "AccountingAttachmentTypeCode", "AccountingAttachmentCode" });

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloAnexos_ArticuloCode",
                schema: "logistica",
                table: "ArticuloAnexos",
                column: "ArticuloCode");

            migrationBuilder.AddForeignKey(
                name: "FK_PlantillaAsientoLineas_TiposVale_ValeTypeCode",
                schema: "contabilidad",
                table: "PlantillaAsientoLineas",
                column: "ValeTypeCode",
                principalSchema: "logistica",
                principalTable: "TiposVale",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlantillasAsiento_ComprobantesPago_ComprobantePagoCode",
                schema: "contabilidad",
                table: "PlantillasAsiento",
                column: "ComprobantePagoCode",
                principalSchema: "comun",
                principalTable: "ComprobantesPago",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlantillaAsientoLineas_TiposVale_ValeTypeCode",
                schema: "contabilidad",
                table: "PlantillaAsientoLineas");

            migrationBuilder.DropForeignKey(
                name: "FK_PlantillasAsiento_ComprobantesPago_ComprobantePagoCode",
                schema: "contabilidad",
                table: "PlantillasAsiento");

            migrationBuilder.DropTable(
                name: "ArticuloAnexos",
                schema: "logistica");

            migrationBuilder.DropIndex(
                name: "IX_PlantillasAsiento_ComprobantePagoCode",
                schema: "contabilidad",
                table: "PlantillasAsiento");

            migrationBuilder.DropIndex(
                name: "IX_PlantillaAsientoLineas_ValeTypeCode",
                schema: "contabilidad",
                table: "PlantillaAsientoLineas");

            migrationBuilder.DropColumn(
                name: "ComprobantePagoCode",
                schema: "contabilidad",
                table: "PlantillasAsiento");

            migrationBuilder.DropColumn(
                name: "DescriptionText",
                schema: "contabilidad",
                table: "PlantillasAsiento");

            migrationBuilder.DropColumn(
                name: "ValeTypeCode",
                schema: "contabilidad",
                table: "PlantillaAsientoLineas");

            migrationBuilder.DropColumn(
                name: "FechaDocumento",
                schema: "contabilidad",
                table: "DocumentosCompra");
        }
    }
}
