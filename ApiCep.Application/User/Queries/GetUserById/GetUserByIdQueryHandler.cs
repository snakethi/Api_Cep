using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.Repositories;
using ApiCep.Application.User.Models;
using MediatR;

namespace ApiCep.Application.User.Queries.GetUserById
{
    public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponse>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (user is null)
                throw new NotFoundException("Usuário não encontrado.");

            return new UserResponse(user.Id,user.Name,user.Email,user.IsActive,user.CreatedAtUtc,user.UpdatedAtUtc);
        }
    }
}
