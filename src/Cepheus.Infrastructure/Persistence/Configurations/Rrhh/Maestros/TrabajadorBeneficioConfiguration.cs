using Cepheus.Domain.Rrhh.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Maestros
{
    public class TrabajadorBeneficioConfiguration : IEntityTypeConfiguration<TrabajadorBeneficio>
    {
        public void Configure(EntityTypeBuilder<TrabajadorBeneficio> builder)
        {
            builder.ToTable("trabajador_beneficio", schema: "rrhh");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.TrabajadorCode).IsRequired();
            builder.HasOne(x => x.Trabajador)
                .WithMany()
                .HasForeignKey(x => x.TrabajadorCode)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:1 con Trabajador (igual que el legacy: UNIQUE(trabajador_id))
            builder.HasIndex(x => x.TrabajadorCode).IsUnique();

            builder.Property(x => x.Cts).IsRequired().HasDefaultValue(true);
            builder.Property(x => x.Gratificacion).IsRequired().HasDefaultValue(true);
            builder.Property(x => x.Vacaciones).IsRequired().HasDefaultValue(true);
            builder.Property(x => x.MovilidadAntesEntrada).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.MovilidadDespuesSalida).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Refrigerio).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Cena).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Vale).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
