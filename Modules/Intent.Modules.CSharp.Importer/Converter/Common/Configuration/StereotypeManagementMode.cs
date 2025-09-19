namespace Intent.MetadataSynchronizer.Configuration;

public enum StereotypeManagementMode
{
    Merge = 0, // Intentionally, the default behavior - if unspecified
    Ignore,
    Fully
}