using System;
using System.Collections.Generic;
using System.Data;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Public
{
    internal static class UuidGenerateV3ResponseExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<UuidGenerateV3Response> uuidGenerateV3Responses)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("UuidGenerateV3", typeof(Guid));

            foreach (var item in uuidGenerateV3Responses)
            {
                dataTable.Rows.Add(item.UuidGenerateV3);
            }

            return dataTable;
        }
    }
}