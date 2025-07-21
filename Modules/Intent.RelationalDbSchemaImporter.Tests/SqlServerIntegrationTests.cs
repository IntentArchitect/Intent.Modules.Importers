using System.Reflection;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.RelationalDbSchemaImporter.Runner;
using Intent.Utils;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Xunit.Abstractions;

namespace Intent.RelationalDbSchemaImporter.Tests;

public class SqlServerIntegrationTests : IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("IntentTest123!")
        .WithCleanUp(true)
        .Build();

    public SqlServerIntegrationTests(ITestOutputHelper outputHelper)
    {
        Logging.SetTracing(new DummyTracer(outputHelper));
        ImporterTool.SetToolDirectory(Path.GetDirectoryName(typeof(SqlServerIntegrationTests).Assembly.Location)!);
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await SetupTestSchema();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }

    [Fact]
    public async Task ShouldExtractSchemaFromSqlServerContainer()
    {
        // Arrange
        var connectionString = _dbContainer.GetConnectionString();
        
        var importRequest = new ImportSchemaRequest
        {
            ConnectionString = connectionString,
            TypesToExport = [ExportType.Table, ExportType.View],
            DatabaseType = Intent.RelationalDbSchemaImporter.Contracts.Enums.DatabaseType.SqlServer
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
        await ExecuteSqlScript(_dbContainer.GetConnectionString(), sqlScript);
    }
    
    private static async Task ExecuteSqlScript(string connectionString, string sqlScript)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        // Split script by GO statements and execute each batch
        var batches = sqlScript.Split(new[] { "\nGO\n", "\nGO\r\n", "\r\nGO\r\n", "\r\nGO\n" }, 
            StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var batch in batches)
        {
            var trimmedBatch = batch.Trim();
            if (string.IsNullOrEmpty(trimmedBatch) || trimmedBatch.Equals("GO", StringComparison.OrdinalIgnoreCase))
                continue;
                
            await using var command = new SqlCommand(trimmedBatch, connection);
            command.CommandTimeout = 60;
            await command.ExecuteNonQueryAsync();
        }
    }
} 