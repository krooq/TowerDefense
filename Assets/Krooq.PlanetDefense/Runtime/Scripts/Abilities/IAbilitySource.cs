using Krooq.Common;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Krooq.PlanetDefense
{
    public interface IAbilitySource
    {
        IReadOnlyList<AbilityData> Abilities { get; }
    }
}
