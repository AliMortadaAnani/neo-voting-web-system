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
            builder.HasKey(sal => sal.Id);

            builder.Property(sal => sal.TimestampUTC)
                .IsRequired();

            builder.Property(sal => sal.ActionType)
                .HasConversion<string>()   // store enum as string
                .IsRequired()
                .HasMaxLength(200);         // optional: cap string length

            builder.Property(sal => sal.Details)
                .HasMaxLength(2000);       // optional: cap details length

            builder.HasOne(sal => sal.User)
                .WithMany()
                .HasForeignKey(sal => sal.UserId) // user might be deleted
                                                  // so userId and navigation are optional
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            

            // Build dynamic check constraint from enum values
            var enumActionValues = string.Join(
                ",",
                Enum.GetNames(typeof(SystemActionTypesEnum))
                    .Select(v => $"'{v}'")
            );

            builder.ToTable("SystemAuditLogs", tb =>
            {
                tb.HasCheckConstraint(
                    "CK_SystemAuditLog_ActionType",
                    $"[ActionType] IN ({enumActionValues})"
                );
            });
        }
    }
}