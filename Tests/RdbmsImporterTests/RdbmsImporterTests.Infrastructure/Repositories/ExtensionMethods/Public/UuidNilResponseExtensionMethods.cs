using System;
using System.Collections.Generic;
using System.Data;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Public
{
    internal static class UuidNilResponseExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<UuidNilResponse> uuidNilResponses)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("UuidNil", typeof(Guid));

            foreach (var item in uuidNilResponses)
            {
                dataTable.Rows.Add(item.UuidNil);
            }

            return dataTable;
        }
    }
}