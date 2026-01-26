using AS.Users.Domain.Entities;
using AS.Users.Domain.Enums;
using AS.Users.Domain.ValueObjects;

namespace AS.Users.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_ValidName_CreatesUser()
    {
        // Arrange & Act
        var password = new Password("Senha@123");
        var user = new User("John Doe", "farmer@google.com");

        // Assert
        Assert.Equal("John Doe", user.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidName_ThrowsException(string invalidName)
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new User(invalidName, "farmer@google.com"));
    }

    [Fact]
    public void Constructor_DefaultRole_IsUser()
    {
        // Arrange & Act
        var user = new User("John Doe", "farmer@google.com");

        // Assert
        Assert.Equal(UserRole.User, user.Role);
    }

    [Fact]
    public void Constructor_AdminRole_SetsRoleCorrectly()
    {
        // Arrange & Act
        var user = new User("John Doe", "farmer@google.com", UserRole.Admin);

        // Assert
        Assert.Equal(UserRole.Admin, user.Role);
    }

    [Fact]
    public void Constructor_ValidEmailVO_CreatesUser()
    {
        // Arrange & Act
        var user = new User("John Doe", "farmer@google.com");


        // Assert
        Assert.Equal("farmer@google.com", user.Email!);
    }
}