/// <reference path="../../TypescriptCore/elementmacro.context.api.d.ts" />

// Align with RDBMS importer UI task/error handling
interface IExecutionResult {
    result?: any;
    warnings: string[];
    errors: string[];
}

async function executeImporterModuleTask(taskTypeId: string, input: any): Promise<IExecutionResult> {
    const payload = JSON.stringify(input);
    console.log(`Executing Module Task ${taskTypeId} => ${payload}`);
    const moduleTaskResultString = await executeModuleTask(taskTypeId, payload);
    console.log(`Module Task ${taskTypeId} Completed`);
    return JSON.parse(moduleTaskResultString) as IExecutionResult;
}

async function importJson(element: MacroApi.Context.IElementApi): Promise<void> {
    // Page 1: Folder Selection
    const folderSelectionPage = createFolderSelectionPage(element);
    
    // Page 2: File Selection
    const fileSelectionPage = createFileSelectionPage();

    const formConfig: MacroApi.Context.IDynamicFormConfig = {
        title: "JSON Import",
        fields: [],
        pages: [folderSelectionPage, fileSelectionPage]
    };

    const inputs = await dialogService.openForm(formConfig);

    if (inputs === null) {
        return; // User cancelled
    }

    // Validate inputs
    if (!inputs.sourceFolder) {
        await dialogService.error("Please specify a source folder.");
        return;
    }

    // Get selected files from the tree view
    const selectedFiles: string[] = [];
    selectedFiles.push(...inputs.selectionTree);

    if (!selectedFiles.length) {
        await dialogService.warn("No JSON files selected to import.");
        return;
    }
    
    const importConfig: IImportConfig = {
        sourceFolder: inputs.sourceFolder,
        packageId: element.getPackage().id,
        targetFolderId: element.specialization === "Folder" ? element.id : null,
        profile: inputs.profile,
        selectedFiles: selectedFiles,
        casingConvention: inputs.casingConvention,
    };

    // Execute import task with structured result handling
    const executionResult = await executeImporterModuleTask(
        "Intent.Modules.Json.Importer.Tasks.JsonImport",
        importConfig
    );

    console.log(`executionResult = ${JSON.stringify(executionResult)}`);

    if ((executionResult.errors ?? []).length > 0) {
        await dialogService.error(executionResult.errors.join("\r\n"));
        return;
    }

    const warnings = executionResult.warnings ?? [];
    if (warnings.length > 0) {
        await dialogService.warn("Import complete.\r\n\r\n" + warnings.join("\r\n"));
        return;
    }

    await dialogService.info("Import complete.");
}

async function getJsonFilesAndPreview(folderPath: string, glob?: string): Promise<IJsonPreviewResult> {
    const request = {
        sourceFolder: folderPath,
        pattern: glob || "**/*.json",
    };
    const executionResult = await executeImporterModuleTask(
        "Intent.Modules.Json.Importer.Tasks.JsonPreview",
        request
    );
    if ((executionResult.errors ?? []).length > 0) {
        throw new Error(executionResult.errors.join("\r\n"));
    }
    return executionResult?.result || { rootPath: folderPath, rootName: "Unknown", files: [] };
}

function createFolderSelectionPage(element: MacroApi.Context.IElementApi): MacroApi.Context.IDynamicFormWizardPageConfig {
    // Determine available profiles based on the designer
    const packageModel = element.getPackage();
    const profileOptions = getAvailableProfiles(packageModel);
    
    return {
        fields: [
            {
                id: "sourceFolder",
                fieldType: "open-directory",
                label: "Source Folder",
                placeholder: "path/to/json/folder",
                hint: "Path to the folder containing JSON files to scan and import.",
                openDirectoryOptions: {
                    title: "Select JSON Source Folder"
                },
                isRequired: true
            },
            {
                id: "pattern",
                fieldType: "text",
                label: "File Pattern",
                placeholder: "**/*.json",
                hint: "Glob pattern to filter JSON files (e.g., **/*.json, data/*.json, **/user*.json).",
                isRequired: true,
                value: "**/*.json"
            },
            {
                id: "profile",
                fieldType: "select",
                label: "Profile",
                hint: "Select the import profile for these files.",
                isRequired: true,
                isHidden: profileOptions.length === 1,
                selectOptions: profileOptions,
                value: profileOptions[0]?.id
            },
            {
                id: "casingConvention",
                fieldType: "select",
                label: "Casing Convention",
                hint: "Select how property names should be cased when imported.",
                isRequired: true,
                selectOptions: [
                    { id: "PascalCase", description: "PascalCase", additionalInfo: "(e.g. FirstName)" },
                    { id: "AsIs", description: "As Is", additionalInfo: "(preserve original casing)" }
                ],
                value: "PascalCase"
            }
        ]
    };
}

function getAvailableProfiles(packageModel: MacroApi.Context.IPackageApi): IProfileOption[] {
    const profiles: IProfileOption[] = [];
    
    if (packageModel.specialization == "Domain Package") {
        profiles.push({ id: "DomainDocumentDB", description: "Domain Document DB Profile" });
    }

    if (packageModel.specialization == "Eventing Package") {
        profiles.push({ id: "EventingMessages", description: "Eventing Messages Profile" });
    }
    
    return profiles;
}

function createFileSelectionPage(): MacroApi.Context.IDynamicFormWizardPageConfig {
    return {
        onInitialize: async (formApi: MacroApi.Context.IDynamicFormApi) => {
            try {
                const sourceFolder = formApi.getField("sourceFolder").value as string;
                if (!sourceFolder) {
                    await dialogService.error("No source folder selected.");
                    return;
                }

                const pattern = formApi.getField("pattern").value as string;
                const previewData = await getJsonFilesAndPreview(sourceFolder, pattern);

                if (previewData.files.length === 0) {
                    await dialogService.warn("No JSON files found in the selected folder.");
                    return;
                }

                // Populate the tree with found files (now supports folders)
                const selectionTree = formApi.getField("selectionTree");
                selectionTree.treeViewOptions!.rootNode = buildTree(previewData.rootName, previewData.files);

            } catch (error) {
                console.error("Error scanning folder:", error);
                await dialogService.error(`Error scanning folder: ${error}`);
            }
        },
        fields: [
            {
                id: "selectionTree",
                fieldType: "tree-view",
                label: "Select JSON Files to Import",
                hint: "Choose which JSON files you want to import into your domain model.",
                isRequired: false,
                treeViewOptions: {
                    height: "400px",
                    width: "100%",
                    isMultiSelect: true,
                    selectableTypes: [
                        { 
                            specializationId: "json-folder", 
                            isSelectable: true,
                            autoSelectChildren: true,
                            autoExpand: true,
                        },
                        { 
                            specializationId: "json-file", 
                            isSelectable: true 
                        },
                    ],
                    rootNode: {
                        specializationId: "json-folder",
                        id: "root",
                        label: "Loading...",
                        isExpanded: true,
                        children: [],
                    },
                },
            }
        ],
        onContinue: async (form) => {
            let result = form.getValues();
            let selections = result.selectionTree.filter((x: any) => !x.startsWith("folder/"));
            form.getField("selectionTree").value = selections;
        }
    };
}

function buildTree(rootName: string, files: IFileData[]) : MacroApi.Context.ISelectableTreeNode {
    const root: MacroApi.Context.ISelectableTreeNode = {
        specializationId: "json-folder",
        id: "root",
        label: `${rootName} (${files.length} found)`,
        icon: Icons.Folder,
        isSelected: true,
        isExpanded: true,
        children: []
    };
    for (const file of files) {
        // file.relativePath is already relative from backend; normalize slashes
        const relPath = file.relativePath.replace(/\\/g, "/");
        const parts = relPath.split("/");
        let node = root;
        for (let i = 0; i < parts.length; i++) {
            const part = parts[i];
            const isFile = i === parts.length - 1;
            let child = node.children.find((c: any) => c.label === part);
            if (!child) {
                child = isFile
                    ? {
                        specializationId: "json-file",
                        id: file.fullPath,
                        label: part,
                        isExpanded: false,
                        isSelected: true,
                        icon: Icons.JsonFile,
                        children: []
                    }
                    : {
                        specializationId: "json-folder",
                        id: "folder/" + node.id + "/" + part,
                        label: part,
                        isExpanded: true,
                        isSelected: true,
                        icon: Icons.Folder,
                        children: []
                    };
                node.children.push(child);
            }
            node = child;
        }
    }
    return root;
}