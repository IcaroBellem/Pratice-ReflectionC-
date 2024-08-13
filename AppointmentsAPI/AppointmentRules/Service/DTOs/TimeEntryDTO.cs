namespace AppointmentRules.Service.DTOs
{
    public class TimeEntryDTO
    {
        public int MemberId{ get; set; }
        public DateTime Entry { get; set; }
        public DateTime? Exit { get; set; }
       
    }
}
