using FDL.Core.Domain;
using FDL.Loc.App;
using FDL.Loc.Domain;

namespace FDL.Loc.Tests.Fakes
{
    /// <summary>
    /// Implémentation en mémoire de <see cref="IRegistreRepository"/> pour les tests du service.
    /// Le repository stocke et renvoie des copies : un registre modifié sans appel à Update
    /// n'est pas persisté, exactement comme avec une vraie base.
    /// Il applique aussi la contrainte d'unicité « un registre par référence de dossier ».
    /// </summary>
    public sealed class RegistreRepositoryEnMemoire : IRegistreRepository
    {
        private readonly List<Registre> _table = [];
        private int _dernierId;
        // ------------------------------------------------------------


        /// <summary>
        /// Ajoute un registre et renvoie l'identifiant attribué (auto-incrément).
        /// Lève une exception si un registre existe déjà pour la même référence de dossier.
        /// </summary>
        public int Add(Registre entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            if (_table.Exists(r => r.RefDossier == entity.RefDossier))
                throw new InvalidOperationException($"Un registre existe déjà pour le dossier {entity.RefDossier}.");

            int id = ++_dernierId;
            _table.Add(Copier(entity, id));
            return id;
        }

        /// <summary>
        /// Remplace le registre stocké. Lève une exception si l'identifiant est inconnu.
        /// </summary>
        public void Update(Registre entity, AuditInfo modifiePar)
        {
            ArgumentNullException.ThrowIfNull(entity);
            int index = _table.FindIndex(r => r.IdRegistre == entity.IdRegistre);
            if (index < 0)
                throw new InvalidOperationException($"Le registre {entity.IdRegistre} n'existe pas.");
            _table[index] = Copier(entity, entity.IdRegistre);
        }
        // ------------------------------------------------------------


        public Registre? GetById(int id)
        {
            Registre? r = _table.Find(r => r.IdRegistre == id);
            return r is null ? null : Copier(r, r.IdRegistre);
        }

        public IEnumerable<Registre> GetAll()
            => _table.Select(r => Copier(r, r.IdRegistre)).ToList();

        public Registre? GetByReferenceDossier(ReferenceDossier reference)
        {
            ArgumentNullException.ThrowIfNull(reference);
            Registre? r = _table.Find(r => r.RefDossier == reference);
            return r is null ? null : Copier(r, r.IdRegistre);
        }
        // ------------------------------------------------------------


        // Simule l'aller-retour en base : nouvelle instance, même état.
        private static Registre Copier(Registre r, int id)
            => Registre.Reconstituer(id, r.RefDossier, r.Type, r.Statut, r.DateStatut,
                r.NbChambresMinimum, r.NbChambresMaximum, [.. r.ListeCommunes], r.SouhaiteAscenseur,
                r.Auteur, r.Commentaire);
    }
}
