using Microsoft.EntityFrameworkCore;
using E_Sorokin_KT_42_22_App.Database.Models;
using E_Sorokin_KT_42_22_App.Database.Models.DTO;
using E_Sorokin_KT_42_22_App.Interfaces;
using E_Sorokin_KT_42_22_App.Filters;
using E_Sorokin_KT_42_22_App.Database;

namespace E_Sorokin_KT_42_22_App.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly SorokinDBContext _context;

        public TeacherService(SorokinDBContext context)
        {
            _context = context;
        }

        public async Task<List<TeacherFilter>> GetTeachersAsync(string departmentName = null, string degreeName = null, string positionName = null)
        {
            var query =
                from teacher in _context.Teachers
                join degree in _context.AcademicDegrees on teacher.AcademicDegreeId equals degree.Id
                join position in _context.JobPositions on teacher.JobPositionId equals position.Id
                join department in _context.Departments on teacher.DepartmentId equals department.Id into departmentJoin
                from department in departmentJoin.DefaultIfEmpty()
                select new { teacher, degree, position, department };

            if (!string.IsNullOrEmpty(departmentName))
            {
                query = query.Where(x => x.department != null && x.department.Name == departmentName);
            }

            if (!string.IsNullOrEmpty(degreeName))
            {
                query = query.Where(x => x.degree.Name == degreeName);
            }

            if (!string.IsNullOrEmpty(positionName))
            {
                query = query.Where(x => x.position.Name == positionName);
            }

            var teachers = await query.Select(x => new TeacherFilter
            {
                Id = x.teacher.Id,
                FirstName = x.teacher.FirstName,
                LastName = x.teacher.LastName,
                AcademicDegree = x.degree.Name,
                JobPosition = x.position.Name,
                Department = x.department != null ? x.department.Name : null
            }).ToListAsync();

            return teachers;
        }

        public async Task<TeacherResponce_DTO> GetTeacherByIdAsync(int id)
        {
            var query =
                from teacher in _context.Teachers
                join degree in _context.AcademicDegrees on teacher.AcademicDegreeId equals degree.Id
                join position in _context.JobPositions on teacher.JobPositionId equals position.Id
                join department in _context.Departments on teacher.DepartmentId equals department.Id into departmentJoin
                from department in departmentJoin.DefaultIfEmpty()
                where teacher.Id == id
                select new TeacherResponce_DTO
                {
                    Id = teacher.Id,
                    FirstName = teacher.FirstName,
                    LastName = teacher.LastName,
                    AcademicDegreeId = teacher.AcademicDegreeId,
                    AcademicDegree = degree.Name,
                    JobPositionId = teacher.JobPositionId,
                    JobPosition = position.Name,
                    DepartmentId = teacher.DepartmentId,
                    Department = department != null ? department.Name : null,
                    WorkLoad = new List<WorkLoad>()
                };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<TeacherResponce_DTO> AddTeacherAsync(string firstName, string lastName, int positionId, int degreeId, int? departmentId)
        {
            var positionExists = await _context.JobPositions.AnyAsync(p => p.Id == positionId);
            if (!positionExists)
            {
                throw new ArgumentException($"Должности с id {positionId} не существует.");
            }

            var degreeExists = await _context.AcademicDegrees.AnyAsync(d => d.Id == degreeId);
            if (!degreeExists)
            {
                throw new ArgumentException($"Степени с id {degreeId} не существует.");
            }

            if (departmentId.HasValue)
            {
                var departmentExists = await _context.Departments.AnyAsync(d => d.Id == departmentId.Value);
                if (!departmentExists)
                {
                    throw new ArgumentException($"Кафедры с id {departmentId} не существует.");
                }
            }

            var teacher = new Teacher
            {
                FirstName = firstName,
                LastName = lastName,
                JobPositionId = positionId,
                AcademicDegreeId = degreeId,
                DepartmentId = departmentId
            };

            if (!teacher.IsNameValid1())
            {
                throw new ArgumentException("Имя и фамилия преподавателя должны начинаться с заглавной буквы.");
            }

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            var query =
                from t in _context.Teachers
                join degree in _context.AcademicDegrees on t.AcademicDegreeId equals degree.Id
                join position in _context.JobPositions on t.JobPositionId equals position.Id
                join department in _context.Departments on t.DepartmentId equals department.Id into departmentJoin
                from department in departmentJoin.DefaultIfEmpty()
                where t.Id == teacher.Id
                select new { degree, position, department };

            var result = await query.FirstOrDefaultAsync();

            var responseDto = new TeacherResponce_DTO
            {
                Id = teacher.Id,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                AcademicDegreeId = teacher.AcademicDegreeId,
                AcademicDegree = result.degree.Name,
                JobPositionId = teacher.JobPositionId,
                JobPosition = result.position.Name,
                DepartmentId = teacher.DepartmentId,
                Department = result.department?.Name,
                WorkLoad = new List<WorkLoad>()
            };

            return responseDto;
        }

        public async Task<TeacherResponce_DTO> UpdateTeacherAsync(int id, string firstName, string lastName, int positionId, int degreeId, int? departmentId)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return null;
            }

            var positionExists = await _context.JobPositions.AnyAsync(p => p.Id == positionId);
            if (!positionExists)
            {
                throw new ArgumentException($"Должности с id {positionId} не существует.");
            }

            var degreeExists = await _context.AcademicDegrees.AnyAsync(d => d.Id == degreeId);
            if (!degreeExists)
            {
                throw new ArgumentException($"Степени с id {degreeId} не существует.");
            }

            if (departmentId.HasValue)
            {
                var departmentExists = await _context.Departments.AnyAsync(d => d.Id == departmentId.Value);
                if (!departmentExists)
                {
                    throw new ArgumentException($"Кафедры с id {departmentId} не существует.");
                }
            }

            teacher.FirstName = firstName;
            teacher.LastName = lastName;
            teacher.JobPositionId = positionId;
            teacher.AcademicDegreeId = degreeId;
            teacher.DepartmentId = departmentId;

            if (!teacher.IsValid())
            {
                throw new ArgumentException("Имя и фамилия преподавателя должны начинаться с заглавной буквы.");
            }

            await _context.SaveChangesAsync();

            var query =
                from t in _context.Teachers
                join degree in _context.AcademicDegrees on t.AcademicDegreeId equals degree.Id
                join position in _context.JobPositions on t.JobPositionId equals position.Id
                join department in _context.Departments on t.DepartmentId equals department.Id into departmentJoin
                from department in departmentJoin.DefaultIfEmpty()
                where t.Id == id
                select new { degree, position, department };

            var result = await query.FirstOrDefaultAsync();

            var responseDto = new TeacherResponce_DTO
            {
                Id = teacher.Id,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                AcademicDegreeId = teacher.AcademicDegreeId,
                AcademicDegree = result.degree.Name,
                JobPositionId = teacher.JobPositionId,
                JobPosition = result.position.Name,
                DepartmentId = teacher.DepartmentId,
                Department = result.department?.Name,
                WorkLoad = new List<WorkLoad>()
            };

            return responseDto;
        }

        public async Task<bool> DeleteTeacherAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<DepartmentFilter>> GetDepartmentsByDisciplineAsync(string disciplineName)
        {
            var query = from discipline in _context.Disciplines
                        where discipline.Name == disciplineName
                        join workload in _context.WorkLoads on discipline.Id equals workload.DisciplineId
                        join teacher in _context.Teachers on workload.TeacherId equals teacher.Id
                        join department in _context.Departments on teacher.DepartmentId equals department.Id
                        join leader in _context.Teachers on department.LeaderId equals leader.Id into leaderJoin
                        from leader in leaderJoin.DefaultIfEmpty()
                        let teacherCount = _context.Teachers.Count(t => t.DepartmentId == department.Id)
                        select new DepartmentFilter
                        {
                            Id = department.Id,
                            Name = department.Name,
                            FoundedDate = department.FoundedDate,
                            Leader = leader != null ? $"{leader.FirstName} {leader.LastName}" : "Нет заведующего",
                            TeacherCount = teacherCount
                        };

            return await query.Distinct().ToListAsync();
        }
    }
}