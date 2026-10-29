// Cepheus.Infrastructure/Persistence/Configurations/Logistica/Transacciones/CotizacionPedidoOrigenConfiguration.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones
{
    public class CotizacionPedidoOrigenConfiguration : IEntityTypeConfiguration<CotizacionPedidoOrigen>
    {
        public void Configure(EntityTypeBuilder<CotizacionPedidoOrigen> builder)
        {
            builder.ToTable("CotizacionPedidoOrigenes", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.CotizacionCode, x.ArticuloCode, x.PedidoCode, x.PedidoItemNumber });

            builder.HasOne(x => x.CotizacionDetalle)
                .WithMany(x => x.Origenes)
                .HasForeignKey(x => new { x.PlantaCode, x.CotizacionCode, x.ArticuloCode })
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.PedidoCode).IsRequired().HasColumnType("char(6)");
            builder.HasOne(x => x.PedidoDetalle)
                .WithMany(x => x.OrigenesCotizacion)
                .HasForeignKey(x => new { x.PlantaCode, x.PedidoCode, x.PedidoItemNumber })
                .OnDelete(DeleteBehavior.Restrict); // no cascada: no se borra un Pedido por borrar una Cotización

            builder.Property(x => x.CantidadTomada).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}