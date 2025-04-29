using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Sorokin_KT_42_22_App.Database.Models.DTO
{
    public class Teacher_DTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int JobPositionId { get; set; }
        public int AcademicDegreeId { get; set; }
        public int? DepartmentId { get; set; }
    }
}