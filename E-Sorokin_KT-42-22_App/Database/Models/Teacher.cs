using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace E_Sorokin_KT_42_22_App.Database.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required] 
        public string LastName { get; set; }

        public int AcademicDegreeId { get; set; }

        public int JobPositionId { get; set; }

        public int? DepartmentId { get; set; }

        [ForeignKey("AcademicDegreeId")]
        public virtual AcademicDegree AcademicDegree { get; set; }

        [ForeignKey("JobPositionId")]
        public virtual JobPosition JobPosition { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        public virtual ICollection<WorkLoad> WorkLoad { get; set; } = new List<WorkLoad>();

        public bool IsFirstNameValid()
        {
            return !string.IsNullOrEmpty(FirstName) 
                && Regex.IsMatch(FirstName, @"^[A-ZА-Я][a-zа-яё]*$");
        }

        public bool IsNameValid1()
        {
            return !string.IsNullOrEmpty(FirstName)
                && !string.IsNullOrEmpty(LastName)
                && Regex.Match(FirstName, @"^[A-ZА-Я][a-zа-яё]*$").Success
                && Regex.Match(LastName, @"^[A-ZА-Я][a-zа-яё]*$").Success;
        }

        public bool IsLastNameValid()
        {
            return !string.IsNullOrEmpty(LastName) 
                && Regex.IsMatch(LastName, @"^[A-ZА-Я][a-zа-яё]*$");
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(FirstName)
                && !string.IsNullOrEmpty(LastName)
                && Regex.IsMatch(FirstName, @"^[A-ZА-Я][a-zа-яё]*$") 
                && Regex.IsMatch(LastName, @"^[A-ZА-Я][a-zа-яё]*$");
        }
    }
}