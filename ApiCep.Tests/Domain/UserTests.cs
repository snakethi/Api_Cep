using ApiCep.Domain.Entities;

namespace ApiCep.Tests.Domain
{
    public sealed class UserTests
    {
        [Fact]
        public void Constructor_WhenDataIsValid_ShouldCreateActiveUser()
        {
            var beforeCreation = DateTime.UtcNow;

            var user = new User( " Thiago Botaro "," Thiago@Email.com ","hash-da-senha");

            var afterCreation = DateTime.UtcNow;

            Assert.NotEqual(Guid.Empty, user.Id);
            Assert.Equal("Thiago Botaro", user.Name);
            Assert.Equal("thiago@email.com", user.Email);
            Assert.Equal("hash-da-senha", user.PasswordHash);
            Assert.True(user.IsActive);

            Assert.InRange(
                user.CreatedAtUtc,
                beforeCreation,
                afterCreation);

            Assert.Null(user.UpdatedAtUtc);
            Assert.Null(user.DeletedAtUtc);
        }

        [Fact]
        public void Update_WhenDataIsValid_ShouldUpdateNameEmailAndDate()
        {
            var user = new User("Thiago Botaro","thiago@email.com","hash-da-senha");

            var beforeUpdate = DateTime.UtcNow;

            user.Update( " Thiago Silva "," Thiago.Silva@Email.com ");

            var afterUpdate = DateTime.UtcNow;

            Assert.Equal("Thiago Silva", user.Name);
            Assert.Equal("thiago.silva@email.com", user.Email);

            Assert.NotNull(user.UpdatedAtUtc);

            Assert.InRange(
                user.UpdatedAtUtc!.Value,
                beforeUpdate,
                afterUpdate);

            Assert.True(user.IsActive);
            Assert.Null(user.DeletedAtUtc);
        }

        [Fact]
        public void ChangePasswordHash_WhenHashIsValid_ShouldUpdatePasswordAndDate()
        {
            var user = new User( "Thiago Botaro","thiago@email.com","hash-antigo");

            var beforeChange = DateTime.UtcNow;

            user.ChangePasswordHash("hash-novo");

            var afterChange = DateTime.UtcNow;

            Assert.Equal("hash-novo", user.PasswordHash);

            Assert.NotNull(user.UpdatedAtUtc);

            Assert.InRange(
                user.UpdatedAtUtc!.Value,
                beforeChange,
                afterChange);

            Assert.True(user.IsActive);
            Assert.Null(user.DeletedAtUtc);
        }

        [Fact]
        public void Deactivate_WhenUserIsActive_ShouldDeactivateUserAndSetDates()
        {
  
            var user = new User("Thiago Botaro","thiago@email.com","hash-da-senha");

            var beforeDeactivation = DateTime.UtcNow;

            user.Deactivate();

            var afterDeactivation = DateTime.UtcNow;

            Assert.False(user.IsActive);
            Assert.NotNull(user.DeletedAtUtc);
            Assert.NotNull(user.UpdatedAtUtc);

            Assert.InRange(
                user.DeletedAtUtc!.Value,
                beforeDeactivation,
                afterDeactivation);

            Assert.InRange(
                user.UpdatedAtUtc!.Value,
                beforeDeactivation,
                afterDeactivation);
        }

        [Fact]
        public void Deactivate_WhenUserIsAlreadyInactive_ShouldKeepOriginalDates()
        {
            var user = new User("Thiago Botaro","thiago@email.com","hash-da-senha");

            user.Deactivate();

            var originalDeletedAt = user.DeletedAtUtc;
            var originalUpdatedAt = user.UpdatedAtUtc;

            user.Deactivate();

            Assert.False(user.IsActive);
            Assert.Equal(originalDeletedAt, user.DeletedAtUtc);
            Assert.Equal(originalUpdatedAt, user.UpdatedAtUtc);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WhenNameIsInvalid_ShouldThrowArgumentException(string? name)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new User(name!, "thiago@email.com","hash-da-senha"));

            Assert.Equal("name", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WhenEmailIsInvalid_ShouldThrowArgumentException(string? email)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new User("Thiago Botaro",email!,"hash-da-senha"));

            Assert.Equal("email", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WhenPasswordHashIsInvalid_ShouldThrowArgumentException(string? passwordHash)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new User("Thiago Botaro", "thiago@email.com", passwordHash!));

            Assert.Equal("passwordHash", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ChangePasswordHash_WhenHashIsInvalid_ShouldThrowArgumentException(string? passwordHash)
        {
            var user = new User("Thiago Botaro","thiago@email.com","hash-original");

            var exception = Assert.Throws<ArgumentException>(() =>
                user.ChangePasswordHash(passwordHash!));

            Assert.Equal("passwordHash", exception.ParamName);
            Assert.Equal("hash-original", user.PasswordHash);
        }

    }

}

