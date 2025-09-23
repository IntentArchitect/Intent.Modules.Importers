using System.Collections.Generic;
using Intent.Persistence;

namespace Intent.MetadataSynchronizer;

public record Persistables(IReadOnlyCollection<IElementPersistable> Elements, IReadOnlyCollection<IAssociationPersistable> Associations);