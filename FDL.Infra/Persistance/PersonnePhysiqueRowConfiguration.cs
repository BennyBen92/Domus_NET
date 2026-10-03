using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FDL.Infra.Persistance
{
    internal sealed class PersonnePhysiqueRowConfiguration : IEntityTypeConfiguration<PersonnePhysiqueRow>
    {
        public void Configure(EntityTypeBuilder<PersonnePhysiqueRow> builder)
        {
            builder.ToTable("personne_physique", t =>
            {
                t.HasCheckConstraint("ck_personne_physique_nom", "length(trim(nom)) > 0");
                t.HasCheckConstraint("ck_personne_physique_numero_national", "numero_national ~ '^[0-9]{11}$'");
                t.HasCheckConstraint("ck_personne_physique_sexe", "sexe IN (0, 1, 2)");
            });

            builder.HasKey(r => r.IdPersonnePhysique);
            builder.Property(r => r.IdPersonnePhysique)
                .UseIdentityAlwaysColumn();

            builder.Property(r => r.NumeroNational)
                .HasMaxLength(11);

            builder.HasIndex(r => r.NumeroNational)
                .HasDatabaseName("ix_personne_physique_numero_national");
        }
    }
}
