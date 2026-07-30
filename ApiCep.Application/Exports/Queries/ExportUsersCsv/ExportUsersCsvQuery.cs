using ApiCep.Application.Exports.Models;
using MediatR;

namespace ApiCep.Application.Exports.Queries.ExportUsersCsv
{
    public sealed record ExportUsersCsvQuery(Guid? UserId = null) : IRequest<ExportFileResult>;
}
