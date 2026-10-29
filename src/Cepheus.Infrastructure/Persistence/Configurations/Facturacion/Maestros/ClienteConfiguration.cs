using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Maestros
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes", schema: "facturacion");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(5)");

            builder.Property(x => x.PersonType)
                 .IsRequired()
                 .HasConversion(
                     v => v == TipoPersona.Empresa ? "E" : "N",
                     v => v == "E" ? TipoPersona.Empresa : TipoPersona.Natural)
                 .HasColumnType("char(1)");

            builder.Property(x => x.DocumentTypeCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(x => x.DocumentNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(x => x.DocumentNumber).IsUnique();

            builder.Property(x => x.Name)
                .HasMaxLength(120);

            builder.HasIndex(x => x.Name)
                .IsUnique()
                .HasFilter("[Name] IS NOT NULL");

            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.UbigeoCode)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(x => x.Phone)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.ParentClientCode)
                .HasMaxLength(5);

            builder.Property(x => x.TipoClienteCode)
                .IsRequired()
                .HasMaxLength(1);

            builder.Property(x => x.ClasificacionClienteCode)
                .IsRequired()
                .HasMaxLength(1);

            builder.Property(x => x.LegalRepresentativeName)
                .HasMaxLength(50);

            builder.Property(x => x.LegalRepresentativePhone)
                .HasMaxLength(50);

            builder.Property(x => x.LegalRepresentativeDni)
                .HasMaxLength(8);

            builder.Property(x => x.ContactName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.ContactPhone)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.ContactEmail)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Observations)
                .HasMaxLength(500);

            builder.Property(x => x.IsVip)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.RequiresCashOnly)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.HasGlobalCreditLine)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.GlobalCreditAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.FormaPagoVentaCode)
                .IsRequired()
                .HasMaxLength(2);

            builder.Property(x => x.CurrencyCode)
                .HasMaxLength(3);

            builder.Property(x => x.Estado)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.DocumentType)
                .WithMany()
                .HasForeignKey(x => x.DocumentTypeCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Ubigeo)
                .WithMany()
                .HasForeignKey(x => x.UbigeoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ParentClient)
                .WithMany()
                .HasForeignKey(x => x.ParentClientCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.TipoCliente)
                .WithMany()
                .HasForeignKey(x => x.TipoClienteCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ClasificacionCliente)
                .WithMany()
                .HasForeignKey(x => x.ClasificacionClienteCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FormaPagoVenta)
                .WithMany()
                .HasForeignKey(x => x.FormaPagoVentaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.DocumentTypeCode);
            builder.HasIndex(x => x.UbigeoCode);
            builder.HasIndex(x => x.ParentClientCode);
            builder.HasIndex(x => x.TipoClienteCode);
            builder.HasIndex(x => x.ClasificacionClienteCode);
            builder.HasIndex(x => x.FormaPagoVentaCode);
            builder.HasIndex(x => x.CurrencyCode);
            builder.HasIndex(x => x.Estado);
        }
    }
}
