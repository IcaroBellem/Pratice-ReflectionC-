using AppointmentRules.Models;
using AppointmentRules.Service.DTOs;

namespace AppointmentRules.Service.Interface
{
    public interface IRuleService
    {
        Task<bool> MakeTimeEntryAsync(TimeEntryResponseDTO timeEntryResponseDTO);
        Task<List<TimeEntryDTO>> GetTimeEntriesByIdAsync(int memberId);
        Task<TimeEntryDTO> ApplyRulesAsync(TimeEntryDTO timeEntryDTO);
    }
}
