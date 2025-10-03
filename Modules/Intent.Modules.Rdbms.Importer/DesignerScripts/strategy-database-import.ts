/// <reference path="./common.ts" />
/// <reference path="./icons.ts" />

class DatabaseImportStrategy {
    public async execute(packageElement: MacroApi.Context.IElementApi): Promise<void> {
        const defaults = this.getDialogDefaults(packageElement);

        const result = await this.presentImportDialog(defaults, packageElement.id);
        if (result == null) {
            return;
        }

        // Handle the save operation when dialog is closed with OK:
        const importModel = JSON.stringify(this.createImportModel(result));
        launchHostedModuleTask("Intent.Modules.Rdbms.Importer.Tasks.DatabaseImport", [importModel], {
            taskName: "Database Import",
            openWindow: true
        });
    }

    private getDialogDefaults(element: MacroApi.Context.IElementApi): ISqlDatabaseImportPackageSettings {
        const domainPackage = element.getPackage();
        const persistedValue = this.getSettingValue(domainPackage, "rdbms-import:typesToExport", "");
        let includeTables = "true";
        let includeViews = "true";
        let includeStoredProcedures = "true";
        let includeIndexes = "true";
        if (persistedValue != null && persistedValue !== "") {
            includeTables = "false";
            includeViews = "false";
            includeStoredProcedures = "false";
            includeIndexes = "false";
            persistedValue.split(";").forEach(i => {
                switch (i.toLocaleLowerCase()) {
                    case 'table':
                        includeTables = "true";
                        break;
                    case 'view':
                        includeViews = "true";
                        break;
                    case 'storedprocedure':
                        includeStoredProcedures = "true";
                        break;
                    case 'index':
                        includeIndexes = "true";
                        break;
                    default:
                        break;
                }
            });
        }

        const result: ISqlDatabaseImportPackageSettings = {
            entityNameConvention: this.getSettingValue(domainPackage, "rdbms-import:entityNameConvention", "SingularEntity"),
            attributeNameConvention: this.getSettingValue(domainPackage, "rdbms-import:attributeNameConvention", "Default"),
            tableStereotypes: this.getSettingValue(domainPackage, "rdbms-import:tableStereotypes", "WhenDifferent"),
            includeTables: includeTables,
            includeViews: includeViews,
            includeStoredProcedures: includeStoredProcedures,
            includeIndexes: includeIndexes,
            importFilterFilePath: this.getSettingValue(domainPackage, "rdbms-import:importFilterFilePath", "db-import-filter.json"),
            connectionString: this.getSettingValue(domainPackage, "rdbms-import:connectionString", null),
            storedProcedureType: this.getSettingValue(domainPackage, "rdbms-import:storedProcedureType", ""),
            settingPersistence: this.getSettingValue(domainPackage, "rdbms-import:settingPersistence", "None"),
            databaseType: this.getSettingValue(domainPackage, "rdbms-import:databaseType", "SqlServer"),
            filterType: this.getSettingValue(domainPackage, "rdbms-import:filterType", "include"),
            allowDeletions: this.getSettingValue(domainPackage, "rdbms-import:allowDeletions", "true"),
            preserveAttributeTypes: this.getSettingValue(domainPackage, "rdbms-import:preserveAttributeTypes", "true")
        };
        return result;
    }

    private async presentImportDialog(defaults: ISqlDatabaseImportPackageSettings, packageId: string): Promise<IFormResult | null> {
        const formConfig: MacroApi.Context.IDynamicFormConfig = {
            title: "RDBMS Import",
            minWidth: "600px",
            height: "80%",
            fields: [],
            sections: [
                {
                    name: "Connection & Settings",
                    fields: [
                        {
                            id: "connectionString",
                            fieldType: "text",
                            label: "Connection String",
                            placeholder: null,
                            hint: null,
                            isRequired: true,
                            value: defaults.connectionString
                        },
                        {
                            id: "databaseType",
                            fieldType: "select",
                            label: "Database Type",
                            placeholder: null,
                            hint: null,
                            isRequired: true,
                            value: defaults.databaseType,
                            selectOptions: [
                                { id: "SqlServer", description: "SQL Server" },
                                { id: "PostgreSQL", description: "PostgreSQL" },
                            ]
                        },
                        {
                            id: "connectionStringTest",
                            fieldType: "button",
                            label: "Test Connection",
                            hint: "Test whether the Connection String is valid to access the Database Server",
                            onClick: async (form: MacroApi.Context.IDynamicFormApi) => {
                                const testConnectionModel: ITestConnectionModel = {
                                    connectionString: form.getField("connectionString").value as string,
                                    databaseType: form.getField("databaseType").value as string
                                };

                                const executionResult = await executeImporterModuleTask(
                                    "Intent.Modules.Rdbms.Importer.Tasks.TestConnection",
                                    testConnectionModel);

                                if ((executionResult.errors ?? []).length > 0) {
                                    form.getField("connectionStringTest").hint = "Failed to connect.";
                                    form.getField("connectionStringTest").hintType = "danger";
                                    await displayExecutionResultErrors(executionResult);
                                } else {
                                    form.getField("connectionStringTest").hint = "Connection established successfully.";
                                    form.getField("connectionStringTest").hintType = "success";
                                }
                            }
                        },
                        {
                            id: "settingPersistence",
                            fieldType: "select",
                            label: "Remember Settings",
                            hint: "Remember these settings for next time you run the import",
                            value: defaults.settingPersistence,
                            selectOptions: [
                                { id: "None", description: "Don't Remember" },
                                { id: "All", description: "All Settings" },
                                { id: "AllSanitisedConnectionString", description: "All (with Sanitized connection string, no password))" },
                                { id: "AllWithoutConnectionString", description: "All (without connection string))" }
                            ]
                        }
                    ],
                    isCollapsed: false,
                    isHidden: false
                },
                {
                    name: "Import Options",
                    fields: [
                        {
                            id: "entityNameConvention",
                            fieldType: "select",
                            label: "Entity name convention",
                            placeholder: "",
                            hint: "",
                            value: defaults.entityNameConvention,
                            selectOptions: [
                                { 
                                    id: "SingularEntity", 
                                    description: "Singularized table name", 
                                    additionalInfo: "(eg. \"Colors\" => \"Color\")" 
                                }, 
                                { 
                                    id: "MatchTable", 
                                    description: "Table name, as is", 
                                    additionalInfo: "(eg. \"tblColor\" => \"tblColor\")"
                                }
                            ]
                        },
                        {
                            id: "attributeNameConvention",
                            fieldType: "select",
                            label: "Attribute Name Convention",
                            placeholder: "",
                            hint: "How column names should be converted to attribute names",
                            value: defaults.attributeNameConvention,
                            selectOptions: [
                                { 
                                    id: "Default", 
                                    description: "Default", 
                                    additionalInfo: "(eg. \"FIRST_NAME\" => \"FirstName\")" 
                                }, 
                                { 
                                    id: "ColumnName", 
                                    description: "Column name, as-is", 
                                    additionalInfo: "(eg. \"FIRST_NAME\" => \"FIRST_NAME\")"
                                }
                            ]
                        },
                        {
                            id: "tableStereotypes",
                            fieldType: "select",
                            label: "Apply Table Stereotypes",
                            placeholder: "",
                            hint: "When to apply Table stereotypes to your domain entities",
                            value: defaults.tableStereotypes,
                            selectOptions: [{ id: "WhenDifferent", description: "If They Differ" }, { id: "Always", description: "Always" }]
                        },
                        {
                            id: "storedProcedureType",
                            fieldType: "select",
                            label: "Stored Procedure Representation",
                            value: defaults.storedProcedureType,
                            selectOptions: [
                                { id: "Default", description: "(Default)" },
                                { id: "StoredProcedureElement", description: "Stored Procedure Element" },
                                { id: "RepositoryOperation", description: "Stored Procedure Operation" }
                            ]
                        }
                    ],
                    isCollapsed: true,
                    isHidden: false
                },
                {
                    name: 'Filtering Options',
                    fields: [
                        {
                            id: "includeIndexes",
                            fieldType: "checkbox",
                            label: "Include Indexes",
                            hint: "If set, the importer will include database indexes in the import.",
                            value: defaults.includeIndexes
                        },
                        {
                            id: "importFilterFilePath",
                            fieldType: "open-file",
                            label: "Import Filter File",
                            hint: "Path to import filter JSON file (see [documentation](https://docs.intentarchitect.com/articles/modules-importers/intent-rdbms-importer/intent-rdbms-importer.html#filter-file-structure))",
                            placeholder: "(optional)",
                            value: defaults.importFilterFilePath,
                            openFileOptions: {
                                fileFilters: [{ name: "JSON", extensions: ["json"] }]
                            },
                            onChange: async (form) => {
                                const selectedFilePath = form.getField("importFilterFilePath").value as string;
                                if (selectedFilePath == null || selectedFilePath === "") {
                                    return;
                                }

                                // Use the new PathResolution task to resolve the path
                                const pathResolutionModel = {
                                    selectedFilePath: selectedFilePath,
                                    applicationId: application.id,
                                    packageId: packageId
                                };

                                const pathResolutionResult = await executeImporterModuleTask(
                                    "Intent.Modules.Rdbms.Importer.Tasks.PathResolution",
                                    pathResolutionModel
                                );

                                if ((pathResolutionResult.errors ?? []).length === 0 && pathResolutionResult.result?.resolvedPath) {
                                    form.getField("importFilterFilePath").value = pathResolutionResult.result.resolvedPath;
                                } else if ((pathResolutionResult.errors ?? []).length > 0) {
                                    await displayExecutionResultErrors(pathResolutionResult);
                                    return;
                                }
                            }
                        }
                    ],
                    isCollapsed: true,
                    isHidden: false
                },
                {
                    name: "Advanced",
                    fields: [
                        {
                            id: "allowDeletions",
                            fieldType: "checkbox",
                            label: "Remove deleted database attributes and associations",
                            hint: "Removes imported attributes and associations that no longer exist in the database",
                            value: defaults.allowDeletions
                        },
                        {
                            id: "preserveAttributeTypes",
                            fieldType: "checkbox",
                            label: "Preserve user-specified attribute types",
                            hint: "If set, the importer will not overwrite any attribute types set by the user.",
                            value: defaults.preserveAttributeTypes
                        }
                    ],
                    isCollapsed: true,
                    isHidden: false
                }
            ],
            pages: [this.presentManageFiltersDialog(packageId, defaults)]
        }

        const capturedInput = await dialogService.openForm(formConfig);
        return capturedInput;
    }

    private createImportModel(capturedInput: IFormResult): IDatabaseImportModel {
        // Always include Tables, Views, and Stored Procedures - filtering is handled by the filter file
        const typesToExport: string[] = ["Table", "View", "StoredProcedure"];
        
        // Index is optional since it's metadata on tables, not a selectable object
        if (capturedInput.includeIndexes === "true") {
            typesToExport.push("Index");
        }

        const domainDesignerId: string = "6ab29b31-27af-4f56-a67c-986d82097d63";

        const importConfig: IDatabaseImportModel = {
            applicationId: application.id,
            designerId: domainDesignerId,
            packageId: element.getPackage().id,
            entityNameConvention: capturedInput.entityNameConvention,
            attributeNameConvention: capturedInput.attributeNameConvention,
            tableStereotype: capturedInput.tableStereotypes,
            typesToExport: typesToExport,
            importFilterFilePath: capturedInput.importFilterFilePath,
            storedProcedureType: capturedInput.storedProcedureType,
            connectionString: capturedInput.connectionString,
            settingPersistence: capturedInput.settingPersistence,
            databaseType: capturedInput.databaseType,
            filterType: capturedInput.filterType ?? "include",
            allowDeletions: capturedInput.allowDeletions === "true",
            preserveAttributeTypes: capturedInput.preserveAttributeTypes === "true"
        };

        return importConfig;
    }

    private presentManageFiltersDialog(packageId: string, defaults: ISqlDatabaseImportPackageSettings): MacroApi.Context.IDynamicFormWizardPageConfig {
            const inclusiveSelection: MacroApi.Context.IDynamicFormFieldConfig = {
                id: "inclusiveSelection",
                fieldType: "tree-view",
                label: "Objects to Include in Import",
                isRequired: false,
                treeViewOptions: {
                    isMultiSelect: true,
                    selectableTypes: [
                        {
                            specializationId: "Database",
                            autoExpand: true,
                            isSelectable: (x) => false
                        },
                        {
                            specializationId: "Schema",
                            autoExpand: true,
                            autoSelectChildren: true,
                            isSelectable: (x) => true,
                        },
                        {
                            specializationId: "Table",
                            autoSelectChildren: true,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "Stored-Procedure",
                            autoSelectChildren: true,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "View",
                            autoSelectChildren: true,
                            isSelectable: (x) => true
                        }
                    ]
                }
            };

            const exclusiveSelection: MacroApi.Context.IDynamicFormFieldConfig = {
                id: "exclusiveSelection",
                fieldType: "tree-view",
                label: "Objects to Exclude from Import",
                isRequired: false,
                isHidden: true, // hidden by default
                treeViewOptions: {
                    isMultiSelect: true,
                    selectableTypes: [
                        {
                            specializationId: "Database",
                            autoExpand: true,
                            isSelectable: (x) => false
                        },
                        {
                            specializationId: "Schema",
                            autoExpand: true,
                            autoSelectChildren: true,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "Table",
                            autoSelectChildren: true,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "Stored-Procedure",
                            autoSelectChildren: true,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "View",
                            autoSelectChildren: true,
                            isSelectable: (x) => true
                        }
                    ]
                }
            };

            

            const includeDependantTablesField: MacroApi.Context.IDynamicFormFieldConfig = {
                id: "includeDependantTables",
                fieldType: "checkbox",
                label: "Include Dependant Tables",
                hint: "When including tables, also include tables that are referenced by foreign keys",
                value: "false",
                isRequired: false
            };

            const formConfig: MacroApi.Context.IDynamicFormWizardPageConfig = {
                onInitialize: async (form) => {
                    try {
                        const connectionString = form.getField("connectionString").value as string;
                        const databaseType = form.getField("databaseType").value as string;
                        const metadata = await this.fetchDatabaseMetadata(connectionString, databaseType);
                        if (metadata == null) {
                            return null;
                        }

                        let importFilterFilePath = form.getField("importFilterFilePath").value as string;
                        if (importFilterFilePath != null && importFilterFilePath !== "") {
                            try {
                                const verifyModel = {
                                    pathToFile: importFilterFilePath,
                                    applicationId: application.id,
                                    packageId: packageId
                                };

                                const verifyResult = await executeImporterModuleTask(
                                    "Intent.Modules.Rdbms.Importer.Tasks.VerifyFilePath",
                                    verifyModel
                                );

                                if ((verifyResult.errors ?? []).length > 0) {
                                    verifyResult.errors.unshift("Import Filter File Path Errors.")
                                    await displayExecutionResultErrors(verifyResult);
                                    return null;
                                }
                            } catch (err) {
                                throw err;
                            }
                        }

                        // Load existing filter data if file path exists
                        let existingFilter: ImportFilterModel | null = null;
                        if (importFilterFilePath != null && importFilterFilePath !== "") {
                            const filterLoadModel = {
                                importFilterFilePath: importFilterFilePath,
                                applicationId: application.id,
                                packageId: packageId
                            };
                            const filterLoadResult = await executeImporterModuleTask(
                                "Intent.Modules.Rdbms.Importer.Tasks.FilterLoad",
                                filterLoadModel
                            );

                            if ((filterLoadResult.errors ?? []).length === 0 && filterLoadResult.result != null) {
                                existingFilter = filterLoadResult.result as ImportFilterModel;
                                // Set this to hidden field, since we will need it later.
                                form.getField("existingImportFilter").value = JSON.stringify(filterLoadResult.result as ImportFilterModel);
                                form.getField("filterType").value = existingFilter.filter_type ?? "include";
                                const include = form.getField("filterType").value === "include";
                                form.getField("inclusiveSelection").isHidden = !include;
                                form.getField("exclusiveSelection").isHidden = include;

                            } else if ((filterLoadResult.errors ?? []).length > 0) {
                                await displayExecutionResultErrors(filterLoadResult);
                                return null;
                            }
                        }
                        
                        // If no existing filter was loaded, use the default filterType to set visibility
                        if (existingFilter == null) {
                            const include = defaults.filterType === "include";
                            form.getField("inclusiveSelection").isHidden = !include;
                            form.getField("exclusiveSelection").isHidden = include;
                        }
                        
                        const distinctSchemas = metadata.schemas;

                        // Create tree nodes with pre-selected states for inclusive filter
                        inclusiveSelection.treeViewOptions.rootNode = {
                            id: "Database",
                            label: "Database",
                            specializationId: "Database",
                            icon: Icons.databaseIcon,
                            children: distinctSchemas.map(schema => {
                                return {
                                    id: `schema.${schema.schemaName}`,
                                    label: schema.schemaName,
                                    specializationId: "Schema",
                                    icon: Icons.schemaIcon,
                                    isSelected: this.isSchemaIncluded(schema.schemaName, existingFilter),
                                    children: this.createSchemaTreeNodes(schema, existingFilter, "include")
                                };
                            })
                        };

                        // Create tree nodes with pre-selected states for exclusive filter
                        exclusiveSelection.treeViewOptions.rootNode = {
                            id: "Database",
                            label: "Database",
                            specializationId: "Database",
                            icon: Icons.databaseIcon,
                            children: distinctSchemas.map(schema => {
                                return {
                                    id: `schema.${schema.schemaName}`,
                                    label: schema.schemaName,
                                    specializationId: "Schema",
                                    icon: Icons.schemaIcon,
                                    isSelected: this.isSchemaExcluded(schema.schemaName, existingFilter),
                                    children: this.createSchemaTreeNodes(schema, existingFilter, "exclude")
                                };
                            })
                        };

                        includeDependantTablesField.value = existingFilter?.include_dependant_tables ? "true" : "false";

                    } catch (error) {
                        throw error;
                    }
                },
                fields: [
                    {
                        id: "existingImportFilter",
                        label: "Hidden Field",
                        fieldType: "text",
                        isHidden: true
                    }, 
                    {
                        id: "filterType",
                        fieldType: "select",
                        label: "Filter Type",
                        value: defaults.filterType,
                        selectOptions: [
                            {
                                id: "include",
                                description: "Include Selected",
                            },
                            {
                                id: "exclude",
                                description: "Exclude Selected",
                            }
                        ],
                        onChange: (api) => {
                            const include = api.getField("filterType").value === "include";
                            api.getField("inclusiveSelection").isHidden = !include;
                            api.getField("exclusiveSelection").isHidden = include;
                        }
                    },
                    includeDependantTablesField, 
                    inclusiveSelection,
                    exclusiveSelection
                ],
                onContinue: async (form) => {
                    const result = form.getValues();
                    const returnedImportFilterFilePath = await this.saveFilterData(result, JSON.parse(result.existingImportFilter), packageId, result.importFilterFilePath);
                    if (returnedImportFilterFilePath != null) {
                        form.getField("importFilterFilePath").value = returnedImportFilterFilePath;
                    }
                }
            };

            return formConfig;
    }

    private async fetchDatabaseMetadata(connectionString: string, databaseType: string): Promise<IDatabaseMetadata | null> {
        // Get database metadata
        const metadataModel: IRetrieveDatabaseObjectsModel = {
            connectionString: connectionString,
            databaseType: databaseType
        };
        const metadataExecutionResult = await executeImporterModuleTask(
            "Intent.Modules.Rdbms.Importer.Tasks.RetrieveDatabaseObjects",
            metadataModel);
        if ((metadataExecutionResult.errors ?? []).length > 0) {
            await displayExecutionResultErrors(metadataExecutionResult);
            return null;
        }

        const metadata = metadataExecutionResult.result as IDatabaseMetadata;
        if (!metadata) {
            await dialogService.error("No database metadata received.");
            return null;
        }

        return metadata;
    }

    private createSchemaTreeNodes(
        metadata: IDatabaseSchema,
        existingFilter: ImportFilterModel | null = null,
        filterType: "include" | "exclude"
    ): MacroApi.Context.ISelectableTreeNode[] {
        const nodes: MacroApi.Context.ISelectableTreeNode[] = [];

        if (metadata.tables.length > 0) {
            nodes.push(this.createCategoryNode(
                metadata.schemaName,
                "tables",
                "Tables",
                "Table",
                Icons.tableIcon,
                metadata.tables,
                existingFilter,
                filterType
            ));
        }

        if (metadata.storedProcedures.length > 0) {
            nodes.push(this.createCategoryNode(
                metadata.schemaName,
                "storedProcedures",
                "Stored Procedures",
                "Stored-Procedure",
                Icons.storedProcIcon,
                metadata.storedProcedures,
                existingFilter,
                filterType
            ));
        }

        if (metadata.views.length > 0) {
            nodes.push(this.createCategoryNode(
                metadata.schemaName,
                "views",
                "Views",
                "View",
                Icons.viewIcon,
                metadata.views,
                existingFilter,
                filterType
            ));
        }

        return nodes;
    }

    private createCategoryNode(
        schemaName: string,
        category: string,
        label: string,
        specializationId: string,
        icon: any,
        items: string[],
        existingFilter: ImportFilterModel | null = null,
        filterType: "include" | "exclude"
    ): MacroApi.Context.ISelectableTreeNode {
        return {
            id: `${schemaName}.${category}`,
            label: label,
            specializationId: specializationId,
            icon: icon,
            isSelected: this.isCategorySelected(schemaName, category, existingFilter, filterType),
            children: items.map(item => ({
                id: `${schemaName}.${category}.${item}`,
                label: item,
                specializationId: specializationId,
                icon: icon,
                isExpanded: this.isItemSelected(schemaName, category, item, existingFilter, filterType),
                isSelected: this.isItemSelected(schemaName, category, item, existingFilter, filterType)
            } as MacroApi.Context.ISelectableTreeNode))
        };
    }

    private getSettingValue(domainPackage: MacroApi.Context.IPackageApi, key: string, defaultValue: string | null): string | null {
        const persistedValue = domainPackage.getMetadata(key);
        return persistedValue ?? defaultValue;
    }

    private isSchemaIncluded(schemaName: string, existingFilter: ImportFilterModel | null): boolean {
        if (existingFilter == null) {
            return false;
        }
        return existingFilter.schemas.includes(schemaName);
    }

    private isSchemaExcluded(schemaName: string, existingFilter: ImportFilterModel | null): boolean {
        if (existingFilter == null) {
            return false;
        }
        // Check if any tables, views, or stored procedures from this schema are in exclude lists
        const excludedTables = existingFilter.exclude_tables?.some(table => table.startsWith(`${schemaName}.`)) ?? false;
        const excludedViews = existingFilter.exclude_views?.some(view => view.startsWith(`${schemaName}.`)) ?? false;
        const excludedStoredProcs = existingFilter.exclude_stored_procedures?.some(sp => sp.startsWith(`${schemaName}.`)) ?? false;
        return excludedTables || excludedViews || excludedStoredProcs;
    }

    private isCategorySelected(
        schemaName: string,
        category: string,
        existingFilter: ImportFilterModel | null,
        filterType: "include" | "exclude"
    ): boolean {
        if (existingFilter == null) {
            return false;
        }

        if (filterType === "include") {
            switch (category) {
                case "tables":
                    return existingFilter.include_tables?.some(x => x.name.includes(`${schemaName}.`));
                case "views":
                    return existingFilter.include_views?.some(x => x.name.includes(`${schemaName}.`));
                case "storedProcedures":
                    return existingFilter.include_stored_procedures?.some(x => x.includes(`${schemaName}.`));
                default:
                    return false;
            }
        } else {
            switch (category) {
                case "tables":
                    return existingFilter.exclude_tables?.some(x => x.includes(`${schemaName}.`));
                case "views":
                    return existingFilter.exclude_views?.some(x => x.includes(`${schemaName}.`));
                case "storedProcedures":
                    return existingFilter.exclude_stored_procedures?.some(x => x.includes(`${schemaName}.`));
                default:
                    return false;
            }
        }
    }

    private isItemSelected(
        schemaName: string,
        category: string,
        item: string,
        existingFilter: ImportFilterModel | null,
        filterType: "include" | "exclude"
    ): boolean {
        if (existingFilter == null) {
            return false;
        }

        const fullItemName = `${schemaName}.${item}`;

        if (filterType === "include") {
            switch (category) {
                case "tables":
                    return existingFilter.include_tables?.some(table => table.name === fullItemName) ?? false;
                case "views":
                    return existingFilter.include_views?.some(view => view.name === fullItemName) ?? false;
                case "storedProcedures":
                    return existingFilter.include_stored_procedures?.includes(fullItemName) ?? false;
                default:
                    return false;
            }
        } else {
            switch (category) {
                case "tables":
                    return existingFilter.exclude_tables?.includes(fullItemName) ?? false;
                case "views":
                    return existingFilter.exclude_views?.includes(fullItemName) ?? false;
                case "storedProcedures":
                    return existingFilter.exclude_stored_procedures?.includes(fullItemName) ?? false;
                default:
                    return false;
            }
        }
    }

    private async saveFilterData(formResult: IFormResult, existingFilter: ImportFilterModel | null, packageId: string, importFilterFilePath: string): Promise<string> {
        try {
            // Extract selections from form result
            const inclusiveSelection = formResult.filterType == "include" ? formResult.inclusiveSelection ?? [] : [];
            const exclusiveSelection = formResult.filterType == "exclude" ? formResult.exclusiveSelection ?? [] : [];

            // Create filter model from selections
            const filterModel: ImportFilterModel = {
                filter_type: formResult.filterType,
                schemas: [],
                include_tables: [],
                include_dependant_tables: formResult.includeDependantTables === "true",
                include_views: [],
                exclude_tables: [],
                exclude_views: [],
                include_stored_procedures: [],
                exclude_stored_procedures: [],
                exclude_table_columns: [],
                exclude_view_columns: []
            };

            // Until we have UI support for this, we will do this
            if (existingFilter) {
                filterModel.exclude_table_columns = [...existingFilter.exclude_table_columns];
                filterModel.exclude_view_columns = [...existingFilter.exclude_view_columns];
            }

            // Process inclusive selections
            inclusiveSelection.forEach((selection: string) => {
                if (selection.startsWith('schema.')) {
                    const schemaName = selection.replace('schema.', '');
                    filterModel.schemas.push(schemaName);
                } else if (selection.includes('.tables.')) {
                    // Extract schema and table name from ID like "schema.tables.tableName"
                    const parts = selection.split('.');
                    console.log(`INCLUDE ${selection} => ${JSON.stringify(parts)}`);
                    if (parts.length >= 3) {
                        const schemaName = parts[0];
                        const tableName = parts[2];
                        const fullTableName = `${schemaName}.${tableName}`;
                        const filterTableModel: FilterTableModel = {
                            name: fullTableName,
                            exclude_columns: existingFilter
                                ? existingFilter.include_tables.filter(x => x.name === fullTableName)[0]?.exclude_columns ?? []
                                : []
                        };
                        filterModel.include_tables.push(filterTableModel);
                    }
                } else if (selection.includes('.views.')) {
                    // Extract schema and view name from ID like "schema.views.viewName"
                    const parts = selection.split('.');
                    if (parts.length >= 3) {
                        const schemaName = parts[0];
                        const viewName = parts[2];
                        const fullViewName = `${schemaName}.${viewName}`;
                        const filterTableModel: FilterTableModel = {
                            name: fullViewName,
                            exclude_columns: existingFilter
                                ? existingFilter.include_views.filter(x => x.name === fullViewName)[0]?.exclude_columns ?? []
                                : []
                        };
                        filterModel.include_views.push(filterTableModel);
                    }
                } else if (selection.includes('.storedProcedures.')) {
                    // Extract schema and stored procedure name from ID like "schema.storedProcedures.spName"
                    const parts = selection.split('.');
                    if (parts.length >= 3) {
                        const schemaName = parts[0];
                        const spName = parts[2];
                        const fullSpName = `${schemaName}.${spName}`;
                        filterModel.include_stored_procedures.push(fullSpName);
                    }
                }
            });

            // Process exclusive selections
            exclusiveSelection.forEach((selection: string) => {
                if (selection.includes('.tables.')) {
                    // Extract schema and table name from ID like "schema.tables.tableName"
                    const parts = selection.split('.');
                    if (parts.length >= 3) {
                        const schemaName = parts[0];
                        const tableName = parts[2];
                        const fullTableName = `${schemaName}.${tableName}`;
                        filterModel.exclude_tables.push(fullTableName);
                    }
                } else if (selection.includes('.views.')) {
                    // Extract schema and view name from ID like "schema.views.viewName"
                    const parts = selection.split('.');
                    if (parts.length >= 3) {
                        const schemaName = parts[0];
                        const viewName = parts[2];
                        const fullViewName = `${schemaName}.${viewName}`;
                        filterModel.exclude_views.push(fullViewName);
                    }
                } else if (selection.includes('.storedProcedures.')) {
                    // Extract schema and stored procedure name from ID like "schema.storedProcedures.spName"
                    const parts = selection.split('.');
                    if (parts.length >= 3) {
                        const schemaName = parts[0];
                        const spName = parts[2];
                        const fullSpName = `${schemaName}.${spName}`;
                        filterModel.exclude_stored_procedures.push(fullSpName);
                    }
                }
            });

            // Determine save path
            let savePath = importFilterFilePath;
            if (!savePath) {
                // Prompt for file name using input dialog
                const fileNameConfig: MacroApi.Context.IDynamicFormConfig = {
                    title: "Save Filter File",
                    fields: [
                        {
                            id: "fileName",
                            fieldType: "text",
                            label: "File Name",
                            placeholder: "filter.json",
                            isRequired: true,
                            value: "filter.json"
                        }
                    ]
                };

                const fileNameResult = await dialogService.openForm(fileNameConfig);
                if (!fileNameResult || !fileNameResult.fileName) {
                    return null;
                }
                // Note: Package directory resolution will be handled by the backend
                savePath = fileNameResult.fileName;
            }

            // Save the filter data
            const saveModel = {
                importFilterFilePath: savePath,
                applicationId: application.id,
                packageId: packageId,
                filterData: filterModel
            };

            const saveResult = await executeImporterModuleTask(
                "Intent.Modules.Rdbms.Importer.Tasks.FilterSave",
                saveModel
            );

            if ((saveResult.errors ?? []).length > 0) {
                await displayExecutionResultErrors(saveResult);
                return null;
            }

            return savePath;
        } catch (error) {
            throw new Error(`Error saving filters: ${error}`);
        }
    }
}

interface ISqlDatabaseImportPackageSettings {
    entityNameConvention: string;
    attributeNameConvention: string;
    tableStereotypes: string;
    includeTables: string;
    includeViews: string;
    includeStoredProcedures: string;
    includeIndexes: string;
    importFilterFilePath: string;
    storedProcedureType: string;
    connectionString: string;
    settingPersistence: string;
    databaseType: string;
    filterType: string;
    allowDeletions: string;
    preserveAttributeTypes: string;
}

interface IDatabaseImportModel {
    applicationId: string;
    designerId: string;
    packageId: string;
    entityNameConvention: string;
    attributeNameConvention: string;
    tableStereotype: string;
    typesToExport: string[];
    importFilterFilePath: string;
    storedProcedureType: string;
    connectionString: string;
    // Ignoring PackageFileName
    settingPersistence: string;
    databaseType: string;
    filterType: string;
    allowDeletions: boolean;
    preserveAttributeTypes: boolean;
}

interface ITestConnectionModel {
    connectionString: string;
    databaseType: string;
}

interface IRetrieveDatabaseObjectsModel {
    connectionString: string;
    databaseType: string;
}

interface IDatabaseMetadata {
    schemas: IDatabaseSchema[];
}

interface IDatabaseSchema {
    schemaName: string;
    tables: string[];
    views: string[];
    storedProcedures: string[];
}

interface ImportFilterModel {
    filter_type: "include" | "exclude",
    schemas: string[];
    include_tables: FilterTableModel[];
    include_dependant_tables: boolean;
    include_views: FilterViewModel[];
    exclude_tables: string[];
    exclude_views: string[];
    include_stored_procedures: string[];
    exclude_stored_procedures: string[];
    exclude_table_columns: string[];
    exclude_view_columns: string[];
}

interface FilterTableModel {
    name: string;
    exclude_columns: string[];
}

interface FilterViewModel {
    name: string;
    exclude_columns: string[];
}

interface IFormResult {
    existingImportFilter: string;
    filterType: "include" | "exclude"
    exclusiveSelection: any[];
    inclusiveSelection: any[];
    includeDependantTables: string;
    databaseType: string;
    settingPersistence: string;
    connectionString: string;
    storedProcedureType: string;
    importFilterFilePath: string;
    tableStereotypes: string;
    entityNameConvention: string;
    attributeNameConvention: string;
    includeIndexes: string;
    includeStoredProcedures: string;
    includeViews: string;
    includeTables: string;
    allowDeletions: string;
    preserveAttributeTypes: string;
}
