using UnityEngine;
using System.Collections.Generic;
using Krooq.Core;
using Krooq.Common;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public class PlayerCaster : Caster
    {
        [SerializeField] private Transform _pivot;

        public override ITargetingInfo TargetingInfo => Player.TargetingReticle;

        [SerializeField, ReadOnly] private SpellData _lastCastSpell;

        protected Player Player => this.GetSingleton<Player>();

        protected override void Update()
        {
            base.Update();
            if (GameManager.State != GameState.Playing) return;
            HandleInput();
        }

        // Disable default behaviors from (NonPlayer) Caster
        protected override void PerformTargeting() { }

        public void Aim(Vector3 targetPosition)
        {
            var dir = (targetPosition - _pivot.position).normalized;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f; // Assuming sprite points up
            var t = 1f; // Don't think we need to make this take any time. //Time.deltaTime * rotationSpeed;
            _pivot.rotation = Quaternion.Lerp(_pivot.rotation, Quaternion.Euler(0, 0, angle), t);
        }

        private void HandleInput()
        {
            // Firing
            if (Player.Inputs.ClickAction.WasPressedThisFrame())
            {
                CastSpell();
            }

            // Quick Casting
            if (Player.Inputs.QuickCast1Action.WasPressedThisFrame()) CastSpell();
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

        protected override void ProcessSpell(SpellData spell)
        {
            base.ProcessSpell(spell);
            _lastCastSpell = spell;
        }
    }
}
