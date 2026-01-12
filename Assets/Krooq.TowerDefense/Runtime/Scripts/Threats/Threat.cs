using UnityEngine;
using Krooq.Core;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace Krooq.TowerDefense
{
    public enum ThreatState
    {
        Idle,
        Moving,
        Attacking,
        Dead
    }

    public class Threat : MonoBehaviour
    {
        [SerializeField, ReadOnly] private ThreatState _state = ThreatState.Idle;
        [SerializeField, ReadOnly] private float _speed = 2f;
        [SerializeField, ReadOnly] private float _health = 10f;
        [SerializeField, ReadOnly] private int _resources = 10;
        [SerializeField, ReadOnly] private float _damage = 1f;
        [SerializeField, ReadOnly] private float _attackRate = 1f;
        [SerializeField, ReadOnly] private float _attackRange = 1f;
        [SerializeField, ReadOnly] private ThreatAttackType _attackType = ThreatAttackType.Melee;
        [SerializeField, ReadOnly] private IThreatMovement _movement;
        [SerializeField, ReadOnly] private ThreatData _data;
        [SerializeField, ReadOnly] private ThreatModel _model;
        [SerializeField, ReadOnly] private UnityEvent _onDeath = new();
        [SerializeField, ReadOnly] private Animator _animator;
        private PlayerTower _currentTarget;
        private float _lastAttackTime;
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
            _damage = data.Damage;
            _attackRate = data.AttackRate;
            _attackRange = data.AttackRange;
            _attackType = data.AttackType;
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

            _animator = GetComponentInChildren<Animator>();
            ChangeState(ThreatState.Moving);
        }

        protected void OnEnable()
        {
            if (GameManager != null) GameManager.Register(this);
        }

        protected void OnDisable()
        {

            if (GameManager != null) GameManager.Unregister(this);
        }

        public void ChangeState(ThreatState newState)
        {
            if (_state == newState) return;

            _state = newState;

            if (_animator != null)
            {
                switch (_state)
                {
                    case ThreatState.Idle:
                        _animator.SetTrigger("Idle");
                        break;
                    case ThreatState.Moving:
                        _animator.SetTrigger("Move");
                        break;
                    case ThreatState.Attacking:
                        _animator.SetTrigger("Attack");
                        break;
                    case ThreatState.Dead:
                        _animator.SetTrigger("Death");
                        break;
                }
            }
        }

        protected void FixedUpdate()
        {
            if (!IsAlive) return;

            CheckAttack();

            switch (_state)
            {
                case ThreatState.Moving:
                    Move();
                    break;
                case ThreatState.Attacking:
                case ThreatState.Idle:
                case ThreatState.Dead:
                    break;
            }
        }

        private void CheckAttack()
        {
            if (_state == ThreatState.Dead) return;
            if (_state == ThreatState.Attacking) return;
            if (Time.time < _lastAttackTime + _attackRate) return;

            if (_attackType == ThreatAttackType.Melee)
            {
                if (_currentTarget != null)
                {
                    Attack(_currentTarget);
                }
            }
            else
            {
                if (PlayerTower == null) return;
                var targetPos = PlayerTower.GetClosestPoint(transform.position);
                if (Vector3.Distance(transform.position, targetPos) <= _attackRange)
                {
                    Attack(PlayerTower);
                }
            }
        }

        public void TakeDamage(float amount)
        {
            _health -= amount;
            if (_health <= 0) Die();
        }

        protected void Die(bool giveResources = true)
        {
            if (_state == ThreatState.Dead) return;
            _onDeath.Invoke();
            ChangeState(ThreatState.Dead);
            if (giveResources) this.GetSingleton<Player>().AddResources(Resources);
            GameManager.Despawn(gameObject);
        }

        public void Attack(PlayerTower tower)
        {
            ChangeState(ThreatState.Attacking);
            tower.TakeDamage((int)_damage);
            _lastAttackTime = Time.time;
        }

        protected void Move()
        {
            _movement.Move(this);
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            var go = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
            var playerTower = go.GetCachedComponent<PlayerTower>();
            if (playerTower != null)
            {
                _currentTarget = playerTower;
            }
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            var go = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
            var playerTower = go.GetCachedComponent<PlayerTower>();
            if (playerTower != null && _currentTarget == playerTower)
            {
                _currentTarget = null;
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
