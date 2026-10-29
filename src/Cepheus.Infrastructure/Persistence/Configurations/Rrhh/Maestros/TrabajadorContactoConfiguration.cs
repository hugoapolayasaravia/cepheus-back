using Cepheus.Domain.Rrhh.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Maestros
{
    public class TrabajadorContactoConfiguration : IEntityTypeConfiguration<TrabajadorContacto>
    {
        public void Configure(EntityTypeBuilder<TrabajadorContacto> builder)
        {
            builder.ToTable("TrabajadorContactos", schema: "rrhh");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TrabajadorCode)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Phone)
                .HasMaxLength(30);

            builder.Property(x => x.ParentescoCode)
                .HasMaxLength(10);

            builder.Property(x => x.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.HasOne(x => x.Trabajador)
                .WithMany()
                .HasForeignKey(x => x.TrabajadorCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Parentesco)
                .WithMany()
                .HasForeignKey(x => x.ParentescoCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.TrabajadorCode);
        }
    }
}