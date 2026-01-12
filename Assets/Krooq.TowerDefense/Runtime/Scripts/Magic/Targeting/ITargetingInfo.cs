using Krooq.Common;
using UnityEngine;
using UnityEngine.Events;

namespace Krooq.TowerDefense
{
    public interface ITarget
    {
        bool IsValid { get; }
        Vector3 Position { get; }
        Vector3 GetClosestPoint(Vector3 reference) => Position;
        Vector3 GetMidPoint() => Position;
        bool IsGroundTarget { get; }
        UnityEvent OnTargetDestroyed { get; }
    }


    public struct ThreatTarget : ITarget
    {
        public Threat Threat { get; set; }
        public bool IsValid => Threat != null;
        public Vector3 Position => Threat != null ? Threat.transform.position : Vector3.zero;
        public Vector3 GetClosestPoint(Vector3 reference) => Threat != null ? Threat.GetClosestPoint(reference) : Vector3.zero;
        public Vector3 GetMidPoint() => Threat != null ? Threat.GetMidPoint() : Vector3.zero;
        public bool IsGroundTarget => Threat != null && Threat.Data.MovementType == ThreatMovementType.Ground;
        public UnityEvent OnTargetDestroyed => Threat.OnDeath;

        public ThreatTarget(Threat threat)
        {
            Threat = threat;
        }
    }

    public struct PositionTarget : ITarget
    {
        public Vector3 Position { get; set; }
        public bool IsValid => true;
        public bool IsGroundTarget { get; set; }
        public UnityEvent OnTargetDestroyed { get; }

        public PositionTarget(Vector3 position, bool isGroundTarget)
        {
            Position = position;
            IsGroundTarget = isGroundTarget;
            OnTargetDestroyed = new UnityEvent();
        }
    }
}
