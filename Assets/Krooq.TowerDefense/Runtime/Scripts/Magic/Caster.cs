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
        [SerializeField, ReadOnly, PropertyOrder(100)] protected List<RelicData> _relics = new();
        [SerializeField, ReadOnly, PropertyOrder(100)] protected List<AbilityData> _abilities = new();

        [Header("State")]
        [SerializeField, ReadOnly, PropertyOrder(100)] protected List<Spell> _spells = new();
        [SerializeField, ReadOnly, PropertyOrder(100)] protected GameObject _model;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected int _spellIndex;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected bool _isCasting;
        [SerializeField, ReadOnly, PropertyOrder(100)] protected float _castTimer;

        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected AudioManager AudioManager => this.GetSingleton<AudioManager>();
        protected GameEventManager GameEventManager => this.GetSingleton<GameEventManager>();
        protected AbilityController AbilityController => this.GetCachedComponent<AbilityController>();

        public IReadOnlyList<AbilityData> Abilities => _abilities;
        public Transform FirePoint => _firePoint;
        public IReadOnlyList<Spell> Spells => _spells;

        public virtual IEnumerable<IAbilitySource> AbilitySources
        {
            get
            {
                yield return this;
                foreach (var spell in _spells)
                    if (spell != null) yield return spell;
            }
        }

        public virtual void Init(CasterData data)
        {
            _spells.Clear();
            if (data.Spells != null)
                foreach (var spellData in data.Spells)
                    if (spellData != null)
                        _spells.Add(new Spell(spellData));

            _abilities = new List<AbilityData>(data.Abilities);

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

            _spellIndex = 0;
            _isCasting = false;
            _castTimer = 0;

            // Rebuild abilities as data changed
            AbilityController.RebuildAbilities();
        }

        protected virtual void Update()
        {
            if (GameManager.State != GameState.Playing) return;

            if (_spells.Count == 0) return;

            foreach (var spell in _spells) spell?.Update(Time.deltaTime);

            if (!_isCasting) TryStartCastingNextSpell();
            else
            {
                _castTimer -= Time.deltaTime;
                if (_castTimer <= 0) _isCasting = false;
            }
        }

        protected virtual void Aim(ITarget target)
        {
            if (_pivot == null) return;

            if (target is not { IsValid: true }) return;

            var dir = (target.Position - _pivot.position).normalized;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            _pivot.rotation = Quaternion.Euler(0, 0, angle);
        }

        protected virtual void TryStartCastingNextSpell()
        {
            if (_isCasting) return;
            if (_spells.Count == 0) return;

            var spell = _spellIndex < _spells.Count && _spellIndex >= 0 ? _spells[_spellIndex] : null;
            // If we can't cast this spell, move to the next one, it's that simple.
            if (spell == null || !spell.CanCast(_firePoint.position, GameManager.Threats))
            {
                _spellIndex = (_spellIndex + 1) % _spells.Count;
            }
            else
            {
                Aim(spell.Target);
                spell.Cast();
                _isCasting = true;
                _castTimer = spell.Data.CastDuration;
                GameEventManager.FireEvent(this, new SpellCastEvent(spell, this));
            }
        }

        public virtual void AddSpell(SpellData spellData)
        {
            if (spellData != null)
            {
                _spells.Add(new Spell(spellData));
                AbilityController.RebuildAbilities();
            }
        }

        public virtual void RemoveSpell(SpellData spellData)
        {
            if (spellData != null)
            {
                _spells.RemoveAll(s => s.Data == spellData);
                AbilityController.RebuildAbilities();
                if (_spellIndex >= _spells.Count)
                {
                    _spellIndex = 0;
                    _isCasting = false; // Interrupt if current removed
                }
            }
        }
    }
}
