using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Infrastructure.DbContext
{
    
    public class PublicVoteLogConfiguration : IEntityTypeConfiguration<PublicVoteLog>
    {
        public void Configure(EntityTypeBuilder<PublicVoteLog> builder)
        {
            // Primary key
            builder.HasKey(e => e.Id);
            builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
            // Properties
            builder.Property(e => e.TimestampUTC)
                .IsRequired();

            // Relationships
            builder.HasOne(e => e.Vote)
                .WithMany()
                .HasForeignKey(e => e.VoteId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Election)
                .WithMany()
                .HasForeignKey(e => e.ElectionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Governorate)
                .WithMany()
                .HasForeignKey(e => e.GovernorateId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Table name and constraints
            builder.ToTable(tb =>
            { 
                // GovernorateId must be between 1 and 5
                tb.HasCheckConstraint("CK_YourEntity_GovernorateId", "[GovernorateId] BETWEEN 1 AND 5");
            });

            builder.HasIndex(pv => new { pv.ElectionId , pv.VoteId}).IsUnique();

        }
    }

}
