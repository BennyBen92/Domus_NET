using FDL.Core.Domain;
using FDL.Loc.App;
using FDL.Loc.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FDL.Infra.Persistance
{
    internal sealed class RegistreRepository(DomusDbContext db) : IRegistreRepository
    {
        public Registre? GetById(int id)
        {
            RegistreRow? rr = db.Registres
                .AsNoTracking()
                .FirstOrDefault(r => r.IdRegistre == id);

            return rr is not null ? RegistreMapper.ToDomain(rr) : null;
        }

        public IEnumerable<Registre> GetAll()
        {
            return db.Registres
                .AsNoTracking()
                .AsEnumerable()                     // SQL exécuté ici
                .Select(RegistreMapper.ToDomain)
                .ToList();
        }

        public Registre? GetByReferenceDossier(ReferenceDossier reference)
        {
            int referenceRecherchee = ReferenceDossierCodec.ToInt(reference);
            RegistreRow? rr = db.Registres
                .AsNoTracking()
                .FirstOrDefault(r => r.ReferenceDossier == referenceRecherchee);

            return rr is not null ? RegistreMapper.ToDomain(rr) : null;
        }



        public int Add(Registre entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            RegistreRow r = RegistreMapper.ToRow(entity);
            try
            {
                db.Registres.Add(r);
                db.SaveChanges();
                return r.IdRegistre;
            }
            catch (DbUpdateException e) when (e.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                throw new InvalidOperationException($"Un registre existe déjà pour la référence dossier {entity.RefDossier}.", e);
            }
            finally
            {
                db.ChangeTracker.Clear();
            }
        }

        public void Update(Registre entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            try
            {
                db.Registres.Update(RegistreMapper.ToRow(entity));
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new InvalidOperationException($"Le registre {entity.IdRegistre} n'existe pas.", e);
            }
            finally
            {
                db.ChangeTracker.Clear();
            }
        }
    }
}
