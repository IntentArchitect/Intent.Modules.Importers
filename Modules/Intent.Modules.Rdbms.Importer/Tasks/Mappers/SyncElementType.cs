using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Rdbms.Importer.Tasks.Mappers;

[Flags]
public enum SyncElementType
{
    None = 0,
    AttributeType = 1,
    GenericType = 2,
    IsCollection = 4,
    IsNullable = 8,
    Stereotypes = 16
}
