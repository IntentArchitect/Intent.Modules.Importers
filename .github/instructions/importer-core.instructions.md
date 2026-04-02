---
applyTo: 'Modules/Intent.MetadataSynchronizer/**/*.cs,Modules/Intent.Modules.CSharp.Importer/**/*.cs,Modules/Intent.Modules.Json.Importer/**/*.cs,Modules/Intent.Modules.OpenApi.Importer/**/*.cs,Modules/Intent.Modules.Rdbms.Importer/**/*.cs,Modules/Intent.RelationalDbSchemaImporter.*/**/*.cs'
description: 'Use when editing importer core logic, synchronizers, mergers, visitors, analyzers, task entrypoints, or mapping code. Focus on ExternalReference stability, idempotent re-imports, thin tasks, and behavior-preserving changes.'
---

When editing importer production code in this repository:

- Preserve identifier stability on re-import. Existing elements and associations should be updated in place when matching rules still resolve them.
- Treat `ExternalReference` generation and lookup precedence as compatibility-sensitive behavior. Extend it carefully and add regression tests when changing it.
- Keep task classes focused on request validation, environment setup, logging, and delegation. Business rules belong in factories, visitors, analyzers, synchronizers, or mergers.
- Prefer adding to the existing shared abstractions before creating a parallel implementation, but only when the logic is truly shared across importers.
- Favor small changes that preserve public task contracts and serialized input model shapes unless the change explicitly requires a contract update.
- When deletions are involved, make the `AllowDeletions` or `deleteExtra` path explicit in code and verify both enabled and disabled behavior.
- If a change affects matching or deduplication, consider collision scenarios, repeated imports, manually created elements, and mixed imported/manual models.
- Keep logging diagnostic and actionable. Avoid noisy logs that do not help understand import flow or merge outcomes.