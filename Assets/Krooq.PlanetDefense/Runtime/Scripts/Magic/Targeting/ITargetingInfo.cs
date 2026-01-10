using UnityEngine;

namespace Krooq.PlanetDefense
{
    public interface ITargetingInfo
    {
        bool IsGroundTarget { get; }
        Vector3 TargetPosition { get; }
        bool IsValid { get; }
    }
}
