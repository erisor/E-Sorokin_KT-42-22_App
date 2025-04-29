using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using E_Sorokin_KT_42_22_App.Database.Models;

namespace E_Sorokin_KT_42_22_App.Database.Configurations
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        private const string TableName = "Teachers";
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.FirstName).IsRequired();
            builder.Property(t => t.LastName).IsRequired();
            builder.HasMany(t => t.WorkLoad).WithOne(x => x.Teacher).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(t => t.Department).WithMany(l => l.Teachers).HasForeignKey(t => t.DepartmentId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}