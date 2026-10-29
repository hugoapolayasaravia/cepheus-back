using Cepheus.Domain.Contabilidad.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Maestros
{
    public class PlantillaAsientoConfiguration : IEntityTypeConfiguration<PlantillaAsiento>
    {
        public void Configure(EntityTypeBuilder<PlantillaAsiento> builder)
        {
            builder.ToTable("PlantillasAsiento", schema: "contabilidad");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

            builder.Property(x => x.Process).IsRequired().HasConversion<int>();
            builder.HasIndex(x => x.Process);

            builder.Property(x => x.TipoAsientoCode).IsRequired().HasColumnType("char(2)");
            builder.HasOne(x => x.TipoAsiento).WithMany().HasForeignKey(x => x.TipoAsientoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.PurchaseAccountCode).HasColumnType("char(8)");
            builder.HasOne(x => x.PurchaseAccount).WithMany().HasForeignKey(x => x.PurchaseAccountCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ComprobantePagoCode).HasMaxLength(2);
            builder.HasOne(x => x.ComprobantePago).WithMany().HasForeignKey(x => x.ComprobantePagoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.DescriptionText).HasMaxLength(500);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasMany(x => x.Lines)
                .WithOne(x => x.Template)
                .HasForeignKey(x => x.TemplateCode)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
