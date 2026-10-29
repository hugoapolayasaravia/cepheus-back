using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones;

public sealed class AjusteInventarioConfiguration : IEntityTypeConfiguration<AjusteInventario>
{
    public void Configure(EntityTypeBuilder<AjusteInventario> builder)
    {
        builder.ToTable("AjustesInventario", "logistica");

        builder.HasKey(x => new { x.PlantaCode, x.Code });

        builder.Property(x => x.PlantaCode).HasColumnType("char(2)").IsRequired();
        builder.Property(x => x.Code).HasColumnType("char(6)").IsRequired();

        builder.HasOne(x => x.Planta)
            .WithMany()
            .HasForeignKey(x => x.PlantaCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Observacion).HasMaxLength(1000);
        builder.Property(x => x.FechaEntrega).IsRequired();

        builder.Property(x => x.Neto).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.Igv).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.Total).HasColumnType("decimal(18,2)").IsRequired();

        // Enum persistido con el código legacy como valor (01, 12, 13, 14, 04).
        builder.Property(x => x.Estado).HasConversion<int>().IsRequired();

        builder.Property(x => x.AsientoContable).HasMaxLength(13);

        builder.Property(x => x.CreatedBy).HasMaxLength(250);
        builder.Property(x => x.UpdatedBy).HasMaxLength(250);
        builder.Property(x => x.RowVersion).IsRowVersion();

        // Búsquedas del listado legacy (Logi_sp_Listado_MAjustes).
        builder.HasIndex(x => new { x.PlantaCode, x.CreatedAt });
        builder.HasIndex(x => new { x.PlantaCode, x.Estado });

        builder.HasMany(x => x.Detalles)
            .WithOne(x => x.Ajuste)
            .HasForeignKey(x => new { x.PlantaCode, x.AjusteCode })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
