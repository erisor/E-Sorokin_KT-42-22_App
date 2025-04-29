using Microsoft.EntityFrameworkCore;
using E_Sorokin_KT_42_22_App.Database.Models;
using E_Sorokin_KT_42_22_App.Database.Models.DTO;
using E_Sorokin_KT_42_22_App.Interfaces;
using E_Sorokin_KT_42_22_App.Filters;
using E_Sorokin_KT_42_22_App.Database;

namespace E_Sorokin_KT_42_22_App.Services
{
    public class DisciplineService : IDisciplineService
    {
        private readonly SorokinDBContext _context;

        public DisciplineService(SorokinDBContext context)
        {
            _context = context;
        }

        public async Task<List<DisciplineFilter>> GetDisciplinesAsync(string firstName = null, string lastName = null, int? minHours = null, int? maxHours = null)
        {
            var query = from discipline in _context.Disciplines
                        join load in _context.WorkLoads on discipline.Id equals load.DisciplineId into loadsGroup
                        from load in loadsGroup.DefaultIfEmpty()
                        join teacher in _context.Teachers on load.TeacherId equals teacher.Id into teachersGroup
                        from teacher in teachersGroup.DefaultIfEmpty()
                        select new
                        {
                            Discipline = discipline,
                            Load = load,
                            Teacher = teacher
                        };

            var result = await query.ToListAsync();

            var groupedData = result
                .GroupBy(x => new { x.Discipline.Id, x.Discipline.Name })
                .Select(g => new DisciplineFilter
                {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    TotalHours = g.Sum(x => x.Load != null ? x.Load.Hours : 0),
                    Teachers = g
                        .Where(x => x.Teacher != null)
                        .Select(x => $"{x.Teacher.FirstName} {x.Teacher.LastName}")
                        .Distinct()
                        .ToList()
                })
                .ToList();

            if (!string.IsNullOrEmpty(firstName))
            {
                groupedData = groupedData.Where(d => d.Teachers.Any(t => t.Contains(firstName))).ToList();
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                groupedData = groupedData.Where(d => d.Teachers.Any(t => t.Contains(lastName))).ToList();
            }

            if (minHours.HasValue)
            {
                groupedData = groupedData.Where(d => d.TotalHours >= minHours.Value).ToList();
            }

            if (maxHours.HasValue)
            {
                groupedData = groupedData.Where(d => d.TotalHours <= maxHours.Value).ToList();
            }

            return groupedData;
        }

        public async Task<Discipline> GetDisciplineByIdAsync(int id)
        {
            var query = from discipline in _context.Disciplines
                        join load in _context.WorkLoads on discipline.Id equals load.DisciplineId into loadsGroup
                        from load in loadsGroup.DefaultIfEmpty()
                        join teacher in _context.Teachers on load.TeacherId equals teacher.Id into teachersGroup
                        from teacher in teachersGroup.DefaultIfEmpty()
                        where discipline.Id == id
                        select new
                        {
                            Discipline = discipline,
                            Load = load,
                            Teacher = teacher
                        };

            var result = await query.ToListAsync();

            if (result.Count == 0)
            {
                return null;
            }

            var disciplineEntity = result.First().Discipline;
            disciplineEntity.WorkLoad = result
                .Where(x => x.Load != null)
                .Select(x => new WorkLoad
                {
                    Id = x.Load.Id,
                    Hours = x.Load.Hours,
                    TeacherId = x.Load.TeacherId,
                    Teacher = x.Teacher
                })
                .ToList();

            return disciplineEntity;
        }

        public async Task<Discipline> AddDisciplineAsync(Discipline_DTO disciplineDto)
        {
            var discipline = new Discipline
            {
                Name = disciplineDto.Name
            };

            _context.Disciplines.Add(discipline);
            await _context.SaveChangesAsync();
            return discipline;
        }

        public async Task<Discipline> UpdateDisciplineAsync(int id, Discipline_DTO disciplineDto)
        {
            var existingDiscipline = await _context.Disciplines.FindAsync(id);
            if (existingDiscipline == null)
            {
                throw new KeyNotFoundException("Discipline not found");
            }

            existingDiscipline.Name = disciplineDto.Name;

            await _context.SaveChangesAsync();
            return existingDiscipline;
        }

        public async Task<bool> DeleteDisciplineAsync(int id)
        {
            var discipline = await _context.Disciplines.FindAsync(id);
            if (discipline == null)
            {
                return false;
            }

            _context.Disciplines.Remove(discipline);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}