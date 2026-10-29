using Cepheus.Domain.Administracion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Administracion
{
    public class ModuloConfiguration : IEntityTypeConfiguration<Modulo>
    {
        public void Configure(EntityTypeBuilder<Modulo> builder)
        {
            builder.ToTable("Modulos", schema: "admin");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(x => x.Code).IsUnique();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);
            //Se mostrará en el sidebar


            builder.HasIndex(x => x.Name).IsUnique();

            builder.Property(x => x.Icon)
                .HasMaxLength(50);

            builder.Property(x => x.Tooltip)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(x => x.DisplayOrder)
                .IsRequired();

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
