# Domus_NET

Démonstrateur d'architecture .NET 10 : portage d'un ERP de gestion immobilière vers une Clean Architecture testable.

## Structure de la solution

| Projet | Rôle |
| --- | --- |
| `FDL.Core` | Noyau métier transverse : `ReferenceDossier`, `Menage`, contrats de dépôt génériques. |
| `FDL.Loc` | Domaine « Locataire » : registre, règles d'éligibilité. |
| `FDL.WF` | Domaine « Workflow » : notes, tâches et documents assignés à un utilisateur ou à un groupe. |
| `*.Tests` | Tests unitaires xUnit du projet homonyme. |

Chaque module suit le même découpage :

- `Domain/` — entités, objets-valeurs et invariants métier. Aucune dépendance vers une technologie.
- `App/` — cas d'usage et *ports* (interfaces de dépôt, objets de filtre). Ne dépend que de `Domain`.
- L'infrastructure (implémentations HFSQL / SQL) vit hors de ces projets et dépend de `App`, jamais l'inverse.

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

### Deux règles complémentaires

**Pas d'accent dans un identifiant.** C# les accepte (`Réassigner` compile), mais aucune API .NET
n'en utilise, et ils se propagent mal dès qu'un outil externe manipule les noms de membres
(sérialiseurs, ORM, générateurs de code). Les accents restent dans les chaînes de caractères et la
documentation : `Reassigner`, jamais `Réassigner`.

**Un retour `bool` impose le préfixe `Try`.** Convention .NET (`int.TryParse`,
`Dictionary.TryGetValue`) : une méthode `Try…` renvoie `false` pour un cas d'échec *attendu*, tandis
qu'une méthode sans ce préfixe lève une exception quand l'opération est impossible. Un `bool`
renvoyé sans le préfixe crée un code retour silencieux, que l'appelant oublie de tester —
`wf.Terminer(id);` compile sans avertissement et masque la violation.

### Documentation du code

Commentaires au format XML de C# (`/// <summary>`).
