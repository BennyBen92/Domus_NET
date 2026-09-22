namespace FDL.Core.Domain
{
    public sealed class Groupe
    {
        public int Id { get; }
        public string Nom { get; }
        // ------------------------------------------------------------

        public Groupe(int id, string nom)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id, nameof(id));
            ArgumentException.ThrowIfNullOrWhiteSpace(nom,nameof(nom));
            Id = id;
            Nom = nom;
        }

    }
}
