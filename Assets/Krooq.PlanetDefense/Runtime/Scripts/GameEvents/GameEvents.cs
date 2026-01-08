using Krooq.Common;
using UnityEngine;

namespace Krooq.PlanetDefense
{
    public struct SpellCastEvent : IGameEvent
    {
        public Spell Spell;
        public Player Source;

        public SpellCastEvent(Spell spell, Player source)
        {
            Spell = spell;
            Source = source;
        }
    }

    public struct ProjectileLaunchedEvent : IGameEvent
    {
        public Projectile Projectile;
        public Spell SourceSpell;
        public Player SourcePlayer;

        public ProjectileLaunchedEvent(Projectile projectile, Spell sourceSpell, Player sourcePlayer)
        {
            Projectile = projectile;
            SourceSpell = sourceSpell;
            SourcePlayer = sourcePlayer;
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
