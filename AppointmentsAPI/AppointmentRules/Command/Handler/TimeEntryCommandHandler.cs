using AppointmentRules.Data;
using AppointmentRules.Service.Interface;
using MediatR;

namespace AppointmentRules.Command.Handler
{
    public class TimeEntryCommandHandler : IRequestHandler<TimeEntryCommand, bool>
    {
        private readonly IRuleService _ruleService;

        public TimeEntryCommandHandler(IRuleService ruleService, AppDbContext context)
        {
            _ruleService = ruleService;
        }

        public async Task<bool> Handle(TimeEntryCommand request, CancellationToken cancellationToken)
        {
            return await _ruleService.MakeTimeEntryAsync(request.TimeEntryDTO);
        }
    }

}
