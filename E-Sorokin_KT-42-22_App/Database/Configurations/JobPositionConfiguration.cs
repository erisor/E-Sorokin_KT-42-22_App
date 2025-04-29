using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using E_Sorokin_KT_42_22_App.Database.Models;

namespace E_Sorokin_KT_42_22_App.Database.Configurations
{
    public class JobPositionConfiguration : IEntityTypeConfiguration<JobPosition>
    {
        private const string TableName = "JobPositions";
        public void Configure(EntityTypeBuilder<JobPosition> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired();
        }
    }
}
