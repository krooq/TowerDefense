using UnityEngine;
using System.Collections.Generic;
using Krooq.Core;
using Krooq.Common;
using Sirenix.OdinInspector;

namespace Krooq.TowerDefense
{
    public class PlayerCaster : Caster
    {
        protected Player Player => this.GetSingleton<Player>();

        public override IEnumerable<IAbilitySource> AbilitySources
        {
            get
            {
                if (Player == null) yield break;
                if (Player.Relics != null)
                {
                    foreach (var relic in Player.Relics)
                        if (relic != null) yield return relic;
                }
                foreach (var abilitySource in base.AbilitySources)
                    if (abilitySource != null) yield return abilitySource;
            }
        }
    }
}
