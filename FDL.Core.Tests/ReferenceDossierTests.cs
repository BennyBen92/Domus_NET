using FDL.Core.Domain;

namespace FDL.Core.Tests
{
    public class ReferenceDossierTests(ITestOutputHelper output)
    {
        private readonly ITestOutputHelper _output = output;

        [Fact]
        public void Creation_AvecSequence0_EstRefusee()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ReferenceDossier.DepuisExistant(1_500_123, 0));
        }
        [Fact]
        public void DepuisExistant_AvecSequence0_EstUneDemandeHeritee()
        {
            var reference = ReferenceDossier.DepuisExistant(1_500_123, 0);
            Assert.True(reference.EstDemande, "La référence devrait être une demande.");
            Assert.True(reference.EstNotationHeritee, "La référence devrait être un numéro hérité de l'ancien système.");
        }
        [Theory]
        [InlineData(1_500_123, 61)]
        [InlineData(9_999_999, 1)]
        public void ToString_PuisParse_RedonneLaMemeReference(int dossier, int sequence)
        {
            var origine = ReferenceDossier.DepuisExistant(dossier, sequence);
            _output.WriteLine($"Origine: {origine}");
            Assert.Equal(origine, ReferenceDossier.Parse(origine.ToString()));
        }
    }
}