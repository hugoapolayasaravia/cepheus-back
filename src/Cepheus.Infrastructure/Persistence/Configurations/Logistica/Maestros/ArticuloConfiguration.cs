using Cepheus.Domain.Contabilidad.Catalogos;
using Cepheus.Domain.Contabilidad.Maestros;
using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class ArticuloConfiguration : IEntityTypeConfiguration<Articulo>
    {
        public void Configure(EntityTypeBuilder<Articulo> builder)
        {
            builder.ToTable("Articulos", schema: "logistica");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(7)");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(80);

            builder.Property(x => x.UnidadMedidaCode)
                .IsRequired()
                .HasMaxLength(2);

            builder.Property(x => x.SubFamiliaCode)
                .IsRequired()
                .HasMaxLength(4);

            builder.Property(x => x.MinStock)
                .IsRequired()
                .HasColumnType("decimal(12,5)")
                .HasDefaultValue(0);

            builder.Property(x => x.MaxStock)
                .IsRequired()
                .HasColumnType("decimal(12,5)")
                .HasDefaultValue(0);

            builder.Property(x => x.IncomingStock)
                .IsRequired()
                .HasColumnType("decimal(12,5)")
                .HasDefaultValue(0);

            builder.Property(x => x.LeadTimeDays)
                .IsRequired()
                .HasColumnType("decimal(12,5)")
                .HasDefaultValue(0);

            builder.Property(x => x.AbcClass)
                .IsRequired()
                .HasMaxLength(1)
                .HasConversion<string>();

            builder.Property(x => x.TipoArticuloCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(x => x.PlanCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(x => x.ManufacturerCode)
                .HasMaxLength(11);

            builder.Property(x => x.Observations)
                .IsRequired()
                .HasColumnType("varchar(max)")
                .HasDefaultValue(string.Empty);

            builder.Property(x => x.SalesTypeCode)
                .HasMaxLength(2);

            builder.Property(x => x.SalesProductCode)
                .HasMaxLength(4);

            builder.Property(x => x.IsAgreement)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.RequiresHorometro)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.AccountingAccountCode)
                .HasMaxLength(8);

            builder.Property(x => x.AccountingAttachmentTypeCode)
                .HasMaxLength(3);

            builder.Property(x => x.PlantOriginCode)
                .HasMaxLength(2);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.UnidadMedida)
                .WithMany()
                .HasForeignKey(x => x.UnidadMedidaCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SubFamilia)
                .WithMany()
                .HasForeignKey(x => x.SubFamiliaCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.TipoArticulo)
                .WithMany()
                .HasForeignKey(x => x.TipoArticuloCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Plan)
                .WithMany()
                .HasForeignKey(x => x.PlanCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PlantOrigin)
                .WithMany()
                .HasForeignKey(x => x.PlantOriginCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<CuentaContable>()
                .WithMany()
                .HasForeignKey(x => x.AccountingAccountCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<TipoAnexo>()
                .WithMany()
                .HasForeignKey(x => x.AccountingAttachmentTypeCode)
                .HasPrincipalKey(a => a.Code)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
