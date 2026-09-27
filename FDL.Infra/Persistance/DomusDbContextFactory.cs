using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FDL.Infra.Persistance
{
    internal sealed class DomusDbContextFactory : IDesignTimeDbContextFactory<DomusDbContext>
    {
        public DomusDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<DomusDbContext>()
                .UseNpgsql("Host=localhost;Database=domus")
                .UseSnakeCaseNamingConvention()
                .Options;
            return new DomusDbContext(options);
        }
    }
}
