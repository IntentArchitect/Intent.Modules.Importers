using System;
using System.Collections.Generic;
using System.Data;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Public
{
    internal static class UuidGenerateV1ResponseExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<UuidGenerateV1Response> uuidGenerateV1Responses)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("UuidGenerateV1", typeof(Guid));

            foreach (var item in uuidGenerateV1Responses)
            {
                dataTable.Rows.Add(item.UuidGenerateV1);
            }

            return dataTable;
        }
    }
}