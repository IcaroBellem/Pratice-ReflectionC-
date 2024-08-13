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

        [HttpPost("Create")]
        public async Task<IActionResult> MakeTimeEntryAsync([FromBody] TimeEntryResponseDTO timeEntryResponseDTO)
        {
            var command = new TimeEntryCommand(timeEntryResponseDTO);
            var result = await _mediator.Send(command);

            if (!result)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }

            return Ok("Time entry created successfully.");
        }

        [HttpGet("{memberId}")]
        public async Task<IActionResult> GetTimeEntriesByIdAsync(int memberId)
        {
            var query = new GetTimeEntryQuery(memberId);
            var timeEntries = await _mediator.Send(query);

            if (timeEntries == null || !timeEntries.Any())
            {
                return NotFound("No time entries found for the specified member.");
            }

            return Ok(timeEntries);
        }


    }
}