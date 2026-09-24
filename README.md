# Domus_NET

[![CI](https://github.com/BennyBen92/Domus_NET/actions/workflows/ci.yml/badge.svg)](https://github.com/BennyBen92/Domus_NET/actions/workflows/ci.yml)

Démonstrateur d'architecture .NET 10 : portage d'un ERP de gestion immobilière vers une Clean Architecture testable.

## Structure de la solution

| Projet | Rôle |
| --- | --- |
| `FDL.Core` | Noyau métier transverse : `ReferenceDossier`, `Menage`, contrats de dépôt génériques. |
| `FDL.Loc` | Domaine « Locataire » : registre, règles d'éligibilité. |
| `FDL.WF` | Domaine « Workflow » : notes, tâches et documents assignés à un utilisateur ou à un groupe. |
| `FDL.Infra` | Infrastructure : correspondance entre lignes de base de données et entités (`WorkflowMapper`), session de l'utilisateur connecté (`SessionUtilisateur`). |
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

## Tests et intégration continue

```
dotnet test
```

Les tests couvrent chaque couche séparément :

- **Domaine** : invariants des entités (un workflow terminé ne peut être ni réassigné ni terminé une seconde fois, validation des données reconstituées…).
- **Cas d'usage** : `WorkflowService` est testé avec un dépôt en mémoire et un utilisateur courant simulé (`FDL.WF.Tests/Fakes`). L'horloge est contrôlée par `FakeTimeProvider`, ce qui rend les dates vérifiables. Le dépôt en mémoire renvoie des copies, comme une vraie base : un oubli de `Update` fait échouer les tests.
- **Infrastructure** : aller-retour entité → ligne → entité, et rejet des lignes corrompues par une `InvalidDataException` explicite.

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
| Méthodes métier : `Terminer`, `Reassigner`, `PourGroupe`, `DepuisExistant` | Opérations de dépôt : `GetById`, `GetAll`, `Add`, `Search` |
| États calculés : `EstContrat`, `EstTermine`, `EstGroupe` | Membres hérités du framework : `Parse`, `TryParse`, `ToString` |
| Énumérations métier : `WorkflowAction.ASigner`, `TypeRevenu.Pension` | |

En cas de doute : si le terme apparaîtrait tel quel dans une réunion avec un gestionnaire, il
s'écrit en français ; s'il ne parle qu'au développeur, il s'écrit en anglais.

### Documentation du code

Commentaires au format XML de C# (`/// <summary>`).
