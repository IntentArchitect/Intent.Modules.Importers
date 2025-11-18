using System;
using System.Collections.Generic;
using System.Data;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Dbo
{
    internal static class GetOrderItemDetailsResponseExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<GetOrderItemDetailsResponse> getOrderItemDetailsResponses)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(Guid));
            dataTable.Columns.Add("OrderId", typeof(Guid));
            dataTable.Columns.Add("Quantity", typeof(int));
            dataTable.Columns.Add("Amount", typeof(decimal));
            dataTable.Columns.Add("ProductId", typeof(Guid));
            dataTable.Columns.Add("RefNo", typeof(string));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Surname", typeof(string));
            dataTable.Columns.Add("Email", typeof(string));

            foreach (var item in getOrderItemDetailsResponses)
            {
                dataTable.Rows.Add(item.Id, item.OrderId, item.Quantity, item.Amount, item.ProductId, item.RefNo, item.Name, item.Surname, item.Email);
            }

            return dataTable;
        }
    }
}