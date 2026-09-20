namespace FDL.WF.Domain
{
    // Représente l'assignation d'un workflow à un utilisateur ou à un groupe.
    public sealed record WorkflowAssignation
    {
        public int? IdUser { get; }
        public int? IdGroup { get; }

        public bool EstUtilisateur => IdUser.HasValue;
        public bool EstGroupe => IdGroup.HasValue;

        private WorkflowAssignation(int? idUser, int? idGroup) => (IdUser, IdGroup) = (idUser, idGroup);

        /// <summary>
        /// Crée une assignation pour un utilisateur.
        /// </summary>
        /// <param name="idUser"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static WorkflowAssignation PourUtilisateur(int idUser)
        {
            return idUser <= 0 ? throw new ArgumentOutOfRangeException(nameof(idUser), "L'identifiant de l'utilisateur doit être supérieur à zéro.") : new WorkflowAssignation(idUser, null);
        }

        /// <summary>
        /// Crée une assignation pour un groupe.
        /// </summary>
        /// <param name="idGroup"></param>
        public static WorkflowAssignation PourGroupe(int idGroup)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idGroup, nameof(idGroup));
            return new WorkflowAssignation(null, idGroup);
        }
    }
}
