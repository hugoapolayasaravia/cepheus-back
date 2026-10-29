using Cepheus.Domain.Comunes; // Directiva para la entidad Planta
using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class ArticuloUbicacionConfiguration : IEntityTypeConfiguration<ArticuloUbicacion>
    {
        public void Configure(EntityTypeBuilder<ArticuloUbicacion> builder)
        {
            builder.ToTable("ArticuloUbicaciones", schema: "logistica");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasColumnType("char(4)");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(80);

            builder.Property(x => x.PlantaCode)
                .IsRequired()
                .HasMaxLength(2);

            builder.Property(x => x.Zone)
                .HasMaxLength(40);

            builder.Property(x => x.Rack)
                .HasMaxLength(20);

            builder.Property(x => x.Shelf)
                .HasMaxLength(20);

            builder.Property(x => x.Locker)
                .HasMaxLength(20);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            // Relación con Planta
            builder.HasOne(x => x.Planta)
                .WithMany()
                .HasForeignKey(x => x.PlantaCode)
                .HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}