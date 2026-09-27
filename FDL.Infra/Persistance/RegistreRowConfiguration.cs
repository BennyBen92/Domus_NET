using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FDL.Infra.Persistance
{
    internal sealed class RegistreRowConfiguration : IEntityTypeConfiguration<RegistreRow>
    {
        public void Configure(EntityTypeBuilder<RegistreRow> builder)
        {
            builder.ToTable("registre");

            // IdRegistre
            builder.HasKey(r => r.IdRegistre);
            builder.Property(r => r.IdRegistre)
                .UseIdentityAlwaysColumn();

            // -- Règles --
            //...
            
            // -- Index --
            builder.HasIndex(r => r.ReferenceDossier)
                .IsUnique()
                .HasDatabaseName("ux_registre_reference_dossier");

        }
    }
}
