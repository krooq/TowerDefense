using UnityEngine;
using System.Collections.Generic;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public class Projectile : MonoBehaviour
    {
        [Header("State")]
        [SerializeField, ReadOnly] private Vector3 _direction;
        [SerializeField, ReadOnly] private float _timer;

        [Header("Stats")]
        [SerializeField, ReadOnly] private Stat _damage;
        [SerializeField, ReadOnly] private Stat _speed;
        [SerializeField, ReadOnly] private Stat _size;
        [SerializeField, ReadOnly] private Stat _pierce;
        [SerializeField, ReadOnly] private Stat _lifetime;
        [SerializeField, ReadOnly] private Stat _explosionRadius;
        [SerializeField, ReadOnly] private Stat _explosionDamageMult;
        [SerializeField, ReadOnly] private Stat _splitCount;
        [SerializeField, ReadOnly] private Stat _fireRate;

        private HashSet<string> _tags = new();
        private List<Modifier> _modifiers = new();

        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected MultiGameObjectPool Pool => this.GetSingleton<MultiGameObjectPool>();
        protected Rigidbody2D Rigidbody2D => this.GetCachedComponent<Rigidbody2D>();

        public float Damage => _damage.Value;
        public float Speed => _speed.Value;
        public float Size => _size.Value;
        public int PierceCount => (int)_pierce.Value;
        public float Lifetime => _lifetime.Value;
        public float FireRate => _fireRate.Value;

        public void Init(Vector3 direction, ProjectileWeaponData weaponData, IEnumerable<Modifier> modifiers)
        {
            _direction = direction;
            _tags.Clear();
            _modifiers = new List<Modifier>(modifiers);

            // Initialize Stats
            _damage = new Stat().WithBaseValue(weaponData.BaseDamage);
            _speed = new Stat().WithBaseValue(weaponData.BaseSpeed);
            _size = new Stat().WithBaseValue(weaponData.BaseSize);
            _pierce = new Stat().WithBaseValue(weaponData.BasePierce);
            _lifetime = new Stat().WithBaseValue(weaponData.BaseLifetime);
            _fireRate = new Stat().WithBaseValue(weaponData.FireRate);

            // Initialize Optional Stats (default to 0 or 1 as appropriate)
            _explosionRadius = new Stat().WithBaseValue(0f);
            _explosionDamageMult = new Stat().WithBaseValue(1f);
            _splitCount = new Stat().WithBaseValue(0f);

            _timer = Lifetime;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

            // Apply Modifiers (Always trigger)
            foreach (var mod in _modifiers)
            {
                mod.Process(this, ModifierTrigger.Always);
            }

            // Update Scale
            transform.localScale = Vector3.one * Size;
        }

        public void AddStatModifier(StatModifier modifier)
        {
            if (modifier.StatData == null) return;

            if (modifier.StatData == GameManager.Data.DamageStat) _damage.AddModifier(modifier);
            else if (modifier.StatData == GameManager.Data.SpeedStat) _speed.AddModifier(modifier);
            else if (modifier.StatData == GameManager.Data.SizeStat) _size.AddModifier(modifier);
            else if (modifier.StatData == GameManager.Data.PierceStat) _pierce.AddModifier(modifier);
            else if (modifier.StatData == GameManager.Data.LifetimeStat) _lifetime.AddModifier(modifier);
            else if (modifier.StatData == GameManager.Data.ExplosionRadiusStat) _explosionRadius.AddModifier(modifier);
            else if (modifier.StatData == GameManager.Data.ExplosionDamageMultStat) _explosionDamageMult.AddModifier(modifier);
            else if (modifier.StatData == GameManager.Data.SplitCountStat) _splitCount.AddModifier(modifier);
            else if (modifier.StatData == GameManager.Data.FireRateStat) _fireRate.AddModifier(modifier);
        }

        public void AddTag(string tag) => _tags.Add(tag);
        public void RemoveTag(string tag) => _tags.Remove(tag);
        public bool HasTag(string tag) => _tags.Contains(tag);

        protected void OnDisable()
        {
            _tags.Clear();
            _modifiers.Clear();
        }

        public void SpawnChild(GameObject prefab, int count)
        {
            if (prefab == null) return;

            for (int i = 0; i < count; i++)
            {
                var obj = Pool.Get(prefab);
                obj.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            }
        }

        protected void FixedUpdate()
        {
            var moveDist = Speed * Time.fixedDeltaTime;
            Rigidbody2D.MovePosition(Rigidbody2D.position + (Vector2)(_direction * moveDist));

            _timer -= Time.fixedDeltaTime;
            if (_timer <= 0)
            {
                foreach (var mod in _modifiers) mod.Process(this, ModifierTrigger.OnDespawn);
                GameManager.Despawn(gameObject);
            }
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            foreach (var mod in _modifiers) mod.Process(this, ModifierTrigger.OnHit);

            var threat = other.attachedRigidbody.gameObject.GetCachedComponent<Threat>();
            if (threat != null)
            {
                threat.TakeDamage(Damage);

                if (HasTag("Explosive"))
                {
                    Explode();
                }

                if (PierceCount > 0)
                {
                    if (GameManager.Data.PierceStat != null)
                    {
                        AddStatModifier(new StatModifier(GameManager.Data.PierceStat, -1, StatModifier.ModifierType.Additive));
                    }
                }
                else
                {
                    GameManager.Despawn(gameObject);
                }
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                if (HasTag("Explosive"))
                {
                    Explode();
                }
                GameManager.Despawn(gameObject);
            }
        }

        private void Explode()
        {
            float radius = _explosionRadius.Value;
            float damageMult = _explosionDamageMult.Value;
            if (damageMult == 0) damageMult = 1f; // Default

            var hits = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Threat>(out var threat))
                {
                    threat.TakeDamage(Damage * damageMult);
                }
            }
        }
    }
}
