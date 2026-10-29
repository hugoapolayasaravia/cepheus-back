using Cepheus.Domain.Mantenimiento.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Mantenimiento.Transacciones
{
    public class UnidadParadaConfiguration : IEntityTypeConfiguration<UnidadParada>
    {
        public void Configure(EntityTypeBuilder<UnidadParada> builder)
        {
            builder.ToTable("UnidadesParada", schema: "mantenimiento");

            builder.HasKey(x => new { x.SubCentroCostoCode, x.Fecha });

            builder.Property(x => x.SubCentroCostoCode)
                .IsRequired()
                .HasColumnType("char(6)");

            builder.Property(x => x.Fecha)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(x => x.Horas)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0m);

            builder.Property(x => x.Estado)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.PlantaAfectadaCode)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.Property(x => x.FallaCode)
                .HasColumnType("char(5)");

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();

            // Relaciones
            builder.HasOne(x => x.SubCentroCosto)
                .WithMany()
                .HasForeignKey(x => x.SubCentroCostoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PlantaAfectada)
                .WithMany()
                .HasForeignKey(x => x.PlantaAfectadaCode)
                .HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Falla)
                .WithMany()
                .HasForeignKey(x => x.FallaCode)
                .OnDelete(DeleteBehavior.Restrict);

            // Consultas por planta y por rango de fechas
            builder.HasIndex(x => new { x.PlantaAfectadaCode, x.Fecha });
        }
    }
}
