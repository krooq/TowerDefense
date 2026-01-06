using UnityEngine;
using Krooq.Core;
using Krooq.Common;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Krooq.PlanetDefense
{
    public class PlayerWeapon : MonoBehaviour
    {
        [SerializeField] private Transform _pivot;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private SpringTransform _recoilTransform;
        [SerializeField] private List<GameObject> _fireEffects;

        [SerializeField, ReadOnly] private float _fireTimer;

        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected AudioManager AudioManager => this.GetSingleton<AudioManager>();

        protected void Update()
        {
            if (GameManager.State != GameState.Playing) return;
            _fireTimer -= Time.deltaTime;
        }

        public void Aim(Vector3 targetPosition, float rotationSpeed)
        {
            var dir = (targetPosition - _pivot.position).normalized;

            // // Restrict to upper semicircle
            // if (dir.y < 0)
            // {
            //     dir.y = 0;
            //     dir.x = Mathf.Sign(dir.x);
            // }

            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f; // Assuming sprite points up

            _pivot.rotation = Quaternion.Lerp(_pivot.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * rotationSpeed);
        }

        public Projectile TryFire(PlayerTargetingReticle targetingReticle, ProjectileWeaponData weaponData, IEnumerable<Modifier> modifiers, float damageMultiplier = 1f)
        {
            if (_fireTimer <= 0 && weaponData != null)
            {
                return Fire(targetingReticle, weaponData, modifiers, damageMultiplier);
            }
            return null;
        }

        private Projectile Fire(PlayerTargetingReticle targetingReticle, ProjectileWeaponData weaponData, IEnumerable<Modifier> modifiers, float damageMultiplier)
        {
            var p = GameManager.Spawn(GameManager.Data.ProjectilePrefab);
            if (p == null) return null;

            p.transform.SetPositionAndRotation(_firePoint.position, _firePoint.rotation);

            // Finalize
            p.Init(_firePoint.up, weaponData, modifiers, targetingReticle);

            if (damageMultiplier != 1f)
            {
                // Create a temporary modifier for damage
                var dmgMod = new StatModifier(GameManager.Data.DamageStat, damageMultiplier, StatModifier.ModifierType.Multiplicative);
                p.AddStatModifier(dmgMod);
            }

            // Set Fire Timer based on projectile stat
            _fireTimer = 1f / p.FireRate;

            foreach (var effect in _fireEffects)
            {
                effect.SetActive(false);
                effect.SetActive(true);
            }

            // _recoilTransform.BumpPosition(_firePoint.localPosition - Vector3.up * 10f);
            if (weaponData.FireSound != null) AudioManager.PlaySound(weaponData.FireSound);

            return p;
        }
    }
}
