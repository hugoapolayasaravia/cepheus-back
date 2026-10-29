using Cepheus.Domain.Rrhh.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Maestros
{
    public class TrabajadorSindicatoConfiguration : IEntityTypeConfiguration<TrabajadorSindicato>
    {
        public void Configure(EntityTypeBuilder<TrabajadorSindicato> builder)
        {
            builder.ToTable("trabajador_sindicato", schema: "rrhh");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.TrabajadorCode).IsRequired();
            builder.HasOne(x => x.Trabajador)
                .WithMany()
                .HasForeignKey(x => x.TrabajadorCode)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:1 con Trabajador (igual que el legacy: UNIQUE(trabajador_id))
            builder.HasIndex(x => x.TrabajadorCode).IsUnique();

            builder.Property(x => x.Afiliado).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
