using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class ProveedorCondicionConfiguration : IEntityTypeConfiguration<ProveedorCondicion>
    {
        public void Configure(EntityTypeBuilder<ProveedorCondicion> builder)
        {
            builder.ToTable("ProveedorCondiciones", schema: "logistica");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProveedorCode)
                .IsRequired()
                 .HasColumnType("char(5)");

            builder.Property(x => x.FormaPagoCode)
                .IsRequired()
                 .HasColumnType("char(2)");

            builder.Property(x => x.PaymentTermDays)
                .IsRequired();

            builder.Property(x => x.MonedaCode)
                .IsRequired()
                 .HasColumnType("char(3)");

            builder.Property(x => x.CreditLimit)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DiscountPercentage)
                .HasColumnType("decimal(5,2)");

            builder.Property(x => x.IsPrimary)
                .IsRequired()
                .HasDefaultValue(true);

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

            builder.HasOne(x => x.FormaPago)
                .WithMany()
                .HasForeignKey(x => x.FormaPagoCode)
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
