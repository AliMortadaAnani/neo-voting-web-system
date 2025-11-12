using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;



namespace NeoVoting.Infrastructure.DbContext
{
  
    namespace NeoVoting.Infrastructure.DbContext
    {
        public class GovernorateConfiguration : IEntityTypeConfiguration<Governorate>
        {
            public void Configure(EntityTypeBuilder<Governorate> builder)
            {
                // Primary key
                builder.HasKey(g => g.Id);
                builder.Property(entity => entity.Id).ValueGeneratedNever();
                // Name property
                builder.Property(g => g.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                // Seed static lookup values
                builder.HasData(
                    Governorate.CreateBeirutGovernorate(),
                    Governorate.CreateMountLebanonGovernorate(),
                    Governorate.CreateSouthGovernorate(),
                    Governorate.CreateEastGovernorate(),
                    Governorate.CreateNorthGovernorate()
                );
            }
        }
    }

}
