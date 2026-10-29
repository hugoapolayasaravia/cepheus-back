using Cepheus.Domain.Administracion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Administracion
{
    public class ProgramaConfiguration : IEntityTypeConfiguration<Programa>
    {
        public void Configure(EntityTypeBuilder<Programa> builder)
        {
            builder.ToTable("Programas", schema: "admin");

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

            builder.Property(x => x.Route)
                .HasMaxLength(200);

            builder.Property(x => x.DisplayOrder)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Único DENTRO del submódulo (no global) — mismo criterio que Submodulo
            // dentro de Modulo, y que la PK compuesta (prgmodul,prgsubmo,prgprogr) del legacy.
            builder.HasIndex(x => new { x.SubmoduloId, x.Code }).IsUnique();
            builder.HasIndex(x => new { x.SubmoduloId, x.Name }).IsUnique();

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.Submodulo)
                .WithMany(x => x.Programas)
                .HasForeignKey(x => x.SubmoduloId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }



}
