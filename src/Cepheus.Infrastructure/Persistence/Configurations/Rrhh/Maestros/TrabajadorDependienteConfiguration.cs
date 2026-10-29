using Cepheus.Domain.Rrhh.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Maestros
{
    public class TrabajadorDependienteConfiguration : IEntityTypeConfiguration<TrabajadorDependiente>
    {
        public void Configure(EntityTypeBuilder<TrabajadorDependiente> builder)
        {
            builder.ToTable("trabajador_dependiente", schema: "rrhh");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.TrabajadorCode).IsRequired();
            builder.HasOne(x => x.Trabajador)
                .WithMany()
                .HasForeignKey(x => x.TrabajadorCode)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Nombre).HasMaxLength(150).IsRequired();
            builder.HasOne(x => x.Parentesco).WithMany().HasForeignKey(x => x.ParentescoCode).OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.Documento).HasMaxLength(50);
            builder.Property(x => x.Asegurado).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
