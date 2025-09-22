using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.CSharp.Importer.Tasks;
public class ImportCSharpFileInputModel
{
    public string SourceFolder { get; set; }
    public string[] SelectedFiles { get; set; }
    public string DesignerId { get; set; }
    public string PackageId { get; set; }
    public string ImportProfileId { get; set; }
    public string? TargetFolderId { get; set; }
}

class ElementSettings(string specializationTypeId, string specializationType) : IElementSettings
{
    public string SpecializationTypeId { get; } = specializationTypeId;
    public string SpecializationType { get; } = specializationType;
}

class AssociationSettings(string specializationTypeId, string specializationType, IAssociationEndSetting sourceEnd, IAssociationEndSetting targetEnd) : IAssociationSettings
{
    public string SpecializationTypeId { get; } = specializationTypeId;
    public string SpecializationType { get; } = specializationType;
    public IAssociationEndSetting SourceEnd { get; } = sourceEnd;
    public IAssociationEndSetting TargetEnd { get; } = targetEnd;
}

class AssociationEndSettings(string specializationTypeId, string specializationType) : IAssociationEndSetting
{
    public string SpecializationTypeId { get; } = specializationTypeId;
    public string SpecializationType { get; } = specializationType;
}
