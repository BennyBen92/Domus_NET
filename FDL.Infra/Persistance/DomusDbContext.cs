using Microsoft.EntityFrameworkCore;

namespace FDL.Infra.Persistance
{
    internal sealed class DomusDbContext(DbContextOptions<DomusDbContext> options) : DbContext(options)
    {
        public DbSet<WorkflowRow> Workflows => Set<WorkflowRow>();

        public DbSet<RegistreRow> Registres => Set<RegistreRow>();

        public DbSet<PersonnePhysiqueRow> Pesonnes => Set<PersonnePhysiqueRow>();

        // ApplyConfigurationsFromAssembly charge automatiquement toutes les classes de configuration du projet
        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DomusDbContext).Assembly);

    }
}
