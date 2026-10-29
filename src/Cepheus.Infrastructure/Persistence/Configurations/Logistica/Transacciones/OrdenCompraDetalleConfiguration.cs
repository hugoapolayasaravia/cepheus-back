// Cepheus.Infrastructure/Persistence/Configurations/Logistica/Transacciones/OrdenCompraDetalleConfiguration.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones
{
    public class OrdenCompraDetalleConfiguration : IEntityTypeConfiguration<OrdenCompraDetalle>
    {
        public void Configure(EntityTypeBuilder<OrdenCompraDetalle> builder)
        {
            builder.ToTable("OrdenCompraDetalles", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.OrdenCompraCode, x.ArticuloCode });

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.OrdenCompraCode).IsRequired().HasColumnType("char(6)");

            builder.HasOne(x => x.OrdenCompra)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => new { x.PlantaCode, x.OrdenCompraCode })
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ArticuloCode).IsRequired().HasColumnType("char(7)");
            builder.HasOne(x => x.Articulo).WithMany().HasForeignKey(x => x.ArticuloCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CantidadArticulo).IsRequired().HasColumnType("decimal(12,5)").HasDefaultValue(0);
            builder.Property(x => x.PrecioArticulo).IsRequired().HasColumnType("decimal(12,6)").HasDefaultValue(0);
            builder.Property(x => x.DescuentoArticulo).IsRequired().HasColumnType("decimal(12,5)").HasDefaultValue(0);
            builder.Property(x => x.TotalArticulo).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.CantidadEntregada).IsRequired().HasColumnType("decimal(12,5)").HasDefaultValue(0);

            builder.Property(x => x.SubCentroCostoCode).IsRequired().HasMaxLength(6);
            builder.HasOne(x => x.SubCentroCosto).WithMany().HasForeignKey(x => x.SubCentroCostoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasMany(x => x.Origenes)
                .WithOne(x => x.OrdenCompraDetalle)
                .HasForeignKey(x => new { x.PlantaCode, x.OrdenCompraCode, x.ArticuloCode })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}