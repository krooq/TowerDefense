using System.Collections.Generic;
using UnityEngine;

namespace Krooq.PlanetDefense
{
    public interface ISpellCaster
    {
        Transform FirePoint { get; }
        ITargetingInfo TargetingInfo { get; }
        GameObject gameObject { get; }
        Transform transform { get; }
        bool TrySpendMana(int amount);
        IEnumerable<IAbilitySource> AbilitySources { get; }
    }
}
