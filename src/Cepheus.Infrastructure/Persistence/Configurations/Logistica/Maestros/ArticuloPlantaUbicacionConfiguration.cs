using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class ArticuloPlantaUbicacionConfiguration : IEntityTypeConfiguration<ArticuloPlantaUbicacion>
    {
        public void Configure(EntityTypeBuilder<ArticuloPlantaUbicacion> builder)
        {
            builder.ToTable("ArticuloPlantaUbicaciones", schema: "logistica");

            builder.HasKey(x => new { x.ArticuloCode, x.PlantaCode });

            builder.Property(x => x.ArticuloCode)
                .IsRequired()
                .HasColumnType("char(7)");

            builder.Property(x => x.PlantaCode)
                .IsRequired()
                .HasMaxLength(2);

            // Ajustado a char(4) para coincidir exactamente con ArticuloUbicacion.Code
            builder.Property(x => x.ArticuloUbicacionCode)
                .IsRequired()
                .HasColumnType("char(4)");

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            // Relaciones
            builder.HasOne(x => x.Articulo)
                .WithMany()
                .HasForeignKey(x => x.ArticuloCode)
                .HasPrincipalKey(a => a.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Planta)
                .WithMany()
                .HasForeignKey(x => x.PlantaCode)
                .HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ArticuloUbicacion)
                .WithMany()
                .HasForeignKey(x => x.ArticuloUbicacionCode)
                .HasPrincipalKey(u => u.Code)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}