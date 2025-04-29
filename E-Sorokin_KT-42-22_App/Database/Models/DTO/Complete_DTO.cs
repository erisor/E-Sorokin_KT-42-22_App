namespace E_Sorokin_KT_42_22_App.Database.Models.DTO
{
    public class Complete_DTO
    {
        public class Discipline2_DTO
        {
            public string Name { get; set; }
            public int Hours { get; set; }
        }

        public class Teacher2_DTO
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Degree { get; set; }
            public string Position { get; set; }
            public List<Discipline2_DTO> Disciplines { get; set; }
        }

        public class Department2Dto
        {
            public string Name { get; set; }
            public string HeadName { get; set; }
            public List<Teacher2_DTO> Teachers { get; set; }
        }
    }
}