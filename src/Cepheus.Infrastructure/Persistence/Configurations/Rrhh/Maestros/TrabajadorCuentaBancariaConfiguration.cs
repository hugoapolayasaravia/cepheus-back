using Cepheus.Domain.Rrhh.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Maestros
{
    public class TrabajadorCuentaBancariaConfiguration : IEntityTypeConfiguration<TrabajadorCuentaBancaria>
    {
        public void Configure(EntityTypeBuilder<TrabajadorCuentaBancaria> builder)
        {
            builder.ToTable("trabajador_cuenta_bancaria", schema: "rrhh");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.TrabajadorCode).IsRequired();
            builder.HasOne(x => x.Trabajador)
                .WithMany()
                .HasForeignKey(x => x.TrabajadorCode)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.TipoCuenta).WithMany().HasForeignKey(x => x.TipoCuentaCode).OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.BancoCode).HasMaxLength(3);
            builder.HasOne(x => x.Banco).WithMany().HasForeignKey(x => x.BancoCode).OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.MonedaCode).HasMaxLength(3);
            builder.HasOne(x => x.Moneda).WithMany().HasForeignKey(x => x.MonedaCode).OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.NumeroCuenta).HasMaxLength(50);
            builder.Property(x => x.TipoOperacion).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Principal).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
