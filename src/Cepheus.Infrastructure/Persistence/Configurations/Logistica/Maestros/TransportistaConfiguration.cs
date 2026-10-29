using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class TransportistaConfiguration : IEntityTypeConfiguration<Transportista>
    {
        public void Configure(EntityTypeBuilder<Transportista> builder)
        {
            builder.ToTable("Transportistas", schema: "logistica");

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

            builder.HasIndex(x => new { x.DocumentTypeCode, x.DocumentNumber }).IsUnique();

            builder.Property(x => x.LegalName)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(x => x.LegalName).IsUnique();

            builder.Property(x => x.TradeName)
                .HasMaxLength(150);

            builder.Property(x => x.Address)
                .HasMaxLength(200);

            builder.Property(x => x.UbigeoCode)
                .HasMaxLength(6);

            builder.Property(x => x.Phone)
                .HasMaxLength(30);

            builder.Property(x => x.Email)
                .HasMaxLength(150);

            builder.Property(x => x.MtcRegistrationNumber)
                .HasMaxLength(30);

            builder.Property(x => x.CertificationNumber)
                .HasMaxLength(40);

            builder.Property(x => x.IsOwnFleet)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.Observations)
                .HasMaxLength(500);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

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

            builder.HasOne(x => x.Ubigeo)
                .WithMany()
                .HasForeignKey(x => x.UbigeoCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
