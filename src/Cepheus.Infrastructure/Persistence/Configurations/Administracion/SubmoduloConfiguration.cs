using Cepheus.Domain.Administracion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Administracion
{
    public class SubmoduloConfiguration : IEntityTypeConfiguration<Submodulo>
    {
        public void Configure(EntityTypeBuilder<Submodulo> builder)
        {
            builder.ToTable("Submodulos", schema: "admin");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);
            //Se mostrará en el sidebar

            builder.Property(x => x.Icon)
                .HasMaxLength(50);

            builder.Property(x => x.Tooltip)
                .HasMaxLength(300);

            builder.Property(x => x.DisplayOrder)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Único DENTRO del módulo (no global) — mismo criterio que la PK
            // compuesta (submodul, subsubmo) del legacy.
            builder.HasIndex(x => new { x.ModuloId, x.Code }).IsUnique();
            builder.HasIndex(x => new { x.ModuloId, x.Name }).IsUnique();

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.Modulo)
                .WithMany(x => x.Submodulos)
                .HasForeignKey(x => x.ModuloId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }



}
