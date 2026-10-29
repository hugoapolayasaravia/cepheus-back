using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones;

public sealed class NotaIngresoDetalleConfiguration : IEntityTypeConfiguration<NotaIngresoDetalle>
{
    public void Configure(EntityTypeBuilder<NotaIngresoDetalle> builder)
    {
        builder.ToTable("NotaIngresoDetalles", "logistica");

        // Mismo criterio que PedidoDetalle: PK compuesta con el correlativo de línea (1..n, lo asigna el servidor).
        builder.HasKey(x => new { x.PlantaCode, x.NotaIngresoCode, x.ItemNumber });

        builder.Property(x => x.PlantaCode).HasColumnType("char(2)").IsRequired();
        builder.Property(x => x.NotaIngresoCode).HasColumnType("char(6)").IsRequired();
        builder.Property(x => x.ItemNumber).IsRequired();
        builder.Property(x => x.ArticuloCode).HasColumnType("char(7)").IsRequired();
        builder.Property(x => x.PedidoCode).HasColumnType("char(6)");
        builder.Property(x => x.GuiaCode).HasColumnType("char(6)");

        builder.HasOne(x => x.Articulo)
            .WithMany()
            .HasForeignKey(x => x.ArticuloCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Pedido)
            .WithMany()
            .HasForeignKey(x => new { x.PlantaCode, x.PedidoCode })
            .HasPrincipalKey(x => new { x.PlantaCode, x.Code })
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Cantidad).HasColumnType("decimal(12,4)").IsRequired();
        builder.Property(x => x.Precio).HasColumnType("decimal(12,6)").IsRequired();
        builder.Property(x => x.Descuento).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Total).HasColumnType("decimal(12,2)").IsRequired();

        builder.Property(x => x.Estado).HasConversion<int>().IsRequired();

        builder.Property(x => x.CreatedBy).HasMaxLength(250);
        builder.Property(x => x.UpdatedBy).HasMaxLength(250);
        builder.Property(x => x.RowVersion).IsRowVersion();

        // PK legacy: (Planta, NotaIngreso, Artículo, Pedido, Guía). HasFilter(null) evita el
        // filtro "IS NOT NULL" que EF agrega por defecto a índices únicos con columnas nulables,
        // de modo que dos líneas del mismo artículo sin pedido/guía también colisionen.
        builder.HasIndex(x => new { x.PlantaCode, x.NotaIngresoCode, x.ArticuloCode, x.PedidoCode, x.GuiaCode })
            .IsUnique()
            .HasFilter(null);
    }
}
