using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.CSharp.Importer.Importer;

public record GenericDataType(string BaseName, IEnumerable<string> TypeParameters);
