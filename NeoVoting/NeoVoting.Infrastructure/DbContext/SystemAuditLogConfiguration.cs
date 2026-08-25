using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Infrastructure.DbContext
{
    public class SystemAuditLogConfiguration : IEntityTypeConfiguration<SystemAuditLog>
    {
        public void Configure(EntityTypeBuilder<SystemAuditLog> builder)
        {
           
            // --- Primary Key ---
            builder.HasKey(sal => sal.Id);
            builder.Property(entity => entity.Id)
                   .ValueGeneratedOnAdd();

            // --- Properties ---
            builder.Property(sal => sal.TimestampUTC)
                .IsRequired();

            builder.Property(sal => sal.AdminId)
                .IsRequired();

            builder.Property(sal => sal.Username)
                .IsRequired()
                .HasMaxLength(100); // Standard Identity Username length


            // Details (JSON or Text)
            builder.Property(sal => sal.Details)
                .IsRequired(false)
                .HasMaxLength(4000); // Allow reasonable JSON payload

            // --- Enum Handling ---
            builder.Property(sal => sal.ActionType)
                .HasConversion<string>()   // Store as "USER_LOGIN", not "1"
                .IsRequired()
                .HasMaxLength(100);

            // --- Constraints ---

            // Dynamic Check Constraint: Ensures DB only accepts values defined in the Enum
            var enumActionValues = string.Join(
                ", ",
                Enum.GetNames(typeof(SystemActionTypesEnum))
                    .Select(v => $"'{v}'")
            );

            // Note: Syntax is SQL Server specific. Remove [ ] if using Postgres.
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_SystemAuditLog_ActionType",
                $"[ActionType] IN ({enumActionValues})"
            ));

            
        }
    }
}