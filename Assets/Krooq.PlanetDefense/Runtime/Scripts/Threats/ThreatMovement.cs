using UnityEngine;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public enum ThreatMovementType
    {
        Ground,
        Air,
        Constant
    }

    public class ThreatMovement : IThreatMovement
    {
        [SerializeField] private ThreatMovementType _type;

        private float _sineOffset;
        private float _frequency;
        private float _amplitude;

        public ThreatMovementType Type => _type;

        public ThreatMovement(ThreatMovementType type)
        {
            _type = type;
            _sineOffset = Random.Range(0f, 100f);
            _frequency = Random.Range(1f, 2f);
            _amplitude = Random.Range(0.2f, 0.3f);
        }

        public void Move(Threat threat)
        {
            if (threat.PlayerTower == null) return;

            switch (_type)
            {
                case ThreatMovementType.Ground:
                    MoveGround(threat);
                    break;
                case ThreatMovementType.Air:
                    MoveAir(threat);
                    break;
                case ThreatMovementType.Constant:
                    MoveConstant(threat);
                    break;
            }
        }

        private void MoveGround(Threat threat)
        {
            var targetPos = threat.PlayerTower.GetClosestPoint(threat.transform.position);
            Vector2 direction = (targetPos - threat.transform.position).normalized;

            if (threat.Rigidbody2D.bodyType == RigidbodyType2D.Dynamic)
            {
                threat.Rigidbody2D.linearVelocity = direction * threat.Speed;
            }
            else
            {
                // Use transform.Translate (or direct position assignment) so we modify the 
                // local offset relative to the parent. This ensures we don't fight the 
                // parent's MovePosition which happens later in the physics step.
                threat.transform.position = (Vector2)threat.transform.position + direction * threat.Speed * Time.fixedDeltaTime;
            }
        }

        private void MoveAir(Threat threat)
        {
            // Move directly towards the base
            var targetPos = threat.PlayerTower.GetClosestPoint(threat.transform.position);
            Vector2 direction = (targetPos - threat.transform.position).normalized;
            Vector2 perp = new Vector2(-direction.y, direction.x);

            float wave = Mathf.Sin(Time.time * _frequency + _sineOffset) * _amplitude;
            Vector2 movement = (direction * threat.Speed + perp * wave) * Time.fixedDeltaTime;

            // Direct transform update to retain parenting offsets
            threat.transform.position = (Vector2)threat.transform.position + movement;

            // Rotate to face direction
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            threat.Rigidbody2D.rotation = angle;
        }

        private void MoveConstant(Threat threat)
        {
            // Move directly towards the base
            var targetPos = threat.PlayerTower.GetClosestPoint(threat.transform.position);
            Vector2 direction = (targetPos - threat.transform.position).normalized;

            // Direct transform update
            threat.transform.position = (Vector2)threat.transform.position + direction * threat.Speed * Time.fixedDeltaTime;

            // Tumble
            threat.Rigidbody2D.rotation += threat.Speed * Time.fixedDeltaTime;
        }
    }
}
