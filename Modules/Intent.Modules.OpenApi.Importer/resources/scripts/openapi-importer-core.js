// noinspection JSUnusedGlobalSymbols
async function importOpenApi(element) {
    var defaults = getDialogDefaults(element);
    let openApiFile = {
        id: "openApiFile",
        fieldType: "text",
        label: "OpenApi File",
        placeholder: null,
        hint: "Path to file, including name.",
        isRequired: true,
        value: defaults.openApiFile
    };
    let serviceType = {
        id: "serviceType",
        fieldType: "select",
        label: "Service Paradigm",
        placeholder: "",
        hint: "Which service paradigm would you like",
        value: defaults.serviceType,
        selectOptions: [{ id: "CQRS", description: "CQRS" }, { id: "Service", description: "Traditional Services" }]
    };
    let allowRemoval = {
        id: "allowRemoval",
        fieldType: "checkbox",
        label: "Allow Removal",
        hint: "Remove previously imported elements?",
        value: defaults.allowRemoval
    };
    let addPostfixes = {
        id: "addPostfixes",
        fieldType: "checkbox",
        label: "Add Postfixes",
        hint: "Add postfixes of Commands/Query/Services if they are missing.",
        value: defaults.addPostFixes
    };
    let settingPersistence = {
        id: "settingPersistence",
        fieldType: "select",
        label: "Persist Settings",
        hint: "Remember these settings for next time you run the import",
        value: defaults.settingPersistence,
        selectOptions: [{ id: "None", description: "(None)" }, { id: "All", description: "All Settings" }]
    };
    let formConfig = {
        title: "OpenApi Import",
        fields: [
            openApiFile,
            serviceType,
            allowRemoval,
            addPostfixes,
            settingPersistence,
        ]
    };
    let inputs = await dialogService.openForm(formConfig);
    let importConfig = {
        openApiSpecificationFile: inputs.openApiFile,
        packageId: element.getPackage().id,
        isAzureFunctions: false, //service sets this based on installed modules
        addPostFixes: inputs.addPostfixes === "true",
        allowRemoval: inputs.allowRemoval === "true",
        serviceType: inputs.serviceType,
        targetFolderId: element.specialization === "Folder" ? element.id : null,
        settingPersistence: inputs.settingPersistence,
    };
    let jsonResponse = await executeModuleTask("Intent.Modules.OpenApi.Importer.Tasks.OpenApiImport", JSON.stringify(importConfig));
    let result = JSON.parse(jsonResponse);
    if (result === null || result === void 0 ? void 0 : result.errorMessage) {
        await dialogService.error(result === null || result === void 0 ? void 0 : result.errorMessage);
    }
    else {
        if (result === null || result === void 0 ? void 0 : result.warnings) {
            await dialogService.warn("Import complete.\r\n\r\n" + (result === null || result === void 0 ? void 0 : result.warnings));
        }
        else {
            await dialogService.info("Import complete.");
        }
    }
}
function getDialogDefaults(element) {
    let package = element.getPackage();
    let result = {
        openApiFile: getSettingValue(package, "open-api-import:open-api-file", null),
        addPostFixes: getSettingValue(package, "open-api-import:add-postfixes", "true"),
        allowRemoval: getSettingValue(package, "open-api-import:allow-removal", "true"),
        serviceType: getSettingValue(package, "open-api-import:service-type", "CQRS"),
        settingPersistence: getSettingValue(package, "open-api-import:setting-persistence", "None"),
    };
    return result;
}
function getSettingValue(package, key, defaultValue) {
    let persistedValue = package.getMetadata(key);
    return persistedValue ? persistedValue : defaultValue;
}
