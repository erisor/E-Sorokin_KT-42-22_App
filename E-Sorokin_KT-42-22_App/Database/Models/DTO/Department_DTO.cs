using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace E_Sorokin_KT_42_22_App.Database.Models.DTO
{
    public class Department_DTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime FoundedDate { get; set; }

        [DefaultValue("null")]
        public int? LeaderId { get; set; }
    }
}
