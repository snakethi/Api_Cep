using ApiCep.Api.RateLimiting;
using ApiCep.Application.Address.Models;
using ApiCep.Application.Address.Queries.GetAddressByZipCode;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ApiCep.Api.Controllers
{
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/zipcodes")]
    [EnableRateLimiting(RateLimitPolicies.Api)]
    [ApiController]
    [Authorize]
    public class ZipCodesController : ControllerBase
    {
        private readonly ISender _sender;

        public ZipCodesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{zipCode}")]
        [ProducesResponseType(typeof(ViaCepAddressResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<ViaCepAddressResult>> GetByZipCodeAsync(string zipCode, CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new GetAddressByZipCodeQuery(zipCode), cancellationToken);

            return Ok(response);
        }
    }
}
