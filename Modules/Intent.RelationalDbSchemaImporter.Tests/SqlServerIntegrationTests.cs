using System.Reflection;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.RelationalDbSchemaImporter.Runner;
using Intent.Utils;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Testcontainers.Xunit;
using Xunit.Abstractions;

namespace Intent.RelationalDbSchemaImporter.Tests;

public class SqlServerIntegrationTests : ContainerTest<MsSqlBuilder, MsSqlContainer>
{
    public SqlServerIntegrationTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        Logging.SetTracing(new DummyTracer(outputHelper));
        ImporterTool.SetToolDirectory(Path.GetDirectoryName(typeof(SqlServerIntegrationTests).Assembly.Location)!);
    }

    protected override MsSqlBuilder Configure(MsSqlBuilder builder)
    {
        return builder.WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("IntentTest123!")
            .WithCleanUp(true);
    }

    protected override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await SetupTestSchema();
    }

    [Fact]
    public async Task ShouldExtractAllSchema()
    {
        // Arrange
        var connectionString = Container.GetConnectionString();

        var importRequest = new ImportSchemaRequest
        {
            ConnectionString = connectionString,
            TypesToExport = [ExportType.Table, ExportType.View, ExportType.Index, ExportType.StoredProcedure],
            DatabaseType = DatabaseType.SqlServer
        };

        // Act
        var result = ImporterTool.Run<ImportSchemaResult>("import-schema", importRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Errors.Count == 0, $"Import failed with errors: {string.Join(", ", result.Errors)}");
        Assert.NotNull(result.Result);

        var schemaData = result.Result.SchemaData;
        Assert.NotNull(schemaData);

        // Verify the schema data structure  
        await Verify(schemaData)
            .UseParameters(nameof(SqlServerIntegrationTests));
    }

    [Fact]
    public async Task ShouldExtractFilteredSchema()
    {
        // Arrange
        var connectionString = Container.GetConnectionString();

        var importRequest = new ImportSchemaRequest
        {
            ConnectionString = connectionString,
            TypesToExport = [ExportType.Table],
            DatabaseType = DatabaseType.SqlServer,
            ImportFilterFilePath = "TestData/SqlServer_Filter.json"
        };

        // Act
        var result = ImporterTool.Run<ImportSchemaResult>("import-schema", importRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Errors.Count == 0, $"Import failed with errors: {string.Join(", ", result.Errors)}");
        Assert.NotNull(result.Result);

        var schemaData = result.Result.SchemaData;
        Assert.NotNull(schemaData);

        // Verify the schema data structure  
        await Verify(schemaData)
            .UseParameters(nameof(SqlServerIntegrationTests));
    }

    private async Task SetupTestSchema()
    {
        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var scriptLocation = Path.GetFullPath(Path.Combine(location, "TestData", "SqlServerTestSchema.sql"));
        var sqlScript = await File.ReadAllTextAsync(scriptLocation);
        await ExecuteSqlScript(Container.GetConnectionString(), sqlScript);
    }

    private static async Task ExecuteSqlScript(string connectionString, string sqlScript)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        // Split script by GO statements and execute each batch
        var batches = sqlScript.Split(["\nGO\n", "\nGO\r\n", "\r\nGO\r\n", "\r\nGO\n"],
            StringSplitOptions.RemoveEmptyEntries);

        foreach (var batch in batches)
        {
            var trimmedBatch = batch.Trim();
            if (string.IsNullOrEmpty(trimmedBatch) || trimmedBatch.Equals("GO", StringComparison.OrdinalIgnoreCase))
                continue;
            
            try
            {
                await using var command = new SqlCommand(trimmedBatch, connection);
                command.CommandTimeout = 60;
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException exception)
            {
                throw new Exception(
                    $"""
                     Script execution error: {exception.Message}
                     Line: {exception.LineNumber}
                     Script:
                     {trimmedBatch}
                     """
                );
            }
        }
    }
}