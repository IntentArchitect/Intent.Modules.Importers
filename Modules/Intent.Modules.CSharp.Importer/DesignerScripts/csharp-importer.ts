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
        title: "C# File Import",
        fields: [],
        pages: [folderSelectionPage, fileSelectionPage]
    };

    const inputs = await dialogService.openForm(formConfig);

    if (inputs === null) {
        return; // User cancelled
    }

    // Get selected files from the tree view
    const selectedFiles: string[] = [];
    selectedFiles.push(...inputs.selectionTree);

    const importConfig: IImportConfig = {
        sourceFolder: inputs.sourceFolder,
        designerId: element.getPackage().designerId,
        packageId: element.getPackage().id,
        targetFolderId: element.specialization === "Folder" ? element.id : null,
        importProfileId: inputs.importProfileId,
        selectedFiles: selectedFiles,
        casingConvention: inputs.casingConvention,
    };

    // Execute import task with structured result handling
    const executionResult = await executeImporterModuleTask(
        "Intent.CSharp.Importer.ImportCSharpFilesTask",
        importConfig
    );

    if ((executionResult.errors ?? []).length > 0) {
        await dialogService.error(executionResult.errors.join("\r\n"));
        return;
    }

    const warnings = executionResult.warnings ?? [];
    if (warnings.length > 0) {
        await dialogService.warn("Import complete.\r\n\r\n" + warnings.join("\r\n"));
        return;
    }
}

async function getCharpFilesAndPreview(folderPath: string, glob?: string): Promise<ICSharpPreviewResult> {
    const request = {
        sourceFolder: folderPath,
        pattern: glob != null && glob != "" ? glob : "**/*.cs",
    };
    const executionResult = await executeImporterModuleTask(
        "Intent.Modules.Importer.FileDirectoryPreviewTask",
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
                placeholder: "path/to/csharp/folder",
                hint: "Path to the folder containing C# files to scan and import.",
                openDirectoryOptions: {
                    title: "Select C# Source Folder"
                },
                isRequired: true
            },
            {
                id: "importProfileId",
                fieldType: "select",
                label: "Profile",
                hint: "Select the import profile for these files.",
                isRequired: true,
                isHidden: profileOptions.length === 1,
                selectOptions: profileOptions,
                value: null
            },
            {
                id: "pattern",
                fieldType: "text",
                label: "File Pattern",
                placeholder: "**/*.cs",
                hint: "Glob pattern to filter C# files (e.g., **/*.cs, data/*.cs, **/user*.cs).",
                isRequired: true,
                value: "**/*.cs"
            },
            // {
            //     id: "casingConvention",
            //     fieldType: "select",
            //     label: "Casing Convention",
            //     hint: "Select how property names should be cased when imported.",
            //     isRequired: true,
            //     selectOptions: [
            //         { id: "PascalCase", description: "PascalCase", additionalInfo: "(e.g. FirstName)" },
            //         { id: "AsIs", description: "As Is", additionalInfo: "(preserve original casing)" }
            //     ],
            //     value: "PascalCase"
            // }
        ]
    };
}

function getAvailableProfiles(packageModel: MacroApi.Context.IPackageApi): IProfileOption[] {
    const profiles: IProfileOption[] = [];
    
    if (packageModel.specialization == "Domain Package") {
        profiles.push({ id: "domain-classes", description: "Classes" });
        profiles.push({ id: "domain-enums", description: "Enums" });
        profiles.push({ id: "domain-events", description: "Domain Events" });
        profiles.push({ id: "domain-contracts", description: "Domain Contracts" });
    }

    if (packageModel.specialization == "Services Package") {
        profiles.push({ id: "services-commands", description: "Commands" });
        profiles.push({ id: "services-dtos", description: "DTOs" });
        profiles.push({ id: "services-enums", description: "Enums" });
    }

    if (packageModel.specialization == "Eventing Package") {
        profiles.push({ id: "eventing-integration-messages", description: "Eventing Messages" });
        profiles.push({ id: "eventing-integration-commands", description: "Integration Commands" });
        profiles.push({ id: "eventing-dtos", description: "Integration DTOs" });
        profiles.push({ id: "services-enums", description: "Enums" });
    }
    
    return profiles;
}

function createFileSelectionPage(): MacroApi.Context.IDynamicFormWizardPageConfig {
    return {
        onInitialize: async (formApi: MacroApi.Context.IDynamicFormApi) => {
            const sourceFolder = formApi.getField("sourceFolder").value as string;
            if (!sourceFolder) {
                await dialogService.error("No source folder selected.");
                return;
            }

            const pattern = formApi.getField("pattern").value as string;
            const previewData = await getCharpFilesAndPreview(sourceFolder, pattern);

            if (previewData.files.length === 0) {
                throw new Error("No C# files found in the selected folder. Please choose another folder.");
            }

            // Populate the tree with found files (now supports folders)
            const selectionTree = formApi.getField("selectionTree");
            selectionTree.treeViewOptions!.rootNode = buildTree(previewData.rootName, previewData.files);
        },
        fields: [
            {
                id: "selectionTree",
                fieldType: "tree-view",
                label: "Select C# Files to Import",
                hint: "Choose which C# files you want to import into your domain model.",
                isRequired: false,
                treeViewOptions: {
                    height: "500px",
                    width: "100%",
                    isMultiSelect: true,
                    selectableTypes: [
                        { 
                            specializationId: "csharp-folder", 
                            isSelectable: true,
                            autoSelectChildren: true,
                            autoExpand: true,
                        },
                        { 
                            specializationId: "csharp-file", 
                            isSelectable: true 
                        },
                    ],
                    rootNode: {
                        specializationId: "csharp-folder",
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
        specializationId: "csharp-folder",
        id: "folder/root",
        label: `${rootName} (${files.length} found)`,
        icon: Icons.Folder,
        expandedIcon: Icons.OpenFolder,
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
                        specializationId: "csharp-file",
                        id: file.fullPath,
                        label: part,
                        isExpanded: false,
                        isSelected: true,
                        icon: Icons.CSharpFile,
                        children: []
                    }
                    : {
                        specializationId: "csharp-folder",
                        id: "folder/" + node.id + "/" + part,
                        label: part,
                        isExpanded: true,
                        isSelected: true,
                        icon: Icons.Folder,
                        expandedIcon: Icons.OpenFolder,
                        children: []
                    };
                node.children.push(child);
            }
            node = child;
        }
    }
    return root;
}