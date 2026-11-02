
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class CancelacionVentaDetalleConfiguration : IEntityTypeConfiguration<CancelacionVentaDetalle>
    {
        public void Configure(EntityTypeBuilder<CancelacionVentaDetalle> builder)
        {
            builder.ToTable("CancelacionesVentaDetalle", schema: "facturacion");

            builder.HasKey(x => new
            {
                x.CancelacionCode,
                x.Item
            });

            builder.Property(x => x.CancelacionCode)
                .IsRequired()
                .HasColumnType("char(10)");

            builder.Property(x => x.Item)
                .IsRequired()
                .HasColumnType("char(3)");

            builder.Property(x => x.PlantaCode)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.Property(x => x.ClienteCode)
                .IsRequired()
                .HasColumnType("char(5)");

            builder.Property(x => x.CancelacionDate)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(x => x.TypeCode)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.Property(x => x.DocumentNumber)
                .IsRequired()
                .HasColumnType("char(15)");

            builder.Property(x => x.DocumentBancoCode)
                .HasColumnType("char(3)");

            builder.Property(x => x.DocumentDate)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(x => x.DocumentCurrencyCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(x => x.DocumentExchangeRate)
                .IsRequired()
                .HasColumnType("decimal(18,4)");

            builder.Property(x => x.NetAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.PaidAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.PaidSoles)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DifferenceSoles)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.ProvisionEntryNumber)
                .HasColumnType("char(13)");

            builder.Property(x => x.ObraCode)
                .HasColumnType("char(3)");

            builder.Property(x => x.IsDoubtful)
                .IsRequired();

            builder.Property(x => x.DoubtfulExchangeRate)
                .IsRequired()
                .HasColumnType("decimal(18,4)");

            builder.Property(x => x.IsDetraction)
                .IsRequired();

            // Auditoría
            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2");

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            // Concurrencia optimista
            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            // Relación con cabecera
            builder.HasOne(x => x.Cancelacion)
                .WithMany(c => c.Details)
                .HasForeignKey(x => x.CancelacionCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(x => x.ClienteCode);
            builder.HasIndex(x => new
            {
                x.TypeCode,
                x.DocumentNumber
            });

            builder.HasIndex(x => new
            {
                x.PlantaCode,
                x.ClienteCode,
                x.CancelacionDate
            });
        }
    }
}
