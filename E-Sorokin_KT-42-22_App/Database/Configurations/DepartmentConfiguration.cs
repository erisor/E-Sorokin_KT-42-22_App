using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using E_Sorokin_KT_42_22_App.Database.Models;

namespace E_Sorokin_KT_42_22_App.Database.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        private const string TableName = "Departments";
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired();
            builder.Property(t => t.FoundedDate).IsRequired();
            builder.HasMany(t => t.Teachers).WithOne(x => x.Department).HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(t => t.Leader).WithOne().HasForeignKey<Department>(t => t.LeaderId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}