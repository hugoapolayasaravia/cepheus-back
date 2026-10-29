using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones;

public class MovimientoArticuloConfiguration : IEntityTypeConfiguration<MovimientoArticulo>
{
    public void Configure(EntityTypeBuilder<MovimientoArticulo> builder)
    {
        builder.ToTable("MovimientosArticulo", schema: "logistica");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
        builder.Property(x => x.ArticuloCode).IsRequired().HasColumnType("char(7)");
        builder.Property(x => x.FechaMovimiento).IsRequired().HasColumnType("date");
        builder.Property(x => x.PeriodCode).IsRequired().HasColumnType("char(6)");

        builder.Property(x => x.Tipo).IsRequired();
        builder.Property(x => x.Origen).IsRequired();

        builder.Property(x => x.DocumentoCode).IsRequired().HasMaxLength(20);
        builder.Property(x => x.ItemNumber);

        builder.Property(x => x.Cantidad).IsRequired().HasColumnType("decimal(12,2)");
        builder.Property(x => x.CostoUnitario).IsRequired().HasColumnType("decimal(18,6)");
        builder.Property(x => x.CostoUnitarioUsd).IsRequired().HasColumnType("decimal(18,6)");
        builder.Property(x => x.StockPosterior).IsRequired().HasColumnType("decimal(12,2)");
        builder.Property(x => x.CostoPromedioPosterior).IsRequired().HasColumnType("decimal(18,6)");

        builder.Property(x => x.EsReversion).IsRequired().HasDefaultValue(false);

        builder.Property(x => x.CreatedBy).HasMaxLength(250);
        builder.Property(x => x.UpdatedBy).HasMaxLength(250);

        builder.HasOne(x => x.Planta)
            .WithMany()
            .HasForeignKey(x => x.PlantaCode)
            .HasPrincipalKey(x => x.Code)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Articulo)
            .WithMany()
            .HasForeignKey(x => x.ArticuloCode)
            .HasPrincipalKey(x => x.Code)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ReversaDeMovimiento)
            .WithMany()
            .HasForeignKey(x => x.ReversaDeMovimientoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Kardex de un artículo: filtra por planta + artículo y ordena por fecha y orden de llegada.
        builder.HasIndex(x => new { x.PlantaCode, x.ArticuloCode, x.FechaMovimiento, x.Id })
            .HasDatabaseName("IX_MovimientosArticulo_Kardex");

        // Cierre mensual: todos los movimientos de un período de una planta, por artículo.
        builder.HasIndex(x => new { x.PlantaCode, x.PeriodCode, x.ArticuloCode, x.FechaMovimiento, x.Id })
            .HasDatabaseName("IX_MovimientosArticulo_Periodo");

        // Búsqueda del movimiento original al anular un documento.
        builder.HasIndex(x => new { x.PlantaCode, x.Origen, x.DocumentoCode })
            .HasDatabaseName("IX_MovimientosArticulo_Documento");

        // Un movimiento solo puede ser revertido una vez. Índice único sobre una columna que admite null:
        // el proveedor de cada motor decide cómo tratar los nulos (SQL Server agrega solo el filtro IS NOT NULL;
        // PostgreSQL ya admite varios nulos). Sin SQL propio del motor. La cantidad positiva y el tipo válido
        // se validan en ArticuloStockMovementService.
    }
}
