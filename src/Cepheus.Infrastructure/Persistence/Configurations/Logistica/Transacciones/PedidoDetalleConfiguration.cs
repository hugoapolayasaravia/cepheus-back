// Cepheus.Infrastructure/Persistence/Configurations/Logistica/Transacciones/PedidoDetalleConfiguration.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones
{
    public class PedidoDetalleConfiguration : IEntityTypeConfiguration<PedidoDetalle>
    {
        public void Configure(EntityTypeBuilder<PedidoDetalle> builder)
        {
            builder.ToTable("PedidoDetalles", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.PedidoCode, x.ItemNumber });

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.PedidoCode).IsRequired().HasColumnType("char(6)");

            builder.HasOne(x => x.Pedido)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => new { x.PlantaCode, x.PedidoCode })
                .OnDelete(DeleteBehavior.Cascade);

            // Sin FK: ArticuloCode puede no corresponder a un Articulo real
            // (línea de descripción libre) — ver comentario en la entidad.
            builder.Property(x => x.ArticuloCode).HasMaxLength(7);

            builder.Property(x => x.DescripcionArticulo).IsRequired().HasMaxLength(80);

            builder.Property(x => x.UnidadMedidaCode).IsRequired().HasMaxLength(2).HasDefaultValue("UN");
            builder.HasOne(x => x.UnidadMedida).WithMany().HasForeignKey(x => x.UnidadMedidaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.PrecioArticulo).IsRequired().HasColumnType("decimal(12,5)").HasDefaultValue(0);
            builder.Property(x => x.CantidadArticulo).IsRequired().HasColumnType("decimal(12,5)").HasDefaultValue(0);
            builder.Property(x => x.TotalArticulo).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);

            builder.Property(x => x.CantidadCotizada).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);

            builder.Property(x => x.CantidadEnCompra).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);

            builder.Property(x => x.EstadoPedidoDetalle).IsRequired().HasConversion<int>();

            builder.Property(x => x.OrdenCompraCode).HasColumnType("char(6)");

            builder.Property(x => x.ProveedorCode).HasColumnType("char(5)");
            builder.HasOne(x => x.Proveedor)
                .WithMany()
                .HasForeignKey(x => x.ProveedorCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}