using UnityEngine;
using Krooq.Common;
using Krooq.Core;

using Cysharp.Threading.Tasks;

namespace Krooq.PlanetDefense
{
    [CreateAssetMenu(fileName = "FireProjectileAbility", menuName = "PlanetDefense/Abilities/FireProjectile")]
    public class FireProjectileAbilityData : AbilityData
    {
        [SerializeField] private ProjectileData _projectileData;
        public ProjectileData ProjectileData => _projectileData;

        public override IAbility Create()
        {
            return new FireProjectileAbility(this);
        }
    }

    public class FireProjectileAbility : Ability
    {
        private FireProjectileAbilityData _data;

        public FireProjectileAbility(FireProjectileAbilityData data)
        {
            _data = data;
        }

        public override async UniTask OnGameEvent(IGameEvent gameEvent)
        {
            if (gameEvent is not SpellCastEvent spellCast) return;
            if (Source is not SpellData sourceSpell) return;
            if (sourceSpell != spellCast.Spell) return;
            FireProjectile(spellCast);
        }

        private void FireProjectile(SpellCastEvent e)
        {
            var gm = Owner.gameObject.GetSingleton<GameManager>();
            var data = _data.ProjectileData;
            if (data == null) return;

            var prefab = gm.Data.ProjectilePrefab;

            var p = gm.Spawn(prefab);
            if (p == null) return;

            p.transform.SetPositionAndRotation(Owner.FirePoint.position, Owner.FirePoint.rotation);

            p.Init(Owner.FirePoint.up, data, e.Spell, Owner, Owner.TargetingInfo);
        }
    }
}
