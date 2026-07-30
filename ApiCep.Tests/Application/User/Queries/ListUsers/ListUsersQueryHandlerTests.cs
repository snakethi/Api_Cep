using ApiCep.Application.Common.Models;
using ApiCep.Application.Interfaces.Repositories;
using ApiCep.Application.User.Queries.ListUsers;
using NSubstitute;
using UserEntity = ApiCep.Domain.Entities.User;

namespace ApiCep.Tests.Application.User.Queries.ListUsers
{
    public sealed class ListUsersQueryHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly ListUsersQueryHandler _handler;

        public ListUsersQueryHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _handler = new ListUsersQueryHandler(_userRepository);
        }

        [Fact]
        public async Task Handle_ShouldReturnPagedUsers()
        {
            var query = new ListUsersQuery(1, 10, "thiago", "name", "asc");
            var firstUser = new UserEntity("Thiago Botaro", "thiago@teste.com", "hash-1");
            var secondUser = new UserEntity("Thiago Silva", "thiago.silva@teste.com", "hash-2");
            var users = new[] { firstUser, secondUser };
            var pagedResult = new PagedResult<UserEntity>(users, 1, 10, 2, 1);

            _userRepository.GetPagedAsync(query.Page, query.PageSize, query.Search, query.SortBy, query.SortDirection, Arg.Any<CancellationToken>()).Returns(pagedResult);

            var response = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(2, response.Items.Count);
            Assert.Equal(1, response.Page);
            Assert.Equal(10, response.PageSize);
            Assert.Equal(2, response.TotalItems);
            Assert.Equal(1, response.TotalPages);

            var firstResponse = response.Items.First();

            Assert.Equal(firstUser.Id, firstResponse.Id);
            Assert.Equal(firstUser.Name, firstResponse.Name);
            Assert.Equal(firstUser.Email, firstResponse.Email);
            Assert.True(firstResponse.IsActive);

            await _userRepository.Received(1).GetPagedAsync(1, 10, "thiago", "name", "asc", Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyPage_WhenRepositoryHasNoUsers()
        {
            var query = new ListUsersQuery(2, 10, null, "createdAtUtc", "desc");
            var pagedResult = new PagedResult<UserEntity>(Array.Empty<UserEntity>(), 2, 10, 0, 0);

            _userRepository.GetPagedAsync(query.Page, query.PageSize, query.Search, query.SortBy, query.SortDirection, Arg.Any<CancellationToken>()).Returns(pagedResult);

            var response = await _handler.Handle(query, CancellationToken.None);

            Assert.Empty(response.Items);
            Assert.Equal(2, response.Page);
            Assert.Equal(10, response.PageSize);
            Assert.Equal(0, response.TotalItems);
            Assert.Equal(0, response.TotalPages);
        }
    }
}
