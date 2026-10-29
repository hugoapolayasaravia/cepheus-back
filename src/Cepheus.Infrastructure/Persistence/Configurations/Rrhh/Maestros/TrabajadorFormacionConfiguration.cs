using Cepheus.Domain.Rrhh.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Maestros
{
    public class TrabajadorFormacionConfiguration : IEntityTypeConfiguration<TrabajadorFormacion>
    {
        public void Configure(EntityTypeBuilder<TrabajadorFormacion> builder)
        {
            builder.ToTable("trabajador_formacion", schema: "rrhh");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.TrabajadorCode).IsRequired();
            builder.HasOne(x => x.Trabajador)
                .WithMany()
                .HasForeignKey(x => x.TrabajadorCode)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.NivelEducativo).WithMany().HasForeignKey(x => x.NivelEducativoCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.GradoInstruccion).WithMany().HasForeignKey(x => x.GradoInstruccionCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Titulo).WithMany().HasForeignKey(x => x.TituloCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Especialidad).WithMany().HasForeignKey(x => x.EspecialidadCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.TipoCentroFormacion).WithMany().HasForeignKey(x => x.TipoCentroFormacionCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.ModalidadFormativa).WithMany().HasForeignKey(x => x.ModalidadFormativaCode).OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
