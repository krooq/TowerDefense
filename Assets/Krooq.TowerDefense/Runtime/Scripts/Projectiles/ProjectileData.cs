using UnityEngine;
using UnityEngine.Audio;
using Krooq.Common;

namespace Krooq.TowerDefense
{
    [CreateAssetMenu(fileName = "ProjectileData", menuName = "Tower Defense/ProjectileData")]
    public class ProjectileData : ScriptableObject
    {
        [Header("Visuals & Effects")]
        [SerializeField] private Sprite _icon;
        [SerializeField] private ProjectileModel _projectileModelPrefab;
        [SerializeField] private GameObject _fireEffectPrefab;
        [SerializeField] private GameObject _impactEffectPrefab;
        [SerializeField] private AudioResource _fireSound;
        [SerializeField] private AudioResource _impactSound;

        [Header("Projectile Stats")]
        [SerializeField] private float _fireRate = 1f;
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _size = 1f;
        [SerializeField] private int _pierce = 0;
        [SerializeField] private float _lifetime = 30f;
        [SerializeField] private float _explosionRadius = 0f;
        [SerializeField] private float _explosionDamageMult = 1f;

        public Sprite Icon => _icon;
        public ProjectileModel ProjectileModelPrefab => _projectileModelPrefab;
        public GameObject FireEffectPrefab => _fireEffectPrefab;
        public GameObject ImpactEffectPrefab => _impactEffectPrefab;
        public AudioResource FireSound => _fireSound;
        public AudioResource ImpactSound => _impactSound;

        public float FireRate => _fireRate;
        public float Damage => _damage;
        public float Speed => _speed;
        public float Size => _size;
        public int Pierce => _pierce;
        public float Lifetime => _lifetime;
        public float ExplosionRadius => _explosionRadius;
        public float ExplosionDamageMult => _explosionDamageMult;
    }
}
