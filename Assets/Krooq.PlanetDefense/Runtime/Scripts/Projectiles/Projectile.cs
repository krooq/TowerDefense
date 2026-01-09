using UnityEngine;
using System.Collections.Generic;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace Krooq.PlanetDefense
{
    public class Projectile : MonoBehaviour
    {
        private const string Explosive = "Explosive";

        [Header("State")]
        [SerializeField, ReadOnly] private Vector3 _direction;
        [SerializeField, ReadOnly] private float _timer;
        [SerializeField, ReadOnly] private Vector3? _target;
        [SerializeField, ReadOnly] private HashSet<string> _tags = new();
        [SerializeField, ReadOnly] private ProjectileModel _model;
        [SerializeField, ReadOnly] private ProjectileData _data;

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

        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected AudioManager AudioManager => this.GetSingleton<AudioManager>();
        protected Rigidbody2D Rigidbody2D => this.GetCachedComponent<Rigidbody2D>();
        protected GameEventManager GameEventManager => this.GetSingleton<GameEventManager>();

        public float Damage => _damage.Value;
        public float Speed => _speed.Value;
        public float Size => _size.Value;
        public int PierceCount => (int)_pierce.Value;
        public float Lifetime => _lifetime.Value;
        public float FireRate => _fireRate.Value;

        public void Init(Vector3 direction, ProjectileData data, SpellData sourceSpell, ICaster sourceCaster, ITargetingInfo targetingInfo = null)
        {
            _data = data;
            _direction = direction;
            _tags.Clear();
            _target = null;
            GameManager.Despawn(_model);

            if (_data.FireEffectPrefab != null)
            {
                SpawnEffect(_data.FireEffectPrefab, transform.position, Quaternion.identity);
            }

            if (_data.FireSound != null)
            {
                AudioManager.PlaySound(_data.FireSound);
            }

            if (_data.ProjectileModelPrefab != null)
            {
                _model = GameManager.Spawn(_data.ProjectileModelPrefab);
                _model.transform.SetParent(transform);
                _model.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _model.Init(this);
            }

            if (targetingInfo != null && targetingInfo.IsGroundTarget)
            {
                var dist = Vector3.Distance(transform.position, targetingInfo.TargetPosition);
                _target = transform.position + (direction.normalized * dist);
            }

            // Initialize Stats
            _damage = new Stat().WithBaseValue(data.BaseDamage);
            _speed = new Stat().WithBaseValue(data.BaseSpeed);
            _size = new Stat().WithBaseValue(data.BaseSize);
            _pierce = new Stat().WithBaseValue(data.BasePierce);
            _lifetime = new Stat().WithBaseValue(data.BaseLifetime);
            _fireRate = new Stat().WithBaseValue(data.BaseFireRate);

            // Initialize Optional Stats (default to 0 or 1 as appropriate)
            _explosionRadius = new Stat().WithBaseValue(0f);
            _explosionDamageMult = new Stat().WithBaseValue(1f);
            _splitCount = new Stat().WithBaseValue(0f);

            _timer = Lifetime;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

            // Fire Event
            GameEventManager.FireEvent(this, new ProjectileLaunchedEvent(this, sourceSpell, sourceCaster));

            // Update Scale
            transform.localScale = Vector3.one * Size;
        }

        private async void SpawnEffect(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null) return;
            var effect = GameManager.Spawn(prefab);
            effect.transform.SetPositionAndRotation(position, rotation);

            await UniTask.Delay(5000, cancellationToken: this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();

            if (effect != null && GameManager != null) GameManager.Despawn(effect);
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

        public void SpawnChild(GameObject prefab, int count)
        {
            if (prefab == null) return;

            for (int i = 0; i < count; i++)
            {
                var obj = GameManager.Spawn(prefab);
                obj.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            }
        }

        protected void FixedUpdate()
        {
            var moveStep = (Vector2)(_direction * (Speed * Time.fixedDeltaTime));

            if (_target.HasValue)
            {
                var distSq = ((Vector2)_target.Value - Rigidbody2D.position).sqrMagnitude;
                if (distSq <= moveStep.sqrMagnitude)
                {
                    GameEventManager.FireEvent(this, new ProjectileHitEvent(this, null));
                    if (HasTag(Explosive)) Explode();
                    HandleImpact(true, Quaternion.LookRotation(Vector3.forward, Vector3.up));
                    return;
                }
            }

            Rigidbody2D.MovePosition(Rigidbody2D.position + moveStep);

            _timer -= Time.fixedDeltaTime;
            if (_timer <= 0)
            {
                GameEventManager.FireEvent(this, new ProjectileDespawnEvent(this));
                HandleImpact(false);
            }
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (!gameObject.activeInHierarchy) return; // Ignore collisions if already dealing with impact

            var rb = other.attachedRigidbody;
            var go = rb != null ? rb.gameObject : other.gameObject;
            GameEventManager.FireEvent(this, new ProjectileHitEvent(this, go));

            var threat = go.GetCachedComponent<Threat>();
            if (threat != null)
            {
                threat.TakeDamage(Damage);

                if (HasTag(Explosive))
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
                    HandleImpact(true, Quaternion.LookRotation(Vector3.forward, -_direction));
                }
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                if (HasTag(Explosive))
                {
                    Explode();
                }
                HandleImpact(true, Quaternion.LookRotation(Vector3.forward, Vector3.up));
            }
        }

        private async void HandleImpact(bool spawnEffects = true, Quaternion? impactRotation = null)
        {
            if (spawnEffects && _data != null)
            {
                if (_data.ImpactSound != null && AudioManager != null)
                {
                    AudioManager.PlaySound(_data.ImpactSound);
                }

                if (_data.ImpactEffectPrefab != null)
                {
                    SpawnEffect(_data.ImpactEffectPrefab, transform.position, impactRotation ?? Quaternion.identity);
                }
            }

            // Wait for effects to finish (simplistic approach: just wait 5s for the longest possible effect)
            // Ideally we track active effects count, but for now we trust the fire-and-forget SpawnEffect logic 
            // has its own cleanup, but we must stay alive to run that cleanup if we are the one awaiting.
            // Actually, SpawnEffect is async void, so it runs detached.
            // But if we return to pool, the gameObject is disabled, which MIGHT cancel the UniTask if it is linked to this GameObject.
            // So we should wait here.
            gameObject.SetActive(false);
            await UniTask.Delay(5000, cancellationToken: this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();

            if (this != null && GameManager != null) GameManager.Despawn(gameObject);
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
