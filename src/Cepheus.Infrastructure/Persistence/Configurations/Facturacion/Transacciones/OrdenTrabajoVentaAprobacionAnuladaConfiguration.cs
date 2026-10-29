using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Cepheus.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class OrdenTrabajoVentaAprobacionAnuladaConfiguration : IEntityTypeConfiguration<OrdenTrabajoVentaAprobacionAnulada>
    {
        public void Configure(EntityTypeBuilder<OrdenTrabajoVentaAprobacionAnulada> builder)
        {
            builder.ToTable("OrdenesTrabajoVentaAprobacionesAnuladas", schema: "facturacion");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrdenTrabajoCode).IsRequired().HasColumnType("char(8)");
            builder.Property(x => x.Reason).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Observations).HasMaxLength(500);

            builder.Property(x => x.DocumentType)
                .IsRequired()
                .HasColumnType("char(1)")
                .HasConversion(new LegacyCharConverter<TipoDocumentoAprobacion>(
                    (TipoDocumentoAprobacion.Factura, "F"),
                    (TipoDocumentoAprobacion.Boleta, "B")));

            builder.Property(x => x.Document1Number).HasColumnType("char(10)");
            builder.Property(x => x.Document2Number).HasColumnType("char(10)");
            builder.Property(x => x.Plant1Code).HasColumnType("char(2)");
            builder.Property(x => x.Plant2Code).HasColumnType("char(2)");
            builder.Property(x => x.ApprovedQuantity).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.ApprovedBy).HasMaxLength(20);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.OrdenTrabajo).WithMany()
                .HasForeignKey(x => x.OrdenTrabajoCode)
                .HasPrincipalKey(o => o.Code)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.OrdenTrabajoCode);
        }
    }
}
