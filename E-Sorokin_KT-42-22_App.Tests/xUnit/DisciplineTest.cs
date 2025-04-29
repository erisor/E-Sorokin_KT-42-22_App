using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using E_Sorokin_KT_42_22_App.Database;
using E_Sorokin_KT_42_22_App.Database.Models;
using E_Sorokin_KT_42_22_App.Database.Models.DTO;
using E_Sorokin_KT_42_22_App.Services;
using E_Sorokin_KT_42_22_App.Filters;

namespace E_Sorokin_KT_42_22_App.Tests.xUnit
{
    public class DisciplineTest : IDisposable
    {
        private readonly SorokinDBContext _context;
        private readonly DisciplineService _service;

        public DisciplineTest()
        {
            var dbName = $"TestDatabase_Discipline_{Guid.NewGuid()}";
            var options = new DbContextOptionsBuilder<SorokinDBContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            _context = new SorokinDBContext(options);
            _context.Database.EnsureCreated();
            _service = new DisciplineService(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task AddDisciplineAsync_ValidData_AddsDiscipline()
        {
            var dto = new Discipline_DTO { Name = "Математика" };

            var result = await _service.AddDisciplineAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("Математика", result.Name);
            Assert.NotEqual(0, result.Id);

            var dbDiscipline = await _context.Disciplines.FindAsync(result.Id);
            Assert.NotNull(dbDiscipline);
        }

        [Fact]
        public async Task AddDisciplineAsync_NullDto_ThrowsException()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => _service.AddDisciplineAsync(null));
        }

        [Fact]
        public async Task UpdateDisciplineAsync_ValidData_UpdatesDiscipline()
        {
            var initialDto = new Discipline_DTO { Name = "Физика" };
            var addedDiscipline = await _service.AddDisciplineAsync(initialDto);
            var updateDto = new Discipline_DTO { Name = "Квантовая физика" };

            var result = await _service.UpdateDisciplineAsync(addedDiscipline.Id, updateDto);

            Assert.NotNull(result);
            Assert.Equal("Квантовая физика", result.Name);
            Assert.Equal(addedDiscipline.Id, result.Id);

            var dbDiscipline = await _context.Disciplines.FindAsync(result.Id);
            Assert.Equal("Квантовая физика", dbDiscipline.Name);
        }

        [Fact]
        public async Task UpdateDisciplineAsync_NonExistingId_ThrowsException()
        {
            var dto = new Discipline_DTO { Name = "Химия" };
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateDisciplineAsync(999, dto));
        }

        [Fact]
        public async Task DeleteDisciplineAsync_ExistingId_DeletesDiscipline()
        {
            var dto = new Discipline_DTO { Name = "Биология" };
            var addedDiscipline = await _service.AddDisciplineAsync(dto);

            var result = await _service.DeleteDisciplineAsync(addedDiscipline.Id);

            Assert.True(result);
            var dbDiscipline = await _context.Disciplines.FindAsync(addedDiscipline.Id);
            Assert.Null(dbDiscipline);
        }

        [Fact]
        public async Task DeleteDisciplineAsync_NonExistingId_ReturnsFalse()
        {
            var result = await _service.DeleteDisciplineAsync(999);
            Assert.False(result);
        }

        [Fact]
        public async Task GetDisciplineByIdAsync_ExistingId_ReturnsDisciplineWithWorkloads()
        {
            var department = new Department { Name = "Кафедра физики" };
            var teacher = new Teacher { FirstName = "Иван", LastName = "Петров", Department = department };
            var discipline = new Discipline { Name = "Термодинамика" };
            var workload = new WorkLoad { Teacher = teacher, Discipline = discipline, Hours = 40 };

            await _context.Departments.AddAsync(department);
            await _context.Teachers.AddAsync(teacher);
            await _context.Disciplines.AddAsync(discipline);
            await _context.WorkLoads.AddAsync(workload);
            await _context.SaveChangesAsync();

            var result = await _service.GetDisciplineByIdAsync(discipline.Id);

            Assert.NotNull(result);
            Assert.Equal("Термодинамика", result.Name);
            Assert.Single(result.WorkLoad);
            Assert.Equal(40, result.WorkLoad.First().Hours);
            Assert.Equal("Иван Петров", $"{result.WorkLoad.First().Teacher.FirstName} {result.WorkLoad.First().Teacher.LastName}");
        }

        [Fact]
        public async Task GetDisciplineByIdAsync_NonExistingId_ReturnsNull()
        {
            var result = await _service.GetDisciplineByIdAsync(999);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetDisciplinesAsync_NoFilters_ReturnsAllDisciplines()
        {
            await SeedTestData();

            var result = await _service.GetDisciplinesAsync();

            Assert.Equal(3, result.Count);
            Assert.Contains(result, d => d.Name == "Математика");
            Assert.Contains(result, d => d.Name == "Физика");
            Assert.Contains(result, d => d.Name == "Химия");
        }

        [Fact]
        public async Task GetDisciplinesAsync_FilterByTeacherFirstName_ReturnsFiltered()
        {
            await SeedTestData();

            var result = await _service.GetDisciplinesAsync(firstName: "Иван");

            Assert.Equal(2, result.Count);
            Assert.Contains(result, d => d.Name == "Математика");
            Assert.Contains(result, d => d.Name == "Физика");
            Assert.DoesNotContain(result, d => d.Name == "Химия");
        }

        [Fact]
        public async Task GetDisciplinesAsync_FilterByTeacherLastName_ReturnsFiltered()
        {
            await SeedTestData();

            var result = await _service.GetDisciplinesAsync(lastName: "Сидоров");

            Assert.Single(result);
            Assert.Equal("Химия", result.First().Name);
        }

        [Fact]
        public async Task GetDisciplinesAsync_FilterByMinHours_ReturnsFiltered()
        {
            await SeedTestData();

            var result = await _service.GetDisciplinesAsync(minHours: 60);

            Assert.Single(result);
            Assert.Equal("Физика", result.First().Name);
            Assert.Equal(80, result.First().TotalHours);
        }

        [Fact]
        public async Task GetDisciplinesAsync_FilterByMaxHours_ReturnsFiltered()
        {
            await SeedTestData();

            var result = await _service.GetDisciplinesAsync(maxHours: 50);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, d => d.Name == "Математика");
            Assert.Contains(result, d => d.Name == "Химия");
            Assert.DoesNotContain(result, d => d.Name == "Физика");
        }

        [Fact]
        public async Task GetDisciplinesAsync_CombinedFilters_ReturnsCorrectResults()
        {
            await SeedTestData();

            var result = await _service.GetDisciplinesAsync(
                firstName: "Иван",
                minHours: 30,
                maxHours: 70);

            Assert.Single(result);
            Assert.Equal("Математика", result.First().Name);
        }

        private async Task SeedTestData()
        {
            var department1 = new Department { Name = "Кафедра математики" };
            var department2 = new Department { Name = "Кафедра физики" };

            var teacher1 = new Teacher { FirstName = "Иван", LastName = "Иванов", Department = department1 };
            var teacher2 = new Teacher { FirstName = "Петр", LastName = "Петров", Department = department2 };
            var teacher3 = new Teacher { FirstName = "Иван", LastName = "Смирнов", Department = department1 };
            var teacher4 = new Teacher { FirstName = "Сергей", LastName = "Сидоров", Department = department2 };

            var discipline1 = new Discipline { Name = "Математика" };
            var discipline2 = new Discipline { Name = "Физика" };
            var discipline3 = new Discipline { Name = "Химия" };

            var workloads = new List<WorkLoad>
            {
                new WorkLoad { Teacher = teacher1, Discipline = discipline1, Hours = 40 },
                new WorkLoad { Teacher = teacher3, Discipline = discipline1, Hours = 10 },
                new WorkLoad { Teacher = teacher2, Discipline = discipline2, Hours = 60 },
                new WorkLoad { Teacher = teacher1, Discipline = discipline2, Hours = 20 },
                new WorkLoad { Teacher = teacher4, Discipline = discipline3, Hours = 30 }
            };

            await _context.Departments.AddRangeAsync(department1, department2);
            await _context.Teachers.AddRangeAsync(teacher1, teacher2, teacher3, teacher4);
            await _context.Disciplines.AddRangeAsync(discipline1, discipline2, discipline3);
            await _context.WorkLoads.AddRangeAsync(workloads);
            await _context.SaveChangesAsync();
        }
    }
}