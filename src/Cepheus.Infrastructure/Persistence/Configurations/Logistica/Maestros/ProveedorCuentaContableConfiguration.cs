using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class ProveedorCuentaContableConfiguration : IEntityTypeConfiguration<ProveedorCuentaContable>
    {
        public void Configure(EntityTypeBuilder<ProveedorCuentaContable> builder)
        {
            builder.ToTable("ProveedorCuentasContables", schema: "logistica");

            builder.HasKey(x => new { x.ProveedorCode, x.MonedaCode });

            builder.Property(x => x.ProveedorCode).IsRequired().HasColumnType("char(5)");
            builder.HasOne(x => x.Proveedor).WithMany().HasForeignKey(x => x.ProveedorCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.MonedaCode).IsRequired().HasMaxLength(3);
            builder.HasOne(x => x.Moneda).WithMany().HasForeignKey(x => x.MonedaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.AccountingAccountCode).IsRequired().HasColumnType("char(8)");
            builder.HasOne(x => x.CuentaContable).WithMany().HasForeignKey(x => x.AccountingAccountCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasIndex(x => x.AccountingAccountCode);
        }
    }
}
