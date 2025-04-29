namespace E_Sorokin_KT_42_22_App.Filters
{
    public class DisciplineFilter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalHours { get; set; }
        public List<string> Teachers { get; set; } = new List<string>();
    }
}