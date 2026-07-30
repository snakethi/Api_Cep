using ApiCep.Api.RateLimiting;
using ApiCep.Application.Authentication.Models;
using ApiCep.Application.Authentication.Commands.Login;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ApiCep.Api.Controllers
{

    [ApiVersion(1.0)]
    [EnableRateLimiting(RateLimitPolicies.Api)]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        [EnableRateLimiting(RateLimitPolicies.Login)]
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest request,CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new LoginCommand(request.Email, request.Password), cancellationToken);

            return Ok(response);
        }
    }
}
