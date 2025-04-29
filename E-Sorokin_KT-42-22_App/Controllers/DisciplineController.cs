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
    public class DisciplinesController : ControllerBase
    {
        private readonly DisciplineService _disciplineService;

        public DisciplinesController(DisciplineService disciplineService)
        {
            _disciplineService = disciplineService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDisciplines([FromQuery] string? firstName, [FromQuery] string? lastName, [FromQuery] int? minHours, [FromQuery] int? maxHours)
        {
            var disciplines = await _disciplineService.GetDisciplinesAsync(firstName, lastName, minHours, maxHours);
            return Ok(disciplines);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDisciplineById(int id)
        {
            var discipline = await _disciplineService.GetDisciplineByIdAsync(id);
            if (discipline == null)
            {
                return NotFound("Дисциплина не найдена.");
            }

            var disciplineFilter = new DisciplineFilter
            {
                Id = discipline.Id,
                Name = discipline.Name,
                TotalHours = discipline.WorkLoad.Sum(l => l.Hours),
                Teachers = discipline.WorkLoad.Select(l => $"{l.Teacher.FirstName} {l.Teacher.LastName}").ToList()
            };

            return Ok(disciplineFilter);
        }

        [HttpPost]
        public async Task<IActionResult> AddDiscipline([FromBody] Discipline_DTO disciplineDto)
        {
            if (disciplineDto == null || string.IsNullOrEmpty(disciplineDto.Name))
            {
                return BadRequest("Дисциплина не указана или имя пустое.");
            }

            var createdDiscipline = await _disciplineService.AddDisciplineAsync(disciplineDto);
            return CreatedAtAction(nameof(GetDisciplineById), new { id = createdDiscipline.Id }, createdDiscipline);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscipline(int id, [FromBody] Discipline_DTO disciplineDto)
        {
            if (disciplineDto == null || string.IsNullOrEmpty(disciplineDto.Name))
            {
                return BadRequest("Дисциплина не указана или имя пустое.");
            }

            try
            {
                var updatedDiscipline = await _disciplineService.UpdateDisciplineAsync(id, disciplineDto);
                return Ok(updatedDiscipline);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Дисциплина не найдена.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscipline(int id)
        {
            var result = await _disciplineService.DeleteDisciplineAsync(id);
            if (!result)
            {
                return NotFound("Дисциплина не найдена.");
            }

            return NoContent();
        }
    }
}