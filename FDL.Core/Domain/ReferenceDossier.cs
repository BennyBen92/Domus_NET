using System.Globalization;

namespace FDL.Core.Domain
{
    public sealed record ReferenceDossier
    {
        /** Propriétés pour le numéro de dossier.*/
        public int NumeroDossier { get; }
        /** Propriétés pour le numéro de séquence (Demande ou Contrat).*/
        public int Sequence { get; }

        public bool EstContrat => Sequence is >= 1 and <= 60;
        public bool EstDemande => Sequence == 0 || Sequence is >= 61 and <= 99;
        public bool EstNotationHeritee => Sequence == 0;

        /**
         * Constructeur privé pour forcer l'utilisation de la méthode statique DepuisExistant.
         */
        private ReferenceDossier(int numeroDossier)
        {
            NumeroDossierValide(numeroDossier);
            NumeroDossier = numeroDossier;
            Sequence = 0;
        }
        /**
         * Constructeur par défaut.
         */
        public ReferenceDossier(int numeroDossier, int sequence)
        {
            NumeroDossierValide(numeroDossier);
            SequenceValide(sequence);
            NumeroDossier = numeroDossier;
            Sequence = sequence;
        }
        /**
         * Méthode statique pour créer une instance de ReferenceDossier à partir d'un numéro Existant.
         * Si la séquence est 0, elle crée une instance avec seulement le numéro de dossier.
         */
        public static ReferenceDossier DepuisExistant(int numeroDossier, int sequence)
        {
            if (sequence == 0)
            {
                return new ReferenceDossier(numeroDossier);
            }
            return new ReferenceDossier(numeroDossier, sequence);
        }
        /**
         * Méthode statique pour créer une instance de ReferenceDossier à partir d'une chaîne de caractères.
         * La chaîne doit être au format "NumeroDossier/Sequence".
         */
        public static ReferenceDossier Parse(string referenceDossier)
        {
            if (string.IsNullOrWhiteSpace(referenceDossier))
                throw new ArgumentNullException(nameof(referenceDossier));

            var parts = referenceDossier.Split('/');

            if (parts.Length != 2)
                throw new FormatException("ReferenceDossier must be in the format 'NumeroDossier/Sequence'");

            if (!int.TryParse(parts[0].Replace(".", ""), NumberStyles.None, CultureInfo.InvariantCulture, out var numeroDossier))
                throw new FormatException("NumeroDossier must be an integer");

            if (!int.TryParse(parts[1], out var sequence))
                throw new FormatException("Sequence must be an integer");

            return new ReferenceDossier(numeroDossier, sequence);
        }

        private static void NumeroDossierValide(int numeroDossier)
        {
            if (numeroDossier is < 1 or > 9_999_999)
            {
                throw new ArgumentOutOfRangeException(nameof(numeroDossier), numeroDossier, "NumeroDossier must be between 1 and 9 999 999");
            }
        }
        private static void SequenceValide(int sequence)
        {
            if (sequence is < 1 or > 99)
            {
                throw new ArgumentOutOfRangeException(nameof(sequence), sequence, "Sequence must be between 1 and 99");
            }
        }
        /**
         * Méthode toString pour retourner la référence du dossier sous forme de chaîne de caractères.
         * Exemple : "1.500.123/01"
         */
        public override string ToString() => $"{NumeroDossier.ToString("#,0", CultureInfo.InvariantCulture).Replace(',', '.')}/{Sequence:00}";
    }
}
