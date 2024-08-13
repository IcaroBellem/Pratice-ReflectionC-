using AppointmentRules.Models;
using AppointmentRules.Service.DTOs;
using MediatR;

namespace AppointmentRules.Command
{
    public class TimeEntryCommand : IRequest<bool>
    {
        public TimeEntryDTO TimeEntryDTO { get; set; }

        public TimeEntryCommand(TimeEntryDTO timeEntryDTO)
        {
            TimeEntryDTO = timeEntryDTO;
        }
    }
}
