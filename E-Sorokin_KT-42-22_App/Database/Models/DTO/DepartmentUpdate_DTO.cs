using System.ComponentModel.DataAnnotations;

namespace E_Sorokin_KT_42_22_App.Database.Models.DTO
{
    public class DepartmentUpdate_DTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime FoundedDate { get; set; }

        public int? LeaderId { get; set; }
    }
}