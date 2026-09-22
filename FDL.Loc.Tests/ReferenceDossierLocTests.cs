using FDL.Core.Domain;
using FDL.Loc.Domain;

namespace FDL.Loc.Tests
{
    public class ReferenceDossierLocTests
    {
        [Fact]
        public void Registre_AvecReferenceDeDemande_EstAccepte()
        {
            var referenceDossier = ReferenceDossier.DepuisExistant(1523456, 61); // Séquence 61 est une demande
            var registre = new Registre(referenceDossier, RegistreType.Logement, RegistreStatut.EnCours);
            Assert.NotNull(registre);
            Assert.Equal(referenceDossier, registre.RefDossier);
        }
        [Fact]
        public void Registre_AvecReferenceDeContrat_LeveUneException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var referenceDossier = ReferenceDossier.DepuisExistant(1523456, 1); // Séquence 1 est un contrat
                var registre = new Registre(referenceDossier, RegistreType.Logement, RegistreStatut.EnCours);
            });
        }
    }
}
