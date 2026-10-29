using Cepheus.Domain.Mantenimiento.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Mantenimiento.Maestros
{
    public class ActividadMantenimientoConfiguration : IEntityTypeConfiguration<ActividadMantenimiento>
    {
        public void Configure(EntityTypeBuilder<ActividadMantenimiento> builder)
        {
            builder.ToTable("ActividadesMantenimiento", schema: "mantenimiento");

            builder.HasKey(x => new { x.ActividadCode, x.ActividadCubiertaCode });

            builder.Property(x => x.ActividadCode).IsRequired().HasColumnType("char(6)");
            builder.Property(x => x.ActividadCubiertaCode).IsRequired().HasColumnType("char(6)");

            builder.Property(x => x.Prioridad).IsRequired().HasDefaultValue(0);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Actividad).WithMany()
                .HasForeignKey(x => x.ActividadCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.ActividadCubierta).WithMany()
                .HasForeignKey(x => x.ActividadCubiertaCode).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
