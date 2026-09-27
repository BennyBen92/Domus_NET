using FDL.Core.Domain;
using FDL.Loc.App;
using FDL.Loc.Domain;
using FDL.Loc.Tests.Fakes;
using Microsoft.Extensions.Time.Testing;

namespace FDL.Loc.Tests
{
    public class RegistreServiceTests
    {
        private static readonly DateTimeOffset Maintenant = new(2026, 10, 20, 12, 0, 0, TimeSpan.Zero);
        private static readonly ReferenceDossier Dossier = ReferenceDossier.DepuisExistant(2050123, 61);
        private static readonly ReferenceDossier AutreDossier = ReferenceDossier.DepuisExistant(2050124, 61);
        private static readonly int[] Communes = [1050, 1080];

        private readonly RegistreRepositoryEnMemoire _repo = new();
        private readonly UtilisateurCourantEnMemoire _utilisateur = new();
        private readonly FakeTimeProvider _horloge = new(Maintenant);
        private readonly RegistreService _service;

        public RegistreServiceTests()
        {
            _service = new RegistreService(_repo, _utilisateur, _horloge);
        }

        // Crée un registre du type demandé via le service
        private int Creer(string type, ReferenceDossier dossier) => type switch
        {
            "Logement" => _service.CreerPourLogement(dossier, 1, 2, Communes, true, "Logement"),
            "PMR" => _service.CreerPourPMR(dossier, 0, 1, Communes, true, "PMR"),
            "Commerce" => _service.CreerPourCommerce(dossier, Communes, "Commerce"),
            "Parking" => _service.CreerPourParking(dossier, Communes, null),
            _ => throw new ArgumentException($"Type inconnu : {type}", nameof(type))
        };

        // Clôture un registre via le service
        private void Cloturer(string action, int id)
        {
            switch (action)
            {
                case "Terminer": _service.Terminer(id); break;
                case "Radier": _service.Radier(id); break;
                default: throw new ArgumentException($"Action inconnue : {action}", nameof(action));
            }
        }
        // ------------------------------------------------------------


        // --- Création -----------------------------------------------

        [Fact]
        public void CreerPourLogement_EnregistreAvecUtilisateurEtHeureCourants()
        {
            // Arrange
            _utilisateur.ChangerUtilisateur(7, [10]);

            // Act
            int id = _service.CreerPourLogement(Dossier, 1, 3, Communes, true, "Un commentaire");

            // Assert : on relit le repository, pas l'objet créé
            Registre? r = _repo.GetById(id);
            Assert.NotNull(r);
            Assert.Equal(new AuditInfo(7, Maintenant.UtcDateTime), r.Auteur);
            Assert.Equal(Dossier, r.RefDossier);
            Assert.Equal(RegistreStatut.EnCours, r.Statut);
            Assert.Equal(Maintenant.UtcDateTime, r.DateStatut);
            Assert.Equal(1, r.NbChambresMinimum);
            Assert.Equal(3, r.NbChambresMaximum);
            Assert.Equal(Communes, r.ListeCommunes);
            Assert.True(r.SouhaiteAscenseur);
            Assert.Equal("Un commentaire", r.Commentaire);
        }

        [Theory]
        [InlineData("Logement", RegistreType.Logement)]
        [InlineData("PMR", RegistreType.PMR)]
        [InlineData("Commerce", RegistreType.Commerce)]
        [InlineData("Parking", RegistreType.Parking)]
        public void Creer_ChaqueType_EnregistreLeBonType(string type, RegistreType attendu)
        {
            int id = Creer(type, Dossier);

            Registre? r = _repo.GetById(id);
            Assert.NotNull(r);
            Assert.Equal(attendu, r.Type);
            Assert.True(r.EstEnCours);
        }

        [Theory]
        [InlineData("Logement")]
        [InlineData("Parking")]
        public void Creer_ReferenceDejaUtilisee_LeveInvalidOperation(string secondType)
        {
            // Arrange : un registre existe déjà pour ce dossier
            Creer("Logement", Dossier);

            // Act + Assert
            Assert.Throws<InvalidOperationException>(() => Creer(secondType, Dossier));
            Assert.Single(_repo.GetAll());   // le second n'a pas été enregistré
        }

        [Fact]
        public void Creer_DeuxDossiersDifferents_DeuxRegistres()
        {
            int id1 = Creer("Logement", Dossier);
            int id2 = Creer("Logement", AutreDossier);

            Assert.NotEqual(id1, id2);
            Assert.Equal(2, _repo.GetAll().Count());
        }

        [Fact]
        public void Creer_DonneesInvalides_RienNestEnregistre()
        {
            Assert.Throws<ArgumentException>(() =>
                _service.CreerPourLogement(Dossier, 1, 2, [], false, null));   // liste vide

            Assert.Empty(_repo.GetAll());
        }
        // ------------------------------------------------------------


        // --- ChangeListeCommunes ------------------------------------

        [Fact]
        public void ChangeListeCommunes_RegistreEnCours_NouvelleListeEnregistree()
        {
            int id = Creer("Logement", Dossier);

            _service.ChangeListeCommunes(id, [1200, 1210]);

            Registre? r = _repo.GetById(id);
            Assert.NotNull(r);
            Assert.Equal([1200, 1210], r.ListeCommunes);
        }

        [Fact]
        public void ChangeListeCommunes_IdInexistant_LeveInvalidOperation()
        {
            Assert.Throws<InvalidOperationException>(() => _service.ChangeListeCommunes(999, [1200]));
        }

        [Theory]
        [InlineData("Terminer")]
        [InlineData("Radier")]
        public void ChangeListeCommunes_RegistreCloture_LeveInvalidOperationEtListeInchangee(string action)
        {
            int id = Creer("Logement", Dossier);
            Cloturer(action, id);

            Assert.Throws<InvalidOperationException>(() => _service.ChangeListeCommunes(id, [1200]));

            Registre? r = _repo.GetById(id);
            Assert.NotNull(r);
            Assert.Equal(Communes, r.ListeCommunes);
        }
        // ------------------------------------------------------------


        // --- Terminer / Radier --------------------------------------

        [Theory]
        [InlineData("Terminer", RegistreStatut.Termine)]
        [InlineData("Radier", RegistreStatut.Radie)]
        public void Cloturer_RegistreEnCours_ChangeStatutAvecHeureCourante(string action, RegistreStatut attendu)
        {
            // Arrange : création, puis clôture 5 jours plus tard
            int id = Creer("Logement", Dossier);
            _horloge.Advance(TimeSpan.FromDays(5));

            // Act
            Cloturer(action, id);

            // Assert
            Registre? r = _repo.GetById(id);
            Assert.NotNull(r);
            Assert.Equal(attendu, r.Statut);
            Assert.Equal(Maintenant.UtcDateTime.AddDays(5), r.DateStatut);
            Assert.Equal(Maintenant.UtcDateTime, r.Auteur.Date);   // la création n'a pas bougé
        }

        [Theory]
        [InlineData("Terminer")]
        [InlineData("Radier")]
        public void Cloturer_IdInexistant_LeveInvalidOperation(string action)
        {
            Assert.Throws<InvalidOperationException>(() => Cloturer(action, 999));
        }

        [Theory]
        [InlineData("Terminer", "Terminer")]
        [InlineData("Terminer", "Radier")]
        [InlineData("Radier", "Terminer")]
        [InlineData("Radier", "Radier")]
        public void Cloturer_RegistreDejaCloture_LeveInvalidOperationEtStatutInchange(string premiere, string seconde)
        {
            int id = Creer("Logement", Dossier);
            Cloturer(premiere, id);
            RegistreStatut statutAvant = _repo.GetById(id)!.Statut;

            Assert.Throws<InvalidOperationException>(() => Cloturer(seconde, id));

            Assert.Equal(statutAvant, _repo.GetById(id)!.Statut);
        }
        // ------------------------------------------------------------


        // --- GetByReferenceDossier ----------------------------------

        [Fact]
        public void GetByReferenceDossier_DossierExistant_RenvoieSonRegistre()
        {
            Creer("Logement", AutreDossier);
            int id = Creer("Parking", Dossier);

            Registre? r = _service.GetByReferenceDossier(Dossier);

            Assert.NotNull(r);
            Assert.Equal(id, r.IdRegistre);
        }

        [Fact]
        public void GetByReferenceDossier_DossierInconnu_RenvoieNull()
        {
            Creer("Logement", Dossier);

            Assert.Null(_service.GetByReferenceDossier(AutreDossier));
        }
    }
}
