using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class ChequeVentaConfiguration : IEntityTypeConfiguration<ChequeVenta>
    {
        public void Configure(EntityTypeBuilder<ChequeVenta> builder)
        {
            builder.ToTable("ChequesVenta", schema: "facturacion");

            builder.HasKey(x => new { x.Number, x.ClienteCode, x.BancoCode });

            builder.Property(x => x.Number).IsRequired().HasMaxLength(15);
            builder.Property(x => x.ClienteCode).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.BancoCode).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");

            builder.Property(x => x.IssueDate).IsRequired();
            builder.Property(x => x.PaymentDate).IsRequired();

            builder.Property(x => x.CobradorCode).HasColumnType("char(4)");

            builder.Property(x => x.MonedaCode).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.ExchangeRate).IsRequired().HasColumnType("decimal(18,4)");

            builder.Property(x => x.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.PendingAmount).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(x => x.ProvisionEntryNumber).HasColumnType("char(13)");
            builder.Property(x => x.ReboteEntryNumber).HasColumnType("char(13)");
            builder.Property(x => x.CancellationEntryNumber).HasColumnType("char(13)");

            builder.Property(x => x.Description).HasMaxLength(255);
            builder.Property(x => x.CancelacionCode).IsRequired().HasColumnType("char(10)");

            builder.Property(x => x.IsDeferred).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.OperationNumber).HasMaxLength(20);

            builder.Property(x => x.MotivoReboteCode).HasColumnType("char(2)");

            builder.Property(x => x.Status).IsRequired().HasConversion<int>()
                .HasDefaultValue(EstadoCheque.Pendiente)
                .HasSentinel(default(EstadoCheque));

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Planta).WithMany().HasForeignKey(x => x.PlantaCode)
                .HasPrincipalKey(p => p.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteCode)
                .HasPrincipalKey(c => c.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Banco).WithMany().HasForeignKey(x => x.BancoCode)
                .HasPrincipalKey(b => b.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Cobrador).WithMany().HasForeignKey(x => x.CobradorCode)
                .HasPrincipalKey(c => c.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Moneda).WithMany().HasForeignKey(x => x.MonedaCode)
                .HasPrincipalKey(m => m.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.MotivoRebote).WithMany().HasForeignKey(x => x.MotivoReboteCode)
                .HasPrincipalKey(m => m.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.CancelacionCode);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.IssueDate);
        }
    }
}
