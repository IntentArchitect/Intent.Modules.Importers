interface IExecutionResult {
    result?: any;
    warnings: string[];
    errors: string[];
}

async function executeImporterModuleTask(taskTypeId: string, input: any): Promise<IExecutionResult> {
    let inputJsonString = JSON.stringify(input);
    console.log(`Executing Module Task ${taskTypeId} => ${inputJsonString}`);
    
    let moduleTaskResultString = await executeModuleTask(taskTypeId, inputJsonString);
    
    console.log(`Module Task ${taskTypeId} Completed`);
    let executionResult = JSON.parse(moduleTaskResultString) as IExecutionResult;
    return executionResult;
}

async function displayExecutionResultErrors(executionResult: IExecutionResult): Promise<void> {
    if (executionResult.errors.length === 0) {
        return;
    }
    
    let errorMessage = executionResult.errors.map(error => `⭕ ${error}`).join("\r\n");
    await dialogService.error(errorMessage);
    console.error(errorMessage);
}

async function displayExecutionResultWarnings(executionResult: IExecutionResult, title: string): Promise<void> {
    if (executionResult.warnings.length === 0) {
        return;
    }
    if (title == null || title === "") {
        title = "Warnings";
    }
    let warningMessage = executionResult.warnings.map(warning => `🟡 ${warning}`).join("\r\n");
    await dialogService.warn(title + "\r\n\r\n" + warningMessage);
    console.warn(title + "\r\n\r\n" + warningMessage);
}