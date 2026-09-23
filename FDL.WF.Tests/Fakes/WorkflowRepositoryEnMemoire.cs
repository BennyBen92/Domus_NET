using FDL.WF.App;
using FDL.WF.Domain;

namespace FDL.WF.Tests.Fakes
{
    /// <summary>
    /// Implémentation en mémoire de <see cref="IWorkflowRepository"/> pour les tests du service.
    /// Le repository stocke et renvoie des copies : un workflow modifié sans appel à Update
    /// n'est pas persisté, exactement comme avec une vraie base.
    /// </summary>
    public sealed class WorkflowRepositoryEnMemoire : IWorkflowRepository
    {
        private readonly List<Workflow> _table = [];
        private int _dernierId;
        // ------------------------------------------------------------


        /// <summary>
        /// Ajoute un workflow et renvoie l'identifiant attribué (auto-incrément).
        /// </summary>
        public int Add(Workflow entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            int id = ++_dernierId;
            _table.Add(Copier(entity, id));
            return id;
        }

        /// <summary>
        /// Remplace le workflow stocké. Lève une exception si l'identifiant est inconnu.
        /// </summary>
        public void Update(Workflow entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            int index = _table.FindIndex(w => w.IdWorkflow == entity.IdWorkflow);
            if (index < 0)
                throw new InvalidOperationException($"Le workflow {entity.IdWorkflow} n'existe pas.");
            _table[index] = Copier(entity, entity.IdWorkflow);
        }
        // ------------------------------------------------------------


        public Workflow? GetById(int id)
        {
            Workflow? wf = _table.Find(w => w.IdWorkflow == id);
            return wf is null ? null : Copier(wf, wf.IdWorkflow);
        }

        public IEnumerable<Workflow> GetAll()
            => _table.Select(w => Copier(w, w.IdWorkflow)).ToList();

        /// <summary>
        /// Filtre combiné en ET. Un critère null n'est pas appliqué.
        /// Une liste de groupes vide ne renvoie rien.
        /// </summary>
        public IReadOnlyList<Workflow> Search(WorkflowFiltre filtre)
        {
            ArgumentNullException.ThrowIfNull(filtre);
            IEnumerable<Workflow> query = _table;

            if (filtre.IdUserAssigne is int idUser)
                query = query.Where(w => w.Assignation.IdUser == idUser);

            if (filtre.IdGroupesAssignes is { } groupes)
                query = query.Where(w => w.Assignation.IdGroup is int idGroupe && groupes.Contains(idGroupe));

            if (filtre.EstTermine is bool estTermine)
                query = query.Where(w => w.EstTermine == estTermine);

            return query.Select(w => Copier(w, w.IdWorkflow)).ToList();
        }
        // ------------------------------------------------------------


        // Simule l'aller-retour en base : nouvelle instance, même état.
        private static Workflow Copier(Workflow wf, int id)
            => Workflow.Reconstituer(id, wf.ReferenceDossier, wf.Expediteur, wf.Assignation,
                wf.Message, wf.Type, wf.Action, wf.IdDocument, wf.Terminaison);
    }
}
