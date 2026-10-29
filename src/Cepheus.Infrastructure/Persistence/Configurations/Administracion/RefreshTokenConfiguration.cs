using Cepheus.Domain.Administracion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Administracion
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens", schema: "admin");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(x => x.Token).IsUnique();

            builder.Property(x => x.ExpiresAt).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            // IsActive es una propiedad calculada en memoria, no se mapea a columna
            builder.Ignore(x => x.IsActive);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
