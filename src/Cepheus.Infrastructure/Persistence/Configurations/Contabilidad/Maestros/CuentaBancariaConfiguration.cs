using Cepheus.Domain.Contabilidad.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Maestros
{
    public class CuentaBancariaConfiguration : IEntityTypeConfiguration<CuentaBancaria>
    {
        public void Configure(EntityTypeBuilder<CuentaBancaria> builder)
        {
            builder.ToTable("CuentasBancarias", schema: "contabilidad");

            builder.HasKey(x => new { x.BankCode, x.AccountNumber });

            builder.Property(x => x.BankCode).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.AccountNumber).IsRequired().HasColumnType("char(20)");

            builder.Property(x => x.CurrencyCode).IsRequired().HasColumnType("char(3)");

            builder.Property(x => x.CheckStart).IsRequired().HasColumnType("char(10)");
            builder.Property(x => x.CheckEnd).IsRequired().HasColumnType("char(10)");
            builder.Property(x => x.CheckCurrent).IsRequired().HasColumnType("char(10)");

            builder.Property(x => x.ControlFlag).IsRequired().HasColumnType("char(1)");

            builder.Property(x => x.AccountCode).IsRequired().HasColumnType("char(8)");

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.Banco)
                .WithMany()
                .HasForeignKey(x => x.BankCode)
                .HasPrincipalKey(b => b.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyCode)
                .HasPrincipalKey(m => m.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Account)
                .WithMany()
                .HasForeignKey(x => x.AccountCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.CurrencyCode);
            builder.HasIndex(x => x.AccountCode);
        }
    }
}
