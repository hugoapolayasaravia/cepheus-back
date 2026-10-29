using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class SolicitudNotaCreditoConfiguration : IEntityTypeConfiguration<SolicitudNotaCredito>
    {
        public void Configure(EntityTypeBuilder<SolicitudNotaCredito> builder)
        {
            builder.ToTable("SolicitudesNotaCredito", schema: "facturacion");

            builder.HasKey(x => new { x.Year, x.Month, x.Code, });

            builder.Property(x => x.Year).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Month).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Code).IsRequired().HasColumnType("char(8)");


            builder.Property(x => x.NegocioCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.IssueDate).IsRequired();
            builder.Property(x => x.ClienteCode).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.DocumentTypeCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.DocumentNumber).IsRequired().HasMaxLength(10);
            builder.Property(x => x.MotivoCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.MonedaCode).IsRequired().HasColumnType("char(3)");

            builder.Property(x => x.SubTotal).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.IgvAmount).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.TotalAmount).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            builder.Property(x => x.Observation).IsRequired().HasMaxLength(200);

            builder.Property(x => x.Status).IsRequired().HasConversion<int>()
                .HasDefaultValue(EstadoSolicitudNotaCredito.Pendiente)
                .HasSentinel(default(EstadoSolicitudNotaCredito));
            builder.Property(x => x.IsEditable).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Relaciones
            builder.HasOne(x => x.Negocio).WithMany().HasForeignKey(x => x.NegocioCode)
                .HasPrincipalKey(n => n.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteCode)
                .HasPrincipalKey(c => c.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DocumentType).WithMany().HasForeignKey(x => x.DocumentTypeCode)
                .HasPrincipalKey(c => c.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Motivo).WithMany().HasForeignKey(x => x.MotivoCode)
                .HasPrincipalKey(m => m.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Moneda).WithMany().HasForeignKey(x => x.MonedaCode)
                .HasPrincipalKey(m => m.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Requester).WithMany().HasForeignKey(x => x.RequesterUserId)
                .HasPrincipalKey(u => u.Id).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Details).WithOne(d => d.Solicitud)
                .HasForeignKey(d => new { d.Code, d.Year, d.Month })
                .HasPrincipalKey(s => new { s.Code, s.Year, s.Month })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Guides).WithOne(g => g.Solicitud)
                .HasForeignKey(g => new { g.Code, g.Year, g.Month })
                .HasPrincipalKey(s => new { s.Code, s.Year, s.Month })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Files).WithOne(f => f.Solicitud)
                .HasForeignKey(f => new { f.Code, f.Year, f.Month })
                .HasPrincipalKey(s => new { s.Code, s.Year, s.Month })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Approvals).WithOne(a => a.Solicitud)
                .HasForeignKey(a => new { a.Code, a.Year, a.Month })
                .HasPrincipalKey(s => new { s.Code, s.Year, s.Month })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ClienteCode);
            builder.HasIndex(x => x.NegocioCode);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.IssueDate);
        }
    }
}
