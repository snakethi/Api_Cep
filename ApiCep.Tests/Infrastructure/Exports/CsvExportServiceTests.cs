using ApiCep.Application.Exports.Models;
using ApiCep.Infrastructure.Exports;
using System.Text;


namespace ApiCep.Tests.Infrastructure.Exports
{
    public sealed class CsvExportServiceTests
    {
        private readonly CsvExportService _service = new();

        [Fact]
        public void GenerateUsersWithAddresses_ShouldGenerateCsvWithHeaderAndData()
        {
            var rows = new[]
            {
            new UserAddressCsvRow(Guid.NewGuid(),"Thiago Botaro","thiago@teste.com",true,Guid.NewGuid(),"01310100","Avenida Paulista","1000","Bela Vista","São Paulo","SP","Apartamento 10")
        };

            var content = _service.GenerateUsersWithAddresses(rows);
            var csv = Encoding.UTF8.GetString(content);

            Assert.NotEmpty(content);
            Assert.Contains("UserId", csv);
            Assert.Contains("Thiago Botaro", csv);
            Assert.Contains("thiago@teste.com", csv);
            Assert.Contains("Avenida Paulista", csv);
            Assert.Contains("São Paulo", csv);
            Assert.Contains("Apartamento 10", csv);
        }

        [Fact]
        public void GenerateUsersWithAddresses_ShouldGenerateHeader_WhenRowsAreEmpty()
        {
            var content = _service.GenerateUsersWithAddresses(Array.Empty<UserAddressCsvRow>());
            var csv = Encoding.UTF8.GetString(content);

            Assert.NotEmpty(content);
            Assert.Contains("UserId", csv);
            Assert.Contains("Name", csv);
            Assert.Contains("Email", csv);
            Assert.DoesNotContain("Thiago Botaro", csv);
        }

        [Fact]
        public void GenerateUsersWithAddresses_ShouldThrowArgumentNullException_WhenRowsAreNull()
        {
            Assert.Throws<ArgumentNullException>(() => _service.GenerateUsersWithAddresses(null!));
        }
    }
}
