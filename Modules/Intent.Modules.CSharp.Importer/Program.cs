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
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.CSharp.Importer.Importer;
using Intent.Modules.CSharp.Importer.Tasks;
using Intent.Persistence;

namespace Intent.MetadataSynchronizer.CSharp.Importer;

class Program
{
    /// <summary>
    /// This is purely for testing purposes
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    static async Task<int> Main(string[] args)
    {
        var folder = @"C:\Dev\Clients\Payflex\Payflex.Backend\Customer.Domain\Models\Events\CustomerEvents\";
        folder = @"C:\Dev\Demos\Payflex\Payflex.Ordering\Payflex.Ordering.Domain\Entities";
        folder = @"C:\Dev\Demos\Payflex\Payflex.Ordering\Payflex.Ordering.Application";
        var classDataElements = await CSharpCodeAnalyzer.ImportMetadataFromFolder(folder);

        var solution = SolutionPersistable.Load(@"C:\Dev\Demos\Payflex\intent\Payflex.isln");
        var application = solution.GetApplication("e5ad233e-d1f8-4d03-8187-2a9f790053cd");
        var designer = application.GetDesigner("6ab29b31-27af-4f56-a67c-986d82097d63");
        var targetPackage = designer.GetPackage("b64cd0db-f3c3-4df9-84de-91cff1374f77");
        var associationSettings = designer.GetAssociationSettings("eaf9ed4e-0b61-4ac1-ba88-09f912c12087");

        var profile = new ImportProfileConfig()
        {
            MapClassesTo = designer.GetElementSettings("04e12b51-ed12-42a3-9667-a6aa81bb6d10"),
            MapPropertiesTo = designer.GetElementSettings("0090fb93-483e-41af-a11d-5ad2dc796adf"),
            MapAssociationsTo = designer.GetAssociationSettings("eaf9ed4e-0b61-4ac1-ba88-09f912c12087"),
            //MapClassesTo = new ElementSettings("04e12b51-ed12-42a3-9667-a6aa81bb6d10", "Class"),
            //MapPropertiesTo = new ElementSettings("0090fb93-483e-41af-a11d-5ad2dc796adf", "Attribute"),
            //MapAssociationsTo = new AssociationSettings(specializationTypeId: "eaf9ed4e-0b61-4ac1-ba88-09f912c12087",
            //    specializationType: "Association",
            //    sourceEnd: new AssociationEndSettings("8d9d2e5b-bd55-4f36-9ae4-2b9e84fd4e58", "Association Source End"),
            //    targetEnd: new AssociationEndSettings("0a66489f-30aa-417b-a75d-b945863366fd", "Association Target End")),
        };
        targetPackage.ImportCSharpTypes(classDataElements, new CSharpConfig()
        {
            ImportProfile = profile,
            TargetFolder = folder,
            TargetFolderId = null,
        });

        targetPackage.Save(false);

        return 0;
    }
}