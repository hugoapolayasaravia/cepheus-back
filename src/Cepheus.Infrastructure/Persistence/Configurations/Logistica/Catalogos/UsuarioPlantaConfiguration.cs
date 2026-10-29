using Cepheus.Domain.Logistica.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Catalogos
{
    public class UsuarioPlantaConfiguration : IEntityTypeConfiguration<UsuarioPlanta>
    {
        public void Configure(EntityTypeBuilder<UsuarioPlanta> builder)
        {
            builder.ToTable("UsuariosPlanta", schema: "logistica");

            builder.HasKey(x => new { x.UserId, x.PlantaCode });

            builder.Property(x => x.PlantaCode)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();

            // Relaciones
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Planta)
                .WithMany()
                .HasForeignKey(x => x.PlantaCode)
                .HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Restrict);

            // Búsqueda inversa: usuarios de una planta
            builder.HasIndex(x => x.PlantaCode);
        }
    }
}
