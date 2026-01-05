using UnityEngine;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public class Threat : MonoBehaviour
    {
        [SerializeField] private float _speed = 2f;
        [SerializeField] private float _health = 10f;
        [SerializeField] private int _resources = 10;
        [SerializeField, ReadOnly] private Vector3 _direction = Vector3.down;
        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected Rigidbody2D Rigidbody2D => this.GetCachedComponent<Rigidbody2D>();

        public float Speed => _speed;
        public float Health => _health;
        public int Resources => _resources;

        public void Init(float speed, float health, int resources, Vector3 direction)
        {
            _speed = speed;
            _health = health;
            _resources = resources;
            _direction = direction;
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
            Rigidbody2D.MovePosition(Rigidbody2D.position + (Vector2)(_direction * Speed * Time.fixedDeltaTime));
        }

        public void TakeDamage(float amount)
        {
            _health -= amount;
            if (_health <= 0)
            {
                Die();
            }
        }

        protected void Die(bool giveResources = true)
        {
            if (giveResources) GameManager.AddResources(Resources);
            GameManager.Despawn(gameObject);
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent<Building>(out var building))
            {
                building.TakeDamage(1);
                Die(false);
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Die(false);
            }
        }
    }
}
