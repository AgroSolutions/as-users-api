using AS.Users.Domain.Entities;
using AS.Users.Infra.Persistence.Data.Mappings;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AS.Users.Infra.Persistence.Data
{
    public class ASDbContext : IdentityDbContext<User>
    {

        public ASDbContext(DbContextOptions<ASDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserMapping());
        }

    }
}