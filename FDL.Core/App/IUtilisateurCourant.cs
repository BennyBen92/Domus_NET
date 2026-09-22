namespace FDL.Core.App
{
    public interface IUtilisateurCourant
    {
        /// <summary>
        /// Identifie l'utilisateur courant dans l'application.
        /// </summary>
        int Id { get; }
        /// <summary>
        /// Liste des groupes de l'utilisateur dans l'application.
        /// </summary>
        IReadOnlyList<int> IdGroupes { get; }

    }
}
