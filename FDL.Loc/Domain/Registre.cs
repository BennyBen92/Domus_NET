using FDL.Core.Domain;

namespace FDL.Loc.Domain
{
    public enum RegistreStatut
    {
        Aucun = 0,
        EnCours = 1,
        Terminé = 2,
        Radié = 3
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
        // Props Ids
        public int IdRegistre { get; }
        public ReferenceDossier RefDossier { get; }

        // Qui a créé le Registre
        public AuditInfo Auteur { get; }

        // Props Infos
        public RegistreType Type { get; private set; }
        public RegistreStatut Statut { get; }
        public DateTime DateStatut { get; }
        public string? Commentaire { get; set; }
        public int NbChambresMinimum { get; private set; }
        public int NbChambresMaximum { get; private set; }
        public string[] ListeCommunes { get; private set; }
        public bool SouhaiteAscenseur { get; private set; }
        // ------------------------------------------------------------

        // Constructeur
        private Registre(int idRegistre, ReferenceDossier referenceDossier, RegistreType type, RegistreStatut statut,
                         int nbChambresMin, int nbChambresMax, string[] listeCommunes, bool souhaiteAscenseur,
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
            DateStatut = auteur.Date;
            NbChambresMinimum = nbChambresMin;
            NbChambresMaximum = nbChambresMax;
            ListeCommunes = listeCommunes;
            SouhaiteAscenseur = souhaiteAscenseur;
            Auteur = auteur;
            Commentaire = commentaire;
        }
        // ------------------------------------------------------------


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
                                            string[] listeCommunes, bool souhaiteAscenseur, AuditInfo auteur,
                                            string? commentaire)
        {
            return new(
                idRegistre: 0,
                referenceDossier: referenceDossier,
                type: RegistreType.Logement,
                statut: RegistreStatut.EnCours,
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
                                       string[] listeCommunes, bool souhaiteAscenseur, AuditInfo auteur,
                                       string? commentaire)
        {
            return new(
              idRegistre: 0,
              referenceDossier: referenceDossier,
              type: RegistreType.PMR,
              statut: RegistreStatut.EnCours,
              nbChambresMin: nbChambresMin,
              nbChambresMax: nbChambresMax,
              listeCommunes: listeCommunes,
              souhaiteAscenseur: souhaiteAscenseur,
              auteur: auteur,
              commentaire: commentaire);
        }

        public static Registre PourCommerce(ReferenceDossier referenceDossier, string[] listeCommunes, AuditInfo auteur,
                                            string? commentaire)
        {
            return new(
              idRegistre: 0,
              referenceDossier: referenceDossier,
              type: RegistreType.Commerce,
              statut: RegistreStatut.EnCours,
              nbChambresMin: 0,
              nbChambresMax: 0,
              listeCommunes: listeCommunes,
              souhaiteAscenseur: false,
              auteur: auteur,
              commentaire: commentaire);
        }

        public static Registre PourParking(ReferenceDossier referenceDossier, string[] listeCommunes, AuditInfo auteur,
                                           string? commentaire)
        {
            return new(
              idRegistre: 0,
              referenceDossier: referenceDossier,
              type: RegistreType.Parking,
              statut: RegistreStatut.EnCours,
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
                                            RegistreStatut statut, int nbChambresMin, int nbChambresMax,
                                            string[] listeCommunes, bool souhaiteAscenseur, AuditInfo auteur,
                                            string? commentaire)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idRegistre, nameof(idRegistre));
            if (!Enum.IsDefined(type))
                throw new InvalidDataException($"Registre {idRegistre} : type invalide ({(int)type}).");
            if (!Enum.IsDefined(statut))
                throw new InvalidDataException($"Registre {idRegistre} : statut invalide ({(int)statut}).");

            return new Registre(idRegistre, referenceDossier, type, statut, nbChambresMin, nbChambresMax, listeCommunes, souhaiteAscenseur, auteur, commentaire);
        }

        // ------------------------------------------------------------

        /// <summary>
        /// Valide l'interval du nombre de chambres souhaités :
        /// - Le nombre minimum est zéro (studio),
        /// - Le nombre minimum doit être inférieur ou égale au nombre maximum,
        /// - Pas plus de 6 chambres
        /// </summary>
        public static void NombresChambresSouhaitesValides(int nbChambresMin, int nbChambresMax)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(nbChambresMin);
            ArgumentOutOfRangeException.ThrowIfNegative(nbChambresMax);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(nbChambresMin, nbChambresMax, nameof(nbChambresMin));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(nbChambresMax, 6, nameof(nbChambresMax));
        }

        /// <summary>
        /// Valide que la collection de noms de communes n'est pas nulle et contient au moins un nom non vide.
        /// </summary>Tableau de noms de communes à valider.Lancée si le tableau est null ou vide (aucune commune).Lancée si la liste contient un ou plusieurs noms de communes null, vides ou constitués uniquement d'espaces.
        public static void ListeCommunesValide(string[] listeCommunes)
        {
            if (listeCommunes is null || listeCommunes.Length == 0)
            {
                throw new ArgumentException("La liste des communes ne peut pas être vide (minimum 1 commune).", nameof(listeCommunes));
            }
            if (listeCommunes.Any(s => string.IsNullOrWhiteSpace(s)))
            {
                throw new ArgumentNullException(nameof(listeCommunes), "La liste contiens une ou plusieurs nom de communes vide.");
            }
        }
        // ------------------------------------------------------------


        /// <summary>
        /// Met à jour la liste des communes
        /// </summary>
        public void ChangeListeCommunes(string[] listeCommunes)
        {
            ListeCommunesValide(listeCommunes);
            ListeCommunes = listeCommunes;
        }

        /// <summary>
        /// Met à jour l'intervale du nombre de chambres souhaité
        /// </summary>
        public void ChangeNombresChambresSouhaites(int nbChambresMin, int nbChambresMax)
        {
            NombresChambresSouhaitesValides(nbChambresMin: nbChambresMin, nbChambresMax: nbChambresMax);
            NbChambresMinimum = nbChambresMin;
            NbChambresMaximum = nbChambresMax;
        }

        public void ChangeType(RegistreType nouveauType) => Type = nouveauType;

    }
}
