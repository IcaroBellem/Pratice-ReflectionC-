namespace AppointmentRules.Models
{
    public class ProjectTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public ICollection<Member> Members { get; set; }
        public ICollection<TimeEntry> TimeEntries { get; set; }


    }
}
