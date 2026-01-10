using UnityEngine;
using System.Collections.Generic;
using Krooq.Core;
using Krooq.Common;
using Sirenix.OdinInspector;
using System.Linq;

namespace Krooq.PlanetDefense
{
    public class Caster : MonoBehaviour, ICaster, IAbilitySource
    {
        [SerializeField] protected Transform _firePoint;
        [SerializeField] protected Transform _pivot;

        [Header("Settings")]
        [SerializeField, ReadOnly, PropertyOrder(100)] protected int _maxMana = 50;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected float _manaRegen = 5f;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected TargetingStrategy _targetingStrategy;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected float _range = 10f;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected List<AbilityData> _abilities = new();

        [Header("State")]
        [SerializeField, ReadOnly, PropertyOrder(100)] protected float _currentMana;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected SpellData _spell;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected GameObject _model;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected float _spellCooldown;

        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected AudioManager AudioManager => this.GetSingleton<AudioManager>();
        protected GameEventManager GameEventManager => this.GetSingleton<GameEventManager>();

        protected AbilityController AbilityController => this.GetCachedComponent<AbilityController>();

        public IReadOnlyList<AbilityData> Abilities => _abilities;
        public Transform FirePoint => _firePoint;
        public int CurrentMana => (int)_currentMana;
        public int MaxMana => _maxMana;
        public SpellData Spell => _spell;
        public int SpellSlots => 1;

        public virtual IEnumerable<IAbilitySource> AbilitySources
        {
            get
            {
                yield return this;
                if (_spell != null) yield return _spell;
            }
        }


        protected ITargetingInfo _targetingInfo;
        public virtual ITargetingInfo TargetingInfo => _targetingInfo;

        public virtual void Init(CasterData data)
        {
            _maxMana = data.MaxMana;
            _manaRegen = data.ManaRegen;
            _range = data.Range;
            _abilities = new List<AbilityData>(data.Abilities);
            _targetingStrategy = new TargetingStrategy(data.TargetingStrategyType);
            _targetingInfo = new CasterTargetingInfo();
            _spell = null;

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
            PerformTargeting();
            Aim();
            UpdateCooldowns();
            CastSpell();
        }

        protected virtual void Aim()
        {
            if (_pivot == null) return;

            if (TargetingInfo is not { IsValid: true }) return;

            var dir = (TargetingInfo.TargetPosition - _pivot.position).normalized;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            _pivot.rotation = Quaternion.Euler(0, 0, angle);
        }

        protected virtual void RegenMana()
        {
            _currentMana += _manaRegen * Time.deltaTime;
            _currentMana = Mathf.Min(_currentMana, _maxMana);
        }

        protected virtual void PerformTargeting()
        {
            if (_targetingStrategy == null) return;

            var target = _targetingStrategy.FindTarget(_firePoint.position, _range, GameManager.Threats);
            _targetingInfo = target == null ? default : new CasterTargetingInfo()
            {
                Owner = this,
                IsGroundTarget = target.Data.MovementType == ThreatMovementType.Ground,
                TargetPosition = target.transform.position,
                TargetThreat = target,
                IsValid = true
            };
        }

        protected virtual void UpdateCooldowns()
        {
            if (_spellCooldown > 0)
            {
                _spellCooldown -= Time.deltaTime;
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

        public virtual SpellData GetSpell(int index = 0)
        {
            return _spell;
        }

        public virtual void SetSpell(SpellData spell, int i = 0)
        {
            _spell = spell;
            AbilityController.RebuildAbilities();
        }

        public virtual void CastSpell(int index) => CastSpell();

        public virtual void CastSpell()
        {
            if (!_targetingInfo.IsValid) return;
            if (_spell == null) return;

            // Check Cooldown
            if (_spellCooldown > 0) return;

            ProcessSpell(_spell);
        }

        protected virtual void ProcessSpell(SpellData spell)
        {
            var cost = Mathf.CeilToInt(spell.ManaCost);
            if (!TrySpendMana(cost))
            {
                // TODO: Feedback
                return;
            }

            // Set Cooldown
            _spellCooldown = spell.Cooldown;

            GameEventManager.FireEvent(this, new SpellCastEvent(spell, this));
        }
    }

    public struct CasterTargetingInfo : ITargetingInfo
    {
        public Caster Owner { get; set; }
        public bool IsGroundTarget { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Threat TargetThreat { get; set; }
        public bool IsValid { get; set; }
    }
}
