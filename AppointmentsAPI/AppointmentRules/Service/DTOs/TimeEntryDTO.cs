using AppointmentRules.Models;

namespace AppointmentRules.Service.DTOs
{
    public class TimeEntryDTO
    {
        public int MemberId { get; set; }
        public int TaskId { get; set; }
        public DateTime Entry { get; set; }
        public DateTime? Exit { get; set; }
        public bool? IsApproved { get; set; }
        public string ApprovalMessage { get; set; }

    }
}
