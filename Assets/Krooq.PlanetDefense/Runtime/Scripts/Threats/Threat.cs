using UnityEngine;
using Krooq.Core;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public class Threat : MonoBehaviour
    {
        [SerializeField, ReadOnly] private float _speed = 2f;
        [SerializeField, ReadOnly] private float _health = 10f;
        [SerializeField, ReadOnly] private int _resources = 10;
        [SerializeField, ReadOnly] private IThreatMovement _movement;
        [SerializeField, ReadOnly] private ThreatData _data;
        [SerializeField, ReadOnly] private ThreatModel _model;

        protected GameManager GameManager => this.GetSingleton<GameManager>();
        public PlayerBase PlayerBase => this.GetSingleton<PlayerBase>();
        public Rigidbody2D Rigidbody2D => this.GetCachedComponent<Rigidbody2D>();

        public float Speed => _speed;
        public float Health => _health;
        public int Resources => _resources;
        public ThreatData Data => _data;

        public void Init(ThreatData data)
        {
            _data = data;
            _speed = data.Speed;
            _health = data.Health;
            _resources = data.Resources;
            _movement = new ThreatMovement(data.MovementType);

            if (data.ModelPrefab != null)
            {
                _model = GameManager.Spawn(data.ModelPrefab);
                _model.transform.SetParent(transform);
                _model.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _model.Init(this);
            }
        }

        protected void OnEnable()
        {
            if (GameManager != null) GameManager.Register(this);
        }

        protected void OnDisable()
        {
            if (_model)
            {
                GameManager.Despawn(_model.gameObject);
                _model = null;
            }
            if (GameManager != null) GameManager.Unregister(this);
        }

        protected void FixedUpdate()
        {
            _movement.Move(this);
        }

        public void TakeDamage(float amount)
        {
            _health -= amount;
            if (_health <= 0) Die();
        }

        protected void Die(bool giveResources = true)
        {
            if (giveResources) this.GetSingleton<Player>().AddResources(Resources);
            GameManager.Despawn(gameObject);
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            var go = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
            if (go.TryGetComponent<Building>(out var building))
            {
                building.TakeDamage(1);
                Die(false);
            }
            else if (go.TryGetComponent<PlayerBase>(out var playerBase))
            {
                playerBase.TakeDamage(1);
                Die(false);
            }
            else if (go.layer == LayerMask.NameToLayer("Ground"))
            {
                if (_data.MovementType == ThreatMovementType.Constant)
                {
                    Die(false);
                }
            }
        }
    }
}
