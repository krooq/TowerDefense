using UnityEngine;
using Krooq.Common;
using Krooq.Core;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public class PlayerTower : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField, ReadOnly] private List<Collider2D> _colliders = new();

        protected Player Player => this.GetSingleton<Player>();

        private void OnEnable()
        {
            _colliders.Clear();
            _colliders.AddRange(GetComponentsInChildren<Collider2D>());
        }

        public Vector3 GetClosestPoint(Vector3 point)
        {
            var closestPoint = transform.position;
            float closestDistanceSqr = (point - closestPoint).sqrMagnitude;
            foreach (var collider in _colliders)
            {
                if (collider == null) continue;
                Vector3 colliderClosestPoint = collider.ClosestPoint(point);
                float distanceSqr = (point - colliderClosestPoint).sqrMagnitude;
                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestPoint = colliderClosestPoint;
                }
            }
            return closestPoint;
        }

        public void TakeDamage(int amount)
        {
            Player.TakeDamage(amount);
            if (Player.CurrentHealth <= 0) _animator.SetTrigger("Death");
        }
        public void Attack()
        {
            _animator.SetTrigger("Attack");
        }
    }
}
