using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace E_Sorokin_KT_42_22_App.Database.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime FoundedDate { get; set; }

        public int? LeaderId { get; set; }

        [ForeignKey("LeaderId")]
        public virtual Teacher? Leader { get; set; }

        public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

        public bool IsValidName()
        {
            return !string.IsNullOrEmpty(Name)
              && (Regex.IsMatch(Name, @"Department", RegexOptions.IgnoreCase)
              || Regex.IsMatch(Name, @"Кафедра", RegexOptions.IgnoreCase));
        }
    }
}