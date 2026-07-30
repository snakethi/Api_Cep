using ApiCep.Api.RateLimiting;
using ApiCep.Application.Exports.Queries.ExportUsersCsv;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ApiCep.Api.Controllers
{
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/exports")]
    [EnableRateLimiting(RateLimitPolicies.Api)]
    [ApiController]
    [Authorize]
    public class ExportsController : ControllerBase
    {
        private readonly ISender _sender;

        public ExportsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("users/csv")]
        [Produces("text/csv")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportUsersCsvAsync([FromQuery] Guid? userId, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new ExportUsersCsvQuery(userId), cancellationToken);

            return File(result.Content, result.ContentType, result.FileName);
        }
    }
}
