using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.IdentityEntities;

namespace NeoVoting.Infrastructure.DbContext
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Note: We do NOT configure Id, UserName, Email, etc.
            // The base IdentityDbContext handles those. We only configure our custom properties.

            // --- Configure Custom Properties ---

            // Configure the FirstName property.
            // We'll give it a max length to prevent overly long inputs.
            // It's optional (not IsRequired()), matching the nullable string? in the entity.
            builder.Property(u => u.FirstName)
                .HasMaxLength(100);

            // Configure the LastName property with the same constraints.
            builder.Property(u => u.LastName)
                .HasMaxLength(100);

            // Configure the Gender property.
            // We can use HasMaxLength(1) for a char, though it's often implicit.
            // Using HasColumnType("nchar(1)") can be more explicit for SQL Server.
            builder.Property(u => u.Gender)
                .HasMaxLength(1);

            


            //we wont add check constraint for date of birth here because it may cause issues with users who are creating accounts on their birthday. (dynamic not static)

            // Configure the RefreshToken property.
            // Refresh tokens can be long, so we give it a larger max length.
            // An index can speed up finding a user by their refresh token.
            builder.Property(u => u.RefreshToken)
                .HasMaxLength(500);

            builder.HasIndex(u => u.RefreshToken)
                .IsUnique(); // A refresh token must be unique to a single user at any given time.


            // --- Configure Custom Relationships ---

            // Define the relationship between ApplicationUser and Governorate.
            // An ApplicationUser has one optional Governorate.
            // A Governorate can have many ApplicationUsers.
            builder.HasOne(u => u.Governorate)
                .WithMany() // We don't need a navigation property on Governorate pointing back to Users.
                .HasForeignKey(u => u.GovernorateID)
                .IsRequired(false) // This makes the foreign key optional (nullable), matching the int? property.
                .OnDelete(DeleteBehavior.Restrict); // Important! Prevents deleting a governorate if users are assigned to it.


            builder.ToTable(tb =>
            tb.HasCheckConstraint(
                "CK_User_GovernorateID",
                "[GovernorateID] BETWEEN 1 AND 5 OR [GovernorateID] IS NULL"
            ));

            builder.ToTable(tb =>
            tb.HasCheckConstraint("CK_User_Gender", "[Gender] IN ('M', 'F') OR [Gender] IS NULL"));

        }
    }
}
