using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Exports.Models;
using ApiCep.Application.Exports.Queries.ExportUsersCsv;
using ApiCep.Application.Interfaces.FileExport;
using ApiCep.Application.Interfaces.Repositories;
using NSubstitute;

namespace ApiCep.Tests.Application.Exports.Queries.ExportUsersCsv
{
    public sealed class ExportUsersCsvQueryHandlerTests
    {
        private readonly IUserExportRepository _userExportRepository;
        private readonly ICsvExportService _csvExportService;
        private readonly ExportUsersCsvQueryHandler _handler;

        public ExportUsersCsvQueryHandlerTests()
        {
            _userExportRepository = Substitute.For<IUserExportRepository>();
            _csvExportService = Substitute.For<ICsvExportService>();
            _handler = new ExportUsersCsvQueryHandler(_userExportRepository, _csvExportService);
        }

        [Fact]
        public async Task Handle_ShouldGenerateCsvFileWithAllUsersAndAddresses_WhenUserIdIsNotProvided()
        {
            var query = new ExportUsersCsvQuery();
            var rows = new[]
            {
        new UserAddressCsvRow(Guid.NewGuid(),"Thiago Botaro","thiago@teste.com",true,Guid.NewGuid(),"01310100","Avenida Paulista","1000","Bela Vista","São Paulo","SP","Apartamento 10")
    };
            var expectedContent = new byte[] { 1, 2, 3, 4 };

            _userExportRepository.GetUsersWithAddressesAsync(null, Arg.Any<CancellationToken>()).Returns(rows);
            _csvExportService.GenerateUsersWithAddresses(rows).Returns(expectedContent);

            var response = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(expectedContent, response.Content);
            Assert.Equal("text/csv; charset=utf-8", response.ContentType);
            Assert.Matches(@"^usuarios-enderecos-\d{8}-\d{6}\.csv$", response.FileName);

            await _userExportRepository.Received(1).GetUsersWithAddressesAsync(null, Arg.Any<CancellationToken>());
            _csvExportService.Received(1).GenerateUsersWithAddresses(rows);
        }

        [Fact]
        public async Task Handle_ShouldGenerateCsvFileForUser_WhenUserIdIsProvided()
        {
            var userId = Guid.NewGuid();
            var query = new ExportUsersCsvQuery(userId);
            var rows = new[]
            {
            new UserAddressCsvRow(userId,"Thiago Botaro","thiago@teste.com",true,Guid.NewGuid(),"01310100","Avenida Paulista","1000","Bela Vista","São Paulo","SP","Apartamento 10")
        };
            var expectedContent = new byte[] { 1, 2, 3, 4 };

            _userExportRepository.GetUsersWithAddressesAsync(userId, Arg.Any<CancellationToken>()).Returns(rows);
            _csvExportService.GenerateUsersWithAddresses(rows).Returns(expectedContent);

            var response = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(expectedContent, response.Content);
            Assert.Equal("text/csv; charset=utf-8", response.ContentType);
            Assert.StartsWith($"usuario-{userId}-enderecos-", response.FileName);
            Assert.EndsWith(".csv", response.FileName);

            await _userExportRepository.Received(1).GetUsersWithAddressesAsync(userId, Arg.Any<CancellationToken>());
            _csvExportService.Received(1).GenerateUsersWithAddresses(rows);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var query = new ExportUsersCsvQuery(userId);

            _userExportRepository.GetUsersWithAddressesAsync(userId, Arg.Any<CancellationToken>()).Returns(Array.Empty<UserAddressCsvRow>());

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Usuário não encontrado.", exception.Message);

            _csvExportService.DidNotReceive().GenerateUsersWithAddresses(Arg.Any<IReadOnlyCollection<UserAddressCsvRow>>());
        }
    }
}
