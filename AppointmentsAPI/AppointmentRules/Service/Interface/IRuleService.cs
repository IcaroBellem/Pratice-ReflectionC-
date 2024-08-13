using AppointmentRules.Service.DTOs;

namespace AppointmentRules.Service.Interface
{
    public interface IRuleService
    {
        Task<bool> MakeTimeEntryAsync(TimeEntryDTO timeEntryDTO);
        Task<List<TimeEntryDTO>> GetTimeEntriesByIdAsync(int memberId);
    }
}
