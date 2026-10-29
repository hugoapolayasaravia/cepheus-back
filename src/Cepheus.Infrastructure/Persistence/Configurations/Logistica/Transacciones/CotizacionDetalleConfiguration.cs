// Cepheus.Infrastructure/Persistence/Configurations/Logistica/Transacciones/CotizacionDetalleConfiguration.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones
{
    public class CotizacionDetalleConfiguration : IEntityTypeConfiguration<CotizacionDetalle>
    {
        public void Configure(EntityTypeBuilder<CotizacionDetalle> builder)
        {
            builder.ToTable("CotizacionDetalles", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.CotizacionCode, x.ArticuloCode });

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.CotizacionCode).IsRequired().HasColumnType("char(6)");

            builder.HasOne(x => x.Cotizacion)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => new { x.PlantaCode, x.CotizacionCode })
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ArticuloCode).IsRequired().HasColumnType("char(7)");
            builder.HasOne(x => x.Articulo).WithMany().HasForeignKey(x => x.ArticuloCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CantidadArticulo).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasMany(x => x.Origenes)
                .WithOne(x => x.CotizacionDetalle)
                .HasForeignKey(x => new { x.PlantaCode, x.CotizacionCode, x.ArticuloCode })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}