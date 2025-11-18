using System;
using System.Collections.Generic;
using System.Data;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Public
{
    internal static class UuidNsUrlResponseExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<UuidNsUrlResponse> uuidNsUrlResponses)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("UuidNsUrl", typeof(Guid));

            foreach (var item in uuidNsUrlResponses)
            {
                dataTable.Rows.Add(item.UuidNsUrl);
            }

            return dataTable;
        }
    }
}