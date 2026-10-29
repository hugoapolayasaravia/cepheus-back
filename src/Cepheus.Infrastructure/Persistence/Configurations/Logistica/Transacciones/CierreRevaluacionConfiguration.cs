using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones;

public class CierreRevaluacionConfiguration : IEntityTypeConfiguration<CierreRevaluacion>
{
    public void Configure(EntityTypeBuilder<CierreRevaluacion> builder)
    {
        builder.ToTable("CierreRevaluaciones", schema: "logistica");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
        builder.Property(x => x.PeriodCode).IsRequired().HasColumnType("char(6)");
        builder.Property(x => x.Origen).IsRequired();
        builder.Property(x => x.DocumentoCode).IsRequired().HasMaxLength(20);
        builder.Property(x => x.ArticuloCode).IsRequired().HasColumnType("char(7)");

        builder.Property(x => x.Cantidad).IsRequired().HasColumnType("decimal(12,2)");
        builder.Property(x => x.PrecioAnterior).IsRequired().HasColumnType("decimal(18,6)");
        builder.Property(x => x.PrecioNuevo).IsRequired().HasColumnType("decimal(18,6)");
        builder.Property(x => x.TotalAnterior).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(x => x.TotalNuevo).IsRequired().HasColumnType("decimal(18,2)");

        builder.Property(x => x.CreatedBy).HasMaxLength(250);
        builder.Property(x => x.UpdatedBy).HasMaxLength(250);

        // Sin FK a planta/artículo: es una bitácora que debe sobrevivir a cambios en los maestros.
        builder.HasIndex(x => new { x.PlantaCode, x.PeriodCode })
            .HasDatabaseName("IX_CierreRevaluaciones_Periodo");
    }
}
