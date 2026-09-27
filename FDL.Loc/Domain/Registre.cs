using FDL.Core.Domain;

namespace FDL.Loc.Domain
{
    public enum RegistreStatut
    {
        EnCours = 1,
        Termine = 2,
        Radie = 3
    }
    public enum RegistreType
    {
        Logement = 1,
        PMR = 2,
        Commerce = 3,
        Parking = 4
    }
    // ------------------------------------------------------------
    // ------------------------------------------------------------

    // Un Registre est une demande d'aide locative.
    public class Registre
    {
        // Constantes
        public const int NombreChambresMaximumAutorise = 6;
        public const int CodePostalMinimum = 1000;
        public const int CodePostalMaximum = 1210;
        // ----------

        // Props Ids
        public int IdRegistre { get; }
        public ReferenceDossier RefDossier { get; }

        // Qui a créé le Registre
        public AuditInfo Auteur { get; }

        // Props Infos
        public RegistreType Type { get; private set; }
        public RegistreStatut Statut { get; private set; }
        public DateTime DateStatut { get; private set; }
        public string? Commentaire { get; set; } // Le commentaire peut changer sans restriction
        public int NbChambresMinimum { get; private set; }
        public int NbChambresMaximum { get; private set; }
        public IReadOnlyList<int> ListeCommunes { get; private set; }
        public bool SouhaiteAscenseur { get; private set; }
        // ------------------------------------------------------------

        // Constructeur
        private Registre(int idRegistre, ReferenceDossier referenceDossier, RegistreType type, RegistreStatut statut,
                         DateTime dateStatut, int nbChambresMin, int nbChambresMax, int[] listeCommunes, bool souhaiteAscenseur,
                         AuditInfo auteur, string? commentaire)
        {
            // Vérifie que la référence de dossier est celle d'une demande
            if (!referenceDossier.EstDemande)
                throw new ArgumentException("Le registre doit être créé à partir d'une référence de demande.");

            ListeCommunesValide(listeCommunes);
            NombresChambresSouhaitesValides(nbChambresMin: nbChambresMin, nbChambresMax: nbChambresMax);

            IdRegistre = idRegistre;
            RefDossier = referenceDossier;
            Statut = statut;
            Type = type;
            DateStatut = dateStatut;
            NbChambresMinimum = nbChambresMin;
            NbChambresMaximum = nbChambresMax;
            ListeCommunes = Array.AsReadOnly([.. listeCommunes]);
            SouhaiteAscenseur = souhaiteAscenseur;
            Auteur = auteur;
            Commentaire = commentaire;
        }
        // ------------------------------------------------------------


        public bool EstEnCours => Statut == RegistreStatut.EnCours;
        public bool EstTermine => Statut == RegistreStatut.Termine;
        public bool EstRadie => Statut == RegistreStatut.Radie;
        public bool EstLogement => Type == RegistreType.Logement;
        public bool EstPMR => Type == RegistreType.PMR;
        public bool EstCommerce => Type == RegistreType.Commerce;
        public bool EstParking => Type == RegistreType.Parking;
        // ------------------------------------------------------------


        // Fabrique pour créer des instances de Registre
        /// <summary>
        /// Crée un Registre de type Logement avec le statut "en cours"
        /// </summary>
        public static Registre PourLogement(ReferenceDossier referenceDossier, int nbChambresMin, int nbChambresMax,
                                            int[] listeCommunes, bool souhaiteAscenseur, AuditInfo auteur,
                                            string? commentaire)
        {
            return new(
                idRegistre: 0,
                referenceDossier: referenceDossier,
                type: RegistreType.Logement,
                statut: RegistreStatut.EnCours,
                dateStatut: auteur.Date,
                nbChambresMin: nbChambresMin,
                nbChambresMax: nbChambresMax,
                listeCommunes: listeCommunes,
                souhaiteAscenseur: souhaiteAscenseur,
                auteur: auteur,
                commentaire: commentaire);
        }

        /// <summary>
        /// Crée un Registre de type PMR avec le statut "en cours"
        /// </summary>
        public static Registre PourPMR(ReferenceDossier referenceDossier, int nbChambresMin, int nbChambresMax,
                                       int[] listeCommunes, bool souhaiteAscenseur, AuditInfo auteur,
                                       string? commentaire)
        {
            return new(
              idRegistre: 0,
              referenceDossier: referenceDossier,
              type: RegistreType.PMR,
              statut: RegistreStatut.EnCours,
              dateStatut: auteur.Date,
              nbChambresMin: nbChambresMin,
              nbChambresMax: nbChambresMax,
              listeCommunes: listeCommunes,
              souhaiteAscenseur: souhaiteAscenseur,
              auteur: auteur,
              commentaire: commentaire);
        }

        public static Registre PourCommerce(ReferenceDossier referenceDossier, int[] listeCommunes, AuditInfo auteur,
                                            string? commentaire)
        {
            return new(
              idRegistre: 0,
              referenceDossier: referenceDossier,
              type: RegistreType.Commerce,
              statut: RegistreStatut.EnCours,
              dateStatut: auteur.Date,
              nbChambresMin: 0,
              nbChambresMax: 0,
              listeCommunes: listeCommunes,
              souhaiteAscenseur: false,
              auteur: auteur,
              commentaire: commentaire);
        }

        public static Registre PourParking(ReferenceDossier referenceDossier, int[] listeCommunes, AuditInfo auteur,
                                           string? commentaire)
        {
            return new(
              idRegistre: 0,
              referenceDossier: referenceDossier,
              type: RegistreType.Parking,
              statut: RegistreStatut.EnCours,
              dateStatut: auteur.Date,
              nbChambresMin: 0,
              nbChambresMax: 0,
              listeCommunes: listeCommunes,
              souhaiteAscenseur: false,
              auteur: auteur,
              commentaire: commentaire);
        }
        // ------------------------------------------------------------

        /// <summary>
        /// Reconstitue un Registre depuis les données persistées
        /// </summary>
        public static Registre Reconstituer(int idRegistre, ReferenceDossier referenceDossier, RegistreType type,
                                            RegistreStatut statut, DateTime dateStatut, int nbChambresMin, int nbChambresMax,
                                            int[] listeCommunes, bool souhaiteAscenseur, AuditInfo auteur,
                                            string? commentaire)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idRegistre, nameof(idRegistre));
            if (!Enum.IsDefined(type))
                throw new InvalidDataException($"Registre {idRegistre} : type invalide ({(int)type}).");
            if (!Enum.IsDefined(statut))
                throw new InvalidDataException($"Registre {idRegistre} : statut invalide ({(int)statut}).");

            return new Registre(idRegistre, referenceDossier, type, statut, dateStatut, nbChambresMin, nbChambresMax, listeCommunes, souhaiteAscenseur, auteur, commentaire);
        }
        // ------------------------------------------------------------


        // Change le statut du Registre
        private void ChangeStatut(RegistreStatut statut, DateTime date)
        {
            if (!EstEnCours)
                throw new InvalidOperationException("Il n'est pas possible de changer le statut d'un Registre \"Terminé\" ou \"Radié\".");
            if (date == default)
                throw new ArgumentException("La date est obligatoire.", nameof(date));
            DateStatut = date;
            Statut = statut;
        }
        /// <summary>
        /// Change le statut du Registre en "Terminé"
        /// </summary>
        public void Terminer(DateTime date) => ChangeStatut(RegistreStatut.Termine, date);
        /// <summary>
        /// Change le statut du Registre en "Radié"
        /// </summary>
        public void Radier(DateTime date) => ChangeStatut(RegistreStatut.Radie, date);
        // ------------------------------------------------------------


        /// <summary>
        /// Valide l'interval du nombre de chambres souhaités :
        /// - Le nombre minimum est zéro (studio),
        /// - Le nombre minimum doit être inférieur ou égale au nombre maximum,
        /// - Pas plus de 6 chambres
        /// </summary>
        public static void NombresChambresSouhaitesValides(int nbChambresMin, int nbChambresMax)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(nbChambresMin,nameof(nbChambresMin));
            ArgumentOutOfRangeException.ThrowIfNegative(nbChambresMax, nameof(nbChambresMax));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(nbChambresMin, nbChambresMax, nameof(nbChambresMin));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(nbChambresMax, NombreChambresMaximumAutorise, nameof(nbChambresMax));
        }

        /// <summary>
        /// Valide que la collection de communes n'est pas nulle et contient au moins un code postal valide.
        /// </summary>
        public static void ListeCommunesValide(int[] listeCommunes)
        {
            ArgumentNullException.ThrowIfNull(listeCommunes, nameof(listeCommunes));
            if (listeCommunes.Length == 0)
                throw new ArgumentException("La liste des communes ne peut pas être vide.", nameof(listeCommunes));
            // La plage des codes postaux est vérifiée à l'aide de 2 bornes,
            // mais pourrait très bien être fait à l'aide d'une liste officielle (HashSet).
            if (listeCommunes.Any(cp => cp is < CodePostalMinimum or > CodePostalMaximum))
                throw new ArgumentOutOfRangeException(nameof(listeCommunes), "Code postal hors Région bruxelloise.");
            if (listeCommunes.Distinct().Count() != listeCommunes.Length)
                throw new ArgumentException("La liste contient des doublons.", nameof(listeCommunes));
        }
        // ------------------------------------------------------------


        /// <summary>
        /// Met à jour la liste des communes
        /// </summary>
        public void ChangeListeCommunes(int[] listeCommunes)
        {
            if (!EstEnCours)
                throw new InvalidOperationException("Le Registre doit être \"En cours\" pour pouvoir changer la liste des communes.");
            ListeCommunesValide(listeCommunes);
            ListeCommunes = Array.AsReadOnly([.. listeCommunes]);
        }

        /// <summary>
        /// Met à jour l'intervale du nombre de chambres souhaité
        /// </summary>
        public void ChangeNombresChambresSouhaites(int nbChambresMin, int nbChambresMax)
        {
            if (!EstEnCours)
                throw new InvalidOperationException("Le Registre doit être \"En cours\" pour pouvoir changer le nombre de chambres souhaités.");
            NombresChambresSouhaitesValides(nbChambresMin: nbChambresMin, nbChambresMax: nbChambresMax);
            NbChambresMinimum = nbChambresMin;
            NbChambresMaximum = nbChambresMax;
        }

        public void ChangeType(RegistreType nouveauType)
        {
            if (!EstEnCours)
                throw new InvalidOperationException("Le Registre doit être \"En cours\" pour pouvoir changer le type.");
            if (!Enum.IsDefined(nouveauType))
                throw new ArgumentOutOfRangeException(nameof(nouveauType), $"Le type est invalide ({(int)nouveauType}).");
            Type = nouveauType;
        }
    }
}
