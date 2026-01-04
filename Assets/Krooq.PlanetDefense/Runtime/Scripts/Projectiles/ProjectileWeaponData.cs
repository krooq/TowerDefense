using UnityEngine;
using UnityEngine.Audio;
using Krooq.Common;

namespace Krooq.PlanetDefense
{
    [CreateAssetMenu(fileName = "ProjectileWeaponData", menuName = "PlanetDefense/ProjectileWeaponData")]
    public class ProjectileWeaponData : ScriptableObject
    {
        [Header("Display")]
        [SerializeField] private string _weaponName;
        [SerializeField] private Sprite _icon;

        [Header("Cannon")]
        [SerializeField] private float _fireRate = 0.5f;
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private AudioResource _fireSound;

        [Header("Projectile Stats")]
        [SerializeField] private float _baseDamage = 10f;
        [SerializeField] private float _baseSpeed = 10f;
        [SerializeField] private float _baseSize = 1f;
        [SerializeField] private int _basePierce = 0;
        [SerializeField] private float _baseLifetime = 5f;

        public string WeaponName => _weaponName;
        public Sprite Icon => _icon;
        public float FireRate => _fireRate;
        public Projectile ProjectilePrefab => _projectilePrefab;
        public AudioResource FireSound => _fireSound;

        public float BaseDamage => _baseDamage;
        public float BaseSpeed => _baseSpeed;
        public float BaseSize => _baseSize;
        public int BasePierce => _basePierce;
        public float BaseLifetime => _baseLifetime;
    }
}
