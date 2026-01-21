async function executeImporterModuleTask(taskTypeId, input) {
    const payload = JSON.stringify(input);
    console.log(`Executing Module Task ${taskTypeId} => ${payload}`);
    const moduleTaskResultString = await executeModuleTask(taskTypeId, payload);
    console.log(`Module Task ${taskTypeId} Completed`);
    return JSON.parse(moduleTaskResultString);
}
async function importJson(element) {
    var _a, _b;
    // Page 1: Folder Selection
    const folderSelectionPage = createFolderSelectionPage(element);
    // Page 2: File Selection
    const fileSelectionPage = createFileSelectionPage();
    const formConfig = {
        title: "C# File Import",
        fields: [],
        pages: [folderSelectionPage, fileSelectionPage]
    };
    const inputs = await dialogService.openForm(formConfig);
    if (inputs === null) {
        return; // User cancelled
    }
    // Get selected files from the tree view
    const selectedFiles = [];
    selectedFiles.push(...inputs.selectionTree);
    const importConfig = {
        sourceFolder: inputs.sourceFolder,
        designerId: element.getPackage().designerId,
        packageId: element.getPackage().id,
        targetFolderId: element.specialization === "Folder" ? element.id : null,
        importProfileId: inputs.importProfileId,
        selectedFiles: selectedFiles,
        casingConvention: inputs.casingConvention,
    };
    // Execute import task with structured result handling
    const executionResult = await executeImporterModuleTask("Intent.CSharp.Importer.ImportCSharpFilesTask", importConfig);
    if (((_a = executionResult.errors) !== null && _a !== void 0 ? _a : []).length > 0) {
        await dialogService.error(executionResult.errors.join("\r\n"));
        return;
    }
    const warnings = (_b = executionResult.warnings) !== null && _b !== void 0 ? _b : [];
    if (warnings.length > 0) {
        await dialogService.warn("Import complete.\r\n\r\n" + warnings.join("\r\n"));
        return;
    }
}
async function getCharpFilesAndPreview(folderPath, glob) {
    var _a;
    const request = {
        sourceFolder: folderPath,
        pattern: glob != null && glob != "" ? glob : "**/*.cs",
    };
    const executionResult = await executeImporterModuleTask("Intent.Modules.Importer.FileDirectoryPreviewTask", request);
    if (((_a = executionResult.errors) !== null && _a !== void 0 ? _a : []).length > 0) {
        throw new Error(executionResult.errors.join("\r\n"));
    }
    return (executionResult === null || executionResult === void 0 ? void 0 : executionResult.result) || { rootPath: folderPath, rootName: "Unknown", files: [] };
}
function createFolderSelectionPage(element) {
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
            }
        ]
    };
}
function getAvailableProfiles(packageModel) {
    const profiles = [];
    // Available in all package types
    profiles.push({ id: "type-definitions-only", description: "Type Definitions Only" });
    if (packageModel.specialization == "Domain Package") {
        profiles.push({ id: "domain-classes", description: "Classes" });
        if (application.installedModules.some(x => x.id == "Intent.Modelers.Domain.Events")) {
            profiles.push({ id: "domain-events", description: "Domain Events" });
        }
        profiles.push({ id: "domain-contracts", description: "Domain Contracts" });
        profiles.push({ id: "domain-enums", description: "Enums Only" });
    }
    if (packageModel.specialization == "Services Package") {
        profiles.push({ id: "services-services", description: "Services" });
        profiles.push({ id: "services-dtos", description: "DTOs" });
        if (application.installedModules.some(x => x.id == "Intent.Modelers.Services.CQRS")) {
            profiles.push({ id: "services-commands", description: "Commands" });
            profiles.push({ id: "services-queries", description: "Queries" });
        }
        profiles.push({ id: "services-enums", description: "Enums Only" });
    }
    if (packageModel.specialization == "Eventing Package") {
        profiles.push({ id: "eventing-integration-messages", description: "Eventing Messages" });
        profiles.push({ id: "eventing-integration-commands", description: "Integration Commands" });
        profiles.push({ id: "eventing-dtos", description: "Integration DTOs" });
        profiles.push({ id: "services-enums", description: "Enums Only" });
    }
    return profiles;
}
function createFileSelectionPage() {
    return {
        onInitialize: async (formApi) => {
            const sourceFolder = formApi.getField("sourceFolder").value;
            if (!sourceFolder) {
                await dialogService.error("No source folder selected.");
                return;
            }
            const pattern = formApi.getField("pattern").value;
            const previewData = await getCharpFilesAndPreview(sourceFolder, pattern);
            if (previewData.files.length === 0) {
                throw new Error("No C# files found in the selected folder. Please choose another folder.");
            }
            // Populate the tree with found files (now supports folders)
            const selectionTree = formApi.getField("selectionTree");
            selectionTree.treeViewOptions.rootNode = buildTree(previewData.rootName, previewData.files);
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
            let selections = result.selectionTree.filter((x) => !x.startsWith("folder/"));
            form.getField("selectionTree").value = selections;
        }
    };
}
function buildTree(rootName, files) {
    const root = {
        specializationId: "csharp-folder",
        id: "folder/root",
        label: `${rootName} (${files.length} found)`,
        icon: Icons.Folder,
        expandedIcon: Icons.OpenFolder,
        isSelected: true,
        isExpanded: true,
        children: []
    };
    // sort - to ensure that folders are at the top by starting with the deepest directories first:
    for (const file of files.sort((x, y) => y.relativePath.split("/").length - x.relativePath.split("/").length)) {
        // file.relativePath is already relative from backend; normalize slashes
        const relPath = file.relativePath.replace(/\\/g, "/");
        const parts = relPath.split("/");
        let node = root;
        for (let i = 0; i < parts.length; i++) {
            const part = parts[i];
            const isFile = i === parts.length - 1;
            let child = node.children.find((c) => c.label === part);
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
class Icons {
}
Icons.Folder = "data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4NCjwhLS0gR2VuZXJhdG9yOiBBZG9iZSBJbGx1c3RyYXRvciAyOC4zLjAsIFNWRyBFeHBvcnQgUGx1Zy1JbiAuIFNWRyBWZXJzaW9uOiA2LjAwIEJ1aWxkIDApICAtLT4NCjxzdmcgdmVyc2lvbj0iMS4xIiBpZD0iZmlsZS1jbG9zZWQiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgeG1sbnM6eGxpbms9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkveGxpbmsiIHg9IjBweCINCgkgeT0iMHB4IiB2aWV3Qm94PSIwIDAgNTAgNTAiIGVuYWJsZS1iYWNrZ3JvdW5kPSJuZXcgMCAwIDUwIDUwIiB4bWw6c3BhY2U9InByZXNlcnZlIj4NCjxsaW5lYXJHcmFkaWVudCBpZD0iU1ZHSURfMV9hZWJjZjU3OC0zYWI0LTQ1ZjQtOTc5Ni01ZDlmNDQ3MGNmNjAiIGdyYWRpZW50VW5pdHM9InVzZXJTcGFjZU9uVXNlIiB4MT0iMjQuODQ1MiIgeTE9IjcuOTc0NSIgeDI9IjI0LjkzNTYiIHkyPSI0NC44MTg1Ij4NCgk8c3RvcCAgb2Zmc2V0PSIwIiBzdHlsZT0ic3RvcC1jb2xvcjojRkJCMDQxIi8+DQoJPHN0b3AgIG9mZnNldD0iMCIgc3R5bGU9InN0b3AtY29sb3I6I0ZDQjY0NSIvPg0KCTxzdG9wICBvZmZzZXQ9IjEiIHN0eWxlPSJzdG9wLWNvbG9yOiNGNTdGMjAiLz4NCjwvbGluZWFyR3JhZGllbnQ+DQo8cGF0aCBmaWxsPSJ1cmwoI1NWR0lEXzFfYWViY2Y1NzgtM2FiNC00NWY0LTk3OTYtNWQ5ZjQ0NzBjZjYwKSIgZD0iTTQ1LjEsOEgyNi43QzI2LDgsMjUuNCw4LjMsMjUsOC43bC00LjcsNC43SDUuMWMtMS41LDAtMi43LDEuMS0yLjcsMi41djI2LjNjMCwxLjQsMS4yLDIuNSwyLjcsMi41DQoJaDM5LjZjMS41LDAsMi43LTEuMSwyLjctMi41VjEwLjJDNDcuNCw5LDQ2LjQsOCw0NS4xLDh6IE00My45LDEzLjRIMjUuMmwyLTJoMTYuN1YxMy40eiIvPg0KPC9zdmc+DQo=";
Icons.OpenFolder = "data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4NCjwhLS0gR2VuZXJhdG9yOiBBZG9iZSBJbGx1c3RyYXRvciAyOC4zLjAsIFNWRyBFeHBvcnQgUGx1Zy1JbiAuIFNWRyBWZXJzaW9uOiA2LjAwIEJ1aWxkIDApICAtLT4NCjxzdmcgdmVyc2lvbj0iMS4xIiBpZD0iZmlsZS1vcGVuIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHhtbG5zOnhsaW5rPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5L3hsaW5rIiB4PSIwcHgiDQoJIHk9IjBweCIgdmlld0JveD0iMCAwIDUwIDUwIiBlbmFibGUtYmFja2dyb3VuZD0ibmV3IDAgMCA1MCA1MCIgeG1sOnNwYWNlPSJwcmVzZXJ2ZSI+DQo8bGluZWFyR3JhZGllbnQgaWQ9IlNWR0lEXzFfNjU1NmMwYTUtMzY4OC00NWZhLWIxNDQtMjk1NGU3N2I3MzUwIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSIgeDE9IjEuODExOCIgeTE9IjIzLjM1MTMiIHgyPSIzLjUxODIiIHkyPSIzMy45ODg3Ij4NCgk8c3RvcCAgb2Zmc2V0PSIwIiBzdHlsZT0ic3RvcC1jb2xvcjojRkJCMDQxIi8+DQoJPHN0b3AgIG9mZnNldD0iMCIgc3R5bGU9InN0b3AtY29sb3I6I0ZDQjY0NSIvPg0KCTxzdG9wICBvZmZzZXQ9IjEiIHN0eWxlPSJzdG9wLWNvbG9yOiNGNTdGMjAiLz4NCjwvbGluZWFyR3JhZGllbnQ+DQo8cG9seWdvbiBmaWxsPSJ1cmwoI1NWR0lEXzFfNjU1NmMwYTUtMzY4OC00NWZhLWIxNDQtMjk1NGU3N2I3MzUwKSIgcG9pbnRzPSIzLjUsMzQgMS44LDIzLjQgMS44LDIzLjMgIi8+DQo8bGluZWFyR3JhZGllbnQgaWQ9IlNWR0lEXzAwMDAwMDQ0MTU5MTYxMzY0OTQ5NDAzNjYwMDAwMDAyNjI0ODkwMDg2NzY2Nzc0MTc2XyIgZ3JhZGllbnRVbml0cz0idXNlclNwYWNlT25Vc2UiIHgxPSIxLjgxMTgiIHkxPSIyMy4zNTEzIiB4Mj0iMy41MTgyIiB5Mj0iMzMuOTg4NyI+DQoJPHN0b3AgIG9mZnNldD0iMCIgc3R5bGU9InN0b3AtY29sb3I6I0ZCQjA0MSIvPg0KCTxzdG9wICBvZmZzZXQ9IjAiIHN0eWxlPSJzdG9wLWNvbG9yOiNGQ0I2NDUiLz4NCgk8c3RvcCAgb2Zmc2V0PSIxIiBzdHlsZT0ic3RvcC1jb2xvcjojRjU3RjIwIi8+DQo8L2xpbmVhckdyYWRpZW50Pg0KPHBvbHlnb24gZmlsbD0idXJsKCNTVkdJRF8wMDAwMDA0NDE1OTE2MTM2NDk0OTQwMzY2MDAwMDAwMjYyNDg5MDA4Njc2Njc3NDE3Nl8pIiBwb2ludHM9IjMuNSwzNCAxLjgsMjMuNCAxLjgsMjMuMyAiLz4NCjxsaW5lYXJHcmFkaWVudCBpZD0iU1ZHSURfMDAwMDAxNzg5MDgyMjI3ODE3NTQxMTY0MTAwMDAwMTcyMjAzNTE0MTU4NjEzNzk5NzVfIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSIgeDE9IjIzLjIyMDgiIHkxPSI2Ljk3ODIiIHgyPSIyOS40MTY1IiB5Mj0iNDUuNjAyMiI+DQoJPHN0b3AgIG9mZnNldD0iMCIgc3R5bGU9InN0b3AtY29sb3I6I0ZCQjA0MSIvPg0KCTxzdG9wICBvZmZzZXQ9IjAiIHN0eWxlPSJzdG9wLWNvbG9yOiNGQ0I2NDUiLz4NCgk8c3RvcCAgb2Zmc2V0PSIxIiBzdHlsZT0ic3RvcC1jb2xvcjojRjU3RjIwIi8+DQo8L2xpbmVhckdyYWRpZW50Pg0KPHBhdGggZmlsbD0idXJsKCNTVkdJRF8wMDAwMDE3ODkwODIyMjc4MTc1NDExNjQxMDAwMDAxNzIyMDM1MTQxNTg2MTM3OTk3NV8pIiBkPSJNNDUuMyw2LjNIMjcuN2MtMC42LDAtMS4yLDAuMy0xLjYsMC44bC0zLjksNC4xDQoJYy0wLjQsMC40LTAuOCwwLjYtMS4zLDAuNkg3LjFjLTEuNCwwLTIuNiwxLjEtMi42LDIuNXY1LjhIMi41Yy0wLjUsMC0xLDAuNS0xLDFjMCwwLjEsMCwwLjEsMCwwLjJsMC4zLDIuMUwzLjUsMzRsMS4xLDcNCgljMC4yLDEuMiwxLjIsMi4xLDIuNSwyLjFoMzcuOGMxLjQsMCwyLjYtMS4xLDIuNi0yLjV2LTMyQzQ3LjUsNy4zLDQ2LjUsNi4zLDQ1LjMsNi4zeiBNNDQuMiwzOC40YzAsMC4zLTAuMSwwLjYtMC4zLDAuOGwtMi41LTE1LjgNCglMNDEsMjEuN2MtMC4xLTEtMS0xLjctMi0xLjdINy45di0zLjZjMC0wLjcsMC41LTEuMiwxLjEtMS4yaDEzLjJjMC41LDAsMS0wLjIsMS4zLTAuNmwyLjgtMi45bDEuMS0xLjFsMC4zLTAuMw0KCUMyOCwxMCwyOC41LDkuOCwyOSw5LjhoMTRjMC42LDAsMS4xLDAuNSwxLjEsMS4yVjM4LjR6Ii8+DQo8L3N2Zz4NCg==";
Icons.CSharpFile = "data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAzMiAzMiI+PHRpdGxlPmZpbGVfdHlwZV9jc2hhcnA8L3RpdGxlPjxwYXRoIGQ9Ik0xOS43OTIsNy4wNzFoMi41NTNWOS42MjRIMjQuOVY3LjA3MWgyLjU1MlY5LjYyNEgzMHYyLjU1MmgtMi41NXYyLjU1MUgzMFYxNy4yOEgyNy40NDl2Mi41NTJIMjQuOXYtMi41NWwtMi41NSwwLDAsMi41NTJIMTkuNzkzdi0yLjU1bC0yLjU1MywwVjE0LjcyNWgyLjU1M1YxMi4xNzlIMTcuMjRWOS42MjJoMi41NTRabTIuNTUzLDcuNjU4SDI0LjlWMTIuMTc2SDIyLjM0NVoiIHN0eWxlPSJmaWxsOiMzNjg4MzIiLz48cGF0aCBkPSJNMTQuNjg5LDI0LjAxM2ExMC4yLDEwLjIsMCwwLDEtNC42NTMuOTE1LDcuNiw3LjYsMCwwLDEtNS44OS0yLjMzNkE4LjgzOSw4LjgzOSwwLDAsMSwyLDE2LjM2Nyw5LjQzNiw5LjQzNiwwLDAsMSw0LjQxMiw5LjY0OGE4LjE4MSw4LjE4MSwwLDAsMSw2LjI1OS0yLjU3NywxMS4xLDExLjEsMCwwLDEsNC4wMTguNjM4djMuNzQ1YTYuODEsNi44MSwwLDAsMC0zLjcyMy0xLjAzNiw0Ljc5Myw0Ljc5MywwLDAsMC0zLjcsMS41MjksNS44NzksNS44NzksMCwwLDAtMS40MDcsNC4xNDIsNS43NzQsNS43NzQsMCwwLDAsMS4zMjgsMy45OTIsNC41NTEsNC41NTEsMCwwLDAsMy41NzUsMS40ODcsNy4yODgsNy4yODgsMCwwLDAsMy45MjctMS4xMDhaIiBzdHlsZT0iZmlsbDojMzY4ODMyIi8+PC9zdmc+";
