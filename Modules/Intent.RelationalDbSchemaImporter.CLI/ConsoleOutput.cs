using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Intent.RelationalDbSchemaImporter.CLI;

public static class ConsoleOutput
{
    public static void JsonOutput(object model, bool prettyPrint)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = prettyPrint,
            Converters = { new JsonStringEnumConverter() }
        };
        var json = JsonSerializer.Serialize(model, options);
        
        Console.Out.WriteLine(json);
    }

    public static void WarnOutput(string warning)
    {
        Console.Error.WriteLine("Warning: " + warning);
    }
    
    public static SectionProgress CreateSectionProgress(string sectionName, int totalItems)
    {
        return new SectionProgress(sectionName, totalItems);
    }
}

public class SectionProgress
{
    private readonly string _sectionName;
    private readonly int _totalItems;
    private int _currentNumber;

    public SectionProgress(string sectionName, int totalItems)
    {
        _sectionName = sectionName;
        _totalItems = totalItems;
    }

    public void OutputNext(string itemName)
    {
        if (_currentNumber == 0)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine(_sectionName);
            Console.Error.WriteLine(new string('=', _sectionName.Length));
            Console.Error.WriteLine();
        }

        _currentNumber++;
        
        Console.Error.WriteLine($"{itemName} ({_currentNumber}/{_totalItems})");
    }
}