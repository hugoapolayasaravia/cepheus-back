using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class ArticuloProveedorConfiguration : IEntityTypeConfiguration<ArticuloProveedor>
    {
        public void Configure(EntityTypeBuilder<ArticuloProveedor> builder)
        {
            builder.ToTable("ArticuloProveedor", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.ArticuloCode, x.ProveedorCode });

            builder.Property(x => x.PlantaCode)
                .IsRequired()
                 .HasColumnType("char(2)");

            builder.Property(x => x.ArticuloCode)
                .IsRequired()
                 .HasColumnType("char(7)");

            builder.Property(x => x.ProveedorCode)
                .IsRequired()
                 .HasColumnType("char(5)");

            builder.Property(x => x.IsAgreement)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.AgreementPrice)
                .HasColumnType("decimal(18,4)");

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.HasOne(x => x.Planta)
                .WithMany()
                .HasForeignKey(x => x.PlantaCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Articulo)
                .WithMany()
                .HasForeignKey(x => x.ArticuloCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Proveedor)
                .WithMany()
                .HasForeignKey(x => x.ProveedorCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ArticuloCode);
            builder.HasIndex(x => x.ProveedorCode);
        }
    }
}
