/// <reference path="./common.ts" />
/// <reference path="./icons.ts" />

class StoredProceduresImportStrategy {
    private static readonly inheritSettingsMessage = "Connection string could not be determined. Please enter a connection string, or rerun the Database Import process and ensure the connection string is persisted.";

    // The host renders a field hint as "md-input-container .hint", which is absolutely positioned at
    // bottom: 7px over space reserved for a single line - so a hint that wraps grows upwards over the
    // input itself. Keep these on one line (the longest that renders correctly here is ~70 chars);
    // inheritSettingsMessage carries the full explanation where there is room for it.
    private static readonly noInheritedConnectionStringHint = "No inherited connection string - please enter one.";
    private static readonly noInheritedDatabaseTypeHint = "No inherited database type - please select one.";

    // Resolved once the dialog is open (see presentImportDialog's onInitialize) so that the
    // settings-resolution module task is never invoked while no dialog is showing.
    // inheritedSettings is the package-level Database Import fallback; repositorySettings is the
    // repository-level remembered/resolved fallback (e.g. a previously saved "Only for me" value).
    // Either one being usable is enough to leave a field blank - see hasInheritableConnectionSettings.
    private inheritedSettings: IInheritedDatabaseImportSettings | null = null;
    private repositorySettings: IInheritedDatabaseImportSettings | null = null;

    public async execute(repositoryElement: MacroApi.Context.IElementApi): Promise<void> {
        let capturedInput = await this.presentImportDialog(repositoryElement);
        if (capturedInput == null) {
            return;
        }

        launchHostedModuleTask("Intent.Modules.Rdbms.Importer.Tasks.StoredProcedureImport", [JSON.stringify(this.createImportModel(capturedInput))]);
    }


    private async presentImportDialog(repositoryElement: MacroApi.Context.IElementApi): Promise<any> {
        let formConfig: MacroApi.Context.IDynamicFormConfig = {
            title: "RDBMS Import",
            onInitialize: async (form) => {
                let persistedSettings: IRepositoryImportSettingsResolutionResult;
                try {
                    persistedSettings = await this.getPersistedSettings(repositoryElement);
                } catch (error) {
                    this.inheritedSettings = null;
                    this.repositorySettings = null;
                    await dialogService.error(`Unable to load persisted stored procedure import settings: ${error}`);
                    // settingPersistence is still unset here, so both connection fields stay required.
                    this.applyConnectionRequirements(form);
                    return;
                }

                this.inheritedSettings = {
                    connectionString: persistedSettings.inheritedConnectionString,
                    databaseType: persistedSettings.inheritedDatabaseType
                };
                this.repositorySettings = {
                    connectionString: persistedSettings.connectionString,
                    databaseType: persistedSettings.databaseType
                };

                form.getField("connectionString").value = persistedSettings.connectionString;
                form.getField("databaseType").value = persistedSettings.databaseType;
                form.getField("storedProcedureType").value = persistedSettings.storedProcedureType;
                form.getField("settingPersistence").value = persistedSettings.settingPersistence;

                this.applyConnectionRequirements(form);
            },
            fields: [
                {
                    id: "connectionString",
                    fieldType: "text",
                    label: "Connection String",
                    // Defaults match the required state below; applyConnectionRequirements relaxes both.
                    placeholder: "Enter a connection string",
                    hint: null,
                    isRequired: true,
                    onChange: (form) => this.applyConnectionRequirements(form)
                },
                {
                    id: "databaseType",
                    fieldType: "select",
                    label: "Database Type",
                    isRequired: true,
                    onChange: (form) => this.applyConnectionRequirements(form),
                    selectOptions: [
                        // Not "(default...)": the import task has no fallback and fails on a missing
                        // database type, so blank is only ever resolved by inheriting.
                        { id: "", description: "(inherited setting)" },
                        { id: "SqlServer", description: "SQL Server" },
                        { id: "PostgreSQL", description: "PostgreSQL" },
                    ]
                },
                {
                    id: "storedProcedureType",
                    fieldType: "select",
                    label: "Stored Procedure Representation",
                    selectOptions: [
                        // This one genuinely has a default (StoredProcedureType.Default) and is never
                        // inherited from the Database Import settings.
                        { id: "", description: "(default)" },
                        { id: "StoredProcedureElement", description: "Stored Procedure Element" },
                        { id: "RepositoryOperation", description: "Stored Procedure Operation" },
                        { id: "RepositoryOperationMapping", description: "Stored Procedure Element mapped to Operation Element" }
                    ]
                },
                {
                    id: "storedProcNames",
                    fieldType: "text",
                    label: "Stored Procedure Names",
                    placeholder: "Enter Stored Procedure names (comma-separated) or use Browse button",
                    hint: "Enter Stored procedure names (comma-separated) or use the browse button.",
                    isRequired: true
                },
                {
                    id: "storedProcBrowse",
                    fieldType: "button",
                    label: "Browse",
                    onClick: async (form: MacroApi.Context.IDynamicFormApi) => {
                        const connectionStringValue = form.getField("connectionString").value as string;
                        const databaseTypeValue = form.getField("databaseType").value as string;

                        const resolved = this.resolveConnectionSettings(connectionStringValue, databaseTypeValue);

                        const validationError = this.getConnectionValidationError(resolved);
                        if (validationError != null) {
                            await dialogService.error(validationError);
                            return;
                        }

                        let storedProcNames = form.getField("storedProcNames").value as string;
                        let capturedStoredProcs = (storedProcNames ?? "").split(",").map(x => x.trim());

                        const selectedProcs = await this.openStoredProcedureBrowseDialog(resolved.connectionString, resolved.databaseType, capturedStoredProcs);
                        if (selectedProcs.length > 0) {
                            const storedProcNamesField = form.getField("storedProcNames");
                            storedProcNamesField.value = selectedProcs.join(", ");
                        }
                    }
                },
                {
                    id: "settingPersistence",
                    fieldType: "select",
                    label: "Remember Settings",
                    hint: "Remember these settings for next time you run the import",
                    onChange: (form) => this.applyConnectionRequirements(form),
                    selectOptions: [
                        { id: "None", description: "Don't Remember" },
                        { id: "InheritDb", description: "Inherit Database Settings" },
                        { id: "UserLocal", description: "Only for me (user-local)" },
                        { id: "SharedMetadata", description: "Team-shared metadata" },
                        { id: "SharedMetadataSanitisedConnectionString", description: "Team-shared metadata (sanitized connection string, no password)" },
                        { id: "SharedMetadataWithoutConnectionString", description: "Team-shared metadata (without connection string)" }
                    ]
                }
            ]
        }

        let capturedInput = await dialogService.openForm(formConfig);
        return capturedInput;
    }

    private createImportModel(capturedInput: any): IStoredProceduresImportModel {
        const storedProcNamesArray = capturedInput.storedProcNames.split(',').map((name: string) => name.trim());


        const domainDesignerId: string = "6ab29b31-27af-4f56-a67c-986d82097d63";

        let importConfig: IStoredProceduresImportModel = {
            applicationId: application.id,
            designerId: domainDesignerId,
            packageId: element.getPackage().id,
            storedProcedureType: capturedInput.storedProcedureType,
            connectionString: capturedInput.connectionString,
            storedProcNames: storedProcNamesArray,
            repositoryElementId: element.id,
            settingPersistence: capturedInput.settingPersistence,
            databaseType: capturedInput.databaseType === "" ? null : capturedInput.databaseType
        };

        return importConfig;
    }

    /**
    * The host gates the Done button on AngularJS form validity, checked BEFORE onContinue, so the
    * connection rules are expressed as isRequired flags rather than by rejecting onContinue -
    * a rejection replaces the form with a page-level error that nothing can clear.
    *
    * Both fields are required by default; they are relaxed whenever either the repository-level
    * remembered settings or the package-level Database Import settings are usable - this fallback
    * is unconditional and does not depend on which Remember Settings option is selected.
    */
    private applyConnectionRequirements(form: MacroApi.Context.IDynamicFormApi): void {
        const canInherit = this.hasInheritableConnectionSettings();

        const connectionStringField = form.getField("connectionString");
        connectionStringField.isRequired = !canInherit;
        // Explain why a connection string is still needed: nothing usable could be inherited AND
        // the user hasn't already entered one - once they type a value the warning should clear.
        const connectionStringIsBlank = ((connectionStringField.value as string) ?? "").trim() === "";
        const showConnectionStringWarning = !canInherit && connectionStringIsBlank;
        connectionStringField.hint = showConnectionStringWarning
            ? StoredProceduresImportStrategy.noInheritedConnectionStringHint
            : null;
        connectionStringField.hintType = showConnectionStringWarning ? "warning" : null;
        // "(optional...)" would otherwise show on a field that is currently required.
        connectionStringField.placeholder = canInherit
            ? "(inherited setting)"
            : "Enter a connection string";

        const databaseTypeField = form.getField("databaseType");
        databaseTypeField.isRequired = !canInherit;
        const databaseTypeIsBlank = ((databaseTypeField.value as string) ?? "").trim() === "";
        const showDatabaseTypeWarning = !canInherit && databaseTypeIsBlank;
        databaseTypeField.hint = showDatabaseTypeWarning
            ? StoredProceduresImportStrategy.noInheritedDatabaseTypeHint
            : null;
        databaseTypeField.hintType = showDatabaseTypeWarning ? "warning" : null;
    }

    /**
    * Whether the import can rely on an inherited fallback: either the repository-level remembered
    * settings or the package-level Database Import settings resolve to a usable value. Independent
    * of the selected Remember Settings option - the fallback happens automatically whenever a field
    * is left blank, no matter which persistence choice is selected. Single-sourced here so the form's
    * isRequired gating and the Browse button's guard cannot disagree about when inheriting is viable.
    */
    private hasInheritableConnectionSettings(): boolean {
        return StoredProceduresImportStrategy.isUsableConnectionSettings(this.repositorySettings)
            || StoredProceduresImportStrategy.isUsableConnectionSettings(this.inheritedSettings);
    }

    private static isUsableConnectionSettings(settings: IInheritedDatabaseImportSettings | null): boolean {
        const hasConnectionString = (settings?.connectionString ?? "").trim() !== "";
        const hasDatabaseType = (settings?.databaseType ?? "").trim() !== "";

        return hasConnectionString && hasDatabaseType;
    }

    /**
    * The connection settings the import will actually use. A value entered on the dialog always wins;
    * the repository-level remembered settings fill in whatever was left blank next, falling back to
    * the package-level Database Import settings after that - regardless of the selected Remember
    * Settings option. Mirrors SettingsHelper.HydrateDbSettings, which applies the same precedence when
    * the import itself runs.
    */
    private resolveConnectionSettings(connectionString: string, databaseType: string): IInheritedDatabaseImportSettings {
        return {
            connectionString: (connectionString ?? "").trim() !== ""
                ? connectionString
                : (this.repositorySettings?.connectionString ?? "").trim() !== ""
                    ? this.repositorySettings?.connectionString
                    : this.inheritedSettings?.connectionString,
            databaseType: (databaseType ?? "").trim() !== ""
                ? databaseType
                : (this.repositorySettings?.databaseType ?? "").trim() !== ""
                    ? this.repositorySettings?.databaseType
                    : this.inheritedSettings?.databaseType
        };
    }

    /**
    * The connection rules used by the Browse button, which is not gated by form validity. Validates
    * what will actually be used, so an explicitly entered connection string and database type are
    * always sufficient - including when nothing usable could be inherited, which is exactly the case
    * applyConnectionRequirements prompts the user to fill in.
    */
    private getConnectionValidationError(resolved: IInheritedDatabaseImportSettings): string | null {
        if ((resolved.connectionString ?? "").trim() === "") {
            return StoredProceduresImportStrategy.inheritSettingsMessage;
        }

        if ((resolved.databaseType ?? "").trim() === "") {
            return "Please select a Database Type, or configure the package-level Database Import settings so it can be inherited.";
        }

        return null;
    }

    private async getPersistedSettings(element: MacroApi.Context.IElementApi): Promise<IRepositoryImportSettingsResolutionResult> {
        const resolutionModel = {
            applicationId: application.id,
            packageId: element.getPackage().id
        };

        const executionResult = await executeImporterModuleTask(
            "Intent.Modules.Rdbms.Importer.Tasks.StoredProcedureImportSettingsResolution",
            resolutionModel);

        if ((executionResult.errors ?? []).length > 0 || executionResult.result == null) {
            throw new Error("Unable to resolve persisted stored procedure import settings.");
        }

        return executionResult.result as IRepositoryImportSettingsResolutionResult;
    }



    private async openStoredProcedureBrowseDialog(connectionString: string, databaseType: string, preSelectedStoredProcs: string[]): Promise<string[]> {
        let inputProcs = this.sanitizePreSelectedStoredProcs(preSelectedStoredProcs);

        let storedProcSelection: MacroApi.Context.IDynamicFormFieldConfig = {
            id: "storedProcSelection",
            fieldType: "tree-view",
            label: "Stored Procedure Selection",
            isRequired: true,
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
                        specializationId: "Stored-Procedure",
                        isSelectable: (x) => true
                    }
                ]
            }
        };

        let browseFormConfig: MacroApi.Context.IDynamicFormConfig = {
            title: "Browse Stored Procedures",
            onInitialize: async (form: MacroApi.Context.IDynamicFormApi) => {
                const input: IStoredProcListInputModel = {
                    connectionString: connectionString,
                    databaseType: databaseType
                };
                let executionResult = await executeImporterModuleTask("Intent.Modules.Rdbms.Importer.Tasks.StoredProcList", input);

                if (executionResult.errors?.length > 0) {
                    await displayExecutionResultErrors(executionResult);
                    return;
                }

                let spListResult = executionResult.result as IStoredProcListResultModel;

                form.getField("storedProcSelection").treeViewOptions.rootNode = {
                    id: "database",
                    specializationId: "Database",
                    label: "Database",
                    icon: Icons.databaseIcon,
                    children: Object.keys(spListResult.storedProcs).map(schemaName => {
                        return {
                            id: `schema.${schemaName}`,
                            label: schemaName,
                            specializationId: "Schema",
                            icon: Icons.schemaIcon,
                            isSelected: inputProcs.some(x => x.startsWith(`sp.${schemaName}`)),
                            children: spListResult.storedProcs[schemaName].map(sp => {
                                return {
                                    id: `sp.${schemaName}.${sp}`,
                                    label: sp,
                                    specializationId: "Stored-Procedure",
                                    icon: Icons.storedProcIcon,
                                    isSelected: inputProcs.some(x => x == `sp.${schemaName}.${sp}`)
                                } as MacroApi.Context.ISelectableTreeNode;
                            })
                        } as MacroApi.Context.ISelectableTreeNode;
                    })
                };
            },
            fields: [
                storedProcSelection
            ]
        };

        let browseInputs = await dialogService.openForm(browseFormConfig);

        if (browseInputs && browseInputs.storedProcSelection) {
            let selection = browseInputs.storedProcSelection as string[];
            let filteredSelection = selection.filter(x => !x.startsWith("schema."));
            return filteredSelection
                .map(x => {
                    let parts = x.split(".");
                    return `${parts[1]}.${parts[2]}`;
                });
        }

        return [];
    }

    private sanitizePreSelectedStoredProcs(preSelectedStoredProcs: string[]): string[] {
        if (preSelectedStoredProcs == null || preSelectedStoredProcs.filter(x => x != "").length === 0) {
            return [];
        }

        return preSelectedStoredProcs.map(x => !x.startsWith("dbo.") ? `sp.dbo.${x}` : `sp.${x}`);
    }
}

interface IStoredProceduresImportModel {
    applicationId: string;
    designerId: string;
    packageId: string;
    storedProcedureType: string;
    connectionString: string;
    storedProcNames: string[];
    repositoryElementId: string;
    settingPersistence: string;
    databaseType: string;
}

interface IInheritedDatabaseImportSettings {
    connectionString: string;
    databaseType: string;
}

interface IRepositoryImportSettingsResolutionResult {
    connectionString: string;
    databaseType: string;
    storedProcedureType: string;
    settingPersistence: string;
    source: string;
    userLocalSettingsPath: string;
    inheritedConnectionString: string;
    inheritedDatabaseType: string;
}


interface IStoredProcListInputModel {
    connectionString: string;
    databaseType: string;
}

interface IStoredProcListResultModel {
    storedProcs: { [key: string]: string[] };
}