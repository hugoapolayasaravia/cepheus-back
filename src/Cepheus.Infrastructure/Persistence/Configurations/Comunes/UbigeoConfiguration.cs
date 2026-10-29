using Cepheus.Domain.Comunes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Comunes
{
    public class UbigeoConfiguration : IEntityTypeConfiguration<Ubigeo>
    {
        public void Configure(EntityTypeBuilder<Ubigeo> builder)
        {
            builder.ToTable("Ubigeos", schema: "comun");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(6)");

            builder.HasIndex(x => x.Code).IsUnique();

            builder.Property(x => x.Department)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Province)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.District)
                .IsRequired()
                .HasMaxLength(50);

            // FullAddress es calculada en memoria (get-only), no se mapea a columna.
            builder.Ignore(x => x.FullAddress);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            // Búsqueda frecuente por jerarquía geográfica (ej. combos dependientes
            // Departamento -> Provincia -> Distrito en el frontend).
            builder.HasIndex(x => new { x.Department, x.Province, x.District });
        }
    }
}
