using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityServerAsp.Data.Migrations.PersistedGrantDb
{
    public class ConfigurationtDbContextFactory : IDesignTimeDbContextFactory<PersistedGrantDbContext>
    {
        public PersistedGrantDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
            optionsBuilder.UseSqlServer("Server=NMIRIC\\SQLEXPRESS;Database=IdentityServerAspEF;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new PersistedGrantDbContext(optionsBuilder.Options, new IdentityServer4.EntityFramework.Options.OperationalStoreOptions());
        }
    }
}
