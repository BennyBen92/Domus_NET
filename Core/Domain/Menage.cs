using System;
using System.Collections.Generic;
using System.Text;

namespace FDL.Core.Domain
{
    public class Menage
    {
        public int NombrePersonnes { get; set; }
        public List<RevenuAnnuel> Revenus { get; }

        public Menage()
        {
            NombrePersonnes = 0;
            Revenus = new();
        }

        public decimal RevenuTotal() => Revenus.Sum(revenu => revenu.Montant);
        
    }
}
