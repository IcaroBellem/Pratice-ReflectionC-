using AppointmentRules.Service.DTOs;
using MediatR;

namespace AppointmentRules.Command.Query
{
    public class GetTimeEntryQuery : IRequest<List<TimeEntryDTO>>
    {
        public int MemberId { get; set; }

        public GetTimeEntryQuery(int memberId)
        {
            MemberId = memberId;
        }
    }
}
