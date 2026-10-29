using Cepheus.Domain.Mantenimiento.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Mantenimiento.Maestros
{
    public class PlanMantenimientoPreventivoConfiguration : IEntityTypeConfiguration<PlanMantenimientoPreventivo>
    {
        public void Configure(EntityTypeBuilder<PlanMantenimientoPreventivo> builder)
        {
            builder.ToTable("PlanesMantenimientoPreventivo", schema: "mantenimiento");

            builder.HasKey(x => new { x.PlantaCode, x.Code });

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Code).IsRequired().HasColumnType("char(6)");
            builder.Property(x => x.Description).IsRequired().HasMaxLength(100);

            builder.Property(x => x.EquipoCode).IsRequired().HasColumnType("char(8)");
            builder.Property(x => x.EspecialidadCode).IsRequired().HasColumnType("char(1)");
            builder.Property(x => x.ResponsableCode).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.SubCentroCostoCode).IsRequired().HasColumnType("char(6)");
            builder.Property(x => x.SubCentroEjecutorCode).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.ActividadCode).IsRequired().HasColumnType("char(6)");

            builder.Property(x => x.Frecuencia).HasColumnType("decimal(12,2)").HasDefaultValue(0m);
            builder.Property(x => x.UnidadFrecuencia).IsRequired().HasConversion<int>();
            builder.Property(x => x.Factor).HasColumnType("decimal(12,2)").HasDefaultValue(0m);

            builder.Property(x => x.FechaInicio).HasColumnType("date");
            builder.Property(x => x.FechaTermino).HasColumnType("date");

            builder.Property(x => x.FrecuenciaGama).HasColumnType("decimal(18,0)").HasDefaultValue(0m);
            builder.Property(x => x.TiempoRestante).HasColumnType("decimal(18,0)").HasDefaultValue(0m);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Planta).WithMany()
                .HasForeignKey(x => x.PlantaCode).HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Equipo).WithMany()
                .HasForeignKey(x => x.EquipoCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Especialidad).WithMany()
                .HasForeignKey(x => x.EspecialidadCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Responsable).WithMany()
                .HasForeignKey(x => x.ResponsableCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.SubCentroCosto).WithMany()
                .HasForeignKey(x => x.SubCentroCostoCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.SubCentroEjecutor).WithMany()
                .HasForeignKey(x => x.SubCentroEjecutorCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Actividad).WithMany()
                .HasForeignKey(x => x.ActividadCode).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.ManoObra).WithOne(d => d.Plan)
                .HasForeignKey(d => new { d.PlantaCode, d.PlanCode })
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Materiales).WithOne(d => d.Plan)
                .HasForeignKey(d => new { d.PlantaCode, d.PlanCode })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.EquipoCode);
        }
    }
}
