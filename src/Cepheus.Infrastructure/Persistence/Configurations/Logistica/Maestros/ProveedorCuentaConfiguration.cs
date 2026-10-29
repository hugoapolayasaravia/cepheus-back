using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class ProveedorCuentaConfiguration : IEntityTypeConfiguration<ProveedorCuenta>
    {
        public void Configure(EntityTypeBuilder<ProveedorCuenta> builder)
        {
            builder.ToTable("ProveedorCuentas", schema: "logistica");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProveedorCode)
                .IsRequired()
                 .HasColumnType("char(5)");

            builder.Property(x => x.BancoCode)
                .IsRequired()
                 .HasColumnType("char(3)");

            builder.Property(x => x.AccountType)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.Property(x => x.AccountNumber)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.InterbankCode)
                .HasMaxLength(20);

            builder.Property(x => x.MonedaCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(x => x.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.HasOne(x => x.Proveedor)
                .WithMany()
                .HasForeignKey(x => x.ProveedorCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Banco)
                .WithMany()
                .HasForeignKey(x => x.BancoCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Moneda)
                .WithMany()
                .HasForeignKey(x => x.MonedaCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ProveedorCode);
        }
    }
}
