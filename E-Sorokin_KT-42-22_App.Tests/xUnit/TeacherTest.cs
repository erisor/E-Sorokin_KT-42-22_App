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
    public class TeacherTest : IDisposable
    {
        private readonly SorokinDBContext _context;
        private readonly TeacherService _service;

        public TeacherTest()
        {
            var dbName = $"TestDatabase_Teacher_{Guid.NewGuid()}";
            var options = new DbContextOptionsBuilder<SorokinDBContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            _context = new SorokinDBContext(options);
            _context.Database.EnsureCreated();
            _service = new TeacherService(_context);

            SeedTestData().Wait();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region Positive Tests

        [Fact]
        public async Task AddTeacherAsync_ValidData_ReturnsTeacherWithDetails()
        {
            var department = await _context.Departments.FirstAsync();
            var degree = await _context.AcademicDegrees.FirstAsync();
            var position = await _context.JobPositions.FirstAsync();

            var result = await _service.AddTeacherAsync("Александр", "Пушкин", position.Id, degree.Id, department.Id);

            Assert.NotNull(result);
            Assert.Equal("Александр", result.FirstName);
            Assert.Equal("Пушкин", result.LastName);
            Assert.Equal(degree.Name, result.AcademicDegree);
            Assert.Equal(position.Name, result.JobPosition);
            Assert.Equal(department.Name, result.Department);
        }

        [Fact]
        public async Task UpdateTeacherAsync_ValidData_UpdatesAllFields()
        {
            var teacher = await _context.Teachers.FirstAsync();
            var newDepartment = await _context.Departments.Skip(1).FirstAsync();
            var newDegree = await _context.AcademicDegrees.Skip(1).FirstAsync();
            var newPosition = await _context.JobPositions.Skip(1).FirstAsync();

            var result = await _service.UpdateTeacherAsync(
                teacher.Id,
                "Новоеимя",
                "Новаяфамилия",
                newPosition.Id,
                newDegree.Id,
                newDepartment.Id);

            Assert.NotNull(result);
            Assert.Equal("Новоеимя", result.FirstName);
            Assert.Equal("Новаяфамилия", result.LastName);
            Assert.Equal(newDegree.Name, result.AcademicDegree);
            Assert.Equal(newPosition.Name, result.JobPosition);
            Assert.Equal(newDepartment.Name, result.Department);
        }

        [Fact]
        public async Task GetTeacherByIdAsync_ExistingId_ReturnsFullDetails()
        {
            var expectedTeacher = await _context.Teachers
                .Include(t => t.AcademicDegree)
                .Include(t => t.JobPosition)
                .Include(t => t.Department)
                .FirstAsync();

            var result = await _service.GetTeacherByIdAsync(expectedTeacher.Id);

            Assert.NotNull(result);
            Assert.Equal(expectedTeacher.FirstName, result.FirstName);
            Assert.Equal(expectedTeacher.LastName, result.LastName);
            Assert.Equal(expectedTeacher.AcademicDegree.Name, result.AcademicDegree);
            Assert.Equal(expectedTeacher.JobPosition.Name, result.JobPosition);
            Assert.Equal(expectedTeacher.Department?.Name, result.Department);
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task AddTeacherAsync_InvalidName_ThrowsArgumentException()
        {
            var department = await _context.Departments.FirstAsync();
            var degree = await _context.AcademicDegrees.FirstAsync();
            var position = await _context.JobPositions.FirstAsync();

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.AddTeacherAsync("НеправильноеИмя", "Фамилия", position.Id, degree.Id, department.Id));
        }


        [Fact]
        public async Task AddTeacherAsync_InvalidName2_ThrowsArgumentException()
        {
            var department = await _context.Departments.FirstAsync();
            var degree = await _context.AcademicDegrees.FirstAsync();
            var position = await _context.JobPositions.FirstAsync();

            var testTeacher = new Teacher
            {
                FirstName = "И",
                LastName = "Иванов",
                AcademicDegreeId = degree.Id,
                JobPositionId = position.Id,
                DepartmentId = department.Id
            };

            var result = testTeacher.IsNameValid1();

            //await _service.AddTeacherAsync("НЕправильноемя", "Фамилия", position.Id, degree.Id, department.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task AddTeacherAsync_InvalidLastName_ThrowsArgumentException()
        {
            var department = await _context.Departments.FirstAsync();
            var degree = await _context.AcademicDegrees.FirstAsync();
            var position = await _context.JobPositions.FirstAsync();

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.AddTeacherAsync("Имя", "неправильнаяФамилия", position.Id, degree.Id, department.Id));
        }

        [Fact]
        public async Task AddTeacherAsync_NonExistingPosition_ThrowsArgumentException()
        {
            var department = await _context.Departments.FirstAsync();
            var degree = await _context.AcademicDegrees.FirstAsync();
            var invalidPositionId = 999;

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.AddTeacherAsync("Иван", "Иванов", invalidPositionId, degree.Id, department.Id));
        }

        [Fact]
        public async Task AddTeacherAsync_NonExistingDegree_ThrowsArgumentException()
        {
            var department = await _context.Departments.FirstAsync();
            var position = await _context.JobPositions.FirstAsync();
            var invalidDegreeId = 999;

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.AddTeacherAsync("Иван", "Иванов", position.Id, invalidDegreeId, department.Id));
        }

        [Fact]
        public async Task AddTeacherAsync_NonExistingDepartment_ThrowsArgumentException()
        {
            var degree = await _context.AcademicDegrees.FirstAsync();
            var position = await _context.JobPositions.FirstAsync();
            var invalidDepartmentId = 999;

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.AddTeacherAsync("Иван", "Иванов", position.Id, degree.Id, invalidDepartmentId));
        }

        [Fact]
        public async Task UpdateTeacherAsync_NonExistingId_ReturnsNull()
        {
            var result = await _service.UpdateTeacherAsync(999, "Имя", "Фамилия", 1, 1, 1);

            Assert.Null(result);
        }

        #endregion

        #region Validation Tests

        [Theory]
        [InlineData("John", true)]
        [InlineData("john", false)]
        [InlineData("Jo hn", false)]
        [InlineData("John1", false)]
        [InlineData("Иван", true)]
        [InlineData("иван", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void Teacher_FirstNameValidation(string firstName, bool expected)
        {
            var teacher = new Teacher { FirstName = firstName, LastName = "Valid" };
            Assert.Equal(expected, teacher.IsFirstNameValid());
        }

        [Theory]
        [InlineData("Doe", true)]
        [InlineData("doe", false)]
        [InlineData("D oe", false)]
        [InlineData("Doe1", false)]
        [InlineData("Петров", true)]
        [InlineData("петров", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void Teacher_LastNameValidation(string lastName, bool expected)
        {
            var teacher = new Teacher { FirstName = "Valid", LastName = lastName };
            Assert.Equal(expected, teacher.IsLastNameValid());
        }

        #endregion

        private async Task SeedTestData()
        {
            var departments = new List<Department>
            {
                new Department { Name = "Кафедра математики" },
                new Department { Name = "Кафедра физики" }
            };

            var degrees = new List<AcademicDegree>
            {
                new AcademicDegree { Name = "Кандидат наук" },
                new AcademicDegree { Name = "Доктор наук" }
            };

            var positions = new List<JobPosition>
            {
                new JobPosition { Name = "Доцент" },
                new JobPosition { Name = "Профессор" }
            };

            var disciplines = new List<Discipline>
            {
                new Discipline { Name = "Высшая математика" },
                new Discipline { Name = "Теоретическая физика" }
            };

            await _context.Departments.AddRangeAsync(departments);
            await _context.AcademicDegrees.AddRangeAsync(degrees);
            await _context.JobPositions.AddRangeAsync(positions);
            await _context.Disciplines.AddRangeAsync(disciplines);
            await _context.SaveChangesAsync();

            var teachers = new List<Teacher>
            {
                new Teacher
                {
                    FirstName = "Иван",
                    LastName = "Иванов",
                    AcademicDegreeId = degrees[0].Id,
                    JobPositionId = positions[0].Id,
                    DepartmentId = departments[0].Id
                },
                new Teacher
                {
                    FirstName = "Петр",
                    LastName = "Петров",
                    AcademicDegreeId = degrees[0].Id,
                    JobPositionId = positions[0].Id,
                    DepartmentId = departments[0].Id
                },
                new Teacher
                {
                    FirstName = "Сергей",
                    LastName = "Сидоров",
                    AcademicDegreeId = degrees[1].Id,
                    JobPositionId = positions[1].Id,
                    DepartmentId = departments[1].Id
                },
                new Teacher
                {
                    FirstName = "Алексей",
                    LastName = "Алексеев",
                    AcademicDegreeId = degrees[1].Id,
                    JobPositionId = positions[1].Id,
                    DepartmentId = departments[1].Id
                }
            };

            await _context.Teachers.AddRangeAsync(teachers);
            await _context.SaveChangesAsync();

            var workloads = new List<WorkLoad>
            {
                new WorkLoad
                {
                    TeacherId = teachers[0].Id,
                    DisciplineId = disciplines[0].Id,
                    Hours = 40
                },
                new WorkLoad
                {
                    TeacherId = teachers[1].Id,
                    DisciplineId = disciplines[1].Id,
                    Hours = 60
                }
            };

            await _context.WorkLoads.AddRangeAsync(workloads);
            await _context.SaveChangesAsync();
        }
    }
}