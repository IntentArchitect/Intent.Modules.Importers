using System;
using System.Collections.Generic;
using System.Data;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Dbo
{
    internal static class GetCustomerOrdersResponseExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<GetCustomerOrdersResponse> getCustomerOrdersResponses)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("OrderDate", typeof(DateTime));
            dataTable.Columns.Add("RefNo", typeof(string));

            foreach (var item in getCustomerOrdersResponses)
            {
                dataTable.Rows.Add(item.OrderDate, item.RefNo);
            }

            return dataTable;
        }
    }
}