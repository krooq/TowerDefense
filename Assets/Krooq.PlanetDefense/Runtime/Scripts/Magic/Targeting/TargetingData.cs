using UnityEngine;
using System.Collections.Generic;

namespace Krooq.PlanetDefense
{
    public class TargetingData : ScriptableObject, ITargetingStrategy
    {
        [SerializeField] private TargetingStrategyType _strategyType;
        public Threat FindTarget(Vector3 sourcePos, float range, IReadOnlyList<Threat> threats)
        {
            return _strategyType switch
            {
                TargetingStrategyType.Closest => TargetClosest(sourcePos, range, threats),
                // Future strategies can be added here.
                _ => null,
            };
        }

        public Threat TargetClosest(Vector3 sourcePos, float range, IReadOnlyList<Threat> threats)
        {
            Threat bestTarget = null;
            float closestDistSqr = range * range;

            foreach (var threat in threats)
            {
                if (threat == null) continue;
                // Add check regarding movement type if needed, but keeping it simple for now.

                float distSqr = (threat.transform.position - sourcePos).sqrMagnitude;
                if (distSqr < closestDistSqr)
                {
                    closestDistSqr = distSqr;
                    bestTarget = threat;
                }
            }

            return bestTarget;
        }
    }

    public enum TargetingStrategyType
    {
        Closest,
        // Future strategies can be added here.
    }
}
