namespace AppointmentRules.Service.DTOs
{
    public class TimeEntryResponseDTO
    {
        public int MemberId { get; set; }
        public int TaskId { get; set; }
        public DateTime Entry { get; set; }
        public DateTime? Exit { get; set; }

    }

}
