using Krooq.Common;
using UnityEngine;

namespace Krooq.TowerDefense
{
    public struct GameEndedEvent : IGameEvent { }

    public struct SpellCastEvent : IGameEvent
    {
        public Spell Spell;
        public ICaster Caster;
        public SpellCastEvent(Spell spell, Caster caster) : this()
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
