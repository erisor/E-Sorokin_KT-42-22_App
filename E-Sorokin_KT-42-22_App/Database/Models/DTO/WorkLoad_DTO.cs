using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Sorokin_KT_42_22_App.Database.Models.DTO
{
    public class WorkLoad_DTO
    {
        public int DisciplineId { get; set; }
        public int TeacherId { get; set; }
        public int Hours { get; set; }
    }
}