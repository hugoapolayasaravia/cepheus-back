using Cepheus.Domain.Rrhh.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Maestros
{
    public class TrabajadorContableConfiguration : IEntityTypeConfiguration<TrabajadorContable>
    {
        public void Configure(EntityTypeBuilder<TrabajadorContable> builder)
        {
            builder.ToTable("trabajador_contable", schema: "rrhh");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.TrabajadorCode).IsRequired();
            builder.HasOne(x => x.Trabajador)
                .WithMany()
                .HasForeignKey(x => x.TrabajadorCode)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.CuentaContable).HasMaxLength(30);
            builder.Property(x => x.Tipo).HasMaxLength(20);
            builder.Property(x => x.Porcentaje).IsRequired().HasColumnType("decimal(7,4)").HasDefaultValue(0);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
