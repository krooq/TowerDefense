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
            // Move horizontally towards the base
            var targetPos = threat.PlayerTower.GetClosestPoint(threat.transform.position);
            float directionX = Mathf.Sign(targetPos.x - threat.transform.position.x);

            if (threat.Rigidbody2D.bodyType == RigidbodyType2D.Dynamic)
            {
                threat.Rigidbody2D.linearVelocity = new Vector2(directionX * threat.Speed, threat.Rigidbody2D.linearVelocity.y);
            }
            else
            {
                Vector2 nextPos = threat.Rigidbody2D.position + new Vector2(directionX * threat.Speed * Time.fixedDeltaTime, 0);
                threat.Rigidbody2D.MovePosition(nextPos);
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

            threat.Rigidbody2D.MovePosition(threat.Rigidbody2D.position + movement);

            // Rotate to face direction
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            threat.Rigidbody2D.rotation = angle;
        }

        private void MoveConstant(Threat threat)
        {
            // Move directly towards the base
            var targetPos = threat.PlayerTower.GetClosestPoint(threat.transform.position);
            Vector2 direction = (targetPos - threat.transform.position).normalized;
            threat.Rigidbody2D.MovePosition(threat.Rigidbody2D.position + direction * threat.Speed * Time.fixedDeltaTime);

            // Tumble
            threat.Rigidbody2D.rotation += threat.Speed * Time.fixedDeltaTime;
        }
    }
}
