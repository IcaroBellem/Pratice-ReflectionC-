namespace AppointmentRules.Models
{
    public class TimeEntry
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool? IsApproved { get; set; }
        public int TaskId { get; set; }
        public ProjectTask Tasks { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
    }
}
