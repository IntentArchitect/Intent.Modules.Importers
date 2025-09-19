# Copilot instructions for Intent.Modules.Importers

This repoFeedback welcome: if any workflow or convention above is unclear or incomplete, tell me which importer/module you're working in and I'll refine this file.

## Architecture at a glance (independent parts)
- Core synchronization library: `Intent.MetadataSynchronizer`
  - Entry: `Helpers.Execute(...)` loads `.isln` → application → designer → package (+ referenced packages), then calls `Synchronizer.Execute(...)` and saves.
  - Matching: primarily by `ExternalReference`; remaps IDs and type refs; `StereotypeManagementMode.Merge` by default; deletion only with `deleteExtra=true`.
- JSON importer CLI: `Intent.MetadataSynchronizer.Json.CLI`
  - Parses JSON files into `Persistables` and invokes the core sync against a specified package.
  - Profiles: `DomainDocumentDB` (Domain designer, classes/attributes + DocumentDB PK on root `Id`) and `EventingMessages` (Services designer, messages/properties). `CasingConvention` supports `AsIs`/`PascalCase`.
- OpenAPI importer CLI: `Intent.MetadataSynchronizer.OpenApi.CLI`
  - Imports OpenAPI 3.x into Services designer; routes/paths derive service + operation names; GET list pluralization; duplicate operation name de-dup with numeric suffixes.
- C# importer CLI: `Intent.MetadataSynchronizer.CSharp.CLI`
  - Scans configured source folders and emits domain/services metadata; infers PKs ([Key], `Id`, `{ClassName}Id`) and FKs (`{Other}Id`).
- RDBMS backend CLI (RPC): `Intent.RelationalDbSchemaImporter.CLI`
  - Standalone JSON-RPC-style executable that importer modules call for DB schema extraction.
  - Commands: `import-schema`, `test-connection`, `list-stored-procedures`, `retrieve-database-objects`.
  - Known limitation: PostgreSQL function overloads (first overload only).
- Intent module example (installable module): `Intent.Modules.Rdbms.Importer`
  - An Intent Architect module (packaged via `.imodspec`) that uses the RDBMS backend to populate Domain models. Reference point for module development structure (`DesignerScripts/`, `modelers/`, `Tasks/`, `resources/`, `Intent.Metadata/`).
- TypeScript SDK typings: `TypescriptCore/`
  - TypeScript declaration files (`*.d.ts`) for Intent module scripting (Element Macros, context APIs) used in `DesignerScripts`.

## Developer workflows (per project)
- Build (any project independently): open solution and build the specific project or run `dotnet build` with the project file.
- Tests: `Intent.MetadataSynchronizer.OpenApi.CLI.Tests` has a skipped in-memory smoke test; `dotnet test` runs the suite.
- JSON CLI quickstart:
  - Generate config: `dotnet run --project Intent.MetadataSynchronizer.Json.CLI -- --generate-config-file`
  - Run import: `dotnet run --project Intent.MetadataSynchronizer.Json.CLI -- --config-file .\config.json`
- RDBMS backend CLI quickstart:
  - `dotnet run --project Intent.RelationalDbSchemaImporter.CLI -- import-schema --payload <json>` (called programmatically by modules; see `Commands.cs` for payloads).
  - For connectivity issues, validate with `test-connection` and ensure correct provider in payload.
- RDBMS module (Intent.Modules.Rdbms.Importer):
  - Review `Intent.Rdbms.Importer.imodspec`, `Tasks/`, and `DesignerScripts/` for module wiring.
  - Module calls the RDBMS CLI via `Intent.RelationalDbSchemaImporter.Runner.ImporterTool` and processes the results into the Domain designer.
  - Docs: `docs/README.md` explains UI flow and filter file shape.
- OpenAPI CLI quickstart:
  - `dotnet run --project Intent.MetadataSynchronizer.OpenApi.CLI -- --open-api-specification-file <api.json|url> --isln-file <path>.isln --application-name <App> --package-id <PkgId> [--service-type CQRS|Service]`
- Global tool installs (optional): supported for end-users; add `--ignore-failed-sources` if private feeds block restore.

## Project conventions and gotchas
- Independent execution: each CLI/library can be built and executed independently of the others.
- Intent context (for synchronizers): requires a valid `.isln`, ApplicationName, DesignerName/PackageId, and restored modules; `Helpers.Execute` resolves references via `modules.config` and the modules cache.
- Stable matching: set meaningful `ExternalReference` values (e.g., fully qualified type names, JSON path, OpenAPI route); this preserves idempotence across runs.
- Unknown types: allow initially (`createAttributesWithUnknownTypes=true`), then remap after ID resolution.
- Associations: end IDs are nulled and reloaded to stabilize across saves; deletion only when `deleteExtra=true`.
- JSON profiles: see `Intent.MetadataSynchronizer.Json.CLI/ProfileFactory.cs` for specialization IDs and designer mapping.
- OpenAPI mapping: update `OpenApiPersistableFactory.LoadOpenApiTypeMapping()` for new primitives; operation naming handles pluralization and duplicate suffixing.
- RDBMS backend CLI: RPC-style JSON payloads; uses provider factory; note PostgreSQL overload limitation.
- C# importer: PK/FK heuristics and folder mapping are implemented in `PersistableFactory`; builders under `Builders/` create elements/associations.
- Logging: Serilog to console; `--debug` increases verbosity.
- C# string style: prefer verbatim strings (@"") for quotes; raw string literals for multi-line.

## Extending
- New CLI importer:
  - Produce `Persistables` (elements + associations) using `MetadataLookup` to resolve types.
  - Invoke `Helpers.Execute(...)` with designer/package context.
  - Use stable, source-derived `ExternalReference` values.
- New Intent module (reference: `Intent.Modules.Rdbms.Importer`):
  - Define `.imodspec`, add `Tasks/` for UI/process steps, `DesignerScripts/` for macros, `resources/` as needed.
  - For DB imports, call the RDBMS backend CLI via the runner utility and map results to domain elements.
  - Use `TypescriptCore/*.d.ts` typings when writing DesignerScripts.

Feedback welcome: if any workflow or convention above is unclear or incomplete, tell me which importer/module you’re working in and I’ll refine this file.