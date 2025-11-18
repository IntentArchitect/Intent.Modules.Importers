using System;
using System.Collections.Generic;
using System.Data;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Public
{
    internal static class UuidNsX500ResponseExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<UuidNsX500Response> uuidNsX500Responses)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("UuidNsX500", typeof(Guid));

            foreach (var item in uuidNsX500Responses)
            {
                dataTable.Rows.Add(item.UuidNsX500);
            }

            return dataTable;
        }
    }
}