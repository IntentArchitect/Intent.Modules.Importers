using System;
using System.Collections.Generic;
using System.Data;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Dbo
{
    internal static class GetProductDetailsAndCountResponseExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<GetProductDetailsAndCountResponse> getProductDetailsAndCountResponses)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(Guid));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("IsActive", typeof(bool));
            dataTable.Columns.Add("CurrentPrice", typeof(decimal));
            dataTable.Columns.Add("TotalPriceRecords", typeof(int));

            foreach (var item in getProductDetailsAndCountResponses)
            {
                dataTable.Rows.Add(item.Id, item.Name, item.Description, item.IsActive, item.CurrentPrice, item.TotalPriceRecords);
            }

            return dataTable;
        }
    }
}