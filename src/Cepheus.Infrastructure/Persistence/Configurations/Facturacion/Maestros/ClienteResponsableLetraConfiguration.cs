using Cepheus.Domain.Facturacion.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Maestros
{
    public class ClienteResponsableLetraConfiguration : IEntityTypeConfiguration<ClienteResponsableLetra>
    {
        public void Configure(EntityTypeBuilder<ClienteResponsableLetra> builder)
        {
            builder.ToTable("ClientesResponsablesLetra", schema: "facturacion");

            builder.HasKey(x => new { x.ClienteCode, x.Sequence });

            builder.Property(x => x.ClienteCode).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.Sequence).IsRequired().HasMaxLength(10);
            builder.Property(x => x.PaternalSurname).HasMaxLength(60);
            builder.Property(x => x.MaternalSurname).HasMaxLength(60);
            builder.Property(x => x.FirstNames).HasMaxLength(100);
            builder.Property(x => x.DocumentTypeCode).HasMaxLength(5);
            builder.Property(x => x.DocumentNumber).HasMaxLength(20);
            builder.Property(x => x.Address).HasMaxLength(150);
            builder.Property(x => x.Phone).HasMaxLength(20);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteCode)
                .HasPrincipalKey(c => c.Code).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
