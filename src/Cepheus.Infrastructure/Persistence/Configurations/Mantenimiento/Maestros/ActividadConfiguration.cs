using Cepheus.Domain.Mantenimiento.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Mantenimiento.Maestros
{
    public class ActividadConfiguration : IEntityTypeConfiguration<Actividad>
    {
        public void Configure(EntityTypeBuilder<Actividad> builder)
        {
            builder.ToTable("Actividades", schema: "mantenimiento");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(6)");

            builder.Property(x => x.VerboActividadCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.HasOne(x => x.VerboActividad)
                .WithMany()
                .HasForeignKey(x => x.VerboActividadCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ObjetoActividadCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.HasOne(x => x.ObjetoActividad)
                .WithMany()
                .HasForeignKey(x => x.ObjetoActividadCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}
