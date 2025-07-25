async function executeImporterModuleTask(taskTypeId, input) {
    let inputJsonString = JSON.stringify(input);
    console.log(`Executing Module Task ${taskTypeId} => ${inputJsonString}`);
    let moduleTaskResultString = await executeModuleTask(taskTypeId, inputJsonString);
    console.log(`Module Task ${taskTypeId} Completed => ${moduleTaskResultString}`);
    let executionResult = JSON.parse(moduleTaskResultString);
    return executionResult;
}
async function displayExecutionResultErrors(executionResult) {
    if (executionResult.errors.length === 0) {
        return;
    }
    await dialogService.error(executionResult.errors.join("\r\n"));
}
async function displayExecutionResultWarnings(executionResult, title) {
    if (executionResult.warnings.length === 0) {
        return;
    }
    await dialogService.warn(title + "\r\n\r\n" + executionResult.warnings.join("\r\n"));
}
class Icons {
}
Icons.databaseIcon = "data:image/png;base64,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";
Icons.schemaIcon = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAANpJREFUOE+tk0ESgjAMRX9mdC9cRG4ALPA8ehI8jyzAG8AJHC8A7mUmTklbgRFt1a4IbV9/8hOCWWW3wR1bG6uPNRqkwW3ybxaQjYsuAbic7lOKLKj8AFko0KJl4BsAYF5M/ACnLgLxEUCkJddg2mMX1G4pmFNFKwqyMHl30ey5FVGg8QSoa/UCQKkcHBxR8pWNEQjXIaUhRc6xCGAS6cQVCA1YA4CLBahaLSsYC9U2OqXwcxHFxnxm48Hdxmcrn7WY2K+RDOAPrTyqos8sqHHubRsLZIX60zg/AOCebhHZxIffAAAAAElFTkSuQmCC";
Icons.storedProcIcon = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAFAAAABQCAYAAACOEfKtAAAAAXNSR0IArs4c6QAAEi1JREFUeF7tXQt4lNWZft9z/pkEghcEBa2AZCYgskUFbauuVVdbL63tdivVWtvSdUWBJNLaZ9utdTe2unarLYVJUKm63qpd3LXV7dqrWG1rbb3VclFIAqJYKBcxICGZOed8+5x/MjCBSWaS+RNin57nGRKeOec97/n+7z+X73JClFEaGqAu24TRiMePEOdGkzIe4N8AqBHBkaSMBTgWQMWebgSyX5cEu31P2QJwkwCbIPK6giwXcA2htqRNZmsQw8aaFDrLoB5Z073E+wC5bi7GGhX7pIJcLERSgEovJAJBH2D6WtVBkAbQKZQ3FfCYg72/JoVn+goUZf2SBLhuFirdwcFpTnABBR8gMVUA1U8iLmwnXVrHUCP7iwWIbAbxSwf82In92bGN+FM/efWrWY8CFICvzschYvVMB3wd4JgSeugUhFqSYfazCZAVEL4BoJ1EO8h2ca4dRIcAMYoMd0oNpyD8QMlI53gciSSAOICAQLxLy4sKWkTut7D/av+MN6Y+FHIZ0FJQgKvmYUIcugHE2SDH9ciAMBC8LJBfK+A3znGtgt7GoLONbWibeDc6+steZkKvOhqHDBccnEnHDoV279LCk4RyOsD3QlCF/LkzryMC7U7k91RYlDjMPsIGZLV+AEo3AW6eixE7dGwWIF8DMLJAf50QrCVlhTg8XNFhfzbuTrw5ALx6hVzZgHiwOXiPVrhIiFMI1BTiK4ChyP+I6K/XNKVXDgTPPQJcPwcj0zpYRmLa/nOSdABMKWeaRgi2Hb4Yu+hnsSFQ/PycHoURqkOfD+Imku/an5a8BcFVyUb7X1FTDgX4Si0mBgx+CITCyy/NELckDvfA+EGenPsz0JUzEY+P0eeBnAPgnG67AiLtnLt6UqO7rT/YPbXhystxWMVw/SLA8XmVdhO4dVinue6oJWiPssPBwBIB183XFznLJhKj/YIY9isQreRjExfZR6LiwZb64Itw+GZuQvYrpxN8MjnaPDWQk29UA+gNZ918HGOtvhPg3+XqUbC8w5lTpy7G21FwYEtd4PdNR3aB7QRxXnKReToK8KGAIQ2It2wNfkrizC4+DhbnJBebJ6Lg5wW4dzEQSSVS9mpmN7d/MaW5NjiTwLLcW6aAG6pT5rooBpgvQFF0ddWLXFMUwEMJY80cVKsgaN3LSR5MpuylUXDspoEi+N2Gt82ZZ5WxAY6CVNQYzfNic6hkcQ5XIEtrUvbiKPrpLkCEJ4ubaxrNV6IAHwoYrXX6QwL+J4DDB1yAeSp+n7b2nycu9mfZd2Z5/fMYls6ozwrVAmStRXvKgGlgtz6AbXDu2mSTW/JOE2FrvbdHBvcCOBmA3pf/YAgw26c3fhIrANynnFla3YT1Q1WY/hzfFmhvbrsY4Ie7LDkF6Q6eAPNVUpABZJkibwmMeX7tGOw8qwHmQAnUny62zEXVTosjEQ/mCmQWwENL4XNABJhHzAlkE8GXIXiaIs+YjP395CXYWgr5cuo8NxuxkXEcD8ROA91pDnw3gQSAWF9wD7QAC3F1BFq8DQ6KqwBppeNWKLbBZNqMQrsmMoFGZkeATLAemYemwpwBqGQbYh0GsWEOsYwg1uFQWcH4wRJzBzsjIzVlvCjl3QYnAXIywL3+lb5IbZAXkX5S69bMeKu0wL/6sACcAEKExk3/8b/7I4+3Miv/UwEUCSf9mBCxLmtKSW6HvhAeihrYF/4HvO5fBVjmI/irAMsX4PdqUvayMmHC5t2tMVEgvgMwKO5fEo3uG1FQZUttsArElCjA3iEYGQVzcnUKL0XBl831+tMU3uX9r1EADnUMAr9SO80Hy3G55o+R/tDdmdE/B3naUB98BPze1qJOntiYfiUCrOwc6P9prsPBFH03yI9FBTzUcITYpYCPJhaZx6PkRh8o5M1WXhM7jG5U4MUCVEXZyQHG8u6JF7Soy7zmNdf5SLF4oiaVXhUFL7bUqWuSKfctD7Z0JvTxYzFDIzQFTY6igwOMsZvirq2qdEvG3oJdnkvoA4f+crLRXhkFN7+N+UFik7mID4XHrbD4VxrQFxH0e6WzouhocDHkdZIPwqn7qxvTK/KjKFrrg7MBmZVYZD8dBScvwK2w5sTkYrxeCLBlDmYw0N8AOEOAQ8oKRYuCcQ8YPqAIkM0CWZBMuUWFqkkDVOu22IMCh0h9IgKsYMacnbwNmwt17CNRP7MF4xz1iSA+CeACgCMGUB7FobPGXv/WvCQODwQxLsNbmVd62p74aIXWevVFgfp3QB6OVIAhW5HH4rBXlBIDs3Qm4tPG4G8DBOeCodl8NCFHOHBUlwXFT9xRWVFyWG2AbPV2VIisFoXHA2N/Xorf5unPY9gYo2YJ1C0Ahg/cWViwwTleOGlx5g/FVSBbQxoQrNqMyoMrEKfB8E6lT4CT6Q6cJOAkDUkKWChUrnAXRBoOrUJpUWCzE1kVc/bpjA4Ntj4uurMv8dHect1Sp+8j6N+cMEBz4ASYHdJuiNyndPDt6oWdq0sVZG/1Xvo0qoYfiuEug3hFgHgnUBEAMUdYWqQ7DdLDhqFTdqOjegl2RBE61zobh7i4+hTBa0BW5/MbaAHu6YuUxZK21yfG4k0eQP9HqQ8xq204CFZfSI1vQXhEoSjWQRNgF/HtpDwnIo92trsHph6AiNRiAvS+kkPj+hwQfmvizf4+vnqvIgCvQLBSKBsF3Kic/CHRZB8rhlvK9300Z4kP2v6RAP+nlFounZn1iduwJYpXrhSyuTp/mo3h6SA+wWpbLcL3C+RTeZGp3um1noKnQCyzgX188gL4IPcBKX0UYDcOPoB8lwjWEPiFAL9RafNMYgnaombqX831dTjBQPkA8zOA0PBRRWCYdHOcSysFX0La/uK+o7CzYQCDy3NjLEeA+8nJO4y8y1PADT5/gwLvmdsizm0DsRNgJ4kMBJ2OyFCgFREHEReReCgQUYcJZXQ2loWHExgLyDG9euNE1lPLN6t3u+9ySejE2lP82Vdc7ARFdwwoY0hVBQndoJ2A2yHCjVC2JXEYVvUnoNQbVF1P6QJRa1LUeGEUPvCUwFxSk8KWHH6YGFSF8QL9BVGcxazfuMccEwJWIFvpuMApc9eGUdheatCA10AfAz0s6sENBp4Q17R1mNRJeVrnDcQQzqXgBBIVe+KjSyPkN+1thPyKirdULzRPFWvmBbgQQH2xikPpewLbnciVNY32oRyv5jocR+qbITw/ilMQgd1OsFBr853qhfhzT+PnmvmYQhs8Q3gLzDujEHJpdcp+P7f6r63Xn3DC76K/Y8ieqwsePUXwsjPmA5NvK7ySZy3StcG5pNwP0E/eQ7lYJ7h+UqP5uifprSvN22K1CnIjgP4YN4xQlsLiESjeQeCgwoOXLRp25sQUntz3ez7RgMBPmOvqYu+zkJ8ga7IakoXA4w7mQ7mzcHN9bB6dpPqzCBKy0QkvrWk0v/SDXVsbfM0BX+0ZS3Zra0+duBjd7ARsnac/nGiyP/Igr9UhkRZ9A8hLhp4E5bVKZ6cf3YRtnltLvf4YhPegR63pcQQOkHvTzjYclxfvGKZDbAunshN7aimQdVrZU/LnRL+I3JhMmWvzGzXX6o+Q/BKyYENjhRZXn2x0Kc9zXS2OdQx+K0BJ8YDhHtRhNYkREJeqHu1uy+35vBvjfWMwJo3gMhA+Nrz3N1DwW6bN+bkDgxfgSyMqzKk5n0FOkE/MQuXRB8WmKLqbRejN+kVzdQdOa6VVYKfueXXr9KMELyypP8EqS14aH5VZXbUZwRFdGUreDLdmm/6cBud3JRr5h1GSDVPovlKzyN3k+2drXWCc4OYX/2y++ok8v0iOXGjJvTo+FeLOBXAeBKeCGF4S+SgqCURELqlpskvDBW+u/hQ17y8FWkTWxq09acKt2J6r/6drMHpXWs2m8HNgd6NDKZjZOrLRxu20yd/C1r1HOeeuvP9wd0ex82PLVTiCMfVxCR1O9J47L0wf9DggGiqQDYG27574HbzlTxjmoOBJAu8pYbBpiDz8Vtp+Jn+j3VIbfA1EuVlK/qHenWy0l+efhdsFcld7hf3y8V0uwGIkm+twOBkb78SN1UomwXGawH9kMsH++Zb37sl8POYWCh6uHm3n+TlrzVxMUTp4sdstIAVJynYI5nSOtj+Y2tA97b+1XjeJcG6xsRX7XoCttGZ6IWPCrzXw1Wc3mV8XeqWLAee+f/1yHJYZHhsnyo2zVo5WRJUIYqTyyXkxH4mqibTA7bbAbiX0KbZvK82NJm02MMAb+5rum2v1HSQvL8Lh2bQzM/0Ku/rzeJe2wXwIZliHGyc3mceb58WnUrvfQnra85U6Qlgl8g89WWN8av8yipmfaMKakiEHsGL4+o4IdpA9BpSLCFYEgXm/f93X1OqPU/E2Ckb5uV6ADenAHH/ct7G9tT64GcA15dOV24uZs3aL4HGl5OGYsz8txWNXPqnCCM3zYtOp5Pke8QXLM2LOGhFHZYfV11PCoID8gHRHcXWJRrc4G94RvEDguDL5ri4mwK5FJzwrpgXyQ9LerBVat7VjV/7kXCaRos1ba/XHhfzvHio6EVwwjGZlh+gnel5dZafttNU+JaO5Vl9O8o6iHRepUJoA94J4c4/xy7gIXyXgrxb5iRPzbE0KO8ol01v7llpVB6qCEQcAlinwCw7y47zk8YJwinLPcxvt5dUjUXFoReDNVTPK4d1XAfbUl4PIqyCXQ/BHoaym4E0l3JUm2ysD7lKW6V2ZThvEYNIWJhgGo9KgVlCKUBkbpjdUWBWv0sZVKYWqjJjWYxuxznfaXBd8hYA3GuxXnOAWAqeTeG9RYQjanFOnTFqcfnl9HY5LI/gd+2eICLsaMIt01rwPC4FF9oIe/9Mn5Pgifo+SG2zXL+yKavCRstr/LnDzkil3u6/XWhv8mxANhQREyHoBJxQVXlcFEbyUHG2m+/+2bNXfJzmz1Lbd6gkkKg3sV/8lNLoumTI3dAnwy0KEx6f9Si/2vJ77kM8mU/Zen83OIPg9Ea7WfS5srg2eJPH+PrcclAbSmEzZOt/V2jpV66BCY0IURQTLvRb6gIGWulgtIH5+LeksnN8/W2r1+QAfBrsnJUdBslwMAv+bSJmPhK9arf57kD8oFzO/vQhu8tn5/kQF6D8QPKov+NmrpWYj1hLXt5P8XF8aD0pd4o3EQjPO3yLyyhxMC4IgktSEPO47tJj3TmzEK83z9Eep6H0sfcn8/GOost6007pNLwBYOyiC6UMnpJmQWITXvN3uxLF6K0vMCS69C7kjmbJXhHE1tcEPSYQaX1LJrv7Z4uNLDonFZlO5awHmLuIpCWcgK1nnrpzcde1ARJaUfekaEuf56H2v5bEguEOyVwUUKxkS5+83aXpzFWLBAwBO7y1tvhh6hN8/kRhlPugn+1fn40hjw/tforaS/zIxynzA9+GPeRS9FGTvmijYDGN8wvf+xR/c7YhgBsh5gPjAxANZtmScmTKlCdv8fL22Uv9YhGdHTGj5rgpzSs6M11IXuxKQXm95I931iUWuoeiyvXo2jtXx4KpQI4ljuwyoEfPvHY6C6xONJtxEt9YF5/josGjfjtBo+56t7Wg7pAJVlODJXvMHRVoSo+0Ur7FFBZgbmk/EkQxGZhj4GLx/lOytkSW3L0fi/l7WuDFTJtyKtX6yb60NFoC4uhzMfdr6DHrv7fM/vWXd+8cLj827GMArahozd3qMfgmg64LaCc7q6UJOF5HJAGsASfbbEt27NHYD7rpcQtBrtTgqTf3kvoGUEQq0Ryh/yS1o/yln7O2XAHtC949mTT2mKegZACdQZALIY0RkPMlKZu9EUF3O6+yVCdljmDdHOUL8Lb/bFeRlJ1xJJSuc2D8mU3hj3yDOV7+IiaZDPw/2IYC9fAm/0GnNGfl3D0YqwN74ZfdxCNorEGR2IBhWBV25C/bt3TDThiGNJeF80qdr98JTFPnoIKXqrjRiLsxZh3JjHTQBlv/wCyOsrcPpjvpBSKHLZyPqlXjeODNzX+H1ew6MiFZUMFxZj3EVLngMxNSoQEMcb65S8r2quL1q38CDvxgNzA1kwzyM6lDBFwhcJcBhZQtSZC1E/YeozD29Jfa841/hfQXVWo/xIsFCIc6mhCFvfRmjz73bLg5Lkoeb67mPT7nQQ+kLeNkPdbAAfHLkJdsrauIwJ4tV5wrc6WCXxTr/z3Fkgyr9BWr+uqqnBPhZZWBfGLcg/IMGJS1o/w/AK9ivBEvoXAAAAABJRU5ErkJggg==";
Icons.tableIcon = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABkAAAAZCAMAAAGEMEXHAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAADYUExURQAAAP+AQNtJJNVVFcZVDrxRB79RBrtOCrlQCrlPCcJSCbtRCb1QCLtRCL9RCr5SB75RB79QB75SCLxSCL5SCL1SCL1SCL1RCb1RCb1QCb5RCb5RCb5SCb5SCL5RCb1RB79SB79RCL9RCL9QCL5RCL9RCL1RCL5QCL9RCL5RCL5QCL5SCL5RCb1QB79RCL5RCL5SCL5RCMJTCL1RCL5RCL9RCMBSCMFSCMJTCMNTCMRUCMVTCMVUCMZUCMZVCMdUCMdVCMhVCMlVCMlWCMpVCMtWCc1YCc9YCZouWWgAAAAzdFJOUwAEBwwSJiwxMzc7PF1eaGpub3l6fYCDh4uMjpGWmaSrrra3uLm6wcXGx8jI0NLT3OHn+hd6f9EAAAAJcEhZcwAAC4gAAAuIAeWOKUkAAAExSURBVChTnZFnc8IwDIZFMdOMMsIKewXMxmRBUmJW/v8/qhLMFa7XfuDR2a9l6V75zhAifF/ctyrn1fudRAgBuztgK18YNgjwMYTskGWZQLFcKpWLMnlwRhtxhvH5hzG4CZq6JmnymqIJF7wIid4+yMctSiJeaO7LJcDQ2NTSmGZNmWbA6akWnJ/wpDniPZ4dAh1KsxVKK1lKO6ACxDMAmTjgsUVIOkdILk1IC2aK0uwqSrepKLPXtpdkw9hixdhqwdjmn7atEO5eiL0rxBZi+cLnbwr5GBi6fOUrugE70w04cDtUmx9CNXcgcBAS93EYkvFxIKLiP/5Z4fblhBy5E+jF4ccgvdj8LTdz3RuNRoOJteyj9pfWZIDaW5tvuenzmqqqjbYzrKPWh067gVqb69+bOUVi00YNywAAAABJRU5ErkJggg==";
Icons.viewIcon = "data:image/png;base64,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";
/// <reference path="./common.ts" />
/// <reference path="./icons.ts" />
class DatabaseImportStrategy {
    async execute(packageElement) {
        var _a, _b;
        let defaults = this.getDialogDefaults(packageElement);
        let capturedInput = await this.presentImportDialog(defaults, packageElement.id);
        if (capturedInput == null) {
            return;
        }
        let importModel = this.createImportModel(capturedInput);
        let executionResult = await executeImporterModuleTask("Intent.Modules.Rdbms.Importer.Tasks.DatabaseImport", importModel);
        if (((_a = executionResult.errors) !== null && _a !== void 0 ? _a : []).length > 0) {
            await displayExecutionResultErrors(executionResult);
        }
        else if (((_b = executionResult.warnings) !== null && _b !== void 0 ? _b : []).length > 0) {
            await displayExecutionResultWarnings(executionResult, "Import Complete.");
        }
        else {
            await dialogService.info("Import Complete.");
        }
    }
    getDialogDefaults(element) {
        let domainPackage = element.getPackage();
        let persistedValue = this.getSettingValue(domainPackage, "rdbms-import:typesToExport", "");
        let includeTables = "true";
        let includeViews = "true";
        let includeStoredProcedures = "true";
        let includeIndexes = "true";
        if (persistedValue) {
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
        let result = {
            entityNameConvention: this.getSettingValue(domainPackage, "rdbms-import:entityNameConvention", "SingularEntity"),
            tableStereotypes: this.getSettingValue(domainPackage, "rdbms-import:tableStereotypes", "WhenDifferent"),
            includeTables: includeTables,
            includeViews: includeViews,
            includeStoredProcedures: includeStoredProcedures,
            includeIndexes: includeIndexes,
            importFilterFilePath: this.getSettingValue(domainPackage, "rdbms-import:importFilterFilePath", null),
            connectionString: this.getSettingValue(domainPackage, "rdbms-import:connectionString", null),
            storedProcedureType: this.getSettingValue(domainPackage, "rdbms-import:storedProcedureType", ""),
            settingPersistence: this.getSettingValue(domainPackage, "rdbms-import:settingPersistence", "None"),
            databaseType: this.getSettingValue(domainPackage, "rdbms-import:databaseType", "SqlServer")
        };
        return result;
    }
    async presentImportDialog(defaults, packageId) {
        let formConfig = {
            title: "RDBMS Import",
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
                            onClick: async (form) => {
                                var _a;
                                let testConnectionModel = {
                                    connectionString: form.getField("connectionString").value,
                                    databaseType: form.getField("databaseType").value
                                };
                                let executionResult = await executeImporterModuleTask("Intent.Modules.Rdbms.Importer.Tasks.TestConnection", testConnectionModel);
                                if (((_a = executionResult.errors) !== null && _a !== void 0 ? _a : []).length > 0) {
                                    form.getField("connectionStringTest").hint = "Failed to connect.";
                                    await displayExecutionResultErrors(executionResult);
                                }
                                else {
                                    form.getField("connectionStringTest").hint = "Connection established successfully.";
                                    await dialogService.info("Connection established successfully.");
                                }
                            }
                        },
                        {
                            id: "settingPersistence",
                            fieldType: "select",
                            label: "Persist Settings",
                            hint: "Remember these settings for next time you run the import",
                            value: defaults.settingPersistence,
                            selectOptions: [
                                { id: "None", description: "(None)" },
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
                            selectOptions: [{ id: "SingularEntity", description: "Singularized table name" }, { id: "MatchTable", description: "Table name, as is" }]
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
                    name: 'Filtering',
                    fields: [
                        {
                            id: "includeTables",
                            fieldType: "checkbox",
                            label: "Include Tables",
                            value: defaults.includeTables
                        },
                        {
                            id: "includeViews",
                            fieldType: "checkbox",
                            label: "Include Views",
                            value: defaults.includeViews
                        },
                        {
                            id: "includeStoredProcedures",
                            fieldType: "checkbox",
                            label: "Include Stored Procedures",
                            value: defaults.includeStoredProcedures
                        },
                        {
                            id: "includeIndexes",
                            fieldType: "checkbox",
                            label: "Include Indexes",
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
                                var _a, _b, _c;
                                const selectedFilePath = form.getField("importFilterFilePath").value;
                                if (!selectedFilePath) {
                                    return;
                                }
                                // Use the new PathResolution task to resolve the path
                                const pathResolutionModel = {
                                    selectedFilePath: selectedFilePath,
                                    applicationId: application.id,
                                    packageId: packageId
                                };
                                const pathResolutionResult = await executeImporterModuleTask("Intent.Modules.Rdbms.Importer.Tasks.PathResolution", pathResolutionModel);
                                if (((_a = pathResolutionResult.errors) !== null && _a !== void 0 ? _a : []).length === 0 && ((_b = pathResolutionResult.result) === null || _b === void 0 ? void 0 : _b.resolvedPath)) {
                                    form.getField("importFilterFilePath").value = pathResolutionResult.result.resolvedPath;
                                }
                                else if (((_c = pathResolutionResult.errors) !== null && _c !== void 0 ? _c : []).length > 0) {
                                    await displayExecutionResultErrors(pathResolutionResult);
                                    return;
                                }
                            }
                        },
                        {
                            id: "manageIncludeFilters",
                            fieldType: "button",
                            label: "Manage Filters",
                            onClick: async (form) => {
                                const connectionString = form.getField("connectionString").value;
                                if (!connectionString) {
                                    await dialogService.error("Please enter a connection string first.");
                                    return;
                                }
                                const databaseType = form.getField("databaseType").value;
                                let importFilterFilePath = form.getField("importFilterFilePath").value;
                                let returnedImportFilterFilePath = await this.presentManageFiltersDialog(connectionString, databaseType, packageId, importFilterFilePath);
                                if (returnedImportFilterFilePath != null) {
                                    form.getField("importFilterFilePath").value = returnedImportFilterFilePath;
                                }
                            }
                        }
                    ],
                    isCollapsed: true,
                    isHidden: false
                }
            ],
            height: "70%"
        };
        let capturedInput = await dialogService.openForm(formConfig);
        return capturedInput;
    }
    createImportModel(capturedInput) {
        const typesToExport = [];
        if (capturedInput.includeTables == "true") {
            typesToExport.push("Table");
        }
        if (capturedInput.includeViews == "true") {
            typesToExport.push("View");
        }
        if (capturedInput.includeStoredProcedures == "true") {
            typesToExport.push("StoredProcedure");
        }
        if (capturedInput.includeIndexes == "true") {
            typesToExport.push("Index");
        }
        const domainDesignerId = "6ab29b31-27af-4f56-a67c-986d82097d63";
        let importConfig = {
            applicationId: application.id,
            designerId: domainDesignerId,
            packageId: element.getPackage().id,
            entityNameConvention: capturedInput.entityNameConvention,
            tableStereotype: capturedInput.tableStereotypes,
            typesToExport: typesToExport,
            importFilterFilePath: capturedInput.importFilterFilePath,
            storedProcedureType: capturedInput.storedProcedureType,
            connectionString: capturedInput.connectionString,
            settingPersistence: capturedInput.settingPersistence,
            databaseType: capturedInput.databaseType
        };
        return importConfig;
    }
    async presentManageFiltersDialog(connectionString, databaseType, packageId, importFilterFilePath) {
        var _a, _b;
        try {
            const metadata = await this.fetchDatabaseMetadata(connectionString, databaseType);
            if (!metadata) {
                return null;
            }
            // Load existing filter data if file path exists
            let existingFilter = null;
            if (importFilterFilePath) {
                const filterLoadModel = {
                    importFilterFilePath: importFilterFilePath,
                    applicationId: application.id,
                    packageId: packageId
                };
                const filterLoadResult = await executeImporterModuleTask("Intent.Modules.Rdbms.Importer.Tasks.FilterLoad", filterLoadModel);
                if (((_a = filterLoadResult.errors) !== null && _a !== void 0 ? _a : []).length === 0 && filterLoadResult.result) {
                    existingFilter = filterLoadResult.result;
                }
                else if (((_b = filterLoadResult.errors) !== null && _b !== void 0 ? _b : []).length > 0) {
                    await displayExecutionResultErrors(filterLoadResult);
                    return null;
                }
            }
            const inclusiveSelection = {
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
                            autoSelectChildren: false,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "Table",
                            autoSelectChildren: false,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "Stored-Procedure",
                            autoSelectChildren: false,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "View",
                            autoSelectChildren: false,
                            isSelectable: (x) => true
                        }
                    ]
                }
            };
            const exclusiveSelection = {
                id: "exclusiveSelection",
                fieldType: "tree-view",
                label: "Objects to Exclude from Import",
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
                            autoSelectChildren: false,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "Table",
                            autoSelectChildren: false,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "Stored-Procedure",
                            autoSelectChildren: false,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "View",
                            autoSelectChildren: false,
                            isSelectable: (x) => true
                        }
                    ]
                }
            };
            let allSchemas = [...Object.keys(metadata.tables), ...Object.keys(metadata.storedProcedures), ...Object.keys(metadata.views)];
            let distinctSchemas = [...new Set(allSchemas)];
            // Create tree nodes with pre-selected states for inclusive filter
            inclusiveSelection.treeViewOptions.rootNode = {
                id: "Database",
                label: "Database",
                specializationId: "Database",
                icon: Icons.databaseIcon,
                children: distinctSchemas.map(schemaName => {
                    return {
                        id: `schema.${schemaName}`,
                        label: schemaName,
                        specializationId: "Schema",
                        icon: Icons.schemaIcon,
                        isSelected: this.isSchemaIncluded(schemaName, existingFilter),
                        children: this.createSchemaTreeNodes(schemaName, metadata, existingFilter, "include")
                    };
                })
            };
            // Create tree nodes with pre-selected states for exclusive filter
            exclusiveSelection.treeViewOptions.rootNode = {
                id: "Database",
                label: "Database",
                specializationId: "Database",
                icon: Icons.databaseIcon,
                children: distinctSchemas.map(schemaName => {
                    return {
                        id: `schema.${schemaName}`,
                        label: schemaName,
                        specializationId: "Schema",
                        icon: Icons.schemaIcon,
                        isSelected: this.isSchemaExcluded(schemaName, existingFilter),
                        children: this.createSchemaTreeNodes(schemaName, metadata, existingFilter, "exclude")
                    };
                })
            };
            const includeDependantTablesField = {
                id: "includeDependantTables",
                fieldType: "checkbox",
                label: "Include Dependant Tables",
                hint: "When including tables, also include tables that are referenced by foreign keys",
                value: (existingFilter === null || existingFilter === void 0 ? void 0 : existingFilter.include_dependant_tables) ? "true" : "false",
                isRequired: false
            };
            const formConfig = {
                title: "Manage Filters",
                fields: [],
                sections: [
                    {
                        name: "General Options",
                        fields: [includeDependantTablesField],
                        isCollapsed: false,
                        isHidden: false
                    },
                    {
                        name: "Inclusive Objects",
                        fields: [inclusiveSelection],
                        isCollapsed: false,
                        isHidden: false
                    },
                    {
                        name: "Exclusive Objects",
                        fields: [exclusiveSelection],
                        isCollapsed: true,
                        isHidden: false
                    }
                ]
            };
            try {
                const result = await dialogService.openForm(formConfig);
                if (result) {
                    // Handle the save operation when dialog is closed with OK
                    let returnedImportFilterFilePath = await this.saveFilterData(result, packageId, importFilterFilePath);
                    if (returnedImportFilterFilePath != null) {
                        return returnedImportFilterFilePath;
                    }
                }
            }
            catch (error) {
                console.error("Error in filter selection dialog:", error);
                return null;
            }
        }
        catch (error) {
            await dialogService.error(`Error loading database metadata: ${error}`);
            return null;
        }
        return importFilterFilePath;
    }
    async fetchDatabaseMetadata(connectionString, databaseType) {
        var _a;
        // Get database metadata
        const metadataModel = {
            connectionString: connectionString,
            databaseType: databaseType
        };
        const metadataExecutionResult = await executeImporterModuleTask("Intent.Modules.Rdbms.Importer.Tasks.RetrieveDatabaseObjects", metadataModel);
        if (((_a = metadataExecutionResult.errors) !== null && _a !== void 0 ? _a : []).length > 0) {
            await displayExecutionResultErrors(metadataExecutionResult);
            return null;
        }
        const metadata = metadataExecutionResult.result;
        if (!metadata) {
            await dialogService.error("No database metadata received.");
            return null;
        }
        return metadata;
    }
    createSchemaTreeNodes(schemaName, metadata, existingFilter = null, filterType) {
        var _a, _b, _c;
        const nodes = [];
        if ((_a = metadata.tables[schemaName]) === null || _a === void 0 ? void 0 : _a.some(x => x)) {
            nodes.push(this.createCategoryNode(schemaName, "tables", "Tables", "Table", Icons.tableIcon, metadata.tables[schemaName], existingFilter, filterType));
        }
        if ((_b = metadata.storedProcedures[schemaName]) === null || _b === void 0 ? void 0 : _b.some(x => x)) {
            nodes.push(this.createCategoryNode(schemaName, "storedProcedures", "Stored Procedures", "Stored-Procedure", Icons.storedProcIcon, metadata.storedProcedures[schemaName], existingFilter, filterType));
        }
        if ((_c = metadata.views[schemaName]) === null || _c === void 0 ? void 0 : _c.some(x => x)) {
            nodes.push(this.createCategoryNode(schemaName, "views", "Views", "View", Icons.viewIcon, metadata.views[schemaName], existingFilter, filterType));
        }
        return nodes;
    }
    createCategoryNode(schemaName, category, label, specializationId, icon, items, existingFilter = null, filterType) {
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
                isSelected: this.isItemSelected(schemaName, category, item, existingFilter, filterType)
            }))
        };
    }
    getSettingValue(domainPackage, key, defaultValue) {
        let persistedValue = domainPackage.getMetadata(key);
        return persistedValue ? persistedValue : defaultValue;
    }
    isSchemaIncluded(schemaName, existingFilter) {
        if (!existingFilter) {
            return false;
        }
        return existingFilter.schemas.includes(schemaName);
    }
    isSchemaExcluded(schemaName, existingFilter) {
        var _a, _b, _c, _d, _e, _f;
        if (!existingFilter) {
            return false;
        }
        // Check if any tables, views, or stored procedures from this schema are in exclude lists
        const excludedTables = (_b = (_a = existingFilter.exclude_tables) === null || _a === void 0 ? void 0 : _a.some(table => table.startsWith(`${schemaName}.`))) !== null && _b !== void 0 ? _b : false;
        const excludedViews = (_d = (_c = existingFilter.exclude_views) === null || _c === void 0 ? void 0 : _c.some(view => view.startsWith(`${schemaName}.`))) !== null && _d !== void 0 ? _d : false;
        const excludedStoredProcs = (_f = (_e = existingFilter.exclude_stored_procedures) === null || _e === void 0 ? void 0 : _e.some(sp => sp.startsWith(`${schemaName}.`))) !== null && _f !== void 0 ? _f : false;
        return excludedTables || excludedViews || excludedStoredProcs;
    }
    isCategorySelected(schemaName, category, existingFilter, filterType) {
        var _a, _b, _c, _d, _e, _f;
        if (!existingFilter) {
            return false;
        }
        if (filterType === "include") {
            switch (category) {
                case "tables":
                    return (_a = existingFilter.include_tables) === null || _a === void 0 ? void 0 : _a.some(x => x.name.includes(`${schemaName}.`));
                case "views":
                    return (_b = existingFilter.include_views) === null || _b === void 0 ? void 0 : _b.some(x => x.name.includes(`${schemaName}.`));
                case "storedProcedures":
                    return (_c = existingFilter.include_stored_procedures) === null || _c === void 0 ? void 0 : _c.some(x => x.includes(`${schemaName}.`));
                default:
                    return false;
            }
        }
        else {
            switch (category) {
                case "tables":
                    return (_d = existingFilter.exclude_tables) === null || _d === void 0 ? void 0 : _d.some(x => x.includes(`${schemaName}.`));
                case "views":
                    return (_e = existingFilter.exclude_views) === null || _e === void 0 ? void 0 : _e.some(x => x.includes(`${schemaName}.`));
                case "storedProcedures":
                    return (_f = existingFilter.exclude_stored_procedures) === null || _f === void 0 ? void 0 : _f.some(x => x.includes(`${schemaName}.`));
                default:
                    return false;
            }
        }
    }
    isItemSelected(schemaName, category, item, existingFilter, filterType) {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m;
        if (!existingFilter) {
            return false;
        }
        const fullItemName = `${schemaName}.${item}`;
        if (filterType === "include") {
            switch (category) {
                case "tables":
                    return (_b = (_a = existingFilter.include_tables) === null || _a === void 0 ? void 0 : _a.some(table => table.name === fullItemName)) !== null && _b !== void 0 ? _b : false;
                case "views":
                    return (_d = (_c = existingFilter.include_views) === null || _c === void 0 ? void 0 : _c.some(view => view.name === fullItemName)) !== null && _d !== void 0 ? _d : false;
                case "storedProcedures":
                    return (_f = (_e = existingFilter.include_stored_procedures) === null || _e === void 0 ? void 0 : _e.includes(fullItemName)) !== null && _f !== void 0 ? _f : false;
                default:
                    return false;
            }
        }
        else {
            switch (category) {
                case "tables":
                    return (_h = (_g = existingFilter.exclude_tables) === null || _g === void 0 ? void 0 : _g.includes(fullItemName)) !== null && _h !== void 0 ? _h : false;
                case "views":
                    return (_k = (_j = existingFilter.exclude_views) === null || _j === void 0 ? void 0 : _j.includes(fullItemName)) !== null && _k !== void 0 ? _k : false;
                case "storedProcedures":
                    return (_m = (_l = existingFilter.exclude_stored_procedures) === null || _l === void 0 ? void 0 : _l.includes(fullItemName)) !== null && _m !== void 0 ? _m : false;
                default:
                    return false;
            }
        }
    }
    async saveFilterData(formResult, packageId, importFilterFilePath) {
        var _a;
        try {
            // Extract selections from form result
            const inclusiveSelections = formResult.inclusiveSelection || [];
            const exclusiveSelections = formResult.exclusiveSelection || [];
            // Create filter model from selections
            const filterModel = {
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
            // Process inclusive selections
            inclusiveSelections.forEach((selection) => {
                if (selection.startsWith('schema.')) {
                    const schemaName = selection.replace('schema.', '');
                    filterModel.schemas.push(schemaName);
                }
                else if (selection.includes('.tables.')) {
                    // Extract schema and table name from ID like "schema.tables.tableName"
                    const parts = selection.split('.');
                    console.log(`INCLUDE ${selection} => ${JSON.stringify(parts)}`);
                    if (parts.length >= 3) {
                        const schemaName = parts[0];
                        const tableName = parts[2];
                        const fullTableName = `${schemaName}.${tableName}`;
                        const filterTableModel = { name: fullTableName, exclude_columns: [] };
                        filterModel.include_tables.push(filterTableModel);
                    }
                }
                else if (selection.includes('.views.')) {
                    // Extract schema and view name from ID like "schema.views.viewName"
                    const parts = selection.split('.');
                    if (parts.length >= 3) {
                        const schemaName = parts[0];
                        const viewName = parts[2];
                        const fullViewName = `${schemaName}.${viewName}`;
                        filterModel.include_views.push({ name: fullViewName, exclude_columns: [] });
                    }
                }
                else if (selection.includes('.storedProcedures.')) {
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
            exclusiveSelections.forEach((selection) => {
                if (selection.includes('.tables.')) {
                    // Extract schema and table name from ID like "schema.tables.tableName"
                    const parts = selection.split('.');
                    if (parts.length >= 3) {
                        const schemaName = parts[0];
                        const tableName = parts[2];
                        const fullTableName = `${schemaName}.${tableName}`;
                        filterModel.exclude_tables.push(fullTableName);
                    }
                }
                else if (selection.includes('.views.')) {
                    // Extract schema and view name from ID like "schema.views.viewName"
                    const parts = selection.split('.');
                    if (parts.length >= 3) {
                        const schemaName = parts[0];
                        const viewName = parts[2];
                        const fullViewName = `${schemaName}.${viewName}`;
                        filterModel.exclude_views.push(fullViewName);
                    }
                }
                else if (selection.includes('.storedProcedures.')) {
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
                const fileNameConfig = {
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
            const saveResult = await executeImporterModuleTask("Intent.Modules.Rdbms.Importer.Tasks.FilterSave", saveModel);
            if (((_a = saveResult.errors) !== null && _a !== void 0 ? _a : []).length > 0) {
                await displayExecutionResultErrors(saveResult);
                return null;
            }
            else {
                await dialogService.info("Filters saved successfully.");
            }
            return savePath;
        }
        catch (error) {
            await dialogService.error(`Error saving filters: ${error}`);
            return null;
        }
    }
}
/// <reference path="./common.ts" />
/// <reference path="./icons.ts" />
class StoredProceduresImportStrategy {
    async execute(repositoryElement) {
        var _a, _b;
        let defaults = this.getDialogDefaults(repositoryElement);
        let capturedInput = await this.presentImportDialog(defaults);
        if (capturedInput == null) {
            return;
        }
        let importModel = await this.createImportModel(capturedInput);
        if (importModel == null) {
            return;
        }
        let executionResult = await executeImporterModuleTask("Intent.Modules.Rdbms.Importer.Tasks.StoredProcedureImport", importModel);
        if (((_a = executionResult.errors) === null || _a === void 0 ? void 0 : _a.length) > 0) {
            await displayExecutionResultErrors(executionResult);
        }
        else if (((_b = executionResult.warnings) === null || _b === void 0 ? void 0 : _b.length) > 0) {
            await displayExecutionResultWarnings(executionResult, "Import Complete.");
        }
        else {
            await dialogService.info("Import Complete.");
        }
    }
    getDialogDefaults(element) {
        let domainPackage = element.getPackage();
        let result = {
            inheritedConnectionString: this.getSettingValue(domainPackage, "rdbms-import:connectionString", null),
            inheritedDatabaseType: this.getSettingValue(domainPackage, "rdbms-import:databaseType", null),
            connectionString: this.getSettingValue(domainPackage, "rdbms-import-repository:connectionString", null),
            storedProcedureType: this.getSettingValue(domainPackage, "rdbms-import-repository:storedProcedureType", ""),
            storedProcNames: "",
            settingPersistence: this.getSettingValue(domainPackage, "rdbms-import-repository:settingPersistence", "None"),
            databaseType: this.getSettingValue(domainPackage, "rdbms-import-repository:databaseType", "")
        };
        return result;
    }
    async presentImportDialog(defaults) {
        let formConfig = {
            title: "RDBMS Import",
            fields: [
                {
                    id: "connectionString",
                    fieldType: "text",
                    label: "Connection String",
                    placeholder: "(optional if inherited setting)",
                    hint: null,
                    value: defaults.connectionString
                },
                {
                    id: "databaseType",
                    fieldType: "select",
                    label: "Database Type",
                    value: defaults.databaseType,
                    selectOptions: [
                        { id: "", description: "(default or inherited setting)" },
                        { id: "SqlServer", description: "SQL Server" },
                        { id: "PostgreSQL", description: "PostgreSQL" },
                    ]
                },
                {
                    id: "storedProcedureType",
                    fieldType: "select",
                    label: "Stored Procedure Representation",
                    value: defaults.storedProcedureType,
                    selectOptions: [
                        { id: "", description: "(default or inherited setting)" },
                        { id: "StoredProcedureElement", description: "Stored Procedure Element" },
                        { id: "RepositoryOperation", description: "Stored Procedure Operation" }
                    ]
                },
                {
                    id: "storedProcNames",
                    fieldType: "text",
                    label: "Stored Procedure Names",
                    placeholder: "Enter Stored Procedure names (comma-separated) or use Browse button",
                    hint: "Enter Stored procedure names (comma-separated) or use the browse button.",
                    value: defaults.storedProcNames,
                    isRequired: true
                },
                {
                    id: "storedProcBrowse",
                    fieldType: "button",
                    label: "Browse",
                    onClick: async (form) => {
                        const connectionStringValue = form.getField("connectionString").value;
                        const settingPersistenceValue = form.getField("settingPersistence").value;
                        const databaseTypeValue = form.getField("databaseType").value;
                        if (settingPersistenceValue != "InheritDb" && (connectionStringValue == null || (connectionStringValue === null || connectionStringValue === void 0 ? void 0 : connectionStringValue.trim()) === "")) {
                            await dialogService.error("Please enter a connection string (or inherit DB settings) before browsing stored procedures.");
                            return;
                        }
                        if (settingPersistenceValue != "InheritDb" && (!databaseTypeValue || (databaseTypeValue === null || databaseTypeValue === void 0 ? void 0 : databaseTypeValue.trim()) === "")) {
                            await dialogService.error("Database Type was not set.");
                            return null;
                        }
                        let connectionStringStr = settingPersistenceValue == "InheritDb" ? defaults.inheritedConnectionString : connectionStringValue;
                        let dataTypeStr = settingPersistenceValue == "InheritDb" ? defaults.inheritedDatabaseType : databaseTypeValue;
                        try {
                            let storedProcNames = form.getField("storedProcNames").value;
                            let capturedStoredProcs = (storedProcNames).split(",").map(x => x.trim());
                            const selectedProcs = await this.openStoredProcedureBrowseDialog(connectionStringStr, dataTypeStr, capturedStoredProcs);
                            if (selectedProcs.length > 0) {
                                const storedProcNamesField = form.getField("storedProcNames");
                                storedProcNamesField.value = selectedProcs.join(", ");
                            }
                        }
                        catch (e) {
                            await dialogService.error("Error browsing stored procedures: " + e);
                        }
                    }
                },
                {
                    id: "settingPersistence",
                    fieldType: "select",
                    label: "Persist Settings",
                    hint: "Remember these settings for next time you run the import",
                    value: defaults.settingPersistence,
                    selectOptions: [
                        { id: "None", description: "(None)" },
                        { id: "InheritDb", description: "Inherit Database Settings" },
                        { id: "All", description: "All Settings" },
                        { id: "AllSanitisedConnectionString", description: "All (with Sanitized connection string, no password))" },
                        { id: "AllWithoutConnectionString", description: "All (without connection string))" }
                    ]
                }
            ]
        };
        let capturedInput = await dialogService.openForm(formConfig);
        return capturedInput;
    }
    async createImportModel(capturedInput) {
        var _a, _b;
        if (capturedInput.settingPersistence != "InheritDb" && (!capturedInput.connectionString || ((_a = capturedInput.connectionString) === null || _a === void 0 ? void 0 : _a.trim()) === "")) {
            await dialogService.error("Connection String was not set.");
            return null;
        }
        if (capturedInput.settingPersistence != "InheritDb" && (!capturedInput.databaseType || ((_b = capturedInput.databaseType) === null || _b === void 0 ? void 0 : _b.trim()) === "")) {
            await dialogService.error("Database Type was not set.");
            return null;
        }
        const storedProcNamesArray = capturedInput.storedProcNames.split(',').map((name) => name.trim());
        const domainDesignerId = "6ab29b31-27af-4f56-a67c-986d82097d63";
        let importConfig = {
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
    getSettingValue(domainPackage, key, defaultValue) {
        let persistedValue = domainPackage.getMetadata(key);
        return persistedValue ? persistedValue : defaultValue;
    }
    async openStoredProcedureBrowseDialog(connectionString, databaseType, preSelectedStoredProcs) {
        var _a;
        let inputProcs = this.sanitizePreSelectedStoredProcs(preSelectedStoredProcs);
        let storedProcSelection = {
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
        try {
            const input = {
                connectionString: connectionString,
                databaseType: databaseType
            };
            let executionResult = await executeImporterModuleTask("Intent.Modules.Rdbms.Importer.Tasks.StoredProcList", input);
            if (((_a = executionResult.errors) === null || _a === void 0 ? void 0 : _a.length) > 0) {
                await displayExecutionResultErrors(executionResult);
                return [];
            }
            let spListResult = executionResult.result;
            storedProcSelection.treeViewOptions.rootNode = {
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
                            };
                        })
                    };
                })
            };
        }
        catch (e) {
            await dialogService.error(e);
            return [];
        }
        let browseFormConfig = {
            title: "Browse Stored Procedures",
            fields: [
                storedProcSelection
            ]
        };
        let browseInputs = await dialogService.openForm(browseFormConfig);
        if (browseInputs && browseInputs.storedProcSelection) {
            let selection = browseInputs.storedProcSelection;
            let filteredSelection = selection.filter(x => !x.startsWith("schema."));
            return filteredSelection
                .map(x => {
                let parts = x.split(".");
                return `${parts[1]}.${parts[2]}`;
            });
        }
        return [];
    }
    sanitizePreSelectedStoredProcs(preSelectedStoredProcs) {
        if (preSelectedStoredProcs == null || preSelectedStoredProcs.filter(x => x != "").length === 0) {
            return [];
        }
        return preSelectedStoredProcs.map(x => !x.startsWith("dbo.") ? `sp.dbo.${x}` : `sp.${x}`);
    }
}
/// <reference path="./strategy-database-import.ts" />
/// <reference path="./strategy-stored-procedures-import.ts" />
// noinspection JSUnusedGlobalSymbols
let RdbmsImporterApi = {
    importDatabase,
    importStoredProcedures
};
async function importDatabase(packageElement) {
    const strategy = new DatabaseImportStrategy();
    await strategy.execute(packageElement);
}
async function importStoredProcedures(repositoryElement) {
    const strategy = new StoredProceduresImportStrategy();
    await strategy.execute(repositoryElement);
}
