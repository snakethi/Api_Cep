using ApiCep.Api.RateLimiting;
using ApiCep.Application.Common.Models;
using ApiCep.Application.User.Commands.CreateUser;
using ApiCep.Application.User.Commands.DeleteUser;
using ApiCep.Application.User.Commands.UpdateUser;
using ApiCep.Application.User.Models;
using ApiCep.Application.User.Queries.GetUserById;
using ApiCep.Application.User.Queries.ListUsers;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ApiCep.Api.Controllers
{
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/users")]
    [EnableRateLimiting(RateLimitPolicies.Api)]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ISender _sender;

        public UsersController(ISender sender)
        {
            _sender = sender;
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<UserResponse>> CreateAsync([FromBody] CreateUserRequest request,CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new CreateUserCommand(request.Name, request.Email, request.Password), cancellationToken);

            return CreatedAtRoute("GetUserById", new { version = "1", id = response.Id }, response);
        }

        [HttpGet("{id:guid}", Name = "GetUserById")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<UserResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new GetUserByIdQuery(id), cancellationToken);

            return Ok(response);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<UserResponse>> UpdateAsync(Guid id,[FromBody] UpdateUserRequest request,CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new UpdateUserCommand(id, request.Name, request.Email), cancellationToken);

            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteUserCommand(id), cancellationToken);

            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<PagedResult<UserResponse>>> ListAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string sortBy = "name", [FromQuery] string sortDirection = "asc", CancellationToken cancellationToken = default)
        {
            var query = new ListUsersQuery(page, pageSize, search, sortBy, sortDirection);
            var response = await _sender.Send(query, cancellationToken);

            return Ok(response);
        }
    }
}
