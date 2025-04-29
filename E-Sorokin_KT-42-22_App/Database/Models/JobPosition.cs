using System.ComponentModel.DataAnnotations;

namespace E_Sorokin_KT_42_22_App.Database.Models
{
    public class JobPosition
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
