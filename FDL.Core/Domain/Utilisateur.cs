namespace FDL.Core.Domain
{
    public sealed class Utilisateur
    {
        private readonly List<Groupe> _groupes = [];

        public int Id { get; }
        public string Nom { get; }
        public IReadOnlyList<Groupe> Groupes => _groupes;
        // ------------------------------------------------------------

        public Utilisateur(int id, string nom)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id, nameof(id));
            ArgumentException.ThrowIfNullOrWhiteSpace(nom);
            Id = id;
            Nom = nom;
        }
    }
}
