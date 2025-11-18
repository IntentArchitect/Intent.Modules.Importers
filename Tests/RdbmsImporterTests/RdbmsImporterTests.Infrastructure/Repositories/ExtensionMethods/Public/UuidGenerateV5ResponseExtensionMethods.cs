using System;
using System.Collections.Generic;
using System.Data;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Public
{
    internal static class UuidGenerateV5ResponseExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<UuidGenerateV5Response> uuidGenerateV5Responses)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("UuidGenerateV5", typeof(Guid));

            foreach (var item in uuidGenerateV5Responses)
            {
                dataTable.Rows.Add(item.UuidGenerateV5);
            }

            return dataTable;
        }
    }
}