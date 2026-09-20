using FDL.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

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

    // Un Registre est une demande d'aide locative.
    public class Registre
    {
        public int IdRegistre { get; }
        public ReferenceDossier RefDossier { get; }

        public RegistreStatut Statut { get; }
        public RegistreType Type { get; }

        public DateTime DateStatut { get; }
        public DateTime DateCreation { get; }
        public int IdUserCreation { get; }
        public DateTime DateUpdate { get; }
        public int IdUserUpdate { get; }

        public Registre(ReferenceDossier referenceDossier, RegistreType type, RegistreStatut statut)
        {
            // Vérifie que la référence de dossier est une demande
            if (!referenceDossier.EstDemande)
                throw new ArgumentException("Le registre doit être créé à partir d'une référence de demande.");

            RefDossier = referenceDossier;
            Statut = statut;
            Type = type;

            DateCreation = DateTime.Now;
            DateStatut = DateCreation;
            DateUpdate = DateCreation;

            IdUserCreation = 0;
            IdUserUpdate = 0;
        }

    }
}
