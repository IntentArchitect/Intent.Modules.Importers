/// <reference path="./strategy-database-import.ts" />
/// <reference path="./strategy-stored-procedures-import.ts" />

// noinspection JSUnusedGlobalSymbols
let RdbmsImporterApi = {
    importDatabase,
    importStoredProcedures
};

async function importDatabase(packageElement: IElementApi): Promise<void> {
    const strategy = new DatabaseImportStrategy();
    await strategy.execute(packageElement);
} 

async function importStoredProcedures(repositoryElement: IElementApi): Promise<void> {
    const strategy = new StoredProceduresImportStrategy();
    await strategy.execute(repositoryElement);
}
