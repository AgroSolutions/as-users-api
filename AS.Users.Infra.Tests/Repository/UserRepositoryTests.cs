using AS.Users.Domain.ValueObjects;
using AS.Users.Domain.Entities;
using AS.Users.Infra.Persistence.Data;
using AS.Users.Infra.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AS.Users.Infra.Tests.Repository;

public class UserRepositoryTests
{
    private readonly IDbContextFactory<ASDbContext> _contextFactory;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ASDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
            .Options;

        // mock/fake do IDbContextFactory
        _contextFactory = new TestDbContextFactory(options);
        _repository = new UserRepository(_contextFactory);
    }

    public class TestDbContextFactory : IDbContextFactory<ASDbContext>
    {
        private readonly DbContextOptions<ASDbContext> _options;

        public TestDbContextFactory(DbContextOptions<ASDbContext> options)
        {
            _options = options;
        }

        public ASDbContext CreateDbContext()
        {
            return new ASDbContext(_options);
        }
    }

    [Fact]
    public async Task AddAsync_ValidUser_ShouldAddUserToDatabase()
    {
        using var dbContext = _contextFactory.CreateDbContext();

        //  Arrange
        var user = new User("John Doe", "farmer@google.com");

        // Act
        await _repository.AddAsync(user);

        // Assert
        var saveUser = await dbContext.Users.FirstOrDefaultAsync();
        Assert.NotNull(saveUser);
        Assert.Equal("farmer@google.com", saveUser.Email!);
    }

    [Fact]
    public async Task DeleteAsync_ExistUser_ShouldDeleteUserFromDatabase()
    {
        using var dbContext = _contextFactory.CreateDbContext();

        // Arrange
        var user = new User("John Doe", "farmer@google.com");

        // Act
        await _repository.AddAsync(user);
        await _repository.DeleteAsync(user.Id!);

        // Assert
        Assert.Null(await dbContext.Users.FindAsync(user.Id));
    }

    [Fact]
    public async Task UpdateAsync_ExistUser_ShouldUpdateUser()
    {
        // Arrange
        var user = new User("John Doe", "farmer@google.com");

        // Act
        await _repository.AddAsync(user);
        user.Email = new Email("farmer@yahoo.com").ToString();
        await _repository.UpdateAsync(user);

        // Assert
        Assert.True(await _repository.ExistsByEmailAsync("farmer@yahoo.com"));
    }
}