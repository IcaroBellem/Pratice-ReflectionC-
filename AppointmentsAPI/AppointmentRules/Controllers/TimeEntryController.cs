using AppointmentRules.Command;
using AppointmentRules.Command.Query;
using AppointmentRules.Service.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentRules.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeEntryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TimeEntryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> MakeTimeEntryAsync([FromBody] TimeEntryDTO timeEntryDTO)
        {
            var command = new TimeEntryCommand(timeEntryDTO);
            var result = await _mediator.Send(command);
            return result ? Ok(result) : BadRequest();
        }

        [HttpGet("{memberId}")]
        public async Task<IActionResult> GetTimeEntriesByIdAsync(int memberId)
        {
            var query = new GetTimeEntryQuery(memberId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
        
}
