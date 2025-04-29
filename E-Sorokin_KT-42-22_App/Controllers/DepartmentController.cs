using E_Sorokin_KT_42_22_App.Database.Models;
using E_Sorokin_KT_42_22_App.Database.Models.DTO;
using E_Sorokin_KT_42_22_App.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace E_Sorokin_KT_42_22_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly DepartmentService _departmentService;

        public DepartmentsController(DepartmentService departmentService)
        {
            _departmentService = departmentService;
        }


        [HttpGet]
        public async Task<IActionResult> GetDepartments([FromQuery] DateTime? foundedAfter = null, [FromQuery] int? minTeacherCount = null)
        {
            var departments = await _departmentService.GetDepartmentsAsync(foundedAfter, minTeacherCount);
            return Ok(departments);
        }

        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] Department_DTO DepartmentDto)
        {
            if (DepartmentDto == null)
            {
                return BadRequest();
            }

            var department = new Department
            {
                Name = DepartmentDto.Name,
                FoundedDate = DepartmentDto.FoundedDate
            };

            await _departmentService.AddDepartmentAsync(department);
            return CreatedAtAction(nameof(GetDepartments), new { id = department.Id }, department);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentUpdate_DTO DepartmentDto)
        {
            if (DepartmentDto == null)
            {
                return BadRequest(new { message = "Данные для кафедры обновления не могут быть пустыми" });
            }

            var department = new Department
            {
                Id = id,
                Name = DepartmentDto.Name,
                FoundedDate = DepartmentDto.FoundedDate,
                LeaderId = DepartmentDto.LeaderId
            };

            var updatedDepartment = await _departmentService.UpdateDepartmentAsync(department);
            if (updatedDepartment == null)
            {
                return NotFound(new { message = "Кафедра не найдена" });
            }


            var updatedDepartmentDto = new Department_DTO
            {
                Name = updatedDepartment.Name,
                FoundedDate = updatedDepartment.FoundedDate,
                LeaderId = updatedDepartment.LeaderId
            };

            return Ok(updatedDepartmentDto);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var result = await _departmentService.DeleteDepartmentAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Кафедра не найдена" });
            }

            return NoContent();
        }
    }
}