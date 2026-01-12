using UnityEngine;
using Krooq.Core;
using Sirenix.OdinInspector;
using Krooq.Common;
using System.Collections.Generic;

namespace Krooq.TowerDefense
{
    public class ThreatModel : MonoBehaviour
    {
        [SerializeField, ReadOnly] private List<Collider2D> _colliders = new();
        [SerializeField, ReadOnly] private Threat _threat;
        [SerializeField, ReadOnly] private float _hopInterval = 0.3f;
        [SerializeField, ReadOnly] private float _nextBumpTime;

        protected SpringTransform SpringTransform => this.GetCachedComponent<SpringTransform>();

        protected void OnEnable()
        {
            _colliders.Clear();
            _colliders.AddRange(GetComponentsInChildren<Collider2D>());
        }

        public void Init(Threat threat)
        {
            _threat = threat;
        }

        public void Update()
        {
            if (_threat == null) return;
            // if (_threat.Data.MovementType != ThreatMovementType.Ground)
            // {
            //     SpringTransform.enabled = false;
            //     return;
            // }
            // if (Time.time >= _nextBumpTime)
            // {
            //     SpringTransform.BumpPosition(Vector3.up * 1f);
            //     _nextBumpTime = Time.time + _hopInterval;
            // }
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

        public Vector3 GetMidPoint()
        {
            if (_colliders.Count == 0) return transform.position;
            Vector3 sum = Vector3.zero;
            int count = 0;
            foreach (var collider in _colliders)
            {
                if (collider == null) continue;
                sum += collider.bounds.center;
                count++;
            }
            return count > 0 ? sum / count : transform.position;
        }

    }
}
