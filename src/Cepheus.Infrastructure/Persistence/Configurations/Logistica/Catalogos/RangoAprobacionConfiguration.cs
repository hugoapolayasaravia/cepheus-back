using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class RangoAprobacionConfiguration : IEntityTypeConfiguration<RangoAprobacion>
    {
        public void Configure(EntityTypeBuilder<RangoAprobacion> builder)
        {
            builder.ToTable("RangosAprobacion", schema: "logistica");

            builder.HasKey(x => new { x.NivelCode, x.TipoTransaccionCode, x.UnidadNegocioCode, x.MonedaCode });

            builder.Property(x => x.NivelCode).IsRequired().HasMaxLength(2);
            builder.HasOne(x => x.Nivel).WithMany().HasForeignKey(x => x.NivelCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.TipoTransaccionCode).IsRequired().HasMaxLength(2);
            builder.HasOne(x => x.TipoTransaccion).WithMany().HasForeignKey(x => x.TipoTransaccionCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.UnidadNegocioCode).IsRequired().HasMaxLength(6);
            builder.HasOne(x => x.UnidadNegocio).WithMany().HasForeignKey(x => x.UnidadNegocioCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.MonedaCode).IsRequired().HasMaxLength(3);
            builder.HasOne(x => x.Moneda).WithMany().HasForeignKey(x => x.MonedaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ImporteMinimo).IsRequired().HasColumnType("decimal(18,4)").HasDefaultValue(0);
            builder.Property(x => x.ImporteMaximo).IsRequired().HasColumnType("decimal(18,4)").HasDefaultValue(0);
            builder.Property(x => x.ImporteAcumuladoDiario).IsRequired().HasColumnType("decimal(18,4)").HasDefaultValue(0);
            builder.Property(x => x.ImporteAcumuladoMensual).IsRequired().HasColumnType("decimal(18,4)").HasDefaultValue(0);
            builder.Property(x => x.PorcentajeTotal).HasColumnType("decimal(5,2)");

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
