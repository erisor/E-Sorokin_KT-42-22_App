using E_Sorokin_KT_42_22_App.Database.Configurations;
using E_Sorokin_KT_42_22_App.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Sorokin_KT_42_22_App.Database
{
    public class SorokinDBContext : DbContext
    {
        public SorokinDBContext(DbContextOptions<SorokinDBContext> opts) : base(opts) {}

        public DbSet<AcademicDegree> AcademicDegrees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<JobPosition> JobPositions { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<WorkLoad> WorkLoads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AcademicDegreeConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new DisciplineConfiguration());
            modelBuilder.ApplyConfiguration(new JobPositionConfiguration());
            modelBuilder.ApplyConfiguration(new TeacherConfiguration());
            modelBuilder.ApplyConfiguration(new WorkLoadConfiguration());
        }
    }
}