using Cepheus.Domain.Facturacion.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Catalogos
{
    public class AtributoConcretoConfiguration : IEntityTypeConfiguration<AtributoConcreto>
    {
        public void Configure(EntityTypeBuilder<AtributoConcreto> builder)
        {
            builder.ToTable("AtributosConcreto", schema: "facturacion");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(4)");

            builder.Property(x => x.AttributeType).IsRequired().HasConversion<int>();

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasIndex(x => x.AttributeType);
        }
    }
}
