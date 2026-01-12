using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Krooq.TowerDefense
{
    public class TargetingStrategy : ITargetingStrategy
    {
        [SerializeField, ReadOnly] private TargetingStrategyType _type;
        [SerializeField, ReadOnly] private float _randomnessBuffer = 1f;

        public TargetingStrategy(TargetingStrategyType strategyType)
        {
            _type = strategyType;
        }

        public Threat FindTarget(Vector3 sourcePos, float range, IReadOnlyList<Threat> threats)
        {
            return _type switch
            {
                TargetingStrategyType.Closest => TargetClosest(sourcePos, range, threats),
                TargetingStrategyType.RandomNearClosest => TargetRandomNearClosest(sourcePos, range, threats),
                // Future strategies can be added here.
                _ => null,
            };
        }

        public Threat TargetClosest(Vector3 sourcePos, float range, IReadOnlyList<Threat> threats)
        {
            Threat bestTarget = null;
            float closestDistSqr = range * range;
            var camera = Camera.main;

            foreach (var threat in threats)
            {
                if (threat == null) continue;

                if (camera != null)
                {
                    var viewportPoint = camera.WorldToViewportPoint(threat.transform.position);
                    bool isOnScreen = viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1;
                    if (!isOnScreen) continue;
                }

                // Add check regarding movement type if needed, but keeping it simple for now.
                var closestPoint = threat.GetClosestPoint(sourcePos);
                float distSqr = (closestPoint - sourcePos).sqrMagnitude;
                if (distSqr < closestDistSqr)
                {
                    closestDistSqr = distSqr;
                    bestTarget = threat;
                }
            }
            return bestTarget;
        }

        public Threat TargetRandomNearClosest(Vector3 sourcePos, float range, IReadOnlyList<Threat> threats)
        {
            var candidates = new List<(Threat threat, float distSqr)>();
            float closestDistSqr = float.MaxValue;
            var camera = Camera.main;

            foreach (var threat in threats)
            {
                if (threat == null) continue;

                if (camera != null)
                {
                    var viewportPoint = camera.WorldToViewportPoint(threat.transform.position);
                    bool isOnScreen = viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1;
                    if (!isOnScreen) continue;
                }

                var closestPoint = threat.GetClosestPoint(sourcePos);
                float distSqr = (closestPoint - sourcePos).sqrMagnitude;

                if (distSqr < range * range)
                {
                    candidates.Add((threat, distSqr));
                    if (distSqr < closestDistSqr)
                    {
                        closestDistSqr = distSqr;
                    }
                }
            }

            if (candidates.Count == 0) return null;

            float closestDist = Mathf.Sqrt(closestDistSqr);
            float maxRandomDist = closestDist + _randomnessBuffer;
            float maxRandomDistSqr = maxRandomDist * maxRandomDist;

            var finalCandidates = new List<Threat>();
            foreach (var candidate in candidates)
            {
                if (candidate.distSqr <= maxRandomDistSqr)
                {
                    finalCandidates.Add(candidate.threat);
                }
            }

            if (finalCandidates.Count == 0) return null;

            return finalCandidates[Random.Range(0, finalCandidates.Count)];
        }
    }

    public enum TargetingStrategyType
    {
        Closest,
        RandomNearClosest,
        // Future strategies can be added here.
    }
}
