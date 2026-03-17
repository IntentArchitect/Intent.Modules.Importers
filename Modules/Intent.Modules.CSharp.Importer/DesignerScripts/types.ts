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
    designerId: string;
    packageId: string;
    targetFolderId: string | null;
    importProfileId: string;
    selectedFiles: string[];
    casingConvention: string;
    preserveAsync: boolean;
}

interface IFileData {
    name: string;
    relativePath: string;
    fullPath: string;
}

interface ICSharpPreviewResult {
    rootPath: string;
    rootName: string;
    files: IFileData[];
}