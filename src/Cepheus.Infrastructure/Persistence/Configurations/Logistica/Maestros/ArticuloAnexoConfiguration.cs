using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class ArticuloAnexoConfiguration : IEntityTypeConfiguration<ArticuloAnexo>
    {
        public void Configure(EntityTypeBuilder<ArticuloAnexo> builder)
        {
            builder.ToTable("ArticuloAnexos", schema: "logistica");

            builder.HasKey(x => new { x.SubCentroCostoCode, x.ArticuloCode });

            builder.Property(x => x.SubCentroCostoCode).IsRequired().HasColumnType("char(6)");
            builder.HasOne(x => x.SubCentroCosto).WithMany().HasForeignKey(x => x.SubCentroCostoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ArticuloCode).IsRequired().HasColumnType("char(7)");
            builder.HasOne(x => x.Articulo).WithMany().HasForeignKey(x => x.ArticuloCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.AccountingAccountCode).IsRequired().HasColumnType("char(8)");
            builder.HasOne(x => x.CuentaContable).WithMany().HasForeignKey(x => x.AccountingAccountCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.AccountingAttachmentTypeCode).HasColumnType("char(3)");
            builder.HasOne(x => x.TipoAnexo).WithMany().HasForeignKey(x => x.AccountingAttachmentTypeCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.AccountingAttachmentCode).HasColumnType("char(5)");
            builder.HasOne(x => x.Anexo).WithMany()
                .HasForeignKey(x => new { x.AccountingAttachmentTypeCode, x.AccountingAttachmentCode })
                .HasPrincipalKey(a => new { a.TipoAnexoCode, a.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasIndex(x => x.AccountingAccountCode);
        }
    }
}
