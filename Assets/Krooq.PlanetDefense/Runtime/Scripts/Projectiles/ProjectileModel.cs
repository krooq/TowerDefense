using UnityEngine;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public class ProjectileModel : MonoBehaviour
    {
        [SerializeField, ReadOnly] private Projectile _projectile;

        public void Init(Projectile projectile)
        {
            _projectile = projectile;
        }
    }
}
