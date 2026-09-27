using FDL.Infra.Persistance;

namespace FDL.Infra.Tests
{
    public class ReferenceDossierCodecTests
    {
        [Theory]
        [InlineData(150012300)]
        [InlineData(150012301)]
        [InlineData(150012361)]
        [InlineData(150012399)]
        public void ReferenceDossierCodec_ConversionToIntValide_EtRetourIdentique(int numeroDossier)
        {
            var refDossier = ReferenceDossierCodec.FromInt(numeroDossier);
            Assert.Equal(numeroDossier, ReferenceDossierCodec.ToInt(refDossier));
        }

        [Fact]
        public void ReferenceDossierCodec_ConversionFromIntInvalide_LeveException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ReferenceDossierCodec.FromInt(0));
        }
    }
}
