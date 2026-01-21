namespace Intent.Modules.CSharp.Importer.Tests.TestData;

/// <summary>
/// Factory for creating test package models - simplified stub
/// </summary>
public static class PackageModels
{
    // Simplified stubs - actual package model creation would be added as needed
    public static object Empty() => new { Id = Guid.NewGuid().ToString(), Name = "TestPackage" };
    
    public static object WithSimpleClass() => Empty();
    
    public static object WithClassAndInterface() => Empty();
    
    public static object WithEnum() => Empty();
    
    public static object WithMethods() => Empty();
}
