using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Infrastructure.DbContext
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.RefreshToken)
                .HasMaxLength(500)
                .IsRequired(false)
                ;

            builder.Property(u => u.RefreshTokenExpirationDateTime)
                .IsRequired(false)
                ;
        }
    }
}