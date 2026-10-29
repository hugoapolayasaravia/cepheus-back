using Cepheus.Domain.Facturacion.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Catalogos
{
    public class ResponsableObraConfiguration : IEntityTypeConfiguration<ResponsableObra>
    {
        public void Configure(EntityTypeBuilder<ResponsableObra> builder)
        {
            builder.ToTable("ResponsablesObra", schema: "facturacion");

            builder.HasKey(x => new { x.ClienteCode, x.ObraCode, x.Code });

            builder.Property(x => x.ClienteCode).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.ObraCode).IsRequired().HasColumnType("char(3)");

            builder.Property(x => x.Code)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Phone).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Position).IsRequired().HasMaxLength(50);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Obra)
                .WithMany()
                .HasForeignKey(x => new { x.ClienteCode, x.ObraCode })
                .HasPrincipalKey(o => new { o.ClienteCode, o.Code })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
