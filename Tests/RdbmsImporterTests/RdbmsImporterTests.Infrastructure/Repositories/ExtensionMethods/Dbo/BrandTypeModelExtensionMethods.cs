using System.Collections.Generic;
using System.Data;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Dbo
{
    internal static class BrandTypeModelExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<BrandTypeModel> brandTypeModels)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("IsActive", typeof(bool));

            foreach (var item in brandTypeModels)
            {
                dataTable.Rows.Add(item.Name, item.IsActive);
            }

            return dataTable;
        }
    }
}