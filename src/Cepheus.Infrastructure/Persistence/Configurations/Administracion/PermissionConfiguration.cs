using Cepheus.Domain.Administracion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Administracion
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions", schema: "admin");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Único DENTRO del Programa (no global) — mismo criterio que
            // Submodulo dentro de Modulo y Programa dentro de Submodulo.
            builder.HasIndex(x => new { x.ProgramaId, x.Code }).IsUnique();

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.Programa)
                .WithMany(x => x.Permissions)
                .HasForeignKey(x => x.ProgramaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }


}
