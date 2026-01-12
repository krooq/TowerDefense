using UnityEngine;
using System.Collections.Generic;

namespace Krooq.TowerDefense
{
    public interface ITargetingStrategy
    {
        public Threat FindTarget(Vector3 sourcePos, float range, IReadOnlyList<Threat> threats);
    }
}
