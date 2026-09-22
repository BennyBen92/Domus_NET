namespace FDL.Core.Domain
{
    public class Utilisateur
    {
        public int IdUser { get; private set; }
        public string Name { get; set; } = string.Empty;
        public List<Groupe> Groupes { get; set; } = [];
        // ------------------------------------------------------------

        public Utilisateur(int idUser, string name)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idUser, nameof(idUser));
            IdUser = idUser;
            Name = name;
        }
    }
}
