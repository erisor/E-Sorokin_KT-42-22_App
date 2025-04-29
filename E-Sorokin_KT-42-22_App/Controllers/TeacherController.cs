using E_Sorokin_KT_42_22_App.Database.Models;
using E_Sorokin_KT_42_22_App.Database.Models.DTO;
using E_Sorokin_KT_42_22_App.Filters;
using E_Sorokin_KT_42_22_App.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace E_Sorokin_KT_42_22_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeachersController : ControllerBase
    {
        private readonly TeacherService _teacherService;

        public TeachersController(TeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeachers([FromQuery] string? departmentName, [FromQuery] string? degreeName, [FromQuery] string? positionName)
        {
            var teachers = await _teacherService.GetTeachersAsync(departmentName, degreeName, positionName);
            return Ok(teachers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Teacher>> GetTeacherById(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
            {
                return NotFound($"Учитель с идентификатором {id} не найден.");
            }
            return Ok(teacher);
        }

        [HttpPost]
        public async Task<IActionResult> AddTeacher([FromBody] Teacher_DTO teacherDto)
        {
            if (teacherDto == null)
            {
                return BadRequest("Некорректные данные учителя.");
            }

            var teacherResponse = await _teacherService.AddTeacherAsync(
                teacherDto.FirstName,
                teacherDto.LastName,
                teacherDto.JobPositionId,
                teacherDto.AcademicDegreeId,
                teacherDto.DepartmentId
            );

            return CreatedAtAction(nameof(GetTeacherById), new { id = teacherResponse.Id }, teacherResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(int id, [FromBody] Teacher_DTO teacherDto)
        {
            if (teacherDto == null)
            {
                return BadRequest("Некорректные данные учителя.");
            }

            var teacherResponse = await _teacherService.UpdateTeacherAsync(
                id,
                teacherDto.FirstName,
                teacherDto.LastName,
                teacherDto.JobPositionId,
                teacherDto.AcademicDegreeId,
                teacherDto.DepartmentId
            );

            if (teacherResponse == null)
            {
                return NotFound($"Учитель с идентификатором {id} не найден.");
            }

            return Ok(teacherResponse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTeacher(int id)
        {
            var result = await _teacherService.DeleteTeacherAsync(id);
            if (!result)
            {
                return NotFound($"Учитель с идентификатором {id} не найден.");
            }
            return NoContent();
        }

        [HttpGet("by-discipline/{disciplineName}")]
        public async Task<IActionResult> GetDepartmentsByDiscipline(string disciplineName)
        {
            if (string.IsNullOrWhiteSpace(disciplineName))
            {
                return BadRequest("Название дисциплины не может быть пустым.");
            }

            try
            {
                var departments = await _teacherService.GetDepartmentsByDisciplineAsync(disciplineName);
                if (departments == null || departments.Count == 0)
                {
                    return NotFound($"Кафедры с преподавателями дисциплины '{disciplineName}' не найдены.");
                }
                return Ok(departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Произошла ошибка при поиске кафедр: {ex.Message}");
            }
        }
    }
}