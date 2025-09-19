using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer.Configuration;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;

namespace Intent.MetadataSynchronizer.CSharp.CLI;

class Program
{
    static async Task<int> Main(string[] args)
    {
        const bool createAttributesWithUnknownTypes = true;
        const StereotypeManagementMode stereotypeManagementMode = StereotypeManagementMode.Merge;
        var classDataElements = await CSharpCodeAnalyzer.ImportMetadataFromFolder(@"C:\Dev\Clients\Payflex\Payflex.Backend\Customer.Domain\Models\Events\CustomerEvents\");

        var solution = SolutionPersistable.Load(@"C:\Dev\Demos\Payflex\intent\Payflex.isln");
        var application = solution.GetApplication("e5ad233e-d1f8-4d03-8187-2a9f790053cd");
        var targetPackage = application.GetDesigner("").GetPackage("");

        PersistableFactory.GetPersistables(new CSharpConfig()
        {

        }, classDataElements, targetPackage);

        return 0;
    }
}