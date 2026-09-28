using FDL.Core.Domain;
using FDL.Loc.App;
using FDL.Loc.Domain;
using FDL.Loc.Tests.Fakes;
using FDL.WF.App;
using FDL.WF.Domain;
using FDL.WF.Tests.Fakes;
using Microsoft.Extensions.Time.Testing;

namespace FDL.Loc.Tests
{
    public class InscriptionServiceTests
    {
        private static readonly DateTimeOffset Maintenant = new(2026, 10, 20, 12, 0, 0, TimeSpan.Zero);
        private static readonly int[] Communes = [1050, 1080];
        private static readonly string MessageCandidat = "Bonjour, je souhaiterais louer un logement.";
        private static readonly ReferenceDossier RefDemande = ReferenceDossier.DepuisExistant(2060123, 61);
        private const int IdGroupeRegistreLocataire = 64;

        private readonly RegistreRepositoryEnMemoire _registreRepo = new();
        private readonly WorkflowRepositoryEnMemoire _workflowRepo = new();
        private readonly FakeTimeProvider _horloge = new(Maintenant);
        private readonly UtilisateurCourantEnMemoire _utilisateur = new();
        private readonly RegistreService _registreService;
        private readonly WorkflowService _workflowService;

        private readonly InscriptionService _inscriptionService;

        public InscriptionServiceTests()
        {
            _registreService = new(_registreRepo, _utilisateur, _horloge);
            _workflowService = new(_workflowRepo, _utilisateur, _horloge);
            _inscriptionService = new(_registreService, _workflowService);
        }

        // Appel standard : seuls les paramètres testés varient
        private (int IdRegistre, int IdWorkflow) Inscrire(ReferenceDossier? reference = null,
                                                         int nbChambresMin = 1, int nbChambresMax = 2,
                                                         int[]? listeCommunes = null, string? message = null)
            => _inscriptionService.InscriptionPourLogement(
                referenceDossier: reference ?? RefDemande,
                nbChambresMin: nbChambresMin,
                nbChambresMax: nbChambresMax,
                listeCommunes: listeCommunes ?? Communes,
                souhaiteAscenseur: true,
                messageDuCandidat: message);
        // ------------------------------------------------------------


        [Fact]
        public void InscriptionPourLogement_DonneesCoherentes_CreeRegistreEtWorkflowLies()
        {
            (int idRegistre, int idWorkflow) = Inscrire(message: MessageCandidat);
            AuditInfo attendu = new(_utilisateur.Id, Maintenant.UtcDateTime);

            Registre? r = _registreRepo.GetById(idRegistre);
            Assert.NotNull(r);
            Assert.Equal(RefDemande, r.ReferenceDossier);
            Assert.Equal(RegistreType.Logement, r.Type);
            Assert.Equal(RegistreStatut.EnCours, r.Statut);
            Assert.Equal(1, r.NbChambresMinimum);
            Assert.Equal(2, r.NbChambresMaximum);
            Assert.Equal(Communes, r.ListeCommunes.ToArray());
            Assert.True(r.SouhaiteAscenseur);
            Assert.Equal(attendu, r.Auteur);

            Workflow? w = _workflowRepo.GetById(idWorkflow);
            Assert.NotNull(w);
            Assert.Equal(RefDemande, w.ReferenceDossier);
            Assert.Equal(WorkflowType.Tache, w.Type);
            Assert.Equal(WorkflowAction.AValider, w.Action);
            Assert.Equal(IdGroupeRegistreLocataire, w.Assignation.IdGroup);
            Assert.Null(w.Assignation.IdUser);
            Assert.Equal(attendu, w.Expediteur);
            Assert.False(w.EstTermine);
            Assert.Contains("2.060.123/61", w.Message);
        }

        [Fact]
        public void InscriptionPourLogement_MessagePresent_AjouteAuRegistreEtAuWorkflow()
        {
            (int idRegistre, int idWorkflow) = Inscrire(message: MessageCandidat);

            Assert.Equal($"Message du candidat :\n{MessageCandidat}", _registreRepo.GetById(idRegistre)?.Commentaire);
            Assert.EndsWith($").\n{MessageCandidat}", _workflowRepo.GetById(idWorkflow)?.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("     ")]
        public void InscriptionPourLogement_MessageAbsent_RienEstAjoute(string? message)
        {
            (int idRegistre, int idWorkflow) = Inscrire(message: message);

            Assert.Null(_registreRepo.GetById(idRegistre)?.Commentaire);
            Assert.EndsWith(").", _workflowRepo.GetById(idWorkflow)?.Message);
        }
        // ------------------------------------------------------------


        [Fact]
        public void InscriptionPourLogement_ListeCommunesVide_LeveExceptionEtRienNestCree()
        {
            Assert.Throws<ArgumentException>(() => Inscrire(listeCommunes: []));

            Assert.Empty(_registreRepo.GetAll());
            Assert.Empty(_workflowRepo.GetAll());
        }

        [Theory]
        [InlineData(1)]     // séquence 01-60 = contrat
        [InlineData(60)]
        public void InscriptionPourLogement_ReferenceDeContrat_LeveExceptionEtRienNestCree(int sequence)
        {
            ReferenceDossier contrat = ReferenceDossier.DepuisExistant(2060123, sequence);

            Assert.Throws<ArgumentException>(() => Inscrire(reference: contrat));

            Assert.Empty(_registreRepo.GetAll());
            Assert.Empty(_workflowRepo.GetAll());
        }

        [Theory]
        [InlineData(3, 2)]      // min > max
        [InlineData(-1, 2)]     // min négatif
        [InlineData(1, 7)]      // au-delà du maximum autorisé (6)
        public void InscriptionPourLogement_NombreChambresInvalide_LeveExceptionEtRienNestCree(int min, int max)
        {
            // ArgumentOutOfRangeException hérite d'ArgumentException : ThrowsAny accepte les deux
            Assert.ThrowsAny<ArgumentException>(() => Inscrire(nbChambresMin: min, nbChambresMax: max));

            Assert.Empty(_registreRepo.GetAll());
            Assert.Empty(_workflowRepo.GetAll());
        }
        // ------------------------------------------------------------


        /// <summary>
        /// Documente une limite connue : les deux créations ne sont pas dans une même transaction.
        /// Si le workflow échoue, le registre reste en base. Solution : Unit of Work / transaction partagée.
        /// </summary>
        [Fact]
        public void InscriptionPourLogement_EchecDuWorkflow_LeRegistreResteCree_LimiteNonAtomique()
        {
            var workflowService = new WorkflowService(new WorkflowRepositoryEnEchec(), _utilisateur, _horloge);
            var service = new InscriptionService(_registreService, workflowService);

            Assert.Throws<InvalidOperationException>(() => service.InscriptionPourLogement(
                RefDemande, 1, 2, Communes, true, null));

            Assert.Single(_registreRepo.GetAll());
        }

        // Repository qui simule une panne à l'écriture
        private sealed class WorkflowRepositoryEnEchec : IWorkflowRepository
        {
            public int Add(Workflow entity) => throw new InvalidOperationException("Panne simulée.");
            public void Update(Workflow entity, AuditInfo modifiePar) => throw new InvalidOperationException("Panne simulée.");
            public Workflow? GetById(int id) => null;
            public IEnumerable<Workflow> GetAll() => [];
            public IReadOnlyList<Workflow> Search(WorkflowFiltre filtre) => [];
        }
    }
}
