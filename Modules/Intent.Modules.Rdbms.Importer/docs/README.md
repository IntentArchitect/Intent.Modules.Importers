# Intent.Rdbms.Importer

This module adds to the Domain Designer the ability to import / reverse engineer domain models from relational databases such as SQL Server and PostgreSQL.

## Domain Designer

In the `Domain Designer`, right-click on your domain package and select the `Database Import` context menu option.

![Database Import context menu item](images/db-import.png)

Selecting this option will provide you with the following dialog:

![Database Import dialog](images/db-import-dialog.png)

> [!NOTE]
>
> The dialog is organized into **collapsible sections** for a better user experience.

### Connection & Settings Section

#### Connection String

The connection string for the database you wish to import. Supports both SQL Server and PostgreSQL databases.

#### Test Connection

A button to validate that the connection string is valid and can successfully connect to the database server before proceeding with the import.

#### Database Type

Select the type of database you're connecting to:

- **SQL Server** - Microsoft SQL Server databases.
- **PostgreSQL** - PostgreSQL databases.

#### Remember Settings

The dialog resolves remembered settings in: **user-local settings**, **team-shared package metadata**, or not at all.

- **Don't Remember** - Clears the user-local settings file for the current domain package and any team-shared package metadata settings for database import.
- **Only for me (user-local)** - Saves all settings for the current domain package to a user-local file under `%APPDATA%\Intent Architect\Intent.Modules.Rdbms.Importer\v1\`, outside source control. Multiple domain packages can keep independent local settings, and the dialog will prefer that package's local settings on next open.
- **Team-shared metadata** - Saves all settings into package metadata that is typically source-controlled and shared with the team.

- **Team-shared metadata (sanitized connection string, no password)** - Saves the shared metadata variant, but strips any password from the persisted connection string.
- **Team-shared metadata (without connection string)** - Saves shared metadata, excluding the connection string.

> [!NOTE]
> Older package metadata persisted using the previous **All Settings** naming continues to load as the **Team-shared metadata** mode for backward compatibility.
>
> User-local settings files written by earlier versions of this module are also still read, so upgrading does not lose remembered settings. Files are rewritten in the current format the next time you run an import.

### Import Options Section

![Import Options](images/db-import-dialog-import-options.png)

#### Entity Name Convention

This setting controls the naming convention of the entities which will be created in the Domain Designer.

- **Singularized table name** - Entity names will be the SQL table names, singularized. e.g. `Customers` -> `Customer`.
- **Table name, as is** - Entity names will be the SQL table names as is. e.g. `tblColor` -> `tblColor`.

#### Attribute Name Convention

This setting controls how database column names are converted to attribute names in the Domain Designer.

- **Default** - Column names will be converted to PascalCase format. e.g. `FIRST_NAME` -> `FirstName`.
- **Column name, as is** - Attribute names will match the column names exactly as they appear in the database. e.g. `FIRST_NAME` -> `FIRST_NAME`.

#### Apply Table Stereotypes

This setting controls under which conditions Table stereotypes are applied to the Entities. Table stereotypes are used to specify the underlying SQL Table name. Sometimes Entity names may not be directly translatable back to the original table name due to differences in allowable character sets.

- **If They Differ** - Only introduce Table stereotypes if the Entity name does not translate back to the original table name.
- **Always** - Always add explicit table names.

#### Stored Procedure Representations

Choose between using Repository Elements and Repository Operations to represent your Stored Procedures.

- **(Default)** - Use the default representation setting.
- **Stored Procedure Element** - Represent as dedicated elements.

  ![Stored Procedure Element](images/stored-procedure-element.png)

- **Stored Procedure Operation** - Represent as repository operations.

  ![Stored Procedure Operation](images/stored-procedure-operation.png)

- **Stored Procedure Operation mapped to Element** - Represent the stored procedure and the operation as two dedicated elements, with a mapping between them. Best when you want the stored procedure’s external representation to differ from its internal implementation.

  ![Stored Procedure Mapping](images/stored-procedure-mapping.png)

### Filtering Options Section

![Import Filtering](images/db-import-dialog-import-filtering.png)

#### Include Indexes

When checked, the importer will include database indexes in the import. Indexes are represented as stereotypes on the domain entities and their attributes.

#### Import Filter File

Specify a JSON file path **(that may be a relative file path to the Package file being imported into)** with a file browser dialog that assists with importing only certain objects from the database.

The import filter JSON file will be automatically updated with settings chosen on the RDBMS wizard dialogue.

For details on the format of this file, refer to the [Filter File Structure](#filter-file-structure) section of this document.

### Advanced Section

![Advanced](images/db-import-dialog-import-advanced.png)

The Advanced section contains options for fine-tuning the import behavior:

#### Remove Deleted Database Attributes, Indexes and Associations

When checked, the importer will remove attributes, associations, and indexes from your domain model that no longer exist in the database. This helps keep your domain model synchronized with the database schema.

- **Enabled** (default) - Attributes, associations, and indexes that no longer exist in the database will be removed during import.
- **Disabled** - Existing attributes, associations, and indexes will be preserved even if they no longer exist in the database.

> [!NOTE]
> Index removal only occurs when both "Include Indexes" is checked and this option is enabled. If "Include Indexes" is unchecked, indexes are not tracked and therefore not subject to removal.

#### Preserve User-Specified Attribute Types

When checked, the importer will preserve any attribute types that you have manually changed in the domain model, preventing them from being overwritten by the database schema types.

- **Enabled** (default) - Manually specified attribute types will be preserved during import. A warning will be logged when the database type differs from the preserved type.
- **Disabled** - Attribute types will always be updated to match the database schema.

> [!NOTE]
> When "Preserve User-Specified Attribute Types" is enabled and a type change is detected, a warning will be displayed in the import output indicating which attributes were preserved and what the database type would have changed them to.

### Import Selection Screen

After pressing the `NEXT` button, the next screen of the wizard will show a loader as the database is queried and then allow you to interactively select which database objects (tables, views, stored procedures) to include or exclude from the import process through a hierarchical tree view. This provides an intuitive way to create and manage filter files without manually editing JSON.

![Import Selection screen](images/db-import-item-selection-screen.png)

#### Filter Type

Choose how the selection tree should be interpreted:

- **Include Selected** - _Only_ the items selected in the tree view below will be imported.
- **Exclude Selected** - All items _except_ those selected in the tree view below will be imported.

#### Include Dependent Tables

When checked, the importer will automatically also import dependent tables of those that were selected, e.g. a table on which a selected table has a foreign key constraint.

#### Objects to Exclude/Include from/in Import

A tree view of database objects organized by schema, with separate categories for:

- **Tables** - Database tables
- **Views** - Database views
- **Stored Procedures** - Database stored procedures

Items can be checked to specify whether they should be included or excluded depending on the [Filter Type](#filter-type) option selected above.

The filter box can be used to search and filter the items visible in the tree view, which is particularly useful when dealing with databases that have many objects.

### Filter File Structure

The filter file is a JSON file that provides fine-grained control over what gets imported from the database. While the visual selection tree in the wizard provides an easy way to configure filters, you can also manually create or edit these files.

The filter file should follow this JSON structure:

```json
{
  "filter_type": "include",
  "include_dependant_tables": true,
  "include_tables": [
    {
      "name": "dbo.ExistingTableName",
      "exclude_columns": [
        "LegacyColumn"
      ]
    }
  ],
  "include_views": [
    {
      "name": "dbo.ExistingViewName",
      "exclude_columns": [
        "LegacyColumn"
      ]
    }
  ],
  "include_stored_procedures": [
    "dbo.ExistingStoredProcedureName"
  ],
  "exclude_tables": [
    "dbo.LegacyTableName"
  ],
  "exclude_views": [
    "dbo.LegacyViewName"
  ],
  "exclude_stored_procedures": [
    "dbo.LegacyStoredProcedureName"
  ],
  "exclude_table_columns" : [
    "LegacyGlobalColumn"
  ],
  "exclude_view_columns" : [
    "LegacyGlobalColumn"
  ]
}
```

#### Filter File Fields

| JSON Field                  | Description                                                                                                                                                                                                    |
| --------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `filter_type`               | Specifies how the filter should be applied. Valid values: `"include"` (only import selected items) or `"exclude"` (import everything except selected items). Default: `"include"`.                             |
| `schemas`                   | Database schema names to import. If empty, all schemas are imported. If specified, only these schemas are imported.                                                                                            |
| `include_dependant_tables`  | Determines whether foreign key dependent tables of included tables are automatically included (default: `false`). All dependent tables will be included, unless explicitly excluded by `exclude_tables`.       |
| `include_tables`            | Database tables to import. If empty, all tables are imported. If specified, only these tables are imported. Tables should be specified in `schema.name` format. Each table can have specific columns excluded. |
| `include_views`             | Database views to import. If empty, all views are imported. If specified, only these views are imported. Views should be specified in `schema.name` format. Each view can have specific columns excluded.      |
| `include_stored_procedures` | Database stored procedures to import. If empty, all stored procedures are imported. If specified, only these stored procedures are imported. Should be specified in `schema.name` format.                      |
| `exclude_tables`            | Database tables to exclude from import. Include settings take precedence over exclude settings if the same item is found. Should be specified in `schema.name` format.                                         |
| `exclude_views`             | Database views to exclude from import. Include settings take precedence over exclude settings if the same item is found. Should be specified in `schema.name` format.                                          |
| `exclude_stored_procedures` | Database stored procedures to exclude from import. Include settings take precedence over exclude settings if the same item is found. Should be specified in `schema.name` format.                              |
| `exclude_table_columns`     | A list of column names that should be excluded from import if they are found in any table during the import process. Useful for globally excluding audit columns like `CreatedBy`, `ModifiedDate`, etc.        |
| `exclude_view_columns`      | A list of column names that should be excluded from import if they are found in any view during the import process.                                                                                            |

> [!TIP]
> The wizard's visual selection tree automatically manages the filter file for you. Any selections made in the tree view are saved to the specified filter file when you complete the wizard.

## Stored Procedure Imports

Stored procedure import still supports **Inherit Database Settings**, but those inherited values now come from the same layered database-import resolution: user-local settings first, then shared package metadata, then built-in defaults. This preserves compatibility with older metadata-backed imports while allowing personal connection settings to remain local.

### Remember Settings

The stored procedure import dialog keeps its own remembered settings, entirely separate from the Database Import dialog's. It resolves them in the same layers: **user-local settings first**, then **team-shared package metadata**, and finally **built-in defaults**.

- **Don't Remember** - Clears the user-local stored procedure settings file for the current domain package and any team-shared package metadata settings for stored procedure import.
- **Inherit Database Settings** - Falls back to the connection string and database type from the resolved Database Import settings for whatever you leave blank. Anything you do enter takes precedence and is remembered - see [Overriding Inherited Connection Settings](#overriding-inherited-connection-settings). Note that this same blank-field fallback also applies under every other **Remember Settings** option below - see [Connection String and Database Type](#connection-string-and-database-type).
- **Only for me (user-local)** - Saves the connection string, database type and stored procedure representation for the current domain package to a user-local file under `%APPDATA%\Intent Architect\Intent.Modules.Rdbms.Importer\v1\`, outside source control. The file is named `db-import-<hash>.storedprocs.json`, alongside but independent of the database import's `db-import-<hash>.json`.
- **Team-shared metadata** - Saves these settings into package metadata that is typically source-controlled and shared with the team.
- **Team-shared metadata (sanitized connection string, no password)** - Saves the shared metadata variant, but strips any password from the persisted connection string.
- **Team-shared metadata (without connection string)** - Saves shared metadata, excluding the connection string.

Stored procedure names are never remembered; the field starts blank each time the dialog opens.

> [!NOTE]
> Older package metadata persisted using the previous **All Settings** naming continues to load as the **Team-shared metadata** mode for backward compatibility.

### Connection String and Database Type

These two fields may always be left blank when there is something usable to fall back on - that is, whenever the resolved package-level Database Import settings supply a connection string and a database type. This fallback applies no matter which **Remember Settings** option is selected; it is not limited to **Inherit Database Settings**.

- When both can be inherited, the fields show a `(inherited setting)` placeholder and may be left blank.
- When nothing usable could be inherited, both fields become required and are marked with a warning explaining what is missing. Filling them in is always sufficient to continue - you do not have to configure the package-level Database Import settings first.

The **Browse** button resolves the connection the same way the import itself does, so the stored procedure list you browse is always the one the import will actually read from.

### Overriding Inherited Connection Settings

A connection string or database type you enter yourself always wins over the inherited value - inheritance only fills in what you leave blank, and it does so regardless of which **Remember Settings** option is selected.

Under **Remember Settings** options other than **Don't Remember** (including **Inherit Database Settings**, **Only for me** and the **Team-shared metadata** variants), only what you actually typed is remembered - never a value that was merely resolved by falling back to the Database Import settings. This keeps a saved override from silently freezing in place: leaving a field blank keeps tracking later changes to the package-level Database Import settings, no matter which **Remember Settings** option is selected.

**Remember Settings** still reads back as whatever you selected afterwards - everything you did not override continues to follow the Database Import settings.

> [!IMPORTANT]
> While an override is in place it **shadows** the inherited value. Later changes to the Database Import connection string or database type will no longer flow through to that repository.
>
> To go back to pure inheritance, clear the field and run the import again - the override is removed and the repository resumes following the Database Import settings.

### Single-Row Result Sets

When a stored procedure returns a result set, the importer cannot determine whether it will always return a single row or multiple rows. As a result, on the first import, the stored procedure (and any associated repository operations) is modeled with a **collection return type**.

For example, the following stored procedure will be imported with `IsCollection` set to `true`:

```sql
CREATE PROCEDURE [dbo].[sp_SingleRowReturn]
  @InputValueOne int,
  @InputValueTwo int
AS
BEGIN
  SELECT @InputValueOne as 'InputOne', @InputValueTwo as 'InputTwo'
END
```

After import, you can manually update the stored procedure (and any associated operations) to return a single item instead of a collection. If the stored procedure is imported again, the **return type will not be reset to a collection** and will remain at the manually configured value.

## Trigger imports

By default, if a qualifying table has a trigger, it will be imported and modeled as follows:

![Trigger Modelling](images/trigger-import.png)

> [!NOTE]
>
> The actual `trigger` implementation is not modeled in the `Domain Designer`. The `trigger` stereotype is used only to mark to the underlying provider (specifically, Entity Framework Core) that the table has an existing trigger. This allows Entity Framework to correctly generate the appropriate SQL statements.
