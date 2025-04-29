using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Sorokin_KT_42_22_App.Database.Models
{
    public class WorkLoad
    {
        [Key]
        public int Id { get; set; }

        public int TeacherId { get; set; }

        public int DisciplineId { get; set; }

        public int Hours { get; set; }

        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; }

        [ForeignKey("DisciplineId")]
        public virtual Discipline Discipline { get; set; }
    }
}
