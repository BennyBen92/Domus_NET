using FDL.Core.Domain;

namespace FDL.Core.Tests
{
    public class ReferenceDossierTests
    {
        [Fact]
        public void Creation_AvecSequence0_EstRefusee()
        {

        }
        [Fact]
        public void DepuisExistant_AvecSequence0_EstUneDemandeHeritee()
        {

        }
        [Theory]
        [InlineData(1_500_123, 61)]
        [InlineData(9_999_999, 1)]
        public void ToString_PuisParse_RedonneLaMemeReference(int dossier, int sequence)
        {
            var origine = new ReferenceDossier(dossier, sequence);
            Assert.Equal(origine, ReferenceDossier.Parse(origine.ToString()));
        }
    }
}