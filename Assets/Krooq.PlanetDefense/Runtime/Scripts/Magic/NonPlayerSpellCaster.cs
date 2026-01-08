using UnityEngine;
using System.Collections.Generic;
using Krooq.Core;
using Krooq.Common;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public class NonPlayerSpellCaster : SpellCaster, IAbilitySource
    {
        [Header("Settings")]
        [SerializeField] private SpellData _initialSpell;
        [SerializeField] private float _maxMana = 50f;
        [SerializeField] private float _manaRegen = 5f;
        [SerializeField] private TargetingData _targetingStrategy;
        [SerializeField] private float _range = 10f;

        [SerializeField] private List<AbilityData> _innateAbilities = new();

        [Header("State")]
        [SerializeField, ReadOnly] private float _currentMana;
        [SerializeField, ReadOnly] private SpellData _equippedSpell;

        private Transform _currentTarget;
        private Vector3 _targetPos;
        private bool _isGroundTarget = true;

        private AbilityController _abilityController;

        // AbilitySource implementation
        public IReadOnlyList<AbilityData> Abilities => _innateAbilities;

        public override IEnumerable<IAbilitySource> AbilitySources
        {
            get
            {
                yield return this;
                if (_equippedSpell != null) yield return _equippedSpell;
            }
        }

        private class AI_TargetingInfo : ITargetingInfo
        {
            private NonPlayerSpellCaster _owner;
            public AI_TargetingInfo(NonPlayerSpellCaster owner) { _owner = owner; }
            public bool IsGroundTarget => _owner._isGroundTarget;
            public Vector3 TargetPosition => _owner._targetPos;
        }

        private AI_TargetingInfo _targetingInfo;
        public override ITargetingInfo TargetingInfo => _targetingInfo;

        private void Awake()
        {
            _targetingInfo = new AI_TargetingInfo(this);
            _currentMana = _maxMana;

            // Setup Ability Controller
            _abilityController = GetComponent<AbilityController>();
            if (_abilityController == null) _abilityController = gameObject.AddComponent<AbilityController>();

            _abilityController.Init(this);

            if (_initialSpell != null) SetSpell(_initialSpell);
        }

        protected override void Update()
        {
            base.Update();
            if (GameManager.State != GameState.Playing) return;

            RegenMana();
            PerformTargeting();
        }

        private void RegenMana()
        {
            _currentMana += _manaRegen * Time.deltaTime;
            _currentMana = Mathf.Min(_currentMana, _maxMana);
        }

        private void PerformTargeting()
        {
            if (_targetingStrategy == null) return;

            var target = _targetingStrategy.FindTarget(transform.position, _range, GameManager.Threats);
            if (target != null)
            {
                _currentTarget = target.transform;
                _targetPos = _currentTarget.position;
                // Assuming simple targeting for now
                CastSpell(0);
            }
        }

        public void SetSpell(SpellData spell)
        {
            _equippedSpell = spell;
            _abilityController.RebuildAbilities();
        }

        public override SpellData GetSpell(int index)
        {
            if (index == 0) return _equippedSpell;
            return null;
        }

        public override bool TrySpendMana(int amount)
        {
            if (_currentMana >= amount)
            {
                _currentMana -= amount;
                return true;
            }
            return false;
        }
    }
}
