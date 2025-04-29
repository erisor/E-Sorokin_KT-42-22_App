using E_Sorokin_KT_42_22_App.Database.Models;
using E_Sorokin_KT_42_22_App.Filters;

namespace E_Sorokin_KT_42_22_App.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<DepartmentFilter>> GetDepartmentsAsync(DateTime? foundedAfter = null, int? minTeacherCount = null);
        Task AddDepartmentAsync(Department department);
        Task<Department> UpdateDepartmentAsync(Department department);
        Task<bool> DeleteDepartmentAsync(int departmentId);
    }
}