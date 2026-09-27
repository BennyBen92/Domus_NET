using FDL.Core.Domain;
using FDL.Loc.Domain;

namespace FDL.Infra.Persistance
{
    internal sealed class RegistreMapper
    {
        // Vers le Domain
        public static Registre ToDomain(RegistreRow r)
        {
            try
            {
                return Registre.Reconstituer(
                    idRegistre: r.IdRegistre,
                    referenceDossier: ReferenceDossierCodec.FromInt(r.ReferenceDossier),
                    type: (RegistreType)r.Type,
                    statut: (RegistreStatut)r.Statut,
                    dateStatut: r.DateStatut,
                    nbChambresMin: r.NbChambresMin,
                    nbChambresMax: r.NbChambresMax,
                    listeCommunes: r.ListeCommunes,
                    souhaiteAscenseur: r.SouhaiteAscenseur,
                    auteur: new AuditInfo(r.IdUserCreation, r.DateCreation),
                    commentaire: r.Commentaire);
            }
            catch (Exception e) when (e is ArgumentException)
            {
                throw new InvalidDataException($"Registre {r.IdRegistre} : données invalides en base.", e);
            }
        }
        // ------------------------------------------------------------

        // Vers le Repo
        public static RegistreRow ToRow(Registre registre) => new()
        {
            IdRegistre = registre.IdRegistre,
            ReferenceDossier = ReferenceDossierCodec.ToInt(registre.RefDossier),
            Type = (int)registre.Type,
            Statut = (int)registre.Statut,
            DateStatut = registre.DateStatut,
            NbChambresMin = registre.NbChambresMinimum,
            NbChambresMax = registre.NbChambresMaximum,
            ListeCommunes = [.. registre.ListeCommunes],
            SouhaiteAscenseur = registre.SouhaiteAscenseur,
            IdUserCreation = registre.Auteur.IdUser,
            DateCreation = registre.Auteur.Date,
            Commentaire = registre.Commentaire
        };
    }
}
