using ApiCep.Domain.Entities;

namespace ApiCep.Tests.Domain
{
    public sealed class AddressTests
    {
        [Fact]
        public void Constructor_WhenDataIsValid_ShouldCreateActiveAddress()
        {
            var userId = Guid.NewGuid();
            var beforeCreation = DateTime.UtcNow;

            var address = new Address(userId, "01310-100","Avenida Paulista","1000", "Bela Vista", "São Paulo","sp","Apartamento 10");

            var afterCreation = DateTime.UtcNow;

            Assert.NotEqual(Guid.Empty, address.Id);
            Assert.Equal(userId, address.UserId);

            Assert.Equal("01310100", address.ZipCode);
            Assert.Equal("Avenida Paulista", address.Street);
            Assert.Equal("1000", address.Number);
            Assert.Equal("Bela Vista", address.Neighborhood);
            Assert.Equal("São Paulo", address.City);
            Assert.Equal("SP", address.State);
            Assert.Equal("Apartamento 10", address.Complement);

            Assert.True(address.IsActive);

            Assert.InRange(
                address.CreatedAtUtc,
                beforeCreation,
                afterCreation);

            Assert.Null(address.UpdatedAtUtc);
            Assert.Null(address.DeletedAtUtc);
        }

        [Fact]
        public void Update_WhenDataIsValid_ShouldUpdateAddressAndDate()
        {
            var address = new Address( Guid.NewGuid(),"01310-100", "Avenida Paulista", "1000","Bela Vista", "São Paulo", "SP", "Apartamento 10");

            var beforeUpdate = DateTime.UtcNow;

            address.Update("04538-132", "Avenida Brigadeiro Faria Lima", "2000", "Itaim Bibi", "São Paulo", "sp", null);

            var afterUpdate = DateTime.UtcNow;

            Assert.Equal("04538132", address.ZipCode);
            Assert.Equal("Avenida Brigadeiro Faria Lima", address.Street);
            Assert.Equal("2000", address.Number);
            Assert.Equal("Itaim Bibi", address.Neighborhood);
            Assert.Equal("São Paulo", address.City);
            Assert.Equal("SP", address.State);
            Assert.Null(address.Complement);

            Assert.NotNull(address.UpdatedAtUtc);

            Assert.InRange(
                address.UpdatedAtUtc!.Value,
                beforeUpdate,
                afterUpdate);

            Assert.True(address.IsActive);
            Assert.Null(address.DeletedAtUtc);
        }

        [Fact]
        public void Deactivate_WhenAddressIsActive_ShouldDeactivateAndSetDates()
        {
            var address = CreateValidAddress();

            var beforeDeactivation = DateTime.UtcNow;

            address.Deactivate();

            var afterDeactivation = DateTime.UtcNow;

            Assert.False(address.IsActive);
            Assert.NotNull(address.DeletedAtUtc);
            Assert.NotNull(address.UpdatedAtUtc);

            Assert.InRange(
                address.DeletedAtUtc!.Value,
                beforeDeactivation,
                afterDeactivation);

            Assert.InRange(
                address.UpdatedAtUtc!.Value,
                beforeDeactivation,
                afterDeactivation);
        }

        [Fact]
        public void Constructor_WhenUserIdIsEmpty_ShouldThrowArgumentException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new Address(Guid.Empty, "01310-100","Avenida Paulista", "1000", "Bela Vista", "São Paulo",  "SP"));

            Assert.Equal("userId", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("123")]
        [InlineData("1234567")]
        [InlineData("123456789")]
        public void Constructor_WhenZipCodeIsInvalid_ShouldThrowArgumentException( string? zipCode)
        {
            Assert.Throws<ArgumentException>(() =>
                new Address( Guid.NewGuid(),zipCode!,"Avenida Paulista", "1000", "Bela Vista","São Paulo","SP"));
        }

        [Theory]
        [InlineData("01310-100")]
        [InlineData("01310 100")]
        [InlineData("01.310-100")]
        public void Constructor_WhenZipCodeIsFormatted_ShouldStoreOnlyNumbers(string zipCode)
        {
            var address = new Address( Guid.NewGuid(),zipCode,"Avenida Paulista","1000", "Bela Vista", "São Paulo", "SP");

            Assert.Equal("01310100", address.ZipCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("S")]
        [InlineData("SPO")]
        public void Constructor_WhenStateIsInvalid_ShouldThrowArgumentException(string? state)
        {
            // Act
            Assert.Throws<ArgumentException>(() =>
                new Address(Guid.NewGuid(),"01310-100", "Avenida Paulista","1000", "Bela Vista","São Paulo",state!));
        }

        [Theory]
        [InlineData("street")]
        [InlineData("number")]
        [InlineData("neighborhood")]
        [InlineData("city")]
        public void Constructor_WhenRequiredFieldIsEmpty_ShouldThrowArgumentException(string field)
        {
            var street = "Avenida Paulista";
            var number = "1000";
            var neighborhood = "Bela Vista";
            var city = "São Paulo";

            switch (field)
            {
                case "street":
                    street = " ";
                    break;

                case "number":
                    number = " ";
                    break;

                case "neighborhood":
                    neighborhood = " ";
                    break;

                case "city":
                    city = " ";
                    break;
            }

            Assert.Throws<ArgumentException>(() =>
                new Address( Guid.NewGuid(),"01310-100", street, number,neighborhood,city,"SP"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WhenComplementIsEmpty_ShouldStoreNull(string? complement)
        {
            var address = new Address(Guid.NewGuid(),"01310-100","Avenida Paulista", "1000", "Bela Vista","São Paulo","SP",complement);

            Assert.Null(address.Complement);
        }

        private static Address CreateValidAddress()
        {
            return new Address( Guid.NewGuid(),"01310-100","Avenida Paulista","1000","Bela Vista","São Paulo", "SP","Apartamento 10");
        }
    }
}
