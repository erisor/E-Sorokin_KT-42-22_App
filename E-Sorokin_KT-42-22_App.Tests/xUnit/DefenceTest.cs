using E_Sorokin_KT_42_22_App.Controllers;
using E_Sorokin_KT_42_22_App.Database;
using E_Sorokin_KT_42_22_App.Database.Models;
using E_Sorokin_KT_42_22_App.Filters;
using E_Sorokin_KT_42_22_App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace E_Sorokin_KT_42_22_App.Tests.xUnit
{
    public class DefenceTest : IDisposable
    {
        private readonly SorokinDBContext _context;
        private readonly TeacherService _teacherService;
        private readonly TeachersController _controller;

        public DefenceTest()
        {
            var dbName = $"TestDatabase_TeachersController_{Guid.NewGuid()}";
            var options = new DbContextOptionsBuilder<SorokinDBContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            _context = new SorokinDBContext(options);
            _context.Database.EnsureCreated();
            _teacherService = new TeacherService(_context);
            _controller = new TeachersController(_teacherService);

            SeedTestData().Wait();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetDepartmentsByDiscipline_ValidDiscipline_ReturnsAllDepartments0()
        {
            var disciplineName = "Математика";

            var result = await _controller.GetDepartmentsByDiscipline(disciplineName);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var departments = Assert.IsAssignableFrom<List<DepartmentFilter>>(okResult.Value);

            Assert.Equal(2, departments.Count);

            Assert.Contains(departments, d => d.Name == "Кафедра 1");
            Assert.Contains(departments, d => d.Name == "Кафедра 2");

            //var department1 = departments.First(d => d.Name == "Кафедра 1");
            //Assert.Equal("Иван Иванов", department1.Leader);

            //var department2 = departments.First(d => d.Name == "Кафедра 2");
            //Assert.Equal("Сидр Сидоров", department2.Leader);
        }

        //[Fact]
        //public async Task GetDepartmentsByDiscipline_ValidDiscipline_ReturnsAllDepartments1()
        //{
        //    var disciplineName = "Физика";

        //    var result = await _controller.GetDepartmentsByDiscipline(disciplineName);

        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var departments = Assert.IsAssignableFrom<List<DepartmentFilter>>(okResult.Value);

        //    Assert.Equal(1, departments.Count);

        //    Assert.Contains(departments, d => d.Name == "Кафедра 1");
        //    Assert.Contains(departments, d => d.Name == "Кафедра 2");

        //    var department2 = departments.First(d => d.Name == "Кафедра 2");
        //    Assert.Equal("Иван Иванов", department2.Leader);
        //}

        private async Task SeedTestData()
        {
            var departments = new List<Department>
            {
                new Department { Name = "Кафедра 1" },
                new Department { Name = "Кафедра 2" }
            };

            await _context.Departments.AddRangeAsync(departments);
            await _context.SaveChangesAsync();

            var teachers = new List<Teacher>
            {
                new Teacher
                {
                    FirstName = "Иван",
                    LastName = "Иванов",
                    DepartmentId = departments[0].Id
                },
                new Teacher
                {
                    FirstName = "Петр",
                    LastName = "Петров",
                    DepartmentId = departments[0].Id
                },
                new Teacher
                {
                    FirstName = "Сидр",
                    LastName = "Сидоров",
                    DepartmentId = departments[1].Id
                }
            };

            await _context.Teachers.AddRangeAsync(teachers);
            await _context.SaveChangesAsync();

            departments[0].LeaderId = teachers[0].Id;
            departments[1].LeaderId = teachers[2].Id;
            await _context.SaveChangesAsync();

            var disciplines = new List<Discipline>
            {
                new Discipline { Name = "Математика" },
                new Discipline { Name = "Физика" }
            };

            await _context.Disciplines.AddRangeAsync(disciplines);
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
                    Hours = 50
                },
                new WorkLoad
                {
                    TeacherId = teachers[2].Id,
                    DisciplineId = disciplines[0].Id,
                    Hours = 60
                }
            };

            await _context.WorkLoads.AddRangeAsync(workloads);
            await _context.SaveChangesAsync();
        }
    }
}