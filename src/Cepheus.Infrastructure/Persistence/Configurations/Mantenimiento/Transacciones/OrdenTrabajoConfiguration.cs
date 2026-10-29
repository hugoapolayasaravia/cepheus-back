using Cepheus.Domain.Mantenimiento.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Mantenimiento.Transacciones
{
    public class OrdenTrabajoConfiguration : IEntityTypeConfiguration<OrdenTrabajo>
    {
        public void Configure(EntityTypeBuilder<OrdenTrabajo> builder)
        {
            builder.ToTable("OrdenesTrabajo", schema: "mantenimiento");

            builder.HasKey(x => new { x.PlantaCode, x.Code });

            builder.Property(x => x.PlantaCode)
                .IsRequired()
                 .HasColumnType("char(2)");

            builder.Property(x => x.Code)
                .IsRequired()
                .HasColumnType("char(6)");

            builder.HasOne(x => x.Planta)
                .WithMany()
                .HasForeignKey(x => x.PlantaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Description).IsRequired().HasMaxLength(254);

            builder.Property(x => x.FechaProceso).IsRequired();

            // Sin FK real todavía: Trabajador no existe como tabla
            builder.Property(x => x.ResponsableCode)
                .IsRequired()
                .HasColumnType("char(5)");


            // Relación con Trabajador
            builder.HasOne(x => x.Responsable)
                .WithMany()
                .HasForeignKey(x => x.ResponsableCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Property(x => x.EspecialidadCode).IsRequired().HasMaxLength(1);
            builder.HasOne(x => x.Especialidad).WithMany().HasForeignKey(x => x.EspecialidadCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.OportunidadCode).IsRequired().HasMaxLength(2);
            builder.HasOne(x => x.Oportunidad).WithMany().HasForeignKey(x => x.OportunidadCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.EquipoCode).IsRequired().HasMaxLength(8);
            builder.HasOne(x => x.Equipo).WithMany().HasForeignKey(x => x.EquipoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.PrioridadCode).IsRequired().HasMaxLength(2);
            builder.HasOne(x => x.Prioridad).WithMany().HasForeignKey(x => x.PrioridadCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.InspeccionCode).IsRequired().HasMaxLength(2);
            builder.HasOne(x => x.Inspeccion).WithMany().HasForeignKey(x => x.InspeccionCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.TipoOrdenCode).IsRequired().HasMaxLength(3);
            builder.HasOne(x => x.TipoOrden).WithMany().HasForeignKey(x => x.TipoOrdenCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ActividadCode).IsRequired().HasMaxLength(6);
            builder.HasOne(x => x.Actividad).WithMany().HasForeignKey(x => x.ActividadCode)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Property(x => x.SubCentroCostoCode).HasMaxLength(6);
            builder.HasOne(x => x.SubCentroCosto).WithMany().HasForeignKey(x => x.SubCentroCostoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.SubCentroEjecutorCode).HasMaxLength(4);
            builder.HasOne(x => x.SubCentroEjecutor).WithMany().HasForeignKey(x => x.SubCentroEjecutorCode)
                .OnDelete(DeleteBehavior.Restrict);

            // Sin FK real todavía: módulo de Planes de Mantenimiento Preventivo no existe
            builder.Property(x => x.PlanMantenimientoPreventivoCode).HasMaxLength(6);

            builder.Property(x => x.DowntimeHours).IsRequired().HasColumnType("decimal(12,5)").HasDefaultValue(0);

            builder.Property(x => x.Estado).IsRequired().HasConversion<int>();

            builder.Property(x => x.Horometro).HasColumnType("decimal(10,2)");

            builder.Property(x => x.Observaciones).HasMaxLength(254);

            builder.Property(x => x.Turno).IsRequired().HasConversion<int>();

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
