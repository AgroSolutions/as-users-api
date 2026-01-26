using AS.Users.Domain.ValueObjects;

namespace AS.Users.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("famer@google.com")]
    [InlineData("fazendeiro@ig.com.br")]
    public void Constructor_ValidEmail_CreatesInstance(string validEmail)
    {
        var email = new Email(validEmail);
        Assert.Equal(validEmail, email.Address);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Constructor_InvalidEmail_ThrowsArgumentException(string invalidEmail)
    {
        Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
    }
}