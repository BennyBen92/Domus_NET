using FDL.WF.App;
using FDL.WF.Domain;
using Microsoft.EntityFrameworkCore;

namespace FDL.Infra.Persistance
{
    internal sealed class WorkflowRepository(DomusDbContext db) : IWorkflowRepository
    {

        public Workflow? GetById(int id)
        {
            WorkflowRow? r = db.Workflows
                .AsNoTracking()
                .FirstOrDefault(w => w.IdWorkflow == id);

            return r is not null ? WorkflowMapper.ToDomain(r) : null;
        }

        public IEnumerable<Workflow> GetAll()
        {
            return db.Workflows.AsNoTracking()
                .AsEnumerable()                     // SQL exécuté ici
                .Select(WorkflowMapper.ToDomain)    // mapping mémoire
                .ToList();
        }

        public IReadOnlyList<Workflow> Search(WorkflowFiltre filtre)
        {
            ArgumentNullException.ThrowIfNull(filtre);
            IQueryable<WorkflowRow> q = db.Workflows.AsNoTracking();

            if (filtre.IdUserAssigne is int idUser)
                q = q.Where(r => r.IdUserAssigne == idUser);

            if (filtre.IdGroupesAssignes is { } groupes)
                q = q.Where(r => r.IdGroupeAssigne != null && groupes.Contains(r.IdGroupeAssigne.Value));

            if (filtre.EstTermine is bool estTermine)
                q = estTermine ? q.Where(r => r.IdUserTerminaison != null)
                               : q.Where(r => r.IdUserTerminaison == null);

            return q.AsEnumerable()
                .Select(WorkflowMapper.ToDomain)
                .ToList();
        }


        public int Add(Workflow entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            WorkflowRow row = WorkflowMapper.ToRow(entity);
            try
            {
                db.Workflows.Add(row);
                db.SaveChanges();
                return row.IdWorkflow;  // l'id attribué par PostgreSQL
            }
            finally
            {
                db.ChangeTracker.Clear(); // le contexte repart propre, succès ou échec
            }
        }

        public void Update(Workflow entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            try
            {
                db.Workflows.Update(WorkflowMapper.ToRow(entity));
                db.SaveChanges();
            }
            finally
            {
                db.ChangeTracker.Clear();
            }
        }
    }
}
