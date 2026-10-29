using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class ProveedorDireccionConfiguration : IEntityTypeConfiguration<ProveedorDireccion>
    {
        public void Configure(EntityTypeBuilder<ProveedorDireccion> builder)
        {
            builder.ToTable("ProveedorDirecciones", schema: "logistica");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProveedorCode)
                .IsRequired()
                 .HasColumnType("char(5)");

            builder.Property(x => x.AddressType)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.UbigeoCode)
                 .HasColumnType("char(6)");

            builder.Property(x => x.Reference)
                .HasMaxLength(200);

            builder.Property(x => x.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.HasOne(x => x.Proveedor)
                .WithMany()
                .HasForeignKey(x => x.ProveedorCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Ubigeo)
                .WithMany()
                .HasForeignKey(x => x.UbigeoCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ProveedorCode);
        }
    }
}
