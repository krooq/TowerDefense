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

        [SerializeField, ReadOnly] private int _selectedSlotIndex = 0;
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
            // Slot Selection
            if (Player.Inputs.Spell1Action.WasPressedThisFrame()) _selectedSlotIndex = 0;
            if (Player.Inputs.Spell2Action.WasPressedThisFrame()) _selectedSlotIndex = 1;
            if (Player.Inputs.Spell3Action.WasPressedThisFrame()) _selectedSlotIndex = 2;
            if (Player.Inputs.Spell4Action.WasPressedThisFrame()) _selectedSlotIndex = 3;

            // Firing
            if (Player.Inputs.ClickAction.WasPressedThisFrame())
            {
                CastSelectedSpell();
            }

            // Quick Casting
            if (Player.Inputs.QuickCast1Action.WasPressedThisFrame()) CastSpell(0);
            if (Player.Inputs.QuickCast2Action.WasPressedThisFrame()) CastSpell(1);
            if (Player.Inputs.QuickCast3Action.WasPressedThisFrame()) CastSpell(2);
            if (Player.Inputs.QuickCast4Action.WasPressedThisFrame()) CastSpell(3);
        }

        private void CastSelectedSpell()
        {
            CastSpell(_selectedSlotIndex);
        }

        public override SpellData GetSpell(int index)
        {
            var spells = Player.Spells;
            if (index < 0 || index >= spells.Count) return null;
            return spells[index];
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
                if (Player.Spells != null)
                {
                    foreach (var spell in Player.Spells)
                        if (spell != null) yield return spell;
                }
            }
        }

        protected override void ProcessSpell(SpellData spell, int slotIndex)
        {
            base.ProcessSpell(spell, slotIndex);
            _lastCastSpell = spell;
        }
    }
}
