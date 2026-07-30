using System.Data.Common;
using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Shouldly;

namespace Intent.Modules.Rdbms.Importer.Tests;

/// <summary>
/// Sanitising is what "Team-shared metadata (sanitized connection string, no password)" does before the
/// connection string goes into the package - and therefore into source control. It used to be done with
/// <c>SqlConnectionStringBuilder</c>, which threw <see cref="System.PlatformNotSupportedException"/> inside the
/// module host (only the netstandard facade of Microsoft.Data.SqlClient ships beside the module) and rejected
/// PostgreSQL keywords outright. These tests pin the provider-agnostic replacement.
/// </summary>
public class ConnectionStringSanitisationTests
{
    [Fact]
    public void SanitiseConnectionString_SqlServerPassword_RemovesPasswordAndKeepsOtherKeywords()
    {
        var result = SettingsHelper.SanitiseConnectionString(
            "Server=localhost;Initial Catalog=AdventureWorks;User ID=sa;Password=Sup3rSecret!;TrustServerCertificate=True");

        result.ShouldNotContain("Sup3rSecret!");
        result.ShouldStartWith("Password=  ;");

        var reparsed = new DbConnectionStringBuilder { ConnectionString = result };
        reparsed["Server"].ShouldBe("localhost");
        reparsed["Initial Catalog"].ShouldBe("AdventureWorks");
        reparsed["User ID"].ShouldBe("sa");
        reparsed["TrustServerCertificate"].ShouldBe("True");
    }

    [Fact]
    public void SanitiseConnectionString_PwdSynonym_RemovesPassword()
    {
        // SqlConnectionStringBuilder knew PWD was a synonym for Password; DbConnectionStringBuilder does not,
        // so this guards against a PWD= password silently leaking into shared metadata.
        var result = SettingsHelper.SanitiseConnectionString("Server=localhost;UID=sa;PWD=Sup3rSecret!");

        result.ShouldNotContain("Sup3rSecret!");
        result.ShouldStartWith("Password=  ;");
        result.ShouldNotContain("PWD", Case.Insensitive);
    }

    [Fact]
    public void SanitiseConnectionString_PostgreSqlKeywords_RemovesPasswordWithoutThrowing()
    {
        // A SQL-Server-specific builder throws "Keyword not supported: 'host'" on this input.
        var result = SettingsHelper.SanitiseConnectionString(
            "Host=localhost;Port=5432;Database=northwind;Username=postgres;Password=Sup3rSecret!");

        result.ShouldNotContain("Sup3rSecret!");
        result.ShouldStartWith("Password=  ;");

        var reparsed = new DbConnectionStringBuilder { ConnectionString = result };
        reparsed["Host"].ShouldBe("localhost");
        reparsed["Port"].ShouldBe("5432");
        reparsed["Database"].ShouldBe("northwind");
        reparsed["Username"].ShouldBe("postgres");
    }

    [Fact]
    public void SanitiseConnectionString_NoPassword_ReturnsConnectionStringWithoutPlaceholder()
    {
        var result = SettingsHelper.SanitiseConnectionString(
            "Server=localhost;Initial Catalog=AdventureWorks;Integrated Security=True");

        result.ShouldNotContain("Password");

        var reparsed = new DbConnectionStringBuilder { ConnectionString = result };
        reparsed["Server"].ShouldBe("localhost");
        reparsed["Integrated Security"].ShouldBe("True");
    }

    [Theory]
    [InlineData("pa;ss")]
    [InlineData("pa=ss")]
    [InlineData("pa\"ss")]
    public void SanitiseConnectionString_PasswordContainingDelimiters_RemovesWholePassword(string password)
    {
        var original = new DbConnectionStringBuilder
        {
            ["Server"] = "localhost",
            ["User ID"] = "sa",
            ["Password"] = password
        };

        var result = SettingsHelper.SanitiseConnectionString(original.ConnectionString);

        result.ShouldNotContain(password);
        result.ShouldStartWith("Password=  ;");

        var reparsed = new DbConnectionStringBuilder { ConnectionString = result };
        reparsed["Server"].ShouldBe("localhost");
        reparsed["User ID"].ShouldBe("sa");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SanitiseConnectionString_NothingToSanitise_ReturnsInputUnchanged(string? connectionString)
    {
        SettingsHelper.SanitiseConnectionString(connectionString).ShouldBe(connectionString ?? string.Empty);
    }
}
