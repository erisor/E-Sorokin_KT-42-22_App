using E_Sorokin_KT_42_22_App.Filters;

namespace E_Sorokin_KT_42_22_App.Interfaces
{
    public interface IWorkLoadService
    {
        Task<List<WorkLoadFilter>> GetLoadsAsync(string teacherFirstName = null, string teacherLastName = null, string departmentName = null, string disciplineName = null);
        Task<WorkLoadFilter> AddLoadAsync(int teacherId, int disciplineId, int hours);
        Task<WorkLoadFilter> UpdateLoadAsync(int loadId, int teacherId, int disciplineId, int hours);
    }
}