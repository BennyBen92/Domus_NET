# Domus_NET

[![CI](https://github.com/BennyBen92/Domus_NET/actions/workflows/ci.yml/badge.svg)](https://github.com/BennyBen92/Domus_NET/actions/workflows/ci.yml)

Démonstrateur d'architecture .NET 10 : portage d'un ERP de gestion immobilière vers une Clean Architecture testable.

## Structure de la solution

| Projet | Rôle |
| --- | --- |
| `FDL.Core` | Noyau métier transverse : `ReferenceDossier`, `Menage`, contrats de dépôt génériques. |
| `FDL.Loc` | Domaine « Locataire » : registre des demandes (logement, PMR, commerce, parking), règles d'éligibilité. |
| `FDL.WF` | Domaine « Workflow » : notes, tâches et documents assignés à un utilisateur ou à un groupe. |
| `FDL.Infra` | Infrastructure : persistance PostgreSQL avec EF Core (lignes, mappers, dépôts, migrations), session de l'utilisateur connecté (`SessionUtilisateur`). |
| `*.Tests` | Tests unitaires xUnit du projet homonyme. |

Chaque module suit le même découpage :

- `Domain/` — entités, objets-valeurs et invariants métier. Aucune dépendance vers une technologie.
- `App/` — cas d'usage et *ports* (interfaces de dépôt, objets de filtre). Ne dépend que de `Domain`.
- L'infrastructure vit dans `FDL.Infra` : elle implémente les ports définis dans `App` et dépend des modules, jamais l'inverse.

```
FDL.Infra ──► FDL.WF  ──► FDL.Core
              FDL.Loc ──► FDL.Core

technique     modules      noyau partagé
              (App → Domain)
```

Le domaine ne connaît donc ni la base de données ni l'interface : il se teste sans elles.

## Persistance

PostgreSQL 17, EF Core 10 et le provider Npgsql.

- **Modèle de persistance séparé du domaine.** Chaque entité a une classe « ligne » (`RegistreRow`, `WorkflowRow`) qui reflète la table, avec des setters publics et des types simples. Un mapper (`RegistreMapper`) fait la conversion dans les deux sens. L'entité du domaine garde ses setters privés et ses invariants.
- **Deux portes d'entrée dans une entité.** Les fabriques (`Registre.PourLogement`, `Workflow.PourNote`…) appliquent les règles métier à la création. `Reconstituer` recharge une entité existante et contrôle l'intégrité des données lues. Une ligne corrompue en base devient une `InvalidDataException` qui cite l'identifiant fautif.
- **Référence de dossier sur une seule colonne.** `2.050.123/61` est stocké sous la forme de l'entier `205012361` (`ReferenceDossierCodec`). Le tri est naturel, la colonne est indexable, et l'affichage reste le rôle de `ToString()`.
- **Contraintes en base.** Les règles essentielles vérifiées par le domaine sont aussi garanties par la base : contraintes `CHECK`, et index unique `ux_registre_reference_dossier` pour « un seul registre par dossier ». Une violation est traduite par le dépôt en `InvalidOperationException`, comme dans le dépôt en mémoire utilisé par les tests.
- **Migrations appliquées par scripts SQL.** Chaque migration EF est exportée en script idempotent (`db/00X_*.sql`), relu puis exécuté par un rôle administrateur dédié. Aucun mot de passe administrateur n'est stocké sur le poste de développement, et l'application se connecte avec un rôle aux droits limités.
- **Audit sur deux niveaux.** L'audit métier (quel utilisateur de l'application a modifié la ligne) sera rempli par l'application, via un intercepteur EF (à venir). L'audit technique (quel rôle PostgreSQL a modifié la ligne, et quand) est rempli par un trigger (table `workflow` pour l'instant), invisible pour EF, et fonctionne même pour une requête manuelle.

## Tests et intégration continue

```
dotnet test
```

Les tests couvrent chaque couche séparément :

- **Domaine** : invariants des entités. Par exemple, un workflow terminé ne peut être ni réassigné ni terminé une seconde fois, et un registre terminé ou radié ne peut plus être modifié. Les codes postaux doivent être bruxellois et sans doublon, et les données reconstituées sont validées.
- **Cas d'usage** : `WorkflowService` et `RegistreService` sont testés avec un dépôt en mémoire et un utilisateur courant simulé (dossiers `Fakes`). L'horloge est contrôlée par `FakeTimeProvider`, ce qui rend les dates vérifiables. Le dépôt en mémoire renvoie des copies, comme une vraie base : un oubli de `Update` fait échouer les tests.
- **Infrastructure** : aller-retour entité → ligne → entité, encodage de la référence de dossier, et rejet des lignes corrompues par une `InvalidDataException` explicite.

GitHub Actions compile la solution et exécute les tests à chaque push et à chaque pull request. La branche `main` n'accepte que des pull requests dont la CI est verte.

## Conventions de nommage

Le code mêle délibérément français et anglais, selon une règle unique :

> **Le vocabulaire métier reste en français. Le vocabulaire technique reste en anglais.**

Le métier est écrit, discuté et validé en français. Traduire
« ménage », « référence de dossier » ou « à signer » romprait la correspondance exacte entre le code
et le langage des utilisateurs — c'est le principe de l'*ubiquitous language* du DDD : un terme
métier porte le même nom dans la conversation, dans la documentation et dans le code.

| En français (métier) | En anglais (technique) |
| --- | --- |
| Entités et objets-valeurs : `Menage`, `ReferenceDossier`, `WorkflowAssignation` | Contrats d'infrastructure : `IReadRepository`, `IWriteRepository` |
| Méthodes métier : `Terminer`, `Radier`, `Reassigner`, `PourGroupe`, `DepuisExistant` | Opérations de dépôt : `GetById`, `GetAll`, `Add`, `Search` |
| États calculés : `EstContrat`, `EstTermine`, `EstRadie`, `EstGroupe` | Membres hérités du framework : `Parse`, `TryParse`, `ToString` |
| Énumérations métier : `WorkflowAction.ASigner`, `TypeRevenu.Pension` | |

En cas de doute : si le terme apparaîtrait tel quel dans une réunion avec un gestionnaire, il
s'écrit en français ; s'il ne parle qu'au développeur, il s'écrit en anglais.

### Documentation du code

Commentaires au format XML de C# (`/// <summary>`).
