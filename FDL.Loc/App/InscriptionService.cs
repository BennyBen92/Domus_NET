using FDL.Core.Domain;
using FDL.WF.App;
using FDL.WF.Domain;

namespace FDL.Loc.App
{
    public sealed class InscriptionService(
        IRegistreService registreService, 
        IWorkflowService workflowService) : IInscriptionService
    {
        private const int IdGroupeRegistreLocataire = 64;

        public (int IdRegistre, int IdWorkflow) InscriptionPourLogement(ReferenceDossier referenceDossier,
                                                                        int nbChambresMin, int nbChambresMax,
                                                                        int[] listeCommunes, bool souhaiteAscenseur,
                                                                        string? messageDuCandidat)
        {
            string? commentaire = string.IsNullOrWhiteSpace(messageDuCandidat) ? null : $"Message du candidat :\n{messageDuCandidat}";

            // On imagine que juste avant, un service de "reference dossier" nous passe la dernière ref. créée.
            // Création du registre "logement"
            int idRegistre = registreService.CreerPourLogement(referenceDossier, nbChambresMin, nbChambresMax, listeCommunes, souhaiteAscenseur, commentaire);

            // Ensuite, le groupe "registre locataire" en est informé par workflow.
            WorkflowAssignation assignation = WorkflowAssignation.PourGroupe(IdGroupeRegistreLocataire);
            
            string message = string.IsNullOrWhiteSpace(messageDuCandidat) ? "" : $"\n{messageDuCandidat}";
            int idWorkflow = workflowService.CreerPourTache(referenceDossier, assignation, $"Nouvelle inscription au registre ({referenceDossier}).{message}");

            return (idRegistre, idWorkflow);
        }
    }
}
