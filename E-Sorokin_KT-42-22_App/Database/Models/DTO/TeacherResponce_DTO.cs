namespace E_Sorokin_KT_42_22_App.Database.Models.DTO
{
    public class TeacherResponce_DTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int AcademicDegreeId { get; set; }
        public string AcademicDegree { get; set; }
        public int JobPositionId { get; set; }
        public string JobPosition { get; set; }
        public int? DepartmentId { get; set; }
        public string Department { get; set; }
        public List<WorkLoad> WorkLoad { get; set; }
    }
}