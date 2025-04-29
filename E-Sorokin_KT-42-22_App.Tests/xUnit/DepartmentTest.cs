using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using E_Sorokin_KT_42_22_App.Database;
using E_Sorokin_KT_42_22_App.Database.Models;
using E_Sorokin_KT_42_22_App.Services;
using E_Sorokin_KT_42_22_App.Filters;

namespace E_Sorokin_KT_42_22_App.Tests.xUnit
{
    public class DepartmentTest : IDisposable
    {
        private readonly SorokinDBContext _context;
        private readonly DepartmentService _service;

        public DepartmentTest()
        {
            var dbName = $"TestDatabase_Department_{Guid.NewGuid()}";
            var options = new DbContextOptionsBuilder<SorokinDBContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            _context = new SorokinDBContext(options);
            _context.Database.EnsureCreated();
            _service = new DepartmentService(_context);

            SeedTestData().Wait();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region CRUD Tests

        [Fact]
        public async Task AddDepartmentAsync_ValidData_CreatesDepartment()
        {
            var department = new Department
            {
                Name = "Кафедра тест",
                FoundedDate = DateTime.Now
            };

            await _service.AddDepartmentAsync(department);

            var dbDepartment = await _context.Departments.FindAsync(department.Id);
            Assert.NotNull(dbDepartment);
            Assert.Equal("Кафедра тест", dbDepartment.Name);
        }

        [Fact]
        public async Task UpdateDepartmentAsync_ValidData_UpdatesDepartment()
        {
            var department = await _context.Departments.FirstAsync();
            var teacher = await _context.Teachers.FirstAsync();

            department.Name = "Обновленная кафедра";
            department.LeaderId = teacher.Id;

            var result = await _service.UpdateDepartmentAsync(department);

            Assert.NotNull(result);
            Assert.Equal("Обновленная кафедра", result.Name);
            Assert.Equal(teacher.Id, result.LeaderId);
        }

        [Fact]
        public async Task DeleteDepartmentAsync_ExistingDepartment_RemovesDepartment()
        {
            var department = await _context.Departments.FirstAsync();

            var result = await _service.DeleteDepartmentAsync(department.Id);

            Assert.True(result);
            Assert.Null(await _context.Departments.FindAsync(department.Id));
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task AddDepartmentAsync_InvalidName_ThrowsArgumentException()
        {
            var department = new Department
            {
                Name = "Неправильное название",
                FoundedDate = DateTime.Now
            };

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.AddDepartmentAsync(department));
        }

        [Fact]
        public async Task UpdateDepartmentAsync_NonExistingDepartment_ReturnsNull()
        {
            var department = new Department
            {
                Id = 999,
                Name = "Кафедра тест",
                FoundedDate = DateTime.Now
            };

            var result = await _service.UpdateDepartmentAsync(department);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteDepartmentAsync_NonExistingDepartment_ReturnsFalse()
        {
            var result = await _service.DeleteDepartmentAsync(999);

            Assert.False(result);
        }

        #endregion

        #region Filter Tests

        [Fact]
        public async Task GetDepartmentsAsync_NoFilters_ReturnsAllDepartments()
        {
            var result = await _service.GetDepartmentsAsync();

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetDepartmentsAsync_FoundedAfter_ReturnsFiltered()
        {
            var result = await _service.GetDepartmentsAsync(
                foundedAfter: new DateTime(2020, 1, 1));

            Assert.Equal(2, result.Count);
            Assert.Contains(result, d => d.Name == "Кафедра математики");
            Assert.Contains(result, d => d.Name == "Кафедра физики");
        }

        [Fact]
        public async Task GetDepartmentsAsync_MinTeacherCount_ReturnsFiltered()
        {
            var result = await _service.GetDepartmentsAsync(
                minTeacherCount: 2);

            Assert.Single(result);
            Assert.Equal("Кафедра математики", result.First().Name);
        }

        [Fact]
        public async Task GetDepartmentsAsync_CombinedFilters_ReturnsCorrectResults()
        {
            var result = await _service.GetDepartmentsAsync(
                foundedAfter: new DateTime(2020, 1, 1),
                minTeacherCount: 1);

            Assert.Equal(2, result.Count);
        }

        #endregion

        private async Task SeedTestData()
        {
            var degree = new AcademicDegree { Name = "Доктор наук" };
            var position = new JobPosition { Name = "Профессор" };
            await _context.AcademicDegrees.AddAsync(degree);
            await _context.JobPositions.AddAsync(position);
            await _context.SaveChangesAsync();

            var departments = new List<Department>
            {
                new Department
                {
                    Name = "Кафедра математики",
                    FoundedDate = new DateTime(2020, 1, 1),
                    Teachers = new List<Teacher>
                    {
                        new Teacher
                        {
                            FirstName = "Иван",
                            LastName = "Иванов",
                            AcademicDegreeId = degree.Id,
                            JobPositionId = position.Id
                        },
                        new Teacher
                        {
                            FirstName = "Петр",
                            LastName = "Петров",
                            AcademicDegreeId = degree.Id,
                            JobPositionId = position.Id
                        }
                    }
                },
                new Department
                {
                    Name = "Кафедра физики",
                    FoundedDate = new DateTime(2021, 1, 1),
                    Teachers = new List<Teacher>
                    {
                        new Teacher
                        {
                            FirstName = "Сергей",
                            LastName = "Сидоров",
                            AcademicDegreeId = degree.Id,
                            JobPositionId = position.Id
                        }
                    }
                },
                new Department
                {
                    Name = "Кафедра химии",
                    FoundedDate = new DateTime(2019, 1, 1)
                }
            };

            await _context.Departments.AddRangeAsync(departments);
            await _context.SaveChangesAsync();

            var mathDepartment = await _context.Departments
                .FirstAsync(d => d.Name == "Кафедра математики");
            var leader = await _context.Teachers
                .FirstAsync(t => t.LastName == "Иванов");

            mathDepartment.LeaderId = leader.Id;
            await _context.SaveChangesAsync();
        }
    }
}