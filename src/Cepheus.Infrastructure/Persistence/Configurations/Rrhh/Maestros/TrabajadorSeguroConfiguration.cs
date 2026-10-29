using Cepheus.Domain.Rrhh.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Maestros
{
    public class TrabajadorSeguroConfiguration : IEntityTypeConfiguration<TrabajadorSeguro>
    {
        public void Configure(EntityTypeBuilder<TrabajadorSeguro> builder)
        {
            builder.ToTable("trabajador_seguro", schema: "rrhh");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.TrabajadorCode).IsRequired();
            builder.HasOne(x => x.Trabajador)
                .WithMany()
                .HasForeignKey(x => x.TrabajadorCode)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:1 con Trabajador (igual que el legacy: UNIQUE(trabajador_id))
            builder.HasIndex(x => x.TrabajadorCode).IsUnique();

            builder.HasOne(x => x.Eps).WithMany().HasForeignKey(x => x.EpsCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.SituacionEps).WithMany().HasForeignKey(x => x.SituacionEpsCode).OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.NumeroSeguro).HasMaxLength(50);
            builder.HasOne(x => x.TipoSctr).WithMany().HasForeignKey(x => x.SctrTipoCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.SctrSalud).WithMany().HasForeignKey(x => x.SctrSaludCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.SctrPension).WithMany().HasForeignKey(x => x.SctrPensionCode).OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.EpsActivo).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.SeguroMedico).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.EssaludVida).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
