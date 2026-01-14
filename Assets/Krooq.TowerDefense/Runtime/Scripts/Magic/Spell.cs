using UnityEngine;
using System.Collections.Generic;
using Krooq.Core;

namespace Krooq.TowerDefense
{
    [System.Serializable]
    public class Spell : IAbilitySource
    {
        private SpellData _data;
        private float _cooldownRemaining;
        private ITargetingStrategy _targetingStrategy;
        private ITarget _target;

        public SpellData Data => _data;
        public float CooldownRemaining => _cooldownRemaining;
        public ITarget Target => _target;

        public IReadOnlyList<AbilityData> Abilities => _data != null ? _data.Abilities : new List<AbilityData>();

        public Spell(SpellData data)
        {
            _data = data;
            _cooldownRemaining = 0;
            _targetingStrategy = new TargetingStrategy(data.TargetingStrategyType);
        }

        public void Update(float deltaTime)
        {
            _cooldownRemaining = Mathf.Clamp(_cooldownRemaining - deltaTime, 0, _data.Cooldown);
        }

        public void FindTarget(Vector3 sourcePos, IReadOnlyList<Threat> threats)
        {
            if (_targetingStrategy == null) return;
            var threat = _targetingStrategy.FindTarget(sourcePos, Data.Range, threats);
            _target = threat == null ? default : new ThreatTarget(threat);
        }

        public bool CanCast(Vector3 sourcePos, IReadOnlyList<Threat> threats)
        {
            if (_cooldownRemaining > 0) return false;
            FindTarget(sourcePos, threats);
            if (_target == null || !_target.IsValid) return false;
            return true;
        }

        public bool TryCast(Vector3 sourcePos, IReadOnlyList<Threat> threats)
        {
            if (!CanCast(sourcePos, threats)) return false;

            // Cast successful.
            Cast();
            return true;
        }

        public void Cast()
        {
            _cooldownRemaining = _data.Cooldown;
        }
    }
}
