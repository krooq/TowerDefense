using UnityEngine;
using Krooq.Common;
using Krooq.Core;

using Cysharp.Threading.Tasks;

namespace Krooq.PlanetDefense
{
    [CreateAssetMenu(fileName = "FireProjectileAbility", menuName = "PlanetDefense/Abilities/FireProjectile")]
    public class FireProjectileAbilityDefinition : AbilityDefinition
    {
        [SerializeField] private ProjectileWeaponData _projectileData;
        public ProjectileWeaponData ProjectileData => _projectileData;

        public override IAbility Create()
        {
            return new FireProjectileAbility(this);
        }
    }

    public class FireProjectileAbility : Ability
    {
        private FireProjectileAbilityDefinition _def;

        public FireProjectileAbility(FireProjectileAbilityDefinition def)
        {
            _def = def;
        }

        public override async UniTask OnGameEvent(IGameEvent gameEvent)
        {
            if (gameEvent is SpellCastEvent spellCast)
            {
                if (Source is Spell sourceSpell && sourceSpell == spellCast.Spell)
                {
                    Fire(spellCast);
                }
            }
        }

        private void Fire(SpellCastEvent e)
        {
            var gm = Owner.GetSingleton<GameManager>();
            var data = _def.ProjectileData;
            if (data == null) return;

            var prefab = gm.Data.ProjectilePrefab;

            var p = gm.Spawn(prefab);
            if (p == null) return;

            var caster = Owner.GetCachedComponent<PlayerSpellCaster>();
            p.transform.SetPositionAndRotation(caster.FirePoint.position, caster.FirePoint.rotation);

            p.Init(caster.FirePoint.up, data, e.Spell, Owner, caster.TargetingReticle);
        }
    }
}
