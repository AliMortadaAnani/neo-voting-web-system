using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Infrastructure.DbContext
{
    public class PublicVoteLogConfiguration : IEntityTypeConfiguration<PublicVoteLog>
    {
        public void Configure(EntityTypeBuilder<PublicVoteLog> builder)
        {
        
            // --- Primary Key ---
            builder.HasKey(e => e.Id);
            builder.Property(entity => entity.Id)
                   .ValueGeneratedOnAdd(); // Identity Column

            // --- Properties ---
            builder.Property(e => e.TimestampUTC)
                .IsRequired();

            // Store just the Error Message string, capped at 1000 chars
            builder.Property(e => e.ErrorMessage)
                .IsRequired(false)
                .HasMaxLength(1000);

            // Foreign Key IDs (Stored as simple columns, NO constraints)
            builder.Property(e => e.VoteId).IsRequired();
            builder.Property(e => e.ElectionId).IsRequired();
            builder.Property(e => e.GovernorateId).IsRequired();

            // Snapshots (Required)
            builder.Property(e => e.GovernorateName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.ElectionName)
                .IsRequired()
                .HasMaxLength(100);

            // --- Indexes (Crucial for Log Performance) ---

           

            // Optimize queries like "Get all votes for Election X"
            builder.HasIndex(pv => pv.ElectionId)
                   .HasDatabaseName("IX_PublicVoteLogs_ElectionId");

            // Optimize queries like "Get all votes for Governorate X"
            builder.HasIndex(pv => pv.GovernorateId)
                   .HasDatabaseName("IX_PublicVoteLogs_GovernorateId");

            

            // 2. Timeline: "Show me logs from yesterday"
            builder.HasIndex(sal => sal.TimestampUTC)
                   .HasDatabaseName("IX_PublicVoteLogs_Timestamp");

            builder.HasIndex(pv => new { pv.ElectionId, pv.TimestampUTC })
    .HasDatabaseName("IX_PublicVoteLogs_ElectionId_Timestamp");

        }
    }
}