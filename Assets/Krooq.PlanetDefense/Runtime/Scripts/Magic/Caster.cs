using UnityEngine;
using System.Collections.Generic;
using Krooq.Core;
using Krooq.Common;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public class Caster : MonoBehaviour, ICaster, IAbilitySource
    {
        [SerializeField] protected Transform _firePoint;

        [Header("Settings")]
        [SerializeField, ReadOnly, PropertyOrder(100)] protected SpellData _initialSpell;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected int _maxMana = 50;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected float _manaRegen = 5f;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected TargetingData _targetingStrategy;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected float _range = 10f;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected List<AbilityData> _abilities = new();

        [Header("State")]
        [SerializeField, ReadOnly, PropertyOrder(100)] protected float _currentMana;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected SpellData _equippedSpell;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected GameObject _model;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected Transform _currentTarget;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected Vector3 _targetPos;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected bool _isGroundTarget = true;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected Dictionary<int, float> _spellCooldowns = new();

        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected AudioManager AudioManager => this.GetSingleton<AudioManager>();
        protected GameEventManager GameEventManager => this.GetSingleton<GameEventManager>();

        protected AbilityController AbilityController => this.GetCachedComponent<AbilityController>();

        public IReadOnlyList<AbilityData> Abilities => _abilities;
        public Transform FirePoint => _firePoint;
        public int CurrentMana => (int)_currentMana;
        public int MaxMana => _maxMana;

        public virtual IEnumerable<IAbilitySource> AbilitySources
        {
            get
            {
                yield return this;
                if (_equippedSpell != null) yield return _equippedSpell;
            }
        }

        protected class DefaultTargetingInfo : ITargetingInfo
        {
            private Caster _owner;
            public DefaultTargetingInfo(Caster owner) { _owner = owner; }
            public bool IsGroundTarget => _owner._isGroundTarget;
            public Vector3 TargetPosition => _owner._targetPos;
        }

        protected ITargetingInfo _targetingInfo;
        public virtual ITargetingInfo TargetingInfo => _targetingInfo;

        public virtual void Init(CasterData data)
        {
            _initialSpell = data.InitialSpell;
            _maxMana = data.MaxMana;
            _manaRegen = data.ManaRegen;
            _targetingStrategy = data.TargetingStrategy;
            _range = data.Range;
            _abilities = new List<AbilityData>(data.Abilities);
            _targetingInfo = new DefaultTargetingInfo(this);

            if (_model != null)
            {
                GameManager.Despawn(_model);
                _model = null;
            }

            if (data.ModelPrefab != null)
            {
                _model = GameManager.Spawn(data.ModelPrefab);
                _model.transform.SetParent(transform);
                _model.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }

            _currentMana = _maxMana;

            if (_initialSpell != null) SetSpell(_initialSpell);

            // Rebuild abilities as data changed
            AbilityController.RebuildAbilities();
        }

        protected virtual void Update()
        {
            if (GameManager.State != GameState.Playing) return;

            RegenMana();
            PerformTargeting();
            UpdateCooldowns();
        }

        protected virtual void RegenMana()
        {
            _currentMana += _manaRegen * Time.deltaTime;
            _currentMana = Mathf.Min(_currentMana, _maxMana);
        }

        protected virtual void PerformTargeting()
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

        protected virtual void UpdateCooldowns()
        {
            var keys = new List<int>(_spellCooldowns.Keys);
            foreach (var key in keys)
            {
                if (_spellCooldowns[key] > 0)
                {
                    _spellCooldowns[key] -= Time.deltaTime;
                }
            }
        }

        public virtual bool TrySpendMana(int amount)
        {
            if (_currentMana >= amount)
            {
                _currentMana -= amount;
                return true;
            }
            return false;
        }

        public virtual SpellData GetSpell(int index)
        {
            if (index == 0) return _equippedSpell;
            return null;
        }

        public virtual void SetSpell(SpellData spell)
        {
            _equippedSpell = spell;
            AbilityController.RebuildAbilities();
        }

        public virtual void CastSpell(int slotIndex)
        {
            var spell = GetSpell(slotIndex);
            if (spell == null) return;

            // Check Cooldown
            if (_spellCooldowns.TryGetValue(slotIndex, out var timer) && timer > 0) return;

            ProcessSpell(spell, slotIndex);
        }

        protected virtual void ProcessSpell(SpellData spell, int slotIndex)
        {
            var cost = Mathf.CeilToInt(spell.ManaCost);
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
