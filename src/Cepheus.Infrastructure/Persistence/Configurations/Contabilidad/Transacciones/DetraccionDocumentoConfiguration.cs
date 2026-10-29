using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Transacciones
{
    public class DetraccionDocumentoConfiguration : IEntityTypeConfiguration<DetraccionDocumento>
    {
        public void Configure(EntityTypeBuilder<DetraccionDocumento> builder)
        {
            builder.ToTable("DetraccionDocumentos", schema: "contabilidad");

            builder.HasKey(x => new
            {
                x.Year, x.Period, x.LotNumber, x.ProveedorCode, x.ItemNumber,
                x.ComprobantePagoCode, x.NumeroDocumento
            });

            builder.Property(x => x.Year).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Period).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.LotNumber).IsRequired().HasColumnType("char(4)");

            builder.Property(x => x.DetraccionSol).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.IncludeInExport).IsRequired().HasDefaultValue(true);

            // Documento de compra (FK compuesta a su PK: ComprobantePagoCode, NumeroDocumento, ProveedorCode).
            builder.HasOne(x => x.DocumentoCompra).WithMany()
                .HasForeignKey(x => new { x.ComprobantePagoCode, x.NumeroDocumento, x.ProveedorCode })
                .HasPrincipalKey(d => new { d.ComprobantePagoCode, d.NumeroDocumento, d.ProveedorCode })
                .OnDelete(DeleteBehavior.Restrict);

            // Un documento de compra solo puede pertenecer a un lote de detracción
            // (legacy: NOT IN COM_DETRACCION_DOCUMENTOS al generar el lote).
            builder.HasIndex(x => new { x.ComprobantePagoCode, x.NumeroDocumento, x.ProveedorCode })
                .IsUnique();
        }
    }
}
