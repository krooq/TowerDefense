using System.Collections.Generic;
using UnityEngine;

namespace Krooq.PlanetDefense
{
    public interface ICaster
    {
        Transform FirePoint { get; }
        ITarget TargetingInfo { get; }
        GameObject gameObject { get; }
        Transform transform { get; }
        int CurrentMana { get; }
        int MaxMana { get; }
        bool TrySpendMana(int amount);
        IEnumerable<IAbilitySource> AbilitySources { get; }
    }
}
