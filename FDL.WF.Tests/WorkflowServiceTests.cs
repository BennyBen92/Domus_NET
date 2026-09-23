using FDL.Core.Domain;
using FDL.WF.App;
using FDL.WF.Domain;
using FDL.WF.Tests.Fakes;
using Microsoft.Extensions.Time.Testing;

namespace FDL.WF.Tests
{
    public class WorkflowServiceTests
    {
        private static readonly DateTimeOffset Maintenant = new(2026, 10, 20, 12, 0, 0, TimeSpan.Zero);
        private static readonly ReferenceDossier Dossier = ReferenceDossier.DepuisExistant(1500123, 1);

        private readonly WorkflowRepositoryEnMemoire _repo = new();
        private readonly UtilisateurCourantEnMemoire _utilisateur = new();
        private readonly FakeTimeProvider _horloge = new(Maintenant);
        private readonly WorkflowService _service;

        public WorkflowServiceTests()
        {
            _service = new WorkflowService(_repo, _utilisateur, _horloge);
        }


        [Fact]
        public void CreerPourNote_ExpEstUtilisateurCourant_HeureCourante()
        {
            // Arrange
            _utilisateur.ChangerUtilisateur(7, [10]);

            // Act
            int id = _service.CreerPourNote(Dossier, WorkflowAssignation.PourUtilisateur(42), "À lire");

            // Assert
            Workflow? wf = _repo.GetById(id);
            Assert.NotNull(wf);
            Assert.Equal(new AuditInfo(7, Maintenant.UtcDateTime), wf.Expediteur);
        }

        [Fact]
        public void Terminer_EnregistreQuiEtQuand()
        {
            // Arrange : l'utilisateur 1 crée, le 42 termine 3 h plus tard
            int id = _service.CreerPourNote(Dossier, WorkflowAssignation.PourUtilisateur(42), "À lire");
            _utilisateur.ChangerUtilisateur(42, [10]);
            _horloge.Advance(TimeSpan.FromHours(3));

            // Act
            _service.Terminer(id);

            // Assert
            Workflow? wf = _repo.GetById(id);
            Assert.NotNull(wf);
            Assert.Equal(new AuditInfo(42, Maintenant.UtcDateTime.AddHours(3)), wf.Terminaison);
        }

        [Fact]
        public void Terminer_IdInexistant_LeveException()
        {
            // Act : On tente de terminer un wf inexistant
            Assert.Throws<InvalidOperationException>(() =>
                _service.Terminer(999)
            );
        }

        [Fact]
        public void Reassigner_ChangeAssignation()
        {
            // Arrange : l'utilisateur 1 crée un wf note pour le 42
            int id = _service.CreerPourNote(Dossier, WorkflowAssignation.PourUtilisateur(42), "A lire");
            WorkflowAssignation nouvelleAssignation = WorkflowAssignation.PourUtilisateur(2);

            // Act : on change l'assignation vers l'utilisateur 2
            _service.Reassigner(id, nouvelleAssignation);

            // Assert
            Workflow? wf = _repo.GetById(id);
            Assert.NotNull(wf);
            Assert.Equal(nouvelleAssignation, wf.Assignation);
        }

        [Fact]
        public void Reassigner_IdInexistant_LeveException()
        {
            // Act : On tente de réassigner un wf inexistant
            Assert.Throws<InvalidOperationException>(() =>
                _service.Reassigner(999, WorkflowAssignation.PourUtilisateur(42))
            );
        }

        [Fact]
        public void MesWorkflowsEnCours_ContientSeulementLesMiens()
        {
            // Arrange : l'utilisateur 1 crée un wf note pour le 42 et un autre pour le 43
            int idWF42 = _service.CreerPourNote(Dossier, WorkflowAssignation.PourUtilisateur(42), "A lire");
            _service.CreerPourNote(Dossier, WorkflowAssignation.PourUtilisateur(43), "A lire");
            _utilisateur.ChangerUtilisateur(42, [10]);

            // Act
            var list = _service.MesWorkflowsEnCours();

            // Assert
            var wf = Assert.Single(list);
            Assert.Equal(idWF42, wf.IdWorkflow);
        }

        [Fact]
        public void MesWorkflowsEnCours_ContientLesMiensPasCeuxDeMesGroupes()
        {
            // Arrange : l'utilisateur 1 crée un wf note pour le 42 et un autre pour le groupe 10
            int idPourMoi = _service.CreerPourNote(Dossier, WorkflowAssignation.PourUtilisateur(42), "A lire");
            _service.CreerPourNote(Dossier, WorkflowAssignation.PourGroupe(10), "A lire");
            _utilisateur.ChangerUtilisateur(42, [10]);

            // Act
            var list = _service.MesWorkflowsEnCours();

            // Assert
            var wf = Assert.Single(list);
            Assert.Equal(idPourMoi, wf.IdWorkflow);
        }

        [Fact]
        public void MesWorkflowsEnCours_ExclutLesTermines()
        {
            // Arrange : l'utilisateur 1 crée deux wf note pour le 42
            int idTermine = _service.CreerPourNote(Dossier, WorkflowAssignation.PourUtilisateur(42), "A lire");
            int id = _service.CreerPourNote(Dossier, WorkflowAssignation.PourUtilisateur(42), "A lire");
            _utilisateur.ChangerUtilisateur(42, [10]);
            _service.Terminer(idTermine);

            // Act : l'utilisateur 42 a terminé un des 2 wf
            var list = _service.MesWorkflowsEnCours();

            // Assert
            var wf = Assert.Single(list);
            Assert.Equal(id, wf.IdWorkflow);
        }

        [Fact]
        public void WorkflowsDeMesGroupes_ContientCeuxDeMesGroupes()
        {
            // Arrange : l'utilisateur 1 crée un wf note pour le groupe 10, 20 et pour l'utilisateur 42
            int id = _service.CreerPourNote(Dossier, WorkflowAssignation.PourGroupe(10), "A lire");
            _service.CreerPourNote(Dossier, WorkflowAssignation.PourGroupe(20), "A lire");
            _service.CreerPourNote(Dossier, WorkflowAssignation.PourUtilisateur(42), "A lire");
            _utilisateur.ChangerUtilisateur(42, [10]);

            // Act
            var list = _service.WorkflowsDeMesGroupes();

            // Assert
            var wf = Assert.Single(list);
            Assert.Equal(id, wf.IdWorkflow);
        }

        [Fact]
        public void WorkflowsDeMesGroupes_UtilisateurSansGroupe_RenvoieVide()
        {
            // Arrange : l'utilisateur 1 crée un wf note pour le groupe 10
            _service.CreerPourNote(Dossier, WorkflowAssignation.PourGroupe(10), "A lire");
            _utilisateur.ChangerUtilisateur(43, []);

            // Act
            var list = _service.WorkflowsDeMesGroupes();

            // Assert
            Assert.Empty(list);
        }

    }
}
