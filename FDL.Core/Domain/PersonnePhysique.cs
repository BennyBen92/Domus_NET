namespace FDL.Core.Domain
{
    public enum Sexe
    {
        Masculin,
        Feminin,
        NonBinaire
    }

    public sealed class PersonnePhysique
    {
        public int IdPersonnePhysique { get; }
        public string Nom { get; }
        public string? Prenom { get; }
        public DateOnly DateNaissance { get; }
        public NumeroNational NumeroNational { get; }
        public Sexe Sexe { get; }
        // ------------------------------------------------------------

        private PersonnePhysique(int idPersonnePhysique, string nom, string? prenom, DateOnly dateNaissance, NumeroNational numeroNational, Sexe sexe)
        {
            if (string.IsNullOrWhiteSpace(nom))
                throw new ArgumentException("Le nom ne peut pas être vide.", nameof(nom));

            if (dateNaissance == default)
                throw new ArgumentException("La date de naissance est obligatoire.", nameof(dateNaissance));

            if (dateNaissance > DateOnly.FromDateTime(DateTime.Today))
                throw new ArgumentException("La date de naissance ne peut pas être dans le futur.", nameof(dateNaissance));

            if (numeroNational is null)
                throw new ArgumentException("Le numéro national ne peut pas être vide.", nameof(numeroNational));

            if (!Enum.IsDefined(sexe))
                throw new InvalidDataException($"Personne physique {idPersonnePhysique} : sexe invalide ({(int)sexe}).");

            if (!numeroNational.EstCoherentAvec(dateNaissance, sexe))
                throw new ArgumentException("Le numéro national n'est pas cohérent avec la date de naissance et le sexe.", nameof(numeroNational));

            IdPersonnePhysique = idPersonnePhysique;
            Nom = nom;
            Prenom = prenom;
            DateNaissance = dateNaissance;
            NumeroNational = numeroNational;
            Sexe = sexe;
        }
        // ------------------------------------------------------------

        /// <summary>
        /// Détermine l'âge de la personne selon la date de référence
        /// </summary>
        public int Age(DateOnly dateReference)
        {
            if (dateReference < DateNaissance)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(dateReference),
                    dateReference,
                    "La date de référence ne peut pas être antérieure à la date de naissance.");
            }
            int age = dateReference.Year - DateNaissance.Year;
            if (dateReference < DateNaissance.AddYears(age))
                age--;
            return age;
        }
        /// <summary>
        /// Détermine l'âge de la personne aujourd'hui
        /// </summary>
        public int AgeAujourdhui => Age(DateOnly.FromDateTime(DateTime.Today));
        // ------------------------------------------------------------

        public static PersonnePhysique Creer(string nom, string? prenom, DateOnly dateNaissance, NumeroNational numeroNational, Sexe sexe)
        {
            return new(idPersonnePhysique: 0,
                        nom: nom,
                        prenom: prenom,
                        dateNaissance: dateNaissance,
                        numeroNational: numeroNational,
                        sexe: sexe);
        }

        public static PersonnePhysique Reconstituer(int idPersonnePhysique, string nom, string? prenom, DateOnly dateNaissance, NumeroNational numeroNational, Sexe sexe)
        {
            // Validation des paramètres
            if (idPersonnePhysique <= 0)
                throw new ArgumentException("L'identifiant de la personne physique doit être supérieur à zéro.", nameof(idPersonnePhysique));

            return new(idPersonnePhysique: idPersonnePhysique,
                         nom: nom,
                         prenom: prenom,
                         dateNaissance: dateNaissance,
                         numeroNational: numeroNational,
                         sexe: sexe);
        }
        // ------------------------------------------------------------

        public string NomComplet() => $"{Prenom} {Nom}".Trim();

    }
}
