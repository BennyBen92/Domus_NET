using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FDL.Infra.Persistance
{
    internal sealed class WorkflowRowConfiguration : IEntityTypeConfiguration<WorkflowRow>
    {
        public void Configure(EntityTypeBuilder<WorkflowRow> builder)
        {
            builder.ToTable("workflow");

            // IdWorkflow
            builder.HasKey(r => r.IdWorkflow);
            builder.Property(r => r.IdWorkflow)
                .UseIdentityAlwaysColumn();

            // -- Règles --
            // Un worflow ne peut pas être ajouté sans une assignation
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("ck_workflow_assignation", "id_user_assigne IS NOT NULL OR id_groupe_assigne IS NOT NULL");
                t.HasCheckConstraint("ck_workflow_document", "type <> 1 OR id_document > 0");   // 1 = WorkflowType.Document
            });

            // -- Index --
            // index partiel pour "Mes workflows en cours"
            builder.HasIndex(w => w.IdUserAssigne)
                .HasFilter("id_user_terminaison IS NULL");
            // index partiel pour "Workflow de mes groupes"
            builder.HasIndex(w => w.IdGroupeAssigne)
                .HasFilter("id_user_terminaison IS NULL");

        }
    }
}
