using System;
using System.Collections.Generic;
using System.Text;

namespace FDL.Loc.Domain
{
    public enum ERegistreStatut
    {
        Aucun = 0,
        EnCours = 1,
        Terminé = 2,
        Radié = 3
    }
    public enum ERegistreType
    {
        Logement = 1,
        PMR = 2,
        Commerce = 3,
        Parking = 4
    }
    internal class Registre
    {
        public int IdRegistre { get; private set; }
        public int IdContrat { get; private set; }
        public ERegistreStatut Statut { get; set; }
        public ERegistreType Type { get; set; }
        public DateTime DateStatut { get; private set; }
        public DateTime DateCreation { get; private set; }
        public int IdUserCreation { get; private set; }
        public DateTime DateUpdate { get; private set; }
        public int IdUserUpdate { get; private set; }

        /**
         * Constructeur par défaut
         */
        public Registre()
        {
            Statut = ERegistreStatut.Aucun;
            Type = ERegistreType.Logement;
            DateStatut = DateTime.Now;
            DateCreation = DateTime.Now;
            DateUpdate = DateTime.Now;
            IdUserCreation = 0;
            IdUserUpdate = 0;
        }

        
    }
}
