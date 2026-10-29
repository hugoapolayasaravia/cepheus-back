using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
    {
        public void Configure(EntityTypeBuilder<Proveedor> builder)
        {
            builder.ToTable("Proveedores", schema: "logistica");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(5)");

            builder.Property(x => x.DocumentTypeCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(x => x.DocumentNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(x => x.DocumentNumber).IsUnique();

            builder.Property(x => x.LegalName)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(x => x.LegalName).IsUnique();

            builder.Property(x => x.TradeName)
                .HasMaxLength(150);

            builder.Property(x => x.ProviderType)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.Property(x => x.Origin)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.Property(x => x.SunatCondition)
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.Property(x => x.SunatStatus)
                .HasMaxLength(30)
                .HasConversion<string>();

            builder.Property(x => x.Observations)
                .HasMaxLength(500);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.DeactivatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.DocumentType)
                .WithMany()
                .HasForeignKey(x => x.DocumentTypeCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
