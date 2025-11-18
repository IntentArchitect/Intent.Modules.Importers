using System;
using System.Collections.Generic;
using System.Data;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Public
{
    internal static class UuidGenerateV1mcResponseExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<UuidGenerateV1mcResponse> uuidGenerateV1mcResponses)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("UuidGenerateV1mc", typeof(Guid));

            foreach (var item in uuidGenerateV1mcResponses)
            {
                dataTable.Rows.Add(item.UuidGenerateV1mc);
            }

            return dataTable;
        }
    }
}