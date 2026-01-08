using UnityEngine;
using System.Collections.Generic;
using Krooq.Core;
using Krooq.Common;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public abstract class SpellCaster : MonoBehaviour, ISpellCaster
    {
        [SerializeField] protected Transform _firePoint;
        public Transform FirePoint => _firePoint;

        public abstract ITargetingInfo TargetingInfo { get; }

        protected Dictionary<int, float> _spellCooldowns = new Dictionary<int, float>();

        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected AudioManager AudioManager => this.GetSingleton<AudioManager>();
        protected GameEventManager GameEventManager => this.GetSingleton<GameEventManager>();

        [SerializeField, ReadOnly] protected float _nextSpellDamageMultiplier = 1f;


        protected virtual void Update()
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
        }

        public abstract bool TrySpendMana(int amount);
        public abstract SpellData GetSpell(int index);
        public abstract IEnumerable<IAbilitySource> AbilitySources { get; }

        public virtual void CastSpell(int slotIndex)
        {
            var spell = GetSpell(slotIndex);
            if (spell == null) return;

            // Check Cooldown
            if (_spellCooldowns.TryGetValue(slotIndex, out var timer) && timer > 0) return;

            // Capture current multiplier
            float dmgMult = _nextSpellDamageMultiplier;

            // Reset for next separate cast chain
            _nextSpellDamageMultiplier = 1f;

            ProcessSpell(spell, slotIndex, 1f, dmgMult);
        }

        protected virtual void ProcessSpell(SpellData spell, int slotIndex, float manaCostMult, float damageMult)
        {
            var cost = Mathf.CeilToInt(spell.ManaCost * manaCostMult);
            if (!TrySpendMana(cost))
            {
                // TODO: Feedback
                return;
            }

            // Set Cooldown
            _spellCooldowns[slotIndex] = spell.Cooldown;

            GameEventManager.FireEvent(this, new SpellCastEvent(spell, this));
        }
    }
}
