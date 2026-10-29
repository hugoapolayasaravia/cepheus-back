// Cepheus.Infrastructure/Persistence/Configurations/Logistica/Transacciones/OrdenCompraPedidoOrigenConfiguration.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones
{
    public class OrdenCompraPedidoOrigenConfiguration : IEntityTypeConfiguration<OrdenCompraPedidoOrigen>
    {
        public void Configure(EntityTypeBuilder<OrdenCompraPedidoOrigen> builder)
        {
            builder.ToTable("OrdenCompraPedidoOrigenes", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.OrdenCompraCode, x.ArticuloCode, x.PedidoCode, x.PedidoItemNumber });

            builder.HasOne(x => x.OrdenCompraDetalle)
                .WithMany(x => x.Origenes)
                .HasForeignKey(x => new { x.PlantaCode, x.OrdenCompraCode, x.ArticuloCode })
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.PedidoCode).IsRequired().HasColumnType("char(6)");
            builder.HasOne(x => x.PedidoDetalle)
                .WithMany(x => x.OrigenesCompra)
                .HasForeignKey(x => new { x.PlantaCode, x.PedidoCode, x.PedidoItemNumber })
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CantidadTomada).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.CantidadEntregada).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}