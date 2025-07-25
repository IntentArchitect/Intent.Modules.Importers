using System.Reflection;
using System.Text;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.RelationalDbSchemaImporter.Runner;
using Intent.Utils;
using Npgsql;
using Testcontainers.PostgreSql;
using Testcontainers.Xunit;
using Xunit.Abstractions;

namespace Intent.RelationalDbSchemaImporter.Tests;

public class PostgreSQLIntegrationTests : ContainerTest<PostgreSqlBuilder, PostgreSqlContainer>
{
    public PostgreSQLIntegrationTests(ITestOutputHelper outputHelper):base(outputHelper)
    {
        Logging.SetTracing(new DummyTracer(outputHelper));
        ImporterTool.SetToolDirectory(Path.GetDirectoryName(typeof(PostgreSQLIntegrationTests).Assembly.Location)!);
    }
    
    protected override PostgreSqlBuilder Configure(PostgreSqlBuilder builder)
    {
        return builder.WithCleanUp(true);
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
            DatabaseType = DatabaseType.PostgreSQL
        };

        // Act
        var result = ImporterTool.Run<ImportSchemaResult>("import-schema", importRequest);

        Assert.NotNull(result.Result);
        // Verify the schema data structure
        await Verify(result.Result.SchemaData)
            .UseParameters(nameof(PostgreSQLIntegrationTests));
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
            DatabaseType = DatabaseType.PostgreSQL,
            ImportFilterFilePath = "TestData/PostgreSQL_Filter.json"
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
            .UseParameters(nameof(PostgreSQLIntegrationTests));
    }

    private async Task SetupTestSchema()
    {
        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var scriptLocation = Path.GetFullPath(Path.Combine(location, "TestData", "PostgreSQLTestSchema.sql"));
        var sqlScript = await File.ReadAllTextAsync(scriptLocation);
        await ExecutePostgreSQLScript(Container.GetConnectionString(), sqlScript);
    }

    private static async Task ExecutePostgreSQLScript(string connectionString, string sqlScript)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var statements = SplitPostgreSQLStatements(sqlScript);

        foreach (var statement in statements)
        {
            var trimmedStatement = statement.Trim();
            if (string.IsNullOrEmpty(trimmedStatement) || trimmedStatement.StartsWith("--"))
                continue;

            try
            {
                await using var command = new NpgsqlCommand(trimmedStatement, connection);
                command.CommandTimeout = 60;
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing statement: {trimmedStatement.Substring(0, Math.Min(100, trimmedStatement.Length))}...");
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }

    private static List<string> SplitPostgreSQLStatements(string script)
    {
        var statements = new List<string>();
        var currentStatement = new StringBuilder();
        var inFunction = false;
        var dollarQuoteDepth = 0;

        var lines = script.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            // Skip empty lines and comments
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("--"))
                continue;

            currentStatement.AppendLine(line);

            // Track $$ delimiters for functions
            if (trimmedLine.Contains("$$"))
            {
                var dollarCount = trimmedLine.Split(new[] { "$$" }, StringSplitOptions.None).Length - 1;
                dollarQuoteDepth += dollarCount;
                inFunction = dollarQuoteDepth % 2 == 1;
            }

            // If we hit a semicolon and we're not inside a function, end the statement
            if (trimmedLine.EndsWith(";") && !inFunction)
            {
                statements.Add(currentStatement.ToString().Trim());
                currentStatement.Clear();
            }
        }

        // Add any remaining statement
        if (currentStatement.Length > 0)
        {
            statements.Add(currentStatement.ToString().Trim());
        }

        return statements;
    }
}