using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using E_Sorokin_KT_42_22_App.Database.Models;

namespace E_Sorokin_KT_42_22_App.Database.Configurations
{
    public class AcademicDegreeConfiguration : IEntityTypeConfiguration<AcademicDegree>
    {
        private const string TableName = "AcademicDegrees";
        public void Configure(EntityTypeBuilder<AcademicDegree> builder) 
        { 
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired();
        }
    }
}