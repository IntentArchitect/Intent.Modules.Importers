using System.Reflection;
using System.Text;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
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
            TypesToExport = [ExportType.Table, ExportType.View],
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