using AS.Users.Domain.Entities;
using AS.Users.Domain.Interfaces;
using AS.Users.Infra.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace AS.Users.Infra.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<ASDbContext> _contextFactory;

    public UserRepository(IDbContextFactory<ASDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task AddAsync(User user)
    {
        using var dbContext = _contextFactory.CreateDbContext();

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        using var dbContext = _contextFactory.CreateDbContext();

        var user = await GetByIdAsync(id);
        if (user is not null)
        {
            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        using var dbContext = _contextFactory.CreateDbContext();

        return await dbContext.Users
            .AnyAsync(u => u.Email == email);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var dbContext = _contextFactory.CreateDbContext();

        return await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        using var dbContext = _contextFactory.CreateDbContext();

        return await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task UpdateAsync(User user)
    {
        using var dbContext = _contextFactory.CreateDbContext();

        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
    }
}