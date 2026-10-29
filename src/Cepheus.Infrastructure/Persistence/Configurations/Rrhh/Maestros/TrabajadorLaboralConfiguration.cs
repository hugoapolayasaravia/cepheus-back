using Cepheus.Domain.Rrhh.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Maestros
{
    public class TrabajadorLaboralConfiguration : IEntityTypeConfiguration<TrabajadorLaboral>
    {
        public void Configure(EntityTypeBuilder<TrabajadorLaboral> builder)
        {
            builder.ToTable("trabajador_laboral", schema: "rrhh");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.TrabajadorCode).IsRequired();
            builder.HasOne(x => x.Trabajador)
                .WithMany()
                .HasForeignKey(x => x.TrabajadorCode)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:1 con Trabajador (igual que el legacy: UNIQUE(trabajador_id))
            builder.HasIndex(x => x.TrabajadorCode).IsUnique();

            builder.HasOne(x => x.TipoTrabajador).WithMany().HasForeignKey(x => x.TipoTrabajadorCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.CategoriaTrabajador).WithMany().HasForeignKey(x => x.CategoriaTrabajadorCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.EstadoTrabajador).WithMany().HasForeignKey(x => x.EstadoTrabajadorCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Area).WithMany().HasForeignKey(x => x.AreaCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Ocupacion).WithMany().HasForeignKey(x => x.OcupacionCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.SubOcupacion).WithMany().HasForeignKey(x => x.SubOcupacionCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Oficina).WithMany().HasForeignKey(x => x.OficinaCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Planta).WithMany().HasForeignKey(x => x.PlantaCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Cargo).WithMany().HasForeignKey(x => x.CargoCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Nivel).WithMany().HasForeignKey(x => x.NivelCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.RegimenLaboral).WithMany().HasForeignKey(x => x.RegimenLaboralCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Proveedor).WithMany().HasForeignKey(x => x.ProveedorCode).OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
