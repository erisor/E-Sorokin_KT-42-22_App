using Microsoft.EntityFrameworkCore;
using E_Sorokin_KT_42_22_App.Database.Models;
using E_Sorokin_KT_42_22_App.Database.Models.DTO;
using E_Sorokin_KT_42_22_App.Interfaces;
using E_Sorokin_KT_42_22_App.Filters;
using E_Sorokin_KT_42_22_App.Database;
using static E_Sorokin_KT_42_22_App.Services.WorkLoadService;

namespace E_Sorokin_KT_42_22_App.Services
{
    public class WorkLoadService : IWorkLoadService
    {
        private readonly SorokinDBContext _context;

        public WorkLoadService(SorokinDBContext context)
        {
            _context = context;
        }

        public async Task<List<WorkLoadFilter>> GetLoadsAsync(string teacherFirstName = null, string teacherLastName = null, string departmentName = null, string disciplineName = null)
        {
            var query = from load in _context.WorkLoads
                        join teacher in _context.Teachers on load.TeacherId equals teacher.Id
                        join department in _context.Departments on teacher.DepartmentId equals department.Id
                        join discipline in _context.Disciplines on load.DisciplineId equals discipline.Id
                        select new { load, teacher, department, discipline };

            if (!string.IsNullOrEmpty(teacherFirstName))
            {
                query = query.Where(x => x.teacher.FirstName.Contains(teacherFirstName));
            }

            if (!string.IsNullOrEmpty(teacherLastName))
            {
                query = query.Where(x => x.teacher.LastName.Contains(teacherLastName));
            }

            if (!string.IsNullOrEmpty(departmentName))
            {
                query = query.Where(x => x.department.Name.Contains(departmentName));
            }

            if (!string.IsNullOrEmpty(disciplineName))
            {
                query = query.Where(x => x.discipline.Name.Contains(disciplineName));
            }

            var loads = await query.Select(x => new WorkLoadFilter
            {
                Id = x.load.Id,
                TeacherName = $"{x.teacher.FirstName} {x.teacher.LastName}",
                DepartmentName = x.department.Name,
                DisciplineName = x.discipline.Name,
                Hours = x.load.Hours
            }).ToListAsync();

            return loads;
        }

        public async Task<WorkLoadFilter> AddLoadAsync(int teacherId, int disciplineId, int hours)
        {
            var checks = await (from t in _context.Teachers.Where(t => t.Id == teacherId).DefaultIfEmpty()
                                join d in _context.Disciplines.Where(d => d.Id == disciplineId).DefaultIfEmpty() on 1 equals 1
                                join w in _context.WorkLoads.Where(w => w.TeacherId == teacherId && w.DisciplineId == disciplineId).DefaultIfEmpty() on 1 equals 1
                                select new
                                {
                                    TeacherExists = t != null,
                                    DisciplineExists = d != null,
                                    LoadExists = w != null
                                }).FirstOrDefaultAsync();

            if (checks == null || !checks.TeacherExists)
                throw new ArgumentException($"Преподаватель с ID {teacherId} не найден");

            if (!checks.DisciplineExists)
                throw new ArgumentException($"Дисциплина с ID {disciplineId} не найдена");

            if (checks.LoadExists)
                throw new InvalidOperationException(
                    $"Нагрузка для преподавателя {teacherId} и дисциплины {disciplineId} уже существует");

            var load = new WorkLoad
            {
                TeacherId = teacherId,
                DisciplineId = disciplineId,
                Hours = hours
            };

            _context.WorkLoads.Add(load);
            await _context.SaveChangesAsync();

            return await GetLoadFilterAsync(load.Id);
        }

        public async Task<WorkLoadFilter> UpdateLoadAsync(int loadId, int teacherId, int disciplineId, int hours)
        {
            if (teacherId <= 0)
            {
                throw new ArgumentException("ID преподавателя должен быть положительным числом");
            }

            if (disciplineId <= 0)
            {
                throw new ArgumentException("ID дисциплины должен быть положительным числом");
            }

            if (hours <= 0)
            {
                throw new ArgumentException("Количество часов должно быть положительным числом");
            }

            var load = await _context.WorkLoads.FindAsync(loadId);
            if (load == null)
            {
                throw new KeyNotFoundException($"Нагрузка с ID {loadId} не найдена");
            }

            bool teacherExists = await _context.Teachers.AnyAsync(t => t.Id == teacherId);
            if (!teacherExists)
            {
                throw new ArgumentException($"Преподаватель с ID {teacherId} не найден", nameof(teacherId));
            }

            bool disciplineExists = await _context.Disciplines.AnyAsync(d => d.Id == disciplineId);
            if (!disciplineExists)
            {
                throw new ArgumentException($"Дисциплина с ID {disciplineId} не найдена", nameof(disciplineId));
            }

            if (load.TeacherId != teacherId || load.DisciplineId != disciplineId)
            {
                bool loadExists = await _context.WorkLoads
                    .AnyAsync(w => w.TeacherId == teacherId &&
                                  w.DisciplineId == disciplineId &&
                                  w.Id != loadId);

                if (loadExists)
                {
                    throw new InvalidOperationException(
                        $"Нагрузка для преподавателя {teacherId} и дисциплины {disciplineId} уже существует");
                }
            }

            load.TeacherId = teacherId;
            load.DisciplineId = disciplineId;
            load.Hours = hours;

            await _context.SaveChangesAsync();

            return await GetLoadFilterAsync(load.Id);
        }

        private async Task<WorkLoadFilter> GetLoadFilterAsync(int loadId)
        {
            return await (from load in _context.WorkLoads
                            join teacher in _context.Teachers on load.TeacherId equals teacher.Id
                            join department in _context.Departments on teacher.DepartmentId equals department.Id
                            join discipline in _context.Disciplines on load.DisciplineId equals discipline.Id
                            where load.Id == loadId
                            select new WorkLoadFilter
                            {
                                Id = load.Id,
                                TeacherName = $"{teacher.FirstName} {teacher.LastName}",
                                DepartmentName = department.Name,
                                DisciplineName = discipline.Name,
                                Hours = load.Hours
                            }).FirstOrDefaultAsync();
        }
    }
}