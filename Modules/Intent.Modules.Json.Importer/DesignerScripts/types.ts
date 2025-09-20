interface IProfileOption {
    id: string;
    description: string;
}

interface IJsonImporterSettings {
    sourceFolder: string;
    pattern: string;
}

interface IImportConfig {
    sourceFolder: string;
    packageId: string;
    targetFolderId: string | null;
    profile: string;
    selectedFiles: string[];
    casingConvention: string;
}

interface IFileData {
    name: string;
    relativePath: string;
    fullPath: string;
}

interface IJsonPreviewResult {
    rootPath: string;
    rootName: string;
    files: IFileData[];
}

interface IExecutionResult {
    result?: any;
    warnings: string[];
    errors: string[];
}