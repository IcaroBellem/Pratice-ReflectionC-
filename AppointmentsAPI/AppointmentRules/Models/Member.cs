namespace AppointmentRules.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ContractType ContractType { get; set; }
        public ICollection<TimeEntry> TimeEntries{ get; set; } 
        public ICollection<ProjectTask> Tasks { get; set; }
    }

    public enum ContractType
    {
        Internal,
        Employee
    }
}

