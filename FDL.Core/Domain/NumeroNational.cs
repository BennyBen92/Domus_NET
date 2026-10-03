using System.Diagnostics.CodeAnalysis;

namespace FDL.Core.Domain
{
    // Numéro national est représenté : YY.MM.DD-SSS.CC
    public sealed record NumeroNational
    {
        /// <summary>
        /// Numéro national temporaire
        /// </summary>
        public static readonly NumeroNational Temporaire = new(0, 0, 0, 0, 97);

        /// <summary>
        /// Année : 00 -> 99
        /// </summary>
        public int YY { get; }
        /// <summary>
        /// Mois : 00 -> 12 avec exception: +20 ou +40 pour les numéros BIS (étrangers non inscrits au registre national)
        /// </summary>
        public int MM { get; }
        /// <summary>
        /// Jour : 00 -> 31
        /// </summary>
        public int DD { get; }
        /// <summary>
        /// Numéro : 000 -> 999
        /// </summary>
        public int SSS { get; }
        /// <summary>
        /// CC correspond au résultat de la formule "97 - (n % 97)", où n vaut YYMMDDSSS.
        /// </summary>
        public int CC { get; }
        // ------------------------------------------------------------


        private NumeroNational(int yy, int mm, int dd, int sss, int cc)
        {
            string? erreur = ErreurDansNumero(yy, mm, dd, sss, cc);
            if (erreur is not null)
                throw new ArgumentException(erreur);

            YY = yy;
            MM = mm;
            DD = dd;
            SSS = sss;
            CC = cc;
        }
        // ------------------------------------------------------------

        public bool EstTemporaire => this == Temporaire;
        public bool EstNumeroBISAvecSexeInconnu => MM is >= 20 and < 40;    // +20
        public bool EstNumeroBISAvecSexeConnu => MM >= 40;                  // +40

        public static int MoisReel(int mois) => mois >= 40 ? mois - 40 : (mois >= 20 ? mois - 20 : mois);
        // ------------------------------------------------------------


        /// <summary>
        /// Méthode statique pour créer une instance de NumeroNational à partir d'une chaîne de caractères.
        /// </summary>
        public static NumeroNational Parse(string numeroNational)
        {
            if (!TryParse(numeroNational, out var result, out var erreur))
                throw new ArgumentException(erreur, nameof(numeroNational));
            return result;
        }
        /// <summary>
        /// Méthode statique pour essayer de créer une instance de NumeroNational à partir d'une chaîne de caractères.
        /// </summary>
        public static bool TryParse(string? numeroNational, [NotNullWhen(true)] out NumeroNational? result)
            => TryParse(numeroNational, out result, out _);

        private static bool TryParse(string? s, [NotNullWhen(true)] out NumeroNational? result, out string? erreur)
        {
            result = null;
            erreur = null;
            string chiffres = string.Concat((s ?? "").Where(c => c is not (' ' or '.' or '-')));

            if (chiffres.Length != 11 || chiffres.All(char.IsAsciiDigit))
            {
                erreur = "Un numéro national est composé de 11 chiffres.";
                return false;
            }

            int yy = int.Parse(chiffres[..2]), mm = int.Parse(chiffres[2..4]), dd = int.Parse(chiffres[4..6]),
                sss = int.Parse(chiffres[6..9]), cc = int.Parse(chiffres[9..11]);

            erreur = ErreurDansNumero(yy, mm, dd, sss, cc);

            if (erreur is not null)
                return false;

            result = new NumeroNational(yy, mm, dd, sss, cc);

            return true;
        }
        // ------------------------------------------------------------


        private static bool CleValide(long n, int cc) => cc == 97 - (n % 97);

        private static void RespecteModulo97(string numeroNational)
        {
            if (!int.TryParse(numeroNational[..9], out int annees1900) || !int.TryParse(numeroNational[9..11], out int cc))
                throw new ArgumentException("Echec de la vérification : la chaine ne représente pas un nombre.");

            // Années 1900 ou années 2000
            long annees2000 = 2_000_000_000L + annees1900; // il faut ajouter un 2 devant
            if (!CleValide(annees1900, cc) && !CleValide(annees2000, cc))
                throw new ArgumentException("Le numéro national est invalide : le numéro de contrôle est incorrect.");
        }

        private static string? ErreurDansNumero(int yy, int mm, int dd, int sss, int cc)
        {
            // 00 -> 99
            if (yy is not (>= 0 and <= 99))
                return $"Le nombre d'année doit être compris entre 0 et 99. Or il vaut {yy:00}";
            // 00 -> 12 avec exception: +20 ou +40
            if (MoisReel(mm) is not (>= 0 and <= 12))
                return $"Le nombre de mois doit être compris entre 0 et 12 (majoré de 20 ou 40). Or il vaut {mm:00}";
            // 00 -> 31
            if (dd is not (>= 0 and <= 31))
                return $"Le nombre de jours doit être compris entre 0 et 31. Or il vaut {dd:00}";
            // 00 -> 999
            if (sss is not (>= 0 and <= 999))
                return $"Le numéro de série doit être compris entre 0 et 999. Or il vaut {sss:000}";
            // 00 -> 99
            if (cc is not (>= 0 and <= 99))
                return $"Le numéro de contrôle doit être compris entre 0 et 99. Or il vaut {cc:00}";

            RespecteModulo97($"{yy:00}{mm:00}{dd:00}{sss:000}{cc:00}");

            return null;
        }

        public bool EstCoherentAvec(DateOnly dateNaissance, Sexe sexe)
        {
            // Les contrôle ne fonctionne que si le numéro n'est pas un temporaire
            if (EstTemporaire)
                return true;

            // Contrôle de parité
            bool sexeCoherent = EstNumeroBISAvecSexeInconnu || sexe switch
            {
                Sexe.Masculin => !int.IsEvenInteger(SSS),
                Sexe.Feminin => int.IsEvenInteger(SSS),
                Sexe.NonBinaire => true,
                _ => false
            };

            // Contrôle sur le mois
            bool dateNaissanceCoherente = dateNaissance.Year % 100 == YY
                                          && dateNaissance.Month == MoisReel(MM)
                                          && dateNaissance.Day == DD;

            bool cleCoherente = false;
            if (int.TryParse($"{YY:00}{MM:00}{DD:00}{SSS:000}", out int b))
                CleValide(dateNaissance.Year >= 2000 ? 2_000_000_000L + b : b, CC);

            return sexeCoherent && dateNaissanceCoherente && cleCoherente;
        }
        // ------------------------------------------------------------


        /// <summary>
        /// Retourne le numéro de registre national sous forme d'une chaine formatée "XX.XX.XX-XXX.XX"
        /// </summary>
        public override string ToString() => $"{YY:00}.{MM:00}.{DD:00}-{SSS:000}.{CC:00}";
        public string ToStringBrute() => $"{YY:00}{MM:00}{DD:00}{SSS:000}{CC:00}";
    }
}
