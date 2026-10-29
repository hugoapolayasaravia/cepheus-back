using Cepheus.Domain.Contabilidad.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Maestros
{
    public class CuentaContableConfiguration : IEntityTypeConfiguration<CuentaContable>
    {
        public void Configure(EntityTypeBuilder<CuentaContable> builder)
        {
            builder.ToTable("CuentasContables", schema: "contabilidad");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code).IsRequired().HasColumnType("char(8)");
            builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(70)");

            builder.Property(x => x.TipoCuentaCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.TipoAnexoCode)
                .IsRequired(false)
                .HasColumnType("char(3)");

            builder.Property(x => x.LinkAccount1Code).IsRequired().HasColumnType("char(8)");
            builder.Property(x => x.LinkAccount2Code).IsRequired().HasColumnType("char(8)");
            builder.Property(x => x.ClosingAccountCode).IsRequired().HasColumnType("char(8)");

            builder.Property(x => x.CurrencyCode).IsRequired().HasColumnType("char(3)");

            builder.Property(x => x.AcmFlag)
                .IsRequired(false)
                .HasColumnType("char(1)");

            builder.Property(x => x.PilotFlag)
                .IsRequired(false)
                .HasColumnType("char(1)");

            builder.Property(x => x.GainsLossesFlag)
                .IsRequired(false)
                .HasColumnType("char(1)");

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.TipoCuenta)
                .WithMany()
                .HasForeignKey(x => x.TipoCuentaCode)
                .HasPrincipalKey(t => t.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.TipoAnexo)
                .WithMany()
                .HasForeignKey(x => x.TipoAnexoCode)
                .HasPrincipalKey(t => t.Code)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyCode)
                .HasPrincipalKey(m => m.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.TipoCuentaCode);
            builder.HasIndex(x => x.TipoAnexoCode);
            builder.HasIndex(x => x.CurrencyCode);
        }
    }
}
