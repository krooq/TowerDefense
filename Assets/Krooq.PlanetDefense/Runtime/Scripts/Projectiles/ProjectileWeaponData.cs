using UnityEngine;
using UnityEngine.Audio;
using Krooq.Common;

namespace Krooq.PlanetDefense
{
    [CreateAssetMenu(fileName = "ProjectileWeaponData", menuName = "PlanetDefense/ProjectileWeaponData")]
    public class ProjectileWeaponData : ScriptableObject
    {
        [Header("Visuals & Effects")]
        [SerializeField] private Sprite _icon;
        [SerializeField] private ProjectileModel _projectileModelPrefab;
        [SerializeField] private AudioResource _fireSound;

        [Header("Projectile Stats")]
        [SerializeField] private float _baseFireRate = 0.5f;
        [SerializeField] private float _baseDamage = 10f;
        [SerializeField] private float _baseSpeed = 10f;
        [SerializeField] private float _baseSize = 1f;
        [SerializeField] private int _basePierce = 0;
        [SerializeField] private float _baseLifetime = 30f;

        public Sprite Icon => _icon;
        public ProjectileModel ProjectileModelPrefab => _projectileModelPrefab;
        public AudioResource FireSound => _fireSound;

        public float BaseFireRate => _baseFireRate;
        public float BaseDamage => _baseDamage;
        public float BaseSpeed => _baseSpeed;
        public float BaseSize => _baseSize;
        public int BasePierce => _basePierce;
        public float BaseLifetime => _baseLifetime;
    }
}
