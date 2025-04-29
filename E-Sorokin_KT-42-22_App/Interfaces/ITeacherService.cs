using E_Sorokin_KT_42_22_App.Database.Models.DTO;
using E_Sorokin_KT_42_22_App.Database.Models;
using E_Sorokin_KT_42_22_App.Filters;

namespace E_Sorokin_KT_42_22_App.Interfaces
{
    public interface ITeacherService
    {
        Task<List<TeacherFilter>> GetTeachersAsync(string departmentName = null, string degreeName = null, string positionName = null);
        Task<TeacherResponce_DTO> GetTeacherByIdAsync(int id);
        Task<TeacherResponce_DTO> AddTeacherAsync(string firstName, string lastName, int positionId, int degreeId, int? departmentId);
        Task<TeacherResponce_DTO> UpdateTeacherAsync(int id, string firstName, string lastName, int positionId, int degreeId, int? departmentId);
        Task<bool> DeleteTeacherAsync(int id);
        Task<List<DepartmentFilter>> GetDepartmentsByDisciplineAsync(string disciplineName);
    }
}