using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using E_Sorokin_KT_42_22_App.Database.Models;

namespace E_Sorokin_KT_42_22_App.Database.Configurations
{
    public class DisciplineConfiguration : IEntityTypeConfiguration<Discipline>
    {
        private const string TableName = "Disciplines";
        public void Configure(EntityTypeBuilder<Discipline> builder)
        {
            builder.ToTable(TableName);
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired();
            builder.HasMany(t => t.WorkLoad).WithOne(x => x.Discipline).HasForeignKey(x => x.DisciplineId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}