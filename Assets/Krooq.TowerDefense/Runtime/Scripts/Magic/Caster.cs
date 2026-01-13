using UnityEngine;
using System.Collections.Generic;
using Krooq.Core;
using Krooq.Common;
using Sirenix.OdinInspector;
using System.Linq;

namespace Krooq.TowerDefense
{
    public class Caster : MonoBehaviour, ICaster, IAbilitySource
    {
        [SerializeField] protected Transform _firePoint;
        [SerializeField] protected Transform _pivot;

        [Header("Settings")]
        [SerializeField, ReadOnly, PropertyOrder(100)] protected int _maxMana = 50;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected float _manaRegen = 5f;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected List<RelicData> _relics = new();
        [SerializeField, ReadOnly, PropertyOrder(100)] protected List<AbilityData> _abilities = new();
        [SerializeField, ReadOnly, PropertyOrder(100)] protected TargetingStrategy _targetingStrategy;

        [Header("State")]
        [SerializeField, ReadOnly, PropertyOrder(100)] protected float _currentMana;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected SpellData _spell;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected GameObject _model;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected float _cooldownRemaining;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected ITarget _target;

        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected AudioManager AudioManager => this.GetSingleton<AudioManager>();
        protected GameEventManager GameEventManager => this.GetSingleton<GameEventManager>();
        protected AbilityController AbilityController => this.GetCachedComponent<AbilityController>();

        public IReadOnlyList<AbilityData> Abilities => _abilities;
        public Transform FirePoint => _firePoint;
        public int CurrentMana => (int)_currentMana;
        public int MaxMana => _maxMana;
        public SpellData Spell => _spell;

        public virtual IEnumerable<IAbilitySource> AbilitySources
        {
            get
            {
                yield return this;
                if (_spell != null) yield return _spell;
            }
        }
        public virtual ITarget TargetingInfo => _target;

        public virtual void Init(CasterData data)
        {
            _maxMana = data.MaxMana;
            _manaRegen = data.ManaRegen;
            _spell = data.InitialSpell;
            _abilities = new List<AbilityData>(data.Abilities);
            _targetingStrategy = new TargetingStrategy(data.TargetingStrategyType);
            _target = new ThreatTarget();

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

            if (data.InitialSpell != null) SetSpell(data.InitialSpell);

            // Rebuild abilities as data changed
            AbilityController.RebuildAbilities();
        }

        protected virtual void Update()
        {
            if (GameManager.State != GameState.Playing) return;

            RegenMana();
            UpdateCooldowns();
            FindTargets();
            Aim();
            CastSpell();
        }

        protected virtual void RegenMana() => _currentMana = Mathf.Clamp(_currentMana + _manaRegen * Time.deltaTime, 0f, _maxMana);

        protected virtual void UpdateCooldowns() => _cooldownRemaining = Mathf.Clamp(_cooldownRemaining - Time.deltaTime, 0f, _spell != null ? _spell.Cooldown : 0f);

        protected virtual void FindTargets()
        {
            if (_targetingStrategy == null) return;
            var range = _spell != null ? _spell.Range : 0f;
            var targetThreat = _targetingStrategy.FindTarget(_firePoint.position, range, GameManager.Threats);
            _target = targetThreat == null ? default : new ThreatTarget(targetThreat);
        }

        protected virtual void Aim()
        {
            if (_pivot == null) return;

            if (TargetingInfo is not { IsValid: true }) return;

            var dir = (TargetingInfo.Position - _pivot.position).normalized;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            _pivot.rotation = Quaternion.Euler(0, 0, angle);
        }


        public bool CanSpendMana(int amount) => _currentMana >= amount;

        public virtual bool TrySpendMana(int amount)
        {
            if (CanSpendMana(amount))
            {
                _currentMana -= amount;
                return true;
            }
            return false;
        }

        public virtual void SetSpell(SpellData spell)
        {
            _spell = spell;
            AbilityController.RebuildAbilities();
        }

        public virtual void CastSpell()
        {
            if (!_target.IsValid) return;
            if (_spell == null) return;
            if (_cooldownRemaining > 0) return;

            var cost = Mathf.CeilToInt(_spell.ManaCost);
            if (!TrySpendMana(cost))
            {
                // TODO: Feedback
                return;
            }
            _cooldownRemaining = _spell.Cooldown;
            GameEventManager.FireEvent(this, new SpellCastEvent(_spell, this));
        }
    }
}
