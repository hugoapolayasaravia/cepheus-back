using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Transacciones
{
    public class PagoProveedorDetalleConfiguration : IEntityTypeConfiguration<PagoProveedorDetalle>
    {
        public void Configure(EntityTypeBuilder<PagoProveedorDetalle> builder)
        {
            builder.ToTable("PagoProveedorDetalles", schema: "contabilidad");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PagoCode).IsRequired().HasColumnType("char(10)");

            builder.Property(x => x.ComprobantePagoCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.NumeroDocumento).IsRequired().HasColumnType("char(15)");
            builder.Property(x => x.ProveedorCode).IsRequired().HasColumnType("char(5)");

            builder.HasOne(x => x.DocumentoCompra).WithMany()
                .HasForeignKey(x => new { x.ComprobantePagoCode, x.NumeroDocumento, x.ProveedorCode })
                .HasPrincipalKey(x => new { x.ComprobantePagoCode, x.NumeroDocumento, x.ProveedorCode })
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.TotalSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);
            builder.Property(x => x.PendienteSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);
            builder.Property(x => x.PagoSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);

            builder.Property(x => x.AsientoProvisionCode).HasColumnType("char(13)");

            builder.Property(x => x.Detraccion).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.BaseRetencionSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);
            builder.Property(x => x.MontoRetenidoSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);

            // Un documento solo puede estar una vez en el mismo pago (PK legacy de DCheCompras).
            builder.HasIndex(x => new { x.PagoCode, x.ComprobantePagoCode, x.NumeroDocumento, x.ProveedorCode })
                .IsUnique();
        }
    }
}
