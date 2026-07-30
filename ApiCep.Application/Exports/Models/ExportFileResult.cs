namespace ApiCep.Application.Exports.Models
{
    public sealed record ExportFileResult(byte[] Content, string ContentType, string FileName);
}
