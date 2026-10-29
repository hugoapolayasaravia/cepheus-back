using Cepheus.Domain.Mantenimiento.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Mantenimiento.Transacciones
{
    public class OTRMaquinaConfiguration : IEntityTypeConfiguration<OTRMaquina>
    {
        public void Configure(EntityTypeBuilder<OTRMaquina> builder)
        {
            builder.ToTable("OTRMaquinas", schema: "mantenimiento");

            builder.HasKey(x => new { x.PlantaCode, x.OrdenTrabajoCode, x.MaquinaCode });

            builder.Property(x => x.PlantaCode)
                .IsRequired()
                 .HasColumnType("char(2)");
            builder.Property(x => x.OrdenTrabajoCode)
                .IsRequired()
                 .HasColumnType("char(6)");

            builder.HasOne(x => x.OrdenTrabajo)
                .WithMany()
                .HasForeignKey(x => new { x.PlantaCode, x.OrdenTrabajoCode })
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.MaquinaCode).IsRequired().HasMaxLength(4);
            builder.HasOne(x => x.Maquina)
                .WithMany()
                .HasForeignKey(x => x.MaquinaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.FechaProceso).IsRequired();

            builder.Property(x => x.Cantidad).IsRequired().HasColumnType("decimal(12,5)").HasDefaultValue(0);
            builder.Property(x => x.Horas).IsRequired().HasColumnType("decimal(10,2)").HasDefaultValue(0);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
