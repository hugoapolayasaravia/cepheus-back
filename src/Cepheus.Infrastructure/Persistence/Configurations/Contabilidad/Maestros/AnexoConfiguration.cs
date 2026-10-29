using Cepheus.Domain.Contabilidad.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Maestros
{
    public class AnexoConfiguration : IEntityTypeConfiguration<Anexo>
    {
        public void Configure(EntityTypeBuilder<Anexo> builder)
        {
            builder.ToTable("Anexos", schema: "contabilidad");

            builder.HasKey(x => new { x.TipoAnexoCode, x.Code });

            builder.Property(x => x.TipoAnexoCode).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.Code).IsRequired().HasColumnType("char(5)");

            builder.Property(x => x.Name).HasColumnType("varchar(150)");

            builder.Property(x => x.LinkAccount1Code).IsRequired().HasColumnType("char(8)");
            builder.Property(x => x.LinkAccount2Code).IsRequired().HasColumnType("char(8)");

            builder.Property(x => x.Flag).HasColumnType("char(1)");

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.TipoAnexo)
                .WithMany()
                .HasForeignKey(x => x.TipoAnexoCode)
                .HasPrincipalKey(t => t.Code)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
