using UnityEngine;
using Krooq.Core;
using Sirenix.OdinInspector;
using UnityEngine.Events;

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
        [SerializeField, ReadOnly] private UnityEvent _onDeath = new();
        protected GameManager GameManager => this.GetSingleton<GameManager>();
        public PlayerTower PlayerTower => this.GetSingleton<PlayerTower>();
        public Rigidbody2D Rigidbody2D => this.GetCachedComponent<Rigidbody2D>();

        public float Speed => _speed;
        public float Health => _health;
        public int Resources => _resources;
        public bool IsAlive => _health > 0;
        public ThreatData Data => _data;
        public UnityEvent OnDeath => _onDeath;

        public void Init(ThreatData data)
        {
            _data = data;
            _speed = data.Speed;
            _health = data.Health;
            _resources = data.Resources;
            _movement = new ThreatMovement(data.MovementType);

            if (_model)
            {
                GameManager.Despawn(_model);
                _model = null;
            }

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
            _onDeath.Invoke();
            if (giveResources) this.GetSingleton<Player>().AddResources(Resources);
            GameManager.Despawn(gameObject);
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            var go = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
            var playerTower = go.GetCachedComponent<PlayerTower>();
            if (playerTower != null)
            {
                playerTower.TakeDamage(1);
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

        public Vector3 GetClosestPoint(Vector3 position)
        {
            if (_model != null) return _model.GetClosestPoint(position);
            return transform.position;
        }
        public Vector3 GetMidPoint()
        {
            if (_model != null) return _model.GetMidPoint();
            return transform.position;
        }
    }
}
