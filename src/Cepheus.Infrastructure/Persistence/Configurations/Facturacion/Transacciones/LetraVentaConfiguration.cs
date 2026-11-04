using Cepheus.Domain.Facturacion.Enum;

using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class LetraVentaConfiguration : IEntityTypeConfiguration<LetraVenta>
    {
        public void Configure(EntityTypeBuilder<LetraVenta> builder)
        {
            builder.ToTable("LetrasVenta", schema: "facturacion");

            builder.HasKey(x => new
            {
                x.PlantaCode,
                x.Number
            });

            builder.Property(x => x.PlantaCode)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.Property(x => x.Number)
                .IsRequired()
                .HasColumnType("char(9)");

            builder.Property(x => x.ClienteCode)
                .IsRequired()
                .HasColumnType("char(5)");

            builder.Property(x => x.BancoCode)
                .HasColumnType("char(3)");

            builder.Property(x => x.IssueDate)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(x => x.DueDate)
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

            builder.Property(x => x.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DoubtfulStatus).IsRequired().HasDefaultValue(EstadoCobranzaDudosa.Normal);
            builder.Property(x => x.DoubtfulExchangeRate).IsRequired().HasColumnType("decimal(18,4)").HasDefaultValue(0m);
            builder.Property(x => x.PendingAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Interest)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.ProvisionEntryNumber)
                .HasColumnType("char(13)");

            builder.Property(x => x.CancellationEntryNumber)
                .HasColumnType("char(13)");

            builder.Property(x => x.CanceledAt)
                .HasColumnType("date");

            builder.Property(x => x.SettledAt)
                .HasColumnType("date");

            builder.Property(x => x.Description)
                .HasMaxLength(250);

            builder.Property(x => x.CancelacionCode)
                .IsRequired()
                .HasColumnType("char(10)");

            builder.Property(x => x.IsPrinted)
                .IsRequired();

            builder.Property(x => x.AcceptedByClient)
                .IsRequired();

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

            builder.HasOne(x => x.Banco)
                .WithMany()
                .HasForeignKey(x => x.BancoCode)
                .HasPrincipalKey(b => b.Code)
                .IsRequired(false)
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

            // Cancelación que originó la letra
            builder.HasOne(x => x.Cancelacion)
                .WithMany()
                .HasForeignKey(x => x.CancelacionCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(x => x.ClienteCode);
            builder.HasIndex(x => x.DueDate);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.CobradorCode);
            builder.HasIndex(x => x.CancelacionCode);
        }
    }
}
