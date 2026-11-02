
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class CancelacionVentaConfiguration : IEntityTypeConfiguration<CancelacionVenta>
    {
        public void Configure(EntityTypeBuilder<CancelacionVenta> builder)
        {
            builder.ToTable("CancelacionesVenta", schema: "facturacion");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasColumnType("char(10)");

            builder.Property(x => x.PlantaCode)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.Property(x => x.ClienteCode)
                .IsRequired()
                .HasColumnType("char(5)");

            builder.Property(x => x.Date)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(x => x.CobradorCode)
                .IsRequired()
                .HasColumnType("char(4)");

            builder.Property(x => x.MonedaCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(x => x.ExchangeRate)
                .IsRequired()
                .HasColumnType("decimal(18,4)");

            builder.Property(x => x.TipoPagoCode)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.Property(x => x.PaymentNumber)
                .HasMaxLength(30);

            builder.Property(x => x.AccountNumber)
                .HasMaxLength(30);

            builder.Property(x => x.BancoDocCode)
                .HasColumnType("char(3)");

            builder.Property(x => x.BancoDepCode)
                .HasColumnType("char(3)");

            builder.Property(x => x.IsDeferred)
                .IsRequired();

            builder.Property(x => x.DeferredDate)
                .HasColumnType("date");

            builder.Property(x => x.OperationNumber)
                .HasMaxLength(30);

            builder.Property(x => x.OperationDate)
                .HasColumnType("date");

            builder.Property(x => x.Description)
                .HasMaxLength(250);

            builder.Property(x => x.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.UsedAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DifferenceAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.AvailableAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.ProvisionEntryNumber)
                .HasColumnType("char(13)");

            builder.Property(x => x.ReceptionDate)
                .HasColumnType("date");

            builder.Property(x => x.InstallmentCount)
                .IsRequired();

            builder.Property(x => x.InstallmentType)
                .HasColumnType("char(1)");

            builder.Property(x => x.FirstPaymentDate)
                .HasColumnType("date");

            builder.Property(x => x.InitialAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Interest)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.CancellationEntryNumber)
                .HasColumnType("char(13)");

            builder.Property(x => x.CanceledBy)
                .HasMaxLength(250);

            builder.Property(x => x.CanceledAt)
                .HasColumnType("datetime2");

            builder.Property(x => x.CancelReason)
                .HasMaxLength(250);

            builder.Property(x => x.ApprovedBy)
                .HasMaxLength(250);

            builder.Property(x => x.ApprovedAt)
                .HasColumnType("datetime2");

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();

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

            // Relaciones
            builder.HasOne(x => x.Planta)
                .WithMany()
                .HasForeignKey(x => x.PlantaCode)
                .HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Cliente)
                .WithMany()
                .HasForeignKey(x => x.ClienteCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Cobrador)
                .WithMany()
                .HasForeignKey(x => x.CobradorCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Moneda)
                .WithMany()
                .HasForeignKey(x => x.MonedaCode)
                .HasPrincipalKey(m => m.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.TipoPago)
                .WithMany()
                .HasForeignKey(x => x.TipoPagoCode)
                .HasPrincipalKey(t => t.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BancoDoc)
                .WithMany()
                .HasForeignKey(x => x.BancoDocCode)
                .HasPrincipalKey(b => b.Code)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BancoDep)
                .WithMany()
                .HasForeignKey(x => x.BancoDepCode)
                .HasPrincipalKey(b => b.Code)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Details)
                .WithOne(d => d.Cancelacion)
                .HasForeignKey(d => d.CancelacionCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(x => x.Date);
            builder.HasIndex(x => x.ClienteCode);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.CobradorCode);
            builder.HasIndex(x => x.TipoPagoCode);
        }
    }
}
