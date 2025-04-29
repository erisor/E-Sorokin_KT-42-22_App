using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using E_Sorokin_KT_42_22_App.Database.Models;

namespace E_Sorokin_KT_42_22_App.Database.Configurations
{
    public class WorkLoadConfiguration : IEntityTypeConfiguration<WorkLoad>
    {
        private const string TableName = "WorkLoads";
        public void Configure(EntityTypeBuilder<WorkLoad> builder)
        {
            builder.ToTable(TableName);
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Hours).IsRequired();
            builder.HasOne(t => t.Teacher).WithMany(x => x.WorkLoad).HasForeignKey(t => t.TeacherId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(t => t.Discipline).WithMany(l => l.WorkLoad).HasForeignKey(t => t.DisciplineId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
