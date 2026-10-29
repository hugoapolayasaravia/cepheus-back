using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones;

public sealed class AjusteInventarioDetalleConfiguration : IEntityTypeConfiguration<AjusteInventarioDetalle>
{
    public void Configure(EntityTypeBuilder<AjusteInventarioDetalle> builder)
    {
        builder.ToTable("AjusteInventarioDetalles", "logistica");

        // PK legacy: (Codigo_Pla, Codigo_Aju, Codigo_Art). Ninguna columna es nula.
        builder.HasKey(x => new { x.PlantaCode, x.AjusteCode, x.ArticuloCode });

        builder.Property(x => x.PlantaCode).HasColumnType("char(2)").IsRequired();
        builder.Property(x => x.AjusteCode).HasColumnType("char(6)").IsRequired();
        builder.Property(x => x.ArticuloCode).HasColumnType("char(7)").IsRequired();

        builder.HasOne(x => x.Articulo)
            .WithMany()
            .HasForeignKey(x => x.ArticuloCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.ItemNumber).IsRequired();

        builder.Property(x => x.Tipo).HasConversion<int>().IsRequired();

        builder.Property(x => x.Cantidad).HasColumnType("decimal(12,4)").IsRequired();
        builder.Property(x => x.Precio).HasColumnType("decimal(18,6)").IsRequired();
        builder.Property(x => x.Total).HasColumnType("decimal(18,2)").IsRequired();

        builder.Property(x => x.Estado).HasConversion<int>().IsRequired();

        builder.Property(x => x.AsientoContable).HasMaxLength(13);

        builder.Property(x => x.CreatedBy).HasMaxLength(250);
        builder.Property(x => x.UpdatedBy).HasMaxLength(250);
        builder.Property(x => x.RowVersion).IsRowVersion();

        // No único: al eliminar una línea los ítems se renumeran 1..n y un índice único chocaría en el UPDATE.
        builder.HasIndex(x => new { x.PlantaCode, x.AjusteCode, x.ItemNumber });

        // Reservado por artículo: líneas pendientes por planta, artículo y tipo.
        builder.HasIndex(x => new { x.PlantaCode, x.ArticuloCode, x.Estado, x.Tipo });
    }
}
