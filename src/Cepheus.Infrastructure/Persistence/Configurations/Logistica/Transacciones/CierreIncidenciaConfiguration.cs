using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones;

public class CierreIncidenciaConfiguration : IEntityTypeConfiguration<CierreIncidencia>
{
    public void Configure(EntityTypeBuilder<CierreIncidencia> builder)
    {
        builder.ToTable("CierreIncidencias", schema: "logistica");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
        builder.Property(x => x.PeriodCode).IsRequired().HasColumnType("char(6)");
        builder.Property(x => x.ArticuloCode).IsRequired().HasColumnType("char(7)");
        builder.Property(x => x.Tipo).IsRequired();
        builder.Property(x => x.DocumentoCode).HasMaxLength(20);
        builder.Property(x => x.Mensaje).IsRequired().HasMaxLength(500);

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

        builder.HasIndex(x => new { x.PlantaCode, x.PeriodCode, x.Tipo })
            .HasDatabaseName("IX_CierreIncidencias_Periodo");
    }
}
