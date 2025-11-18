using System;
using System.Collections.Generic;
using System.Data;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Public
{
    internal static class UuidNsOidResponseExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<UuidNsOidResponse> uuidNsOidResponses)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("UuidNsOid", typeof(Guid));

            foreach (var item in uuidNsOidResponses)
            {
                dataTable.Rows.Add(item.UuidNsOid);
            }

            return dataTable;
        }
    }
}