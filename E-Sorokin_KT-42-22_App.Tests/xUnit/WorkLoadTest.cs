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
    public class WorkLoadTest : IDisposable
    {
        private readonly SorokinDBContext _context;
        private readonly WorkLoadService _service;

        public WorkLoadTest()
        {
            var dbName = $"TestDatabase_Load_{Guid.NewGuid()}";
            var options = new DbContextOptionsBuilder<SorokinDBContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            _context = new SorokinDBContext(options);
            _context.Database.EnsureCreated();
            _service = new WorkLoadService(_context);

            SeedTestData().Wait();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region Positive Tests

        [Fact]
        public async Task AddLoadAsync_ValidData_CreatesLoadAndReturnsDetails()
        {
            var teacher = await _context.Teachers.OrderBy(t => t.Id).Skip(2).FirstAsync();
            var discipline = await _context.Disciplines.OrderBy(d => d.Id).Skip(2).FirstAsync();
            const int hours = 40;

            var result = await _service.AddLoadAsync(teacher.Id, discipline.Id, hours);

            Assert.NotNull(result);
            Assert.Equal(hours, result.Hours);
            Assert.Equal($"{teacher.FirstName} {teacher.LastName}", result.TeacherName);
            Assert.Equal(teacher.Department.Name, result.DepartmentName);
            Assert.Equal(discipline.Name, result.DisciplineName);

            var dbLoad = await _context.WorkLoads.FirstOrDefaultAsync(w =>
                w.TeacherId == teacher.Id &&
                w.DisciplineId == discipline.Id);
            Assert.NotNull(dbLoad);
            Assert.Equal(hours, dbLoad.Hours);
        }

        [Fact]
        public async Task UpdateLoadAsync_ValidData_UpdatesLoad()
        {
            var load = await _context.WorkLoads.FirstAsync();
            var newTeacher = await _context.Teachers.Skip(1).FirstAsync();
            var newDiscipline = await _context.Disciplines.Skip(1).FirstAsync();
            const int newHours = 60;

            var result = await _service.UpdateLoadAsync(
                load.Id,
                newTeacher.Id,
                newDiscipline.Id,
                newHours);

            Assert.NotNull(result);
            Assert.Equal(newHours, result.Hours);
            Assert.Equal($"{newTeacher.FirstName} {newTeacher.LastName}", result.TeacherName);
            Assert.Equal(newDiscipline.Name, result.DisciplineName);

            var updatedLoad = await _context.WorkLoads.FindAsync(load.Id);
            Assert.Equal(newTeacher.Id, updatedLoad.TeacherId);
            Assert.Equal(newDiscipline.Id, updatedLoad.DisciplineId);
            Assert.Equal(newHours, updatedLoad.Hours);
        }

        [Fact]
        public async Task GetLoadsAsync_FilterByTeacherFirstName_ReturnsFiltered()
        {
            var teacher = await _context.Teachers.FirstAsync();

            var result = await _service.GetLoadsAsync(teacherFirstName: teacher.FirstName);

            Assert.Single(result);
            Assert.Equal(teacher.FirstName, result.First().TeacherName.Split()[0]);
        }

        [Fact]
        public async Task GetLoadsAsync_FilterByTeacherLastName_ReturnsFiltered()
        {
            var teacher = await _context.Teachers.FirstAsync();

            var result = await _service.GetLoadsAsync(teacherLastName: teacher.LastName);

            Assert.Single(result);
            Assert.Equal(teacher.LastName, result.First().TeacherName.Split()[1]);
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task AddLoadAsync_NonExistingTeacher_ThrowsException()
        {
            var discipline = await _context.Disciplines.FirstAsync();
            const int invalidTeacherId = 999;

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.AddLoadAsync(invalidTeacherId, discipline.Id, 10));
        }

        [Fact]
        public async Task AddLoadAsync_NonExistingDiscipline_ThrowsException()
        {
            var teacher = await _context.Teachers.FirstAsync();
            const int invalidDisciplineId = 999;

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.AddLoadAsync(teacher.Id, invalidDisciplineId, 10));
        }

        [Fact]
        public async Task AddLoadAsync_DuplicateLoad_ThrowsException()
        {
            var existingLoad = await _context.WorkLoads.FirstAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AddLoadAsync(
                    existingLoad.TeacherId,
                    existingLoad.DisciplineId,
                    existingLoad.Hours));
        }

        [Fact]
        public async Task AddLoadAsync_InvalidHours_ThrowsException()
        {
            var teacher = await _context.Teachers.FirstAsync();
            var discipline = await _context.Disciplines.FirstAsync();
            const int invalidHours = 0;

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AddLoadAsync(teacher.Id, discipline.Id, invalidHours));
        }

        [Fact]
        public async Task AddLoadAsync_ExistingLoad_ThrowsException()
        {
            var existingLoad = await _context.WorkLoads
                .Include(w => w.Teacher)
                .Include(w => w.Discipline)
                .FirstAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AddLoadAsync(
                    existingLoad.TeacherId,
                    existingLoad.DisciplineId,
                    existingLoad.Hours));
        }

        #endregion

        private async Task SeedTestData()
        {
            var departments = new List<Department>
            {
                new Department { Name = "Кафедра математики" },
                new Department { Name = "Кафедра физики" }
            };

            var teachers = new List<Teacher>
            {
                new Teacher { FirstName = "Иван", LastName = "Иванов", Department = departments[0] },
                new Teacher { FirstName = "Петр", LastName = "Петров", Department = departments[0] },
                new Teacher { FirstName = "Сергей", LastName = "Сидоров", Department = departments[1] }
            };

            var disciplines = new List<Discipline>
            {
                new Discipline { Name = "Высшая математика" },
                new Discipline { Name = "Теоретическая физика" },
                new Discipline { Name = "Дифференциальные уравнения" }
            };

            await _context.Departments.AddRangeAsync(departments);
            await _context.Teachers.AddRangeAsync(teachers);
            await _context.Disciplines.AddRangeAsync(disciplines);
            await _context.SaveChangesAsync();

            var workloads = new List<WorkLoad>
            {
                new WorkLoad
                {
                    Teacher = teachers[0],
                    Discipline = disciplines[0],
                    Hours = 40
                },
                new WorkLoad
                {
                    Teacher = teachers[1],
                    Discipline = disciplines[1],
                    Hours = 60
                }
            };

            await _context.WorkLoads.AddRangeAsync(workloads);
            await _context.SaveChangesAsync();
        }
    }
}