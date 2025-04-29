using E_Sorokin_KT_42_22_App.Database.Models;
using E_Sorokin_KT_42_22_App.Database.Models.DTO;
using E_Sorokin_KT_42_22_App.Filters;

namespace E_Sorokin_KT_42_22_App.Interfaces
{
    public interface IDisciplineService
    {
        Task<List<DisciplineFilter>> GetDisciplinesAsync(string firstName = null, string lastName = null, int? minHours = null, int? maxHours = null);
        Task<Discipline> GetDisciplineByIdAsync(int id);
        Task<Discipline> AddDisciplineAsync(Discipline_DTO discipline_Dto);
        Task<Discipline> UpdateDisciplineAsync(int id, Discipline_DTO discipline_Dto);
        Task<bool> DeleteDisciplineAsync(int id);
    }
}