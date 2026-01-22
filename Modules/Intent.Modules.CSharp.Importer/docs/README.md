# Intent.CSharp.Importer

Intent Architect’s C# importer pulls existing `.cs` files into Domain, Services, and Eventing designers by reverse-engineering metadata from source.

## Quick start

- Right-click the Domain, Services, or Eventing package (or a folder inside it).
- Choose `Import from C# Files`.
- Complete the two-step wizard to run the import.

Repeated runs track external references (`Namespace.TypeName`) so updated files refresh existing elements while untouched content stays as-is until you remove it.

## Step 1 · Source settings

![Import dialog](images/import-dialog.png)

- **Source Folder**: Root path scanned for C# files; relative structure is recreated in the target package.
- **Profile**: Chooses the specialization to create. Options depend on the current designer and installed modules (see *Profiles* below).
- **File Pattern**: Glob filter (default `**/*.cs`). Examples: `src/**/*.cs`, `Models/*.cs`, `**/Dto*.cs`.
- **Target Folder**: Auto-selected from the node you launched the wizard on; otherwise defaults to the package root.

## Step 2 · Pick files

![File selection dialog](images/file-selection-dialog.png)

- Tree view lists folders and files matching the pattern; whole folders can be toggled at once.
- Deselect items you do not want to analyse; only selected files are processed.
- Paths are tracked internally so re-imports reconcile by reference rather than duplicating content.

## Profiles

Profiles map Roslyn symbols to Intent elements and may pull in dependent artefacts such as DTOs.

### Universal

- **All Types as Type Definition** (`all-types-as-type-definition`): Imports all C# types (classes, interfaces, enums) as `Type-Definition` elements without members. Each definition is annotated with a C# stereotype containing the detected namespace, enabling later specialization into specific types.

### Domain designer

- **Classes** (`domain-classes`): Entities and records with attributes, associations, and inheritance.
- **Domain Events** (`domain-events`): Events plus supporting contracts (requires Domain Events module).
- **Domain Contracts** (`domain-contracts`): Data contracts with hierarchies intact.
- **Enums Only** (`domain-enums`): Enum types and their literals.

### Services designer

- **Services** (`services-services`): Services or service contracts; methods become operations, parameters import DTOs.
- **DTOs** (`services-dtos`): DTOs including inheritance and collections.
- **Commands** (`services-commands`) and **Queries** (`services-queries`): CQRS artefacts (requires Services CQRS module); depend on DTO imports.
- **Enums Only** (`services-enums`): Enums without extra items.

### Eventing designer

- **Eventing Messages** (`eventing-integration-messages`): Messages plus body DTOs.
- **Integration Commands** (`eventing-integration-commands`): Integration commands with DTO support.
- **Integration DTOs** (`eventing-dtos`): Integration DTOs.
- **Enums Only**: Reuses the Services enum profile.

## Import behaviour

- **Roslyn analysis** combines partial types and recognises classes, records, interfaces, and enums.
- **Folder scaffolding** mirrors relative paths beneath the source folder.
- **Associations & dependencies** detect related types, set multiplicity/nullability (e.g. `ICollection<T>` → collection association).
- **Inheritance** keeps base types and interface hierarchies intact.
- **Constructors & methods** (where supported) capture signatures, async flags, returns, and generics.
- **Enums** carry literal values when explicitly assigned.
- **Type resolution** maps known primitives; unknown types become `Type-Definition` placeholders for later mapping.
- **C# stereotype** (when using the "All Types as Type Definition" profile) annotates each element with the source C# namespace, enabling type-safe reference resolution and later conversion to specialized types.

## Tips

- Import DTOs or other dependencies first, or rely on profile dependencies to scaffold them.
- Use glob patterns to skip generated or test files.
- Review associations and unresolved types after import to confirm the model matches your intent.

