using Intent.Persistence;

namespace Intent.MetadataSynchronizer.CSharp.CLI.Builders;

public abstract class ElementBuilderBase
{
    protected IElementPersistable _element;
    private bool _isBuilt;

    internal IElementPersistable InternalElement => _element;

    public string Name => _element.Name;

    public string ExternalReference => _element.ExternalReference;

    public IElementPersistable Build()
    {
        if (_isBuilt)
        {
            throw new InvalidOperationException(
                $"The builder for {_element.ExternalReference} had already built the element. Make a new builder if you intended modify the element details.");
        }

        PerformIntermediateBuildStep();

        _isBuilt = true;

        return _element;
    }

    protected virtual void PerformIntermediateBuildStep()
    {
    }
}