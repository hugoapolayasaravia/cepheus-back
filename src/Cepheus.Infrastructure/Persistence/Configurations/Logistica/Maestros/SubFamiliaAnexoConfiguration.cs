using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros;

public class SubFamiliaAnexoConfiguration : IEntityTypeConfiguration<SubFamiliaAnexo>
{
    public void Configure(EntityTypeBuilder<SubFamiliaAnexo> builder)
    {
        builder.ToTable("SubFamiliaAnexos", schema: "logistica");

        builder.HasKey(x => new { x.SubCentroCostoCode, x.SubFamiliaCode });

        builder.Property(x => x.SubCentroCostoCode).IsRequired().HasColumnType("char(6)");
        builder.Property(x => x.SubFamiliaCode).IsRequired().HasColumnType("char(4)");
        builder.Property(x => x.AccountingAccountCode).IsRequired().HasColumnType("char(8)");
        builder.Property(x => x.AccountingAttachmentTypeCode).HasColumnType("char(3)");
        builder.Property(x => x.AccountingAttachmentCode).HasColumnType("char(5)");

        builder.Property(x => x.CreatedBy).HasMaxLength(250);
        builder.Property(x => x.UpdatedBy).HasMaxLength(250);
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasOne(x => x.SubCentroCosto)
            .WithMany()
            .HasForeignKey(x => x.SubCentroCostoCode)
            .HasPrincipalKey(x => x.Code)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SubFamilia)
            .WithMany()
            .HasForeignKey(x => x.SubFamiliaCode)
            .HasPrincipalKey(x => x.Code)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CuentaContable)
            .WithMany()
            .HasForeignKey(x => x.AccountingAccountCode)
            .HasPrincipalKey(x => x.Code)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TipoAnexo)
            .WithMany()
            .HasForeignKey(x => x.AccountingAttachmentTypeCode)
            .HasPrincipalKey(x => x.Code)
            .OnDelete(DeleteBehavior.Restrict);

        // Anexo: clave compuesta (tipo de anexo, código). Si ambos son nulos no hay enlace a anexo.
        builder.HasOne(x => x.Anexo)
            .WithMany()
            .HasForeignKey(x => new { x.AccountingAttachmentTypeCode, x.AccountingAttachmentCode })
            .HasPrincipalKey(x => new { x.TipoAnexoCode, x.Code })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.SubFamiliaCode);
        builder.HasIndex(x => x.AccountingAccountCode);
    }
}
