using UnityEngine;
using System.Collections.Generic;
using Krooq.Core;
using Krooq.Common;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public class PlayerSpellCaster : MonoBehaviour
    {
        [SerializeField] private Transform _pivot;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private PlayerTargetingReticle _targetingReticle;

        public Transform FirePoint => _firePoint;
        public PlayerTargetingReticle TargetingReticle => _targetingReticle;

        [SerializeField, ReadOnly] private int _selectedSlotIndex = 0;
        [SerializeField, ReadOnly] private Spell _lastCastSpell;
        [SerializeField, ReadOnly] private float _nextSpellDamageMultiplier = 1f;

        private Dictionary<int, float> _spellCooldowns = new Dictionary<int, float>();

        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected Player Player => this.GetSingleton<Player>();
        protected AudioManager AudioManager => this.GetSingleton<AudioManager>();
        protected Krooq.Common.GameEventManager GameEventManager => this.GetSingleton<Krooq.Common.GameEventManager>();

        private void Update()
        {
            if (GameManager.State != GameState.Playing) return;

            // Update Cooldowns
            var keys = new List<int>(_spellCooldowns.Keys);
            foreach (var key in keys)
            {
                if (_spellCooldowns[key] > 0)
                {
                    _spellCooldowns[key] -= Time.deltaTime;
                }
            }

            HandleInput();
        }

        public void Aim(Vector3 targetPosition, float rotationSpeed)
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

        private void CastSelectedSpell() => CastSpell(_selectedSlotIndex);

        private void CastSpell(int slotIndex)
        {
            var spells = GameManager.Spells;
            if (slotIndex < 0 || slotIndex >= spells.Count) return;
            var spell = spells[slotIndex];
            if (spell == null) return;

            // Check Cooldown
            if (_spellCooldowns.TryGetValue(slotIndex, out var timer) && timer > 0) return;

            // Capture current multiplier
            float dmgMult = _nextSpellDamageMultiplier;

            // Reset for next separate cast chain
            _nextSpellDamageMultiplier = 1f;

            ProcessSpell(spell, slotIndex, 1f, dmgMult);
        }

        private void ProcessSpell(Spell spell, int slotIndex, float manaCostMult, float damageMult)
        {
            var cost = Mathf.CeilToInt(spell.ManaCost * manaCostMult);
            if (!Player.TrySpendMana(cost))
            {
                // TODO: Feedback
                return;
            }

            // Set Cooldown
            _spellCooldowns[slotIndex] = spell.Cooldown;

            GameEventManager?.FireEvent(this, new SpellCastEvent(spell, Player));

            _lastCastSpell = spell;
        }
    }
}
