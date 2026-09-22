using FDL.Core.App;
using FDL.Core.Domain;
using FDL.WF.Domain;

namespace FDL.WF.App
{
    public sealed class WorkflowService(
        IWorkflowRepository workflowRepository,
        IUtilisateurCourant utilisateurCourant,
        TimeProvider timeProvider) : IWorkflowService
    {
        private readonly IWorkflowRepository _workflowRepository = workflowRepository;
        private readonly IUtilisateurCourant _utilisateurCourant = utilisateurCourant;
        private readonly TimeProvider _timeProvider = timeProvider;
        // ------------------------------------------------------------

        // Utilisateur qui agit
        private AuditInfo FaitPar() => new(_utilisateurCourant.Id, _timeProvider.GetUtcNow().UtcDateTime);
        // ------------------------------------------------------------


        /// <summary>
        /// Crée un workflow pour un document
        /// </summary>
        public int CreerPourDocument(ReferenceDossier refDossier, WorkflowAssignation assignation, string message, WorkflowAction action, int idDocument)
        {
            var workflow = Workflow.PourDocument(refDossier, FaitPar(), assignation, message, action, idDocument);
            return _workflowRepository.Add(workflow);
        }

        /// <summary>
        /// Crée un workflow pour une note  
        /// </summary>
        public int CreerPourNote(ReferenceDossier refDossier, WorkflowAssignation assignation, string message)
        {
            var workflow = Workflow.PourNote(refDossier, FaitPar(), assignation, message);
            return _workflowRepository.Add(workflow);
        }

        /// <summary>
        /// Crée un workflow pour une tache
        /// </summary>
        public int CreerPourTache(ReferenceDossier refDossier, WorkflowAssignation assignation, string message)
        {
            var workflow = Workflow.PourTache(refDossier, FaitPar(), assignation, message);
            return _workflowRepository.Add(workflow);
        }
        // ------------------------------------------------------------


        /// <summary>
        /// Récupère la liste des workflows en cours pour l'utilisateur courant
        /// </summary>
        public IReadOnlyList<Workflow> MesWorkflowsEnCours()
        {
            return _workflowRepository.Search(new WorkflowFiltre(
                IdUserAssigne: _utilisateurCourant.Id,
                IdGroupesAssignes: null,
                EstTermine: false));
        }

        /// <summary>
        /// Récupère la liste des workflows en cours de mes groupes
        /// </summary>
        public IReadOnlyList<Workflow> WorkflowsDeMesGroupes()
        {
            return _workflowRepository.Search(new WorkflowFiltre(
                IdUserAssigne: null,
                IdGroupesAssignes: _utilisateurCourant.IdGroupes,
                EstTermine: false));
        }
        // ------------------------------------------------------------


        /// <summary>
        /// Réassigne un workflow a un autre utilisateur
        /// </summary>
        public void Reassigner(int idWorkflow, WorkflowAssignation nouvelleAssignation)
        {
            Workflow? wf = _workflowRepository.GetById(idWorkflow)
                ?? throw new InvalidOperationException($"Le workflow avec l'ID {idWorkflow} n'existe pas.");

            wf.Reassigner(nouvelleAssignation);
            _workflowRepository.Update(wf);
        }

        /// <summary>
        /// Termine un workflow en cours
        /// </summary>
        public void Terminer(int idWorkflow)
        {
            Workflow? wf = _workflowRepository.GetById(idWorkflow)
                ?? throw new InvalidOperationException($"Le workflow avec l'ID {idWorkflow} n'existe pas.");

            wf.Terminer(FaitPar());
            _workflowRepository.Update(wf);
        }

    }
}
