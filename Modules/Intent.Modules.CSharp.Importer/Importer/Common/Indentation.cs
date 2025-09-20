using System;

namespace Intent.MetadataSynchronizer;

public class Indentation : IDisposable
{
    private static readonly object Lock = new();
    private static int _indentationLength;

    public Indentation()
    {
        lock (Lock)
        {
            _indentationLength++;
        }
    }

    public static string Get()
    {
        lock (Lock)
        {
            return new string(' ', _indentationLength * 2);
        }
    }

    public void Dispose()
    {
        lock (Lock)
        {
            if (_indentationLength == 0) throw new Exception("Indentation already at 0");
            _indentationLength--;
        }
    }
}