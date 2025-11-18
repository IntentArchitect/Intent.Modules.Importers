using System.Collections.Generic;
using System.Data;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Public
{
    internal static class GetOrderItemDetailsResponseExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<GetOrderItemDetailsResponse> getOrderItemDetailsResponses)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Result", typeof(string));

            foreach (var item in getOrderItemDetailsResponses)
            {
                dataTable.Rows.Add(item.Result);
            }

            return dataTable;
        }
    }
}