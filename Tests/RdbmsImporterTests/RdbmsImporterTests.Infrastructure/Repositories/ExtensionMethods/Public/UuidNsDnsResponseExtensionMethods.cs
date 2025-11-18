using System;
using System.Collections.Generic;
using System.Data;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Public
{
    internal static class UuidNsDnsResponseExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<UuidNsDnsResponse> uuidNsDnsResponses)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("UuidNsDns", typeof(Guid));

            foreach (var item in uuidNsDnsResponses)
            {
                dataTable.Rows.Add(item.UuidNsDns);
            }

            return dataTable;
        }
    }
}