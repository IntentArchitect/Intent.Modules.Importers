using System;
using System.Collections.Generic;
using System.Data;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Public
{
    internal static class UuidGenerateV4ResponseExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<UuidGenerateV4Response> uuidGenerateV4Responses)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("UuidGenerateV4", typeof(Guid));

            foreach (var item in uuidGenerateV4Responses)
            {
                dataTable.Rows.Add(item.UuidGenerateV4);
            }

            return dataTable;
        }
    }
}