using Cepheus.Domain.Rrhh.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Maestros
{
    public class TrabajadorRemuneracionConfiguration : IEntityTypeConfiguration<TrabajadorRemuneracion>
    {
        public void Configure(EntityTypeBuilder<TrabajadorRemuneracion> builder)
        {
            builder.ToTable("trabajador_remuneracion", schema: "rrhh");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.TrabajadorCode).IsRequired();
            builder.HasOne(x => x.Trabajador)
                .WithMany()
                .HasForeignKey(x => x.TrabajadorCode)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:1 con Trabajador (igual que el legacy: UNIQUE(trabajador_id))
            builder.HasIndex(x => x.TrabajadorCode).IsUnique();

            builder.Property(x => x.SueldoBasico).IsRequired().HasColumnType("decimal(18,4)").HasDefaultValue(0);
            builder.Property(x => x.MonedaCode).HasMaxLength(3);
            builder.HasOne(x => x.Moneda).WithMany().HasForeignKey(x => x.MonedaCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.ModoPago).WithMany().HasForeignKey(x => x.ModoPagoCode).OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
