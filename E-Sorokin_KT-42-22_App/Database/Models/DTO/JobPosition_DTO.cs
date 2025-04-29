using System.ComponentModel.DataAnnotations;

namespace E_Sorokin_KT_42_22_App.Database.Models.DTO
{
    public class JobPosition_DTO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
