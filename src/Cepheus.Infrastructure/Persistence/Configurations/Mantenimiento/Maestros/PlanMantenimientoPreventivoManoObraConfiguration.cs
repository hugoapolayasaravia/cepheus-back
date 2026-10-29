using Cepheus.Domain.Mantenimiento.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Mantenimiento.Maestros
{
    public class PlanMantenimientoPreventivoManoObraConfiguration : IEntityTypeConfiguration<PlanMantenimientoPreventivoManoObra>
    {
        public void Configure(EntityTypeBuilder<PlanMantenimientoPreventivoManoObra> builder)
        {
            builder.ToTable("PlanesMantenimientoPreventivoManoObra", schema: "mantenimiento");

            builder.HasKey(x => new { x.PlantaCode, x.PlanCode, x.TrabajadorCode });

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.PlanCode).IsRequired().HasColumnType("char(6)");
            builder.Property(x => x.TrabajadorCode).IsRequired().HasColumnType("char(5)");

            builder.Property(x => x.Tiempo).HasColumnType("decimal(12,5)").HasDefaultValue(0m);
            builder.Property(x => x.CostoTotal).HasColumnType("decimal(12,5)").HasDefaultValue(0m);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Trabajador).WithMany()
                .HasForeignKey(x => x.TrabajadorCode).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
