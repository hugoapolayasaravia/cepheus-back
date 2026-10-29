// Cepheus.Infrastructure/Persistence/Configurations/Logistica/Transacciones/CotizacionProveedorDetalleConfiguration.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones
{
    public class CotizacionProveedorDetalleConfiguration : IEntityTypeConfiguration<CotizacionProveedorDetalle>
    {
        public void Configure(EntityTypeBuilder<CotizacionProveedorDetalle> builder)
        {
            builder.ToTable("CotizacionProveedorDetalles", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.CotizacionCode, x.ProveedorCode, x.ArticuloCode });

            builder.HasOne(x => x.CotizacionProveedor)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => new { x.PlantaCode, x.CotizacionCode, x.ProveedorCode })
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ArticuloCode).IsRequired().HasColumnType("char(7)");
            builder.HasOne(x => x.Articulo).WithMany().HasForeignKey(x => x.ArticuloCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CantidadArticulo).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);
            builder.Property(x => x.PrecioArticulo).IsRequired().HasColumnType("decimal(18,4)").HasDefaultValue(0);
            builder.Property(x => x.DescuentoArticulo).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);
            builder.Property(x => x.TotalLinea).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}