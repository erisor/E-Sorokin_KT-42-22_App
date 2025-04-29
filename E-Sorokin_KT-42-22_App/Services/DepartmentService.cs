using Microsoft.EntityFrameworkCore;
using E_Sorokin_KT_42_22_App.Database.Models;
using E_Sorokin_KT_42_22_App.Database.Models.DTO;
using E_Sorokin_KT_42_22_App.Interfaces;
using E_Sorokin_KT_42_22_App.Filters;
using E_Sorokin_KT_42_22_App.Database;

namespace E_Sorokin_KT_42_22_App.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly SorokinDBContext _context;

        public DepartmentService(SorokinDBContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentFilter>> GetDepartmentsAsync(DateTime? foundedAfter = null, int? minTeacherCount = null)
        {
            var query = from department in _context.Departments
                        where !foundedAfter.HasValue || department.FoundedDate >= foundedAfter.Value
                        join leader in _context.Teachers on department.LeaderId equals leader.Id into leaderJoin
                        from leader in leaderJoin.DefaultIfEmpty()
                        let teacherCount = _context.Teachers.Count(t => t.DepartmentId == department.Id)
                        where !minTeacherCount.HasValue || teacherCount >= minTeacherCount.Value
                        select new DepartmentFilter
                        {
                            Id = department.Id,
                            Name = department.Name,
                            FoundedDate = department.FoundedDate,
                            Leader = leader != null ? $"{leader.FirstName} {leader.LastName}" : "Нет заведующего",
                            TeacherCount = teacherCount
                        };

            return await query.ToListAsync();
        }

        public async Task AddDepartmentAsync(Department department)
        {
            if (!department.IsValidName())
            {
                throw new ArgumentException("Название кафедры должно содержать 'Кафедра'.");
            }

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
        }

        public async Task<Department> UpdateDepartmentAsync(Department department)
        {
            if (!department.IsValidName())
            {
                throw new ArgumentException("Название кафедры должно содержать 'Кафедра'.");
            }

            var existingDepartment = await _context.Departments.FindAsync(department.Id);
            if (existingDepartment == null)
            {
                return null;
            }

            existingDepartment.Name = department.Name;
            existingDepartment.FoundedDate = department.FoundedDate;

            if (department.LeaderId != null)
            {
                var leaderExists = await _context.Teachers.AnyAsync(t => t.Id == department.LeaderId);
                if (!leaderExists)
                {
                    existingDepartment.LeaderId = null;
                    //throw new InvalidOperationException("Указанный LeaderId не существует в таблице Teachers");
                }
                else
                {
                    existingDepartment.LeaderId = department.LeaderId;
                }
            }
            else
            {
                existingDepartment.LeaderId = department.LeaderId;
            }

            await _context.SaveChangesAsync();
            return existingDepartment;
        }

        public async Task<bool> DeleteDepartmentAsync(int departmentId)
        {
            var departmentWithTeachers = await (from d in _context.Departments
                                                where d.Id == departmentId
                                                select new
                                                {
                                                    Department = d,
                                                    Teachers = _context.Teachers.Where(t => t.DepartmentId == d.Id).ToList()
                                                })
                                              .FirstOrDefaultAsync();

            if (departmentWithTeachers == null)
            {
                return false;
            }

            departmentWithTeachers.Department.LeaderId = null;
            await _context.SaveChangesAsync();

            _context.Teachers.RemoveRange(departmentWithTeachers.Teachers);
            await _context.SaveChangesAsync();

            _context.Departments.Remove(departmentWithTeachers.Department);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}