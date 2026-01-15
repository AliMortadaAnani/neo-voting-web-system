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

            builder.Property(sal => sal.UserId)
                .IsRequired();

            builder.Property(sal => sal.Username)
                .IsRequired()
                .HasMaxLength(100); // Standard Identity Username length

            // Optional Links
            builder.Property(sal => sal.CandidateProfileId).IsRequired(false);
            builder.Property(sal => sal.ElectionId).IsRequired(false);
            builder.Property(sal => sal.ElectionName).IsRequired(false).HasMaxLength(100);

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

            // --- Indexes ---

            // 1. Security Auditing: "Show me everything User X did"
            builder.HasIndex(sal => sal.UserId)
                   .HasDatabaseName("IX_SystemAuditLogs_UserId");

        

            builder.HasIndex(sal => sal.ElectionId)
                   .HasDatabaseName("IX_SystemAuditLogs_ElectionId");

          

            // 2. Timeline: "Show me logs from yesterday"
            builder.HasIndex(sal => sal.TimestampUTC)
                   .HasDatabaseName("IX_SystemAuditLogs_Timestamp");

            // 3. Action Filtering: "Show me all CANDIDATE_CREATED events"
            builder.HasIndex(sal => sal.ActionType)
                   .HasDatabaseName("IX_SystemAuditLogs_ActionType");
        }
    }
}