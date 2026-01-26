using AS.Users.Domain.Entities;
using AS.Users.Infra.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace AS.Users.Infra.Tests;
public class FCGDbContextTests
{
    [Fact]
    public void CanSaveUserWithEmail()
    {
        var options = new DbContextOptionsBuilder<ASDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDB")
            .Options;

        using (var context = new ASDbContext(options))
        {
            var user = new User("Test", "valid@fiap.com.br");
            context.Users.Add(user);
            context.SaveChanges();
        }

        using (var context = new ASDbContext(options))
        {
            Assert.Equal(1, context.Users.Count());
        }
    }
}