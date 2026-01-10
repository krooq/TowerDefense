using UnityEngine;
using System.Collections.Generic;
using Krooq.Core;
using Krooq.Common;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public class PlayerCaster : Caster
    {
        [SerializeField] private bool _manualControlEnabled;

        public override ITarget TargetingInfo => _manualControlEnabled ? Player.TargetingReticle : base.TargetingInfo;

        protected Player Player => this.GetSingleton<Player>();

        protected override void Update()
        {
            base.Update();
            if (GameManager.State != GameState.Playing) return;
            HandleInput();
        }

        private void HandleInput()
        {
            if (!_manualControlEnabled) return;

            // Firing
            if (Player.Inputs.ClickAction.WasPressedThisFrame())
            {
                CastSpell();
                Player.Attack();
            }

            // Quick Casting
            if (Player.Inputs.QuickCast1Action.WasPressedThisFrame())
            {
                CastSpell();
                Player.Attack();
            }
        }

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
                    yield return abilitySource;
            }
        }
    }
}
