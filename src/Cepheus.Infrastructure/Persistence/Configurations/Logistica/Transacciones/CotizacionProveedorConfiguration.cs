// Cepheus.Infrastructure/Persistence/Configurations/Logistica/Transacciones/CotizacionProveedorConfiguration.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones
{
    public class CotizacionProveedorConfiguration : IEntityTypeConfiguration<CotizacionProveedor>
    {
        public void Configure(EntityTypeBuilder<CotizacionProveedor> builder)
        {
            builder.ToTable("CotizacionProveedores", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.CotizacionCode, x.ProveedorCode });

            builder.HasOne(x => x.Cotizacion)
                .WithMany(x => x.Proveedores)
                .HasForeignKey(x => new { x.PlantaCode, x.CotizacionCode })
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ProveedorCode).IsRequired().HasColumnType("char(5)");
            builder.HasOne(x => x.Proveedor).WithMany().HasForeignKey(x => x.ProveedorCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.MonedaCode).IsRequired().HasMaxLength(3);
            builder.HasOne(x => x.Moneda).WithMany().HasForeignKey(x => x.MonedaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.NetoCotizacion).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);
            builder.Property(x => x.IgvCotizacion).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);
            builder.Property(x => x.TotalCotizacion).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);

            builder.Property(x => x.Observaciones).IsRequired().HasMaxLength(200).HasDefaultValue(string.Empty);

            builder.Property(x => x.Estado).IsRequired().HasConversion<int>();

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasMany(x => x.Detalles)
                .WithOne(x => x.CotizacionProveedor)
                .HasForeignKey(x => new { x.PlantaCode, x.CotizacionCode, x.ProveedorCode })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}