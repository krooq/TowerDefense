using System.Collections.Generic;
using UnityEngine;

namespace Krooq.TowerDefense
{
    public interface ICaster
    {
        Transform FirePoint { get; }
        GameObject gameObject { get; }
        Transform transform { get; }
        IEnumerable<IAbilitySource> AbilitySources { get; }
    }
}
