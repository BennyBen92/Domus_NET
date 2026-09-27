# Migrations EF → scripts SQL PostgreSQL

## Le principe en une ligne

```text
Code C# (Row + Configuration)  →  migration .cs  →  script .sql  →  base PostgreSQL
        ce que TU modifies        dotnet ef ...     dotnet ef ...    pgAdmin (domus_admin)
```

EF compare le **modèle C# actuel** avec le **snapshot** (`DomusDbContextModelSnapshot.cs`, la photo
du modèle à la dernière migration). La différence devient une migration. EF ne regarde **jamais**
la vraie base pour ça.

## Quand faut-il une migration ?

Chaque fois que la **forme de la base** change :

- nouvelle table (nouveau `XxxRow` + `DbSet` + `XxxRowConfiguration`) ;
- colonne ajoutée, supprimée, renommée ou changée de type dans un `Row` ;
- index, contrainte `CHECK`, index unique, clé ajoutés ou modifiés dans une `Configuration` ;
- SQL que EF ne sait pas générer (trigger, fonction) → migration vide remplie à la main (voir plus bas).

**Pas** de migration pour : domaine, mapper, repository, service, tests.

## Les étapes, dans l'ordre

Toutes les commandes depuis la **racine du dépôt**.

| # | Quand | Quoi |
|---|---|---|
| 1 | Après avoir modifié `Row` / `Configuration` | `dotnet build` : la solution doit compiler |
| 2 | Juste après | **Générer la migration** (ne touche pas la base) |
| 3 | Juste après | **Relire** `Up` / `Down` dans le `.cs` généré |
| 4 | Migration relue | **Générer le script SQL** de cette migration uniquement |
| 5 | Script généré | **Relire** le `.sql` |
| 6 | Script relu | **Exécuter** le script dans pgAdmin, connecté en `domus_admin` |
| 7 | Script exécuté | **Vérifier** en base |
| 8 | Tout est vert | **Committer ensemble** : migration `.cs` + `.Designer.cs` + snapshot + script `.sql` |

### 2. Générer la migration

```text
dotnet ef migrations add NomDeLaMigration --project FDL.Infra --startup-project FDL.Infra --output-dir Persistance/Migrations
```

- Nom en PascalCase qui dit **ce qui change** : `CreationRegistre`, `RegistreReferenceUnique`.
- Crée 2 fichiers (`<date>_Nom.cs`, `<date>_Nom.Designer.cs`) et met à jour le snapshot.

### 3. Relire la migration

- `Up` = ce qui sera appliqué ; `Down` = comment annuler.
- Vérifier qu'il n'y a **que** ce que tu voulais (pas de `DropColumn` surprise).
- Migration vide alors que tu attendais un changement → le build n'a pas pris ta modif, ou elle
  est dans une classe non chargée par le `DbContext`.

### 4. Générer le script SQL

```text
dotnet ef migrations script MigrationPrecedente NomDeLaMigration --idempotent --project FDL.Infra --startup-project FDL.Infra -o db/00X_nom.sql
```

- `MigrationPrecedente` = la **dernière migration déjà scriptée** (voir le tableau en bas, ou
  `dotnet ef migrations list --project FDL.Infra --startup-project FDL.Infra`).
- Le script contient alors **seulement** la nouvelle migration.
- Numéro `00X` = numéro suivant dans `db/`.
- Toute première migration : pas de `MigrationPrecedente`
  (`dotnet ef migrations script --idempotent ... -o db/001_creation_workflow.sql`).
- `--idempotent` : chaque bloc vérifie `__EFMigrationsHistory` → relancer le script ne fait rien de plus.
- Le script est dans une transaction : tout ou rien.

### 6. Exécuter dans pgAdmin

- Se connecter en **`domus_admin`** (pas `pgadmin`) : les `ALTER DEFAULT PRIVILEGES` ne s'appliquent
  qu'aux tables créées par `domus_admin`. Créée par `pgadmin`, la table serait inaccessible à `domus_app`.
- Ouvrir le `.sql`, exécuter.

### 7. Vérifier

```sql
SELECT "MigrationId" FROM "__EFMigrationsHistory" ORDER BY 1;   -- la nouvelle migration est listée
```

Puis contrôler l'objet créé (table, colonne, index) dans l'arborescence pgAdmin, et lancer
l'application ou un test d'intégration.

**Pourquoi un script plutôt que `dotnet ef database update`** : aucun mot de passe admin stocké
sur le PC, et le SQL est relu avant exécution.

## Corriger une migration

| Situation | Quoi faire |
|---|---|
| **Pas encore exécutée** sur la base | `dotnet ef migrations remove --project FDL.Infra --startup-project FDL.Infra` (supprime la dernière migration et remet le snapshot), corriger le code, refaire les étapes 2 à 5 en **écrasant** le même fichier `.sql` |
| **Déjà exécutée** sur la base | Ne jamais la modifier : nouvelle migration (étapes 1 à 8) |

## Migrations existantes

| Migration | Script | Contenu |
| --- | --- | --- |
| `CreationWorkflow` | `db/001_creation_workflow.sql` | Table `workflow`, identity, 2 contraintes CHECK, 2 index partiels |
| `AuditTechniqueDb` | `db/002_audit_technique_db.sql` | Colonnes `date_maj_db`, `role_maj_db` + fonction et trigger |
| `CreationRegistre` | `db/003_creation_registre.sql` | Table `registre`, identity, index `ix_registre_reference_dossier` |
| `RegistreReferenceUnique` | `db/004_registre_reference_unique.sql` | Index remplacé par l'index unique `ux_registre_reference_dossier` |

## Installer l'outil (une fois par dépôt)

`dotnet-ef` est un **outil**, pas une bibliothèque : il ne s'ajoute pas comme paquet NuGet d'un projet.

```text
dotnet new tool-manifest          # crée dotnet-tools.json (à committer)
dotnet tool install dotnet-ef     # enregistre la version dans le manifeste
dotnet tool restore               # sur une autre machine / en CI
dotnet ef --version
```

Garder `dotnet-ef` à la même version que les paquets EF :
`dotnet tool update dotnet-ef --version 10.0.12`

## Migration « SQL brut » (ce qu'EF ne sait pas faire)

Créer une migration vide (le modèle n'a pas changé), puis remplir `Up`/`Down` :

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<DateTime>(name: "date_maj_db", table: "workflow",
        type: "timestamptz", nullable: false, defaultValueSql: "now()");
    migrationBuilder.AddColumn<string>(name: "role_maj_db", table: "workflow",
        type: "text", nullable: false, defaultValueSql: "current_user");

    migrationBuilder.Sql(@"
        CREATE FUNCTION audit_technique() RETURNS trigger LANGUAGE plpgsql AS $$
        BEGIN
            NEW.date_maj_db := now();
            NEW.role_maj_db := current_user;
            RETURN NEW;
        END $$;");

    migrationBuilder.Sql(@"
        CREATE TRIGGER trg_workflow_audit_technique
        BEFORE INSERT OR UPDATE ON workflow
        FOR EACH ROW EXECUTE FUNCTION audit_technique();");
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql(@"
        DROP TRIGGER trg_workflow_audit_technique ON workflow;
        DROP FUNCTION audit_technique();
        ALTER TABLE workflow DROP COLUMN date_maj_db, DROP COLUMN role_maj_db;");
}
```

- Colonnes **absentes de `WorkflowRow`** → EF ne les lit ni ne les écrit, et ne les supprimera pas
  (il compare ses modèles, pas la base réelle).
- `audit_technique()` est réutilisable : pour une nouvelle table, ajouter les 2 colonnes + un `CREATE TRIGGER`.

## Erreurs rencontrées (à revérifier à chaque fois)

- `timestampz` → le bon type est `timestamptz`.
- Noms de colonnes différents entre `AddColumn` et la fonction du trigger → chaque INSERT échoue.
- Copier-coller `NEW.role_maj_db := now();` au lieu de `date_maj_db` → **aucune erreur**, mais la
  date ne change jamais. Toujours tester : insérer, attendre, modifier, relire.
- Index unique sur une table qui contient déjà des doublons → le script échoue (transaction annulée) :
  nettoyer les doublons d'abord.
