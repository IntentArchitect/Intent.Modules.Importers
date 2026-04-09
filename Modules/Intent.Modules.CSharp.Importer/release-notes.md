### Version 1.0.8

- Improvement: Support around generic types has been improved - the generic type as well as the generic type parameters will now be imported as separate elements, with the correct references to the types where the generic type is used.

### Version 1.0.7

- Improvement: Added option to `Preserve original sync/async method definitions` when importing services, which will add the appropriate `async/sync` stereotypes

### Version 1.0.6

- Fixed: Issue when multiple packages had folders with the same external reference value.

### Version 1.0.5

- Improvement: When importing Interfaces as Services, inherited interfaces are now taken into account and imported onto the Service as well.
- Fixed: Return types correct set on service operations when importing from C# files.
- Fixed: Software Factory will no longer crash after importing C# files without first saving the designer.
- Fixed: Internal refactor to ensure that `Intent.Modules.Importer.FileDirectoryPreviewTask` no longer exists as other importers who have shared code could end up registering the same Task ID and cause a crash.
- Fixed: Removed hard dependency on ElementPersistable when creating the CSharp stereotype.

### Version 1.0.4

- Improvement: Import any C# type as a `Type-Definition` inside a Services or Domain designer.
- Improvement: If element cannot be found by ExternalReference, fall back to searching by name before creating a new element.

### Version 1.0.3

- Improvement: Added ProjectUrl link.

### Version 1.0.2

- Improvement: Updated module documentation to use centralized documentation site.

### Version 1.0.1

- Improvement: Documentation added.

### Version 1.0.0

- Simple support for importing C# files into various profiles
