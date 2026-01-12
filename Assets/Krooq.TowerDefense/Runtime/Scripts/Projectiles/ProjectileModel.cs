using UnityEngine;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;

namespace Krooq.TowerDefense
{
    public class ProjectileModel : MonoBehaviour
    {
        [SerializeField, ReadOnly] private Projectile _projectile;
        [SerializeField, ReadOnly] private CircleCollider2D _collider;

        public float Radius => _collider != null ? _collider.radius : 0f;

        protected void OnEnable()
        {
            _collider = GetComponentInChildren<CircleCollider2D>();
        }

        public void Init(Projectile projectile)
        {
            _projectile = projectile;
        }
    }
}
