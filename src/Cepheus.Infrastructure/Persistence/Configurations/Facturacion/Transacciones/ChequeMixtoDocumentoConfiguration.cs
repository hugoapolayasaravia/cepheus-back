using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class ChequeMixtoDocumentoConfiguration : IEntityTypeConfiguration<ChequeMixtoDocumento>
    {
        public void Configure(EntityTypeBuilder<ChequeMixtoDocumento> builder)
        {
            builder.ToTable("ChequesMixtosDocumentos", schema: "facturacion");

            builder.HasKey(x => new { x.ChequeCode, x.ChequeType, x.ChequeNumber, x.BancoCode, x.TypeCode, x.DocumentNumber });

            builder.Property(x => x.ChequeCode).IsRequired().HasColumnType("char(10)");
            builder.Property(x => x.ChequeType).IsRequired().HasColumnType("char(1)");
            builder.Property(x => x.ChequeNumber).IsRequired().HasMaxLength(10);
            builder.Property(x => x.BancoCode).IsRequired().HasMaxLength(3);
            builder.Property(x => x.TypeCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.DocumentNumber).IsRequired().HasColumnType("char(15)");
            builder.Property(x => x.ProveedorCode).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.ClienteCode).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.CurrencyCode).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.ProvisionEntryNumber).HasMaxLength(13);

            builder.Property(x => x.DocumentTotalSoles).HasPrecision(12, 2);
            builder.Property(x => x.DocumentPendingSoles).HasPrecision(12, 2);
            builder.Property(x => x.PaidSoles).HasPrecision(12, 2);
            builder.Property(x => x.RealPaidSoles).HasPrecision(18, 2);
            builder.Property(x => x.DifferenceSoles).HasPrecision(12, 2);
            builder.Property(x => x.RetentionAmount).HasPrecision(18, 2);
            builder.Property(x => x.RetentionBase).HasPrecision(18, 2);
            builder.Property(x => x.DocumentExchangeRate).HasPrecision(7, 4);

            // Búsqueda del saldo aplicado de un documento de cliente.
            builder.HasIndex(x => new { x.ClienteCode, x.TypeCode, x.DocumentNumber, x.ChequeType });
        }
    }
}
