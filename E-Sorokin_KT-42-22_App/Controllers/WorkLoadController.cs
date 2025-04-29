using E_Sorokin_KT_42_22_App.Database.Models;
using E_Sorokin_KT_42_22_App.Database.Models.DTO;
using E_Sorokin_KT_42_22_App.Filters;
using E_Sorokin_KT_42_22_App.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static E_Sorokin_KT_42_22_App.Services.WorkLoadService;

namespace E_Sorokin_KT_42_22_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkLoadsController : ControllerBase
    {
        private readonly WorkLoadService _loadService;

        public WorkLoadsController(WorkLoadService loadService)
        {
            _loadService = loadService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLoads([FromQuery] string? teacherFirstName, [FromQuery] string? teacherLastName, [FromQuery] string? departmentName, [FromQuery] string? disciplineName)
        {
            var loads = await _loadService.GetLoadsAsync(teacherFirstName, teacherLastName, departmentName, disciplineName);
            return Ok(loads);
        }

        [HttpPost]
        public async Task<IActionResult> AddLoad([FromBody] WorkLoad_DTO loadDto)
        {
            if (loadDto == null || loadDto.Hours <= 0)
            {
                return BadRequest("Некорректные данные нагрузки.");
            }

            var load = await _loadService.AddLoadAsync(loadDto.TeacherId, loadDto.DisciplineId, loadDto.Hours);
            return CreatedAtAction(nameof(GetLoads), new { id = load.Id }, load);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLoad(int id, [FromBody] WorkLoad_DTO loadDto)
        {
            if (loadDto == null || loadDto.Hours <= 0)
            {
                return BadRequest("Некорректные данные нагрузки.");
            }

            try
            {
                var updatedLoad = await _loadService.UpdateLoadAsync(id, loadDto.TeacherId, loadDto.DisciplineId, loadDto.Hours);
                return Ok(updatedLoad);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Нагрузка с id {id} не найдена.");
            }
        }
    }
}