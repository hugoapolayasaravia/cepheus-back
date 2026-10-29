using Cepheus.Domain.Facturacion.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Maestros
{
    public class UsuarioSolicitudNotaCreditoConfiguration : IEntityTypeConfiguration<UsuarioSolicitudNotaCredito>
    {
        public void Configure(EntityTypeBuilder<UsuarioSolicitudNotaCredito> builder)
        {
            builder.ToTable("UsuariosSolicitudNotaCredito", schema: "facturacion");

            builder.HasKey(x => new { x.NegocioCode, x.Stage, x.UserId });

            builder.Property(x => x.NegocioCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Stage).IsRequired().HasConversion<int>();
            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.IsPrincipal).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.CanEnter).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.CanManage).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.CanApprove).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Negocio).WithMany()
                .HasForeignKey(x => x.NegocioCode)
                .HasPrincipalKey(n => n.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User).WithMany()
                .HasForeignKey(x => x.UserId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.NegocioCode, x.Stage });
            builder.HasIndex(x => x.UserId);
        }
    }
}
