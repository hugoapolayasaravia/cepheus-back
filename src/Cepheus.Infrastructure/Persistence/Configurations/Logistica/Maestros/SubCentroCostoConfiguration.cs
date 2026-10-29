using Cepheus.Domain.Contabilidad.Catalogos;
using Cepheus.Domain.Contabilidad.Maestros;
using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class SubCentroCostoConfiguration : IEntityTypeConfiguration<SubCentroCosto>
    {
        public void Configure(EntityTypeBuilder<SubCentroCosto> builder)
        {
            builder.ToTable("SubCentrosCosto", schema: "logistica");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(6)");

            builder.Property(x => x.CentroCostoCode)
                .HasMaxLength(3);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.AccountingAccountCode)
                .HasMaxLength(8);

            builder.Property(x => x.AccountingAttachmentTypeCode)
                .HasMaxLength(3);

            builder.Property(x => x.PlantaCode)
                .IsRequired()
                .HasMaxLength(2);

            builder.Property(x => x.ParentCode)
                .HasMaxLength(6);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.CentroCosto)
                .WithMany()
                .HasForeignKey(x => x.CentroCostoCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Planta)
                .WithMany()
                .HasForeignKey(x => x.PlantaCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<CuentaContable>()
                .WithMany()
                .HasForeignKey(x => x.AccountingAccountCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<TipoAnexo>()
                .WithMany()
                .HasForeignKey(x => x.AccountingAttachmentTypeCode)
                .HasPrincipalKey(a => a.Code)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
