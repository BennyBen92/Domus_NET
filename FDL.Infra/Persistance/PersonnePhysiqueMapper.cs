using FDL.Core.Domain;

namespace FDL.Infra.Persistance
{
    internal static class PersonnePhysiqueMapper
    {
        public static PersonnePhysique ToDomain(PersonnePhysiqueRow r)
        {
            try
            {
                return PersonnePhysique.Reconstituer(
                    idPersonnePhysique: r.IdPersonnePhysique,
                    nom: r.Nom,
                    prenom: r.Prenom,
                    dateNaissance: r.DateNaissance,
                    numeroNational: NumeroNational.Parse(r.NumeroNational),
                    sexe: (Sexe)r.Sexe);
            }
            catch (Exception e) when (e is ArgumentException)
            {
                throw new InvalidDataException($"Personne physique {r.IdPersonnePhysique} : données invalides en base.", e);
            }
        }

        public static PersonnePhysiqueRow ToRow(PersonnePhysique p) => new()
        {
            IdPersonnePhysique = p.IdPersonnePhysique,
            Nom = p.Nom,
            Prenom = p.Prenom,
            DateNaissance = p.DateNaissance,
            NumeroNational = p.NumeroNational.ToStringBrute(),
            Sexe = (int)p.Sexe
        };
    }
}
