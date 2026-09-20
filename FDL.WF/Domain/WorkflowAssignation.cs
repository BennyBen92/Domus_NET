namespace FDL.WF.Domain
{
    /**
     * Classe représentant l'assignation d'un workflow à un utilisateur ou à un groupe.
     * Cette classe est immuable et utilise des méthodes statiques pour créer des instances.
     */
    public sealed record WorkflowAssignation
    {
        public int? IdUser { get; }
        public int? IdGroup { get; }

        public bool EstUtilisateur => IdUser.HasValue;
        public bool EstGroupe => IdGroup.HasValue;

        // Constructeur privé pour forcer l'utilisation des méthodes statiques PourUtilisateur et PourGroupe
        private WorkflowAssignation(int? idUser, int? idGroup) => (IdUser, IdGroup) = (idUser, idGroup);

        // Méthode statique pour créer une assignation pour un utilisateur
        public static WorkflowAssignation PourUtilisateur(int idUser)
        {
            return idUser < 0 ? throw new ArgumentOutOfRangeException(nameof(idUser), "L'identifiant de l'utilisateur doit être supérieur à zéro.") : new WorkflowAssignation(idUser, null);
        }
        // Méthode statique pour créer une assignation pour un groupe
        public static WorkflowAssignation PourGroupe(int idGroup)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(idGroup, nameof(idGroup));
            return new WorkflowAssignation(null, idGroup);
        }
    }
}
