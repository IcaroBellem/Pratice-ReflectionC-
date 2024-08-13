using AppointmentRules.Models;
using AppointmentRules.Service.DTOs;
using MediatR;

namespace AppointmentRules.Command
{
    public class TimeEntryCommand : IRequest<bool>
    {
        public TimeEntryResponseDTO TimeEntryResponseDTO { get; set; }

        public TimeEntryCommand(TimeEntryResponseDTO timeEntryResponseDTO)
        {
            TimeEntryResponseDTO = timeEntryResponseDTO;
        }
    }
}
