namespace FDL.Core.Domain
{
    public class Groupe
    {
        public int IdGroupe { get; private set; }
        public string Name { get; set; } = string.Empty;
        // ------------------------------------------------------------

        public Groupe(int idGroupe, string name)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idGroupe, nameof(idGroupe));
            IdGroupe = idGroupe;
            Name = name;
        }

    }
}
