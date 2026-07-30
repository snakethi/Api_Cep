using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Exports.Models;
using ApiCep.Application.Interfaces.FileExport;
using ApiCep.Application.Interfaces.Repositories;
using MediatR;

namespace ApiCep.Application.Exports.Queries.ExportUsersCsv
{
    public sealed class ExportUsersCsvQueryHandler : IRequestHandler<ExportUsersCsvQuery, ExportFileResult>
    {
        private readonly IUserExportRepository _userExportRepository;
        private readonly ICsvExportService _csvExportService;

        public ExportUsersCsvQueryHandler(IUserExportRepository userExportRepository, ICsvExportService csvExportService)
        {
            _userExportRepository = userExportRepository;
            _csvExportService = csvExportService;
        }

        public async Task<ExportFileResult> Handle(ExportUsersCsvQuery request, CancellationToken cancellationToken)
        {
            var rows = await _userExportRepository.GetUsersWithAddressesAsync(request.UserId, cancellationToken);

            if (request.UserId.HasValue && rows.Count == 0)
                throw new NotFoundException("Usuário não encontrado.");

            var content = _csvExportService.GenerateUsersWithAddresses(rows);

            var fileName = request.UserId.HasValue
                ? $"usuario-{request.UserId}-enderecos-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv"
                : $"usuarios-enderecos-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv";

            return new ExportFileResult(content, "text/csv; charset=utf-8", fileName);
        }
    }
}
