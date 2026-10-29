using Cepheus.Domain.Facturacion.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Maestros
{
    public class ObraConfiguration : IEntityTypeConfiguration<Obra>
    {
        public void Configure(EntityTypeBuilder<Obra> builder)
        {
            builder.ToTable("Obras", schema: "facturacion");

            builder.HasKey(x => new { x.ClienteCode, x.Code });

            builder.Property(x => x.ClienteCode)
                .IsRequired()
                 .HasColumnType("char(5)");

            builder.Property(x => x.Code)
                .IsRequired()
                .HasColumnType("char(3)");

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(80);

            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.UbigeoCode)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(x => x.Observations)
                .HasMaxLength(150);

            builder.Property(x => x.Estado)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.DeliveryAddress)
                .HasMaxLength(100);

            builder.Property(x => x.DeliveryUbigeoCode)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(x => x.BillingAddress)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.BillingUbigeoCode)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(x => x.ResponsibleName)
                .HasMaxLength(50);

            builder.Property(x => x.ResponsiblePhone)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.ResponsibleEmail)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.FormaPagoVentaCode)
                .HasMaxLength(2);

            builder.Property(x => x.CobradorCode)
                .IsRequired()
                .HasMaxLength(4);

            builder.Property(x => x.VendedorCode)
                .IsRequired()
                .HasMaxLength(4);

            builder.Property(x => x.AnalisisVentaCode)
                .HasMaxLength(3);

            builder.Property(x => x.CreditLimit)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.CreditCurrencyCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(x => x.HasSurcharge)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.ShortName)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.TipoValorizacionCode)
                .HasMaxLength(1);

            builder.Property(x => x.ScheduledWeekday)
                .HasConversion<int?>();

            builder.Property(x => x.IsProject)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.RequiresPrinting)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.CompletionUser)
                .HasMaxLength(250);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.Cliente)
                .WithMany()
                .HasForeignKey(x => x.ClienteCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Ubigeo)
                .WithMany()
                .HasForeignKey(x => x.UbigeoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DeliveryUbigeo)
                .WithMany()
                .HasForeignKey(x => x.DeliveryUbigeoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BillingUbigeo)
                .WithMany()
                .HasForeignKey(x => x.BillingUbigeoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FormaPagoVenta)
                .WithMany()
                .HasForeignKey(x => x.FormaPagoVentaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AnalisisVenta)
                .WithMany()
                .HasForeignKey(x => x.AnalisisVentaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreditCurrency)
                .WithMany()
                .HasForeignKey(x => x.CreditCurrencyCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.TipoValorizacion)
                .WithMany()
                .HasForeignKey(x => x.TipoValorizacionCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.UbigeoCode);
            builder.HasIndex(x => x.DeliveryUbigeoCode);
            builder.HasIndex(x => x.BillingUbigeoCode);
            builder.HasIndex(x => x.FormaPagoVentaCode);
            builder.HasIndex(x => x.CobradorCode);
            builder.HasIndex(x => x.VendedorCode);
            builder.HasIndex(x => x.AnalisisVentaCode);
            builder.HasIndex(x => x.CreditCurrencyCode);
            builder.HasIndex(x => x.TipoValorizacionCode);
            builder.HasIndex(x => x.Estado);
        }
    }
}
