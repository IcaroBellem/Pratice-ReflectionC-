using AppointmentRules.Command.Query;
using AppointmentRules.Service.DTOs;
using AppointmentRules.Service.Interface;
using MediatR;
using System.Net;

namespace AppointmentRules.Command.Handler
{
    public class GetTimeEntryByIdCommandHandler : IRequestHandler<GetTimeEntryQuery, List<TimeEntryDTO>>
    {
        private readonly IRuleService _ruleService;

        public GetTimeEntryByIdCommandHandler(IRuleService ruleService)
        {
            _ruleService = ruleService;
        }

        public async Task<List<TimeEntryDTO>> Handle(GetTimeEntryQuery request, CancellationToken cancellationToken)
        {
            var timeEntries = await _ruleService.GetTimeEntriesByIdAsync(request.MemberId);
            if (timeEntries == null)
            {
                throw new Exception("Time entries not found");
            }
            return timeEntries;
        }
    }
}
