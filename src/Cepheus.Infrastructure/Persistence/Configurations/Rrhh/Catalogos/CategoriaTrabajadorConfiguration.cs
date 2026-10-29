using Cepheus.Domain.Rrhh.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Catalogos
{
    public class CategoriaTrabajadorConfiguration : IEntityTypeConfiguration<CategoriaTrabajador>
    {
        public void Configure(EntityTypeBuilder<CategoriaTrabajador> builder)
        {
            builder.ToTable("CategoriasTrabajador", schema: "rrhh");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasColumnType("char(3)");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            // El script solo define UNIQUE(codigo); no hay UNIQUE(nombre) en rrhh.categoria_trabajador.

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}