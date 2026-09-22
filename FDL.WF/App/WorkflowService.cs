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

        private AuditInfo FaitPar() => new(_utilisateurCourant.Id, _timeProvider.GetUtcNow().UtcDateTime);
        // ------------------------------------------------------------


        /// <summary>
        /// Crée un workflow pour un document
        /// </summary>
        /// <param name="refDossier"></param>
        /// <param name="action"></param>
        /// <param name="assignation"></param>
        /// <param name="message"></param>
        /// <param name="idDocument"></param>
        /// <returns name="idWorkflow"></returns>
        public int CreerWorkflowDocument(ReferenceDossier refDossier, WorkflowAssignation assignation, string message, WorkflowAction action, int idDocument)
        {
            var workflow = Workflow.PourDocument(refDossier, FaitPar(), assignation, message, action, idDocument);
            return _workflowRepository.Add(workflow);
        }

        /// <summary>
        /// Crée un workflow pour une note  
        /// </summary>
        /// <param name="refDossier"></param>
        /// <param name="assignation"></param>
        /// <param name="message"></param>
        /// <returns name="idWorkflow"></returns>
        public int CreerWorkflowNote(ReferenceDossier refDossier, WorkflowAssignation assignation, string message)
        {
            var workflow = Workflow.PourNote(refDossier, FaitPar(), assignation, message);
            return _workflowRepository.Add(workflow);
        }

        /// <summary>
        /// Crée un workflow pour une tache
        /// </summary>
        /// <param name="refDossier"></param>
        /// <param name="assignation"></param>
        /// <param name="message"></param>
        /// <returns name="idWorkflow"></returns>
        public int CreerWorkflowTache(ReferenceDossier refDossier, WorkflowAssignation assignation, string message)
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
                IdGroupAssigne: null,
                EstTermine: false));
        }

        /// <summary>
        /// Termine un workflow en cours
        /// </summary>
        /// <param name="idWorkflow"></param>
        public void Terminer(int idWorkflow)
        {
            Workflow? wf = _workflowRepository.GetById(idWorkflow)
                ?? throw new InvalidOperationException($"Le workflow avec l'ID {idWorkflow} n'existe pas.");

            wf.Terminer(FaitPar());
            _workflowRepository.Update(wf);
        }
    }
}
