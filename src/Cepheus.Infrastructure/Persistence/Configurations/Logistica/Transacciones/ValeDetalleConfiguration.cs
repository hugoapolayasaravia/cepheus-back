using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones;

public sealed class ValeDetalleConfiguration : IEntityTypeConfiguration<ValeDetalle>
{
    public void Configure(EntityTypeBuilder<ValeDetalle> builder)
    {
        builder.ToTable("ValeDetalles", "logistica");

        // PK legacy: (Codigo_Pla, Codigo_Val, Codigo_Art). Ninguna columna es nula.
        builder.HasKey(x => new { x.PlantaCode, x.ValeCode, x.ArticuloCode });

        builder.Property(x => x.PlantaCode).HasColumnType("char(2)").IsRequired();
        builder.Property(x => x.ValeCode).HasColumnType("char(7)").IsRequired();
        builder.Property(x => x.ArticuloCode).HasColumnType("char(7)").IsRequired();

        builder.HasOne(x => x.Articulo)
            .WithMany()
            .HasForeignKey(x => x.ArticuloCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.ItemNumber).IsRequired();

        builder.Property(x => x.Cantidad).HasColumnType("decimal(12,4)").IsRequired();
        builder.Property(x => x.Precio).HasColumnType("decimal(18,6)").IsRequired();
        builder.Property(x => x.Total).HasColumnType("decimal(12,2)").IsRequired();

        builder.Property(x => x.Estado).HasConversion<int>().IsRequired();

        builder.Property(x => x.AsientoContable).HasMaxLength(13);
        builder.Property(x => x.Propiedad01).HasMaxLength(7);

        builder.Property(x => x.CreatedBy).HasMaxLength(250);
        builder.Property(x => x.UpdatedBy).HasMaxLength(250);
        builder.Property(x => x.RowVersion).IsRowVersion();

        // No único: al eliminar una línea los ítems se renumeran 1..n y un índice único chocaría en el UPDATE.
        builder.HasIndex(x => new { x.PlantaCode, x.ValeCode, x.ItemNumber });

        // Reservado por artículo (Logi_sp_Consulta_Pendiente_Articulos): líneas pendientes por planta y artículo.
        builder.HasIndex(x => new { x.PlantaCode, x.ArticuloCode, x.Estado });
    }
}
