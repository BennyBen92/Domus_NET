using FDL.Core.App;
using FDL.Core.Domain;
using FDL.WF.App;
using FDL.WF.Domain;

namespace FDL.Loc.App
{
    public sealed class InscriptionService(
        IRegistreService registreService,
        IWorkflowService workflowService,
        IUnitOfWork unitOfWork) : IInscriptionService
    {
        private const int IdGroupeRegistreLocataire = 64;

        public (int IdRegistre, int IdWorkflow) InscriptionPourLogement(ReferenceDossier referenceDossier,
                                                                        int nbChambresMin, int nbChambresMax,
                                                                        int[] listeCommunes, bool souhaiteAscenseur,
                                                                        string? messageDuCandidat)
        {
            // On imagine que juste avant, un service de "reference dossier" nous passe la dernière ref. créée.
            // Ensuite, le groupe "registre locataire" en est informé par workflow.

            string? commentaireRegistre = string.IsNullOrWhiteSpace(messageDuCandidat) ? null : $"Message du candidat :\n{messageDuCandidat}";

            WorkflowAssignation assignation = WorkflowAssignation.PourGroupe(IdGroupeRegistreLocataire);
            string messageWorkflow = string.IsNullOrWhiteSpace(messageDuCandidat) ? "" : $"\n{messageDuCandidat}";

            // Crée une transaction pour s'assurer de la cohérence des données (Configuration du DbContext en Scoped).
            return unitOfWork.Execute(() =>
            {
                // Création du registre "logement"
                int idRegistre = registreService.CreerPourLogement(referenceDossier, nbChambresMin, nbChambresMax, listeCommunes, souhaiteAscenseur, commentaireRegistre);
                // Création du workflow
                int idWorkflow = workflowService.CreerPourTache(referenceDossier, assignation, $"Nouvelle inscription au registre ({referenceDossier}).{messageWorkflow}");
                return (idRegistre, idWorkflow);
            });

        }
    }
}
