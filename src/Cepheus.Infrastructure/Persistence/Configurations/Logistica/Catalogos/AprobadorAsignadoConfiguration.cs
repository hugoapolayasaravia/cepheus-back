using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class AprobadorAsignadoConfiguration : IEntityTypeConfiguration<AprobadorAsignado>
    {
        public void Configure(EntityTypeBuilder<AprobadorAsignado> builder)
        {
            builder.ToTable("AprobadoresAsignados", schema: "logistica");

            builder.HasKey(x => new { x.NivelCode, x.TipoTransaccionCode, x.UnidadNegocioCode, x.MonedaCode, x.TrabajadorCode });

            builder.Property(x => x.NivelCode).IsRequired().HasMaxLength(2);
            builder.Property(x => x.TipoTransaccionCode).IsRequired().HasMaxLength(2);
            builder.Property(x => x.UnidadNegocioCode).IsRequired().HasMaxLength(6);
            builder.Property(x => x.MonedaCode).IsRequired().HasMaxLength(3);

            builder.HasOne(x => x.RangoAprobacion)
                .WithMany()
                .HasForeignKey(x => new { x.NivelCode, x.TipoTransaccionCode, x.UnidadNegocioCode, x.MonedaCode })
                .OnDelete(DeleteBehavior.Cascade);

            // Sin FK real todavía: Trabajador no existe como tabla
            builder.Property(x => x.TrabajadorCode)
                .IsRequired()
                 .HasColumnType("char(5)");

            builder.HasOne(x => x.Trabajador)
            .WithMany()
            .HasForeignKey(x => x.TrabajadorCode)
            .HasPrincipalKey(x => x.Code)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.SuplenteTrabajadorCode).HasMaxLength(5);
            builder.Property(x => x.SuperiorTrabajadorCode).HasMaxLength(5);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
