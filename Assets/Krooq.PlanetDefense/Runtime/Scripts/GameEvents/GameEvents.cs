using Krooq.Common;
using UnityEngine;

namespace Krooq.PlanetDefense
{
    public struct GameEndedEvent : IGameEvent { }

    public struct SpellCastEvent : IGameEvent
    {
        public SpellData Spell;
        public ICaster Caster;

        public SpellCastEvent(SpellData spell, ICaster caster)
        {
            Spell = spell;
            Caster = caster;
        }
    }

    public struct ProjectileLaunchedEvent : IGameEvent
    {
        public Projectile Projectile;
        public SpellData SourceSpell;
        public ICaster SourceCaster;

        public ProjectileLaunchedEvent(Projectile projectile, SpellData sourceSpell, ICaster sourceCaster)
        {
            Projectile = projectile;
            SourceSpell = sourceSpell;
            SourceCaster = sourceCaster;
        }
    }

    public struct ProjectileHitEvent : IGameEvent
    {
        public Projectile Projectile;
        public GameObject Target;

        public ProjectileHitEvent(Projectile projectile, GameObject target)
        {
            Projectile = projectile;
            Target = target;
        }
    }

    public struct ProjectileDespawnEvent : IGameEvent
    {
        public Projectile Projectile;

        public ProjectileDespawnEvent(Projectile projectile)
        {
            Projectile = projectile;
        }
    }
}
