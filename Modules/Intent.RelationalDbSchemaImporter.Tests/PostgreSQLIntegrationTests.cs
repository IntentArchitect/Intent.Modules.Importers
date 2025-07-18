using System.Reflection;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.RelationalDbSchemaImporter.Contracts.Models;
using Intent.RelationalDbSchemaImporter.Runner;
using Intent.Utils;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;

namespace Intent.RelationalDbSchemaImporter.Tests;

public class PostgreSQLIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("TestPassword123!")
        .WithCleanUp(true)
        .Build();

    public PostgreSQLIntegrationTests(ITestOutputHelper outputHelper)
    {
        Logging.SetTracing(new DummyTracer(outputHelper));
        ImporterTool.SetToolDirectory(Path.GetDirectoryName(typeof(PostgreSQLIntegrationTests).Assembly.Location)!);
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
    public async Task ShouldExtractSchemaFromPostgreSQLContainer()
    {
        // Arrange
        var connectionString = _dbContainer.GetConnectionString();
        
        var importRequest = new ImportSchemaRequest
        {
            ConnectionString = connectionString,
            ApplicationId = Guid.NewGuid().ToString(),
            PackageFileName = "TestPackagePostgreSQL.pkg",
            EntityNameConvention = EntityNameConvention.SingularEntity,
            TableStereotype = TableStereotype.WhenDifferent,
            TypesToExport = [ExportType.Table, ExportType.View],
            StoredProcedureType = StoredProcedureType.StoredProcedureElement,
            DatabaseType = Intent.RelationalDbSchemaImporter.Contracts.Enums.DatabaseType.PostgreSQL
        };

        // Act
        var result = ImporterTool.Run<ImportSchemaResult>("import-schema", importRequest);

        Assert.NotNull(result.Result);
        // Verify the schema data structure
        await Verify(result.Result.SchemaData)
            .UseParameters(nameof(PostgreSQLIntegrationTests));
    }

    private async Task SetupTestSchema()
    {
        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var scriptLocation = Path.GetFullPath(Path.Combine(location, "TestData", "PostgreSQLTestSchema.sql"));
        var sqlScript = await File.ReadAllTextAsync(scriptLocation);
        await ExecutePostgreSQLScript(_dbContainer.GetConnectionString(), sqlScript);
    }
    
    private static async Task ExecutePostgreSQLScript(string connectionString, string sqlScript)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        
        // PostgreSQL doesn't use GO statements, but we can split on semicolons for safety
        var statements = sqlScript.Split(';', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var statement in statements)
        {
            var trimmedStatement = statement.Trim();
            if (string.IsNullOrEmpty(trimmedStatement))
                continue;
                
            try
            {
                await using var command = new NpgsqlCommand(trimmedStatement, connection);
                command.CommandTimeout = 60;
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex) when (trimmedStatement.Contains("-- ") || trimmedStatement.StartsWith("--"))
            {
                // Skip comment lines that might cause issues
                continue;
            }
        }
    }
} 