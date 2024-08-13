using AppointmentRules.Command;
using AppointmentRules.Data;
using AppointmentRules.Models;
using AppointmentRules.Service.Interface;
using MediatR;

public class TimeEntryHandler : IRequestHandler<TimeEntryCommand, bool>
{
    private readonly AppDbContext _context;
    private readonly IRuleService _ruleService;

    public TimeEntryHandler(AppDbContext context, IRuleService ruleService)
    {
        _context = context;
        _ruleService = ruleService;
    }

    public async Task<bool> Handle(TimeEntryCommand request, CancellationToken cancellationToken)
    {
     return await _ruleService.MakeTimeEntryAsync(request.TimeEntryResponseDTO);
    }
}
