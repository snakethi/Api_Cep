using ApiCep.Api.RateLimiting;
using ApiCep.Application.Address.Models;
using ApiCep.Application.Address.Commands.CreateAddress;
using ApiCep.Application.Address.Commands.DeleteAddress;
using ApiCep.Application.Address.Commands.UpdateAddress;
using ApiCep.Application.Address.Queries.GetAddressById;
using ApiCep.Application.Address.Queries.ListAddressesByUser;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;


namespace ApiCep.Api.Controllers
{
    
    [ApiController]
    [Authorize]
    [ApiVersion(1.0)]
    [EnableRateLimiting(RateLimitPolicies.Api)]
    [Route("api/v{version:apiVersion}/users/{userId:guid}/addresses")]
    public sealed class AddressesController : ControllerBase
    {
        private readonly ISender _sender;

        public AddressesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [ProducesResponseType(typeof(AddressResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<AddressResponse>> CreateAsync(Guid userId, [FromBody] CreateAddressRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateAddressCommand(userId, request.ZipCode, request.Number, request.Complement, request.Street, request.Neighborhood);
            var response = await _sender.Send(command, cancellationToken);

            return CreatedAtRoute("GetAddressById", new { version = "1", userId, addressId = response.Id }, response);
        }


        [HttpGet("{addressId:guid}", Name = "GetAddressById")]
        [ProducesResponseType(typeof(AddressResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<AddressResponse>> GetByIdAsync(Guid userId, Guid addressId, CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new GetAddressByIdQuery(userId, addressId), cancellationToken);

            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<AddressResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<IReadOnlyCollection<AddressResponse>>> ListAsync(Guid userId, CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new ListAddressesByUserQuery(userId), cancellationToken);

            return Ok(response);
        }

        [HttpPut("{addressId:guid}")]
        [ProducesResponseType(typeof(AddressResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AddressResponse>> UpdateAsync(Guid userId, Guid addressId, [FromBody] UpdateAddressRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateAddressCommand(userId, addressId, request.ZipCode, request.Number, request.Complement, request.Street, request.Neighborhood);
            var response = await _sender.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpDelete("{addressId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid userId, Guid addressId, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteAddressCommand(userId, addressId), cancellationToken);

            return NoContent();
        }
    }
}
