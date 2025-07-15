using System.Collections.Generic;
using Intent.IArchitect.Agent.Persistence.Model;

namespace Intent.MetadataSynchronizer;

public record Persistables(IReadOnlyCollection<ElementPersistable> Elements, IReadOnlyCollection<AssociationPersistable> Associations);