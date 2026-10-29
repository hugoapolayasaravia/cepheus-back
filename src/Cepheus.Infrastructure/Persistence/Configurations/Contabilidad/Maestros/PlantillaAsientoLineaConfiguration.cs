using Cepheus.Domain.Contabilidad.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Maestros
{
    public class PlantillaAsientoLineaConfiguration : IEntityTypeConfiguration<PlantillaAsientoLinea>
    {
        public void Configure(EntityTypeBuilder<PlantillaAsientoLinea> builder)
        {
            builder.ToTable("PlantillaAsientoLineas", schema: "contabilidad");

            builder.HasKey(x => new { x.TemplateCode, x.Order });

            builder.Property(x => x.TemplateCode).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.Order).IsRequired();

            builder.Property(x => x.Concept).IsRequired().HasConversion<int>();
            builder.Property(x => x.Nature).IsRequired().HasConversion<int>();
            builder.Property(x => x.AccountSource).IsRequired().HasConversion<int>();

            builder.Property(x => x.AccountCode).HasColumnType("char(8)");
            builder.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.AnnexSource).IsRequired().HasConversion<int>();
            builder.Property(x => x.DescriptionSource).IsRequired().HasConversion<int>();

            builder.Property(x => x.InvertsOnCreditNote).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.IsDestination).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.AccountingType).IsRequired().HasConversion<int>();

            builder.Property(x => x.ValeTypeCode).HasColumnType("char(3)");
            builder.HasOne(x => x.ValeType).WithMany().HasForeignKey(x => x.ValeTypeCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
