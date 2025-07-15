using System.ComponentModel;
using Intent.MetadataSynchronizer.Configuration;

namespace Intent.MetadataSynchronizer.Java;

public class Config
{
    [Description("The root folder containing source .java files, all sub-folders are scanned.")]
    public string SourcesPath { get; set; }

    [Description("Path of the Intent Architect .isln file.")]
    public string IntentSolutionPath { get; set; }

    [Description("The application name (as per the Application Settings view) containing the metadata.")]
    public string ApplicationName { get; set; }

    [Description("The designer name containing the metadata.")]
    public string DesignerName { get; set; }

    [Description("The package Id containing the metadata. This can be obtained from the properties " +
                 "pane which is visible when selecting the package node.")]
    public string PackageId { get; set; }

    [Description("Optional. When specified uses folder as root import location. When not specified " +
                 "the package itself is considered the root.")]
    public string TargetFolderId { get; set; }

    [Description("When set to true then the logging level will be set to debug, otherwise it will " +
                 "just be set to informational.")]
    public bool Debug { get; set; }

    [Description("Array of wild card patterns of files to ignore, supports * and ? patterns such as " +
                 "'*Factory*.java'.")]
    public IReadOnlyCollection<string> Ignores { get; set; }

    [Description("Unless true, \"Press any key to exit...\" will be shown at the end of the " +
                 "program execution.")]
    public bool SkipPressAnyKeyToExit { get; set; }

    [Description("When false, attributes will not be created if the type is unknown. When true, " +
                 "attributes will be created with the type unset and will show in the designer " +
                 "as having an error.")]
    public bool CreateAttributesWithUnknownTypes { get; set; }

    [Description("When true, will delete any elements in the designer which were not found in the " +
                 "data import source.")]
    public bool DeleteExtraElements { get; set; }

    [Description("Controls synchronization behaviour of stereotypes, valid values: " +
                 $"{nameof(StereotypeManagementMode.Ignore)}, " +
                 $"{nameof(StereotypeManagementMode.Merge)} (default), " +
                 $"{nameof(StereotypeManagementMode.Fully)}")]
    public StereotypeManagementMode StereotypeManagementMode { get; set; }

    [Description("When \"true\" will result in every imported type having a \"Java\" stereotype " +
                 "added to it with the imported type's package name set.")]
    public bool ApplyJavaStereotypes { get; set; }
}