using Krooq.Core;
using Krooq.Common;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public class Player : MonoBehaviour
    {
        [SerializeField, ReadOnly] private int _currentHealth;
        [SerializeField, ReadOnly] private int _maxHealth;
        [SerializeField, ReadOnly] private int _currentMana;
        [SerializeField, ReadOnly] private int _maxMana;
        [SerializeField, ReadOnly] private int _resources;
        [SerializeField, ReadOnly] private SpellData[] _spells = new SpellData[4];
        [SerializeField, ReadOnly] private RelicData[] _relics;
        [SerializeField, ReadOnly] private ProjectileData _selectedWeapon;

        private AbilityController _abilityController;
        private float _manaRegenAccumulator;

        public PlayerInputs Inputs => this.GetCachedComponent<PlayerInputs>();
        protected GameManager GameManager => this.GetSingleton<GameManager>();

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public int CurrentMana => _currentMana;
        public int MaxMana => _maxMana;
        public int Resources => _resources;
        public IReadOnlyList<SpellData> Spells => _spells;
        public IReadOnlyList<RelicData> Relics => _relics;
        public ProjectileData SelectedWeapon => _selectedWeapon;
        public AbilityController AbilityController => _abilityController;

        private void Start()
        {
            EnsureAbilityController();
            if (GameManager != null && GameManager.Data != null)
            {
                ResetPlayer();
            }
        }

        private void EnsureAbilityController()
        {
            if (_abilityController == null)
            {
                _abilityController = GetComponent<AbilityController>();
                if (_abilityController == null) _abilityController = gameObject.AddComponent<AbilityController>();
                _abilityController.Init(GetComponent<PlayerSpellCaster>());
            }
        }

        public void SetSpell(int index, SpellData spell)
        {
            if (index >= 0 && index < _spells.Length)
            {
                _spells[index] = spell;
                _abilityController.RebuildAbilities();
            }
        }

        public void SetRelic(int index, RelicData relic)
        {
            if (index >= 0 && index < _relics.Length)
            {
                _relics[index] = relic;
                _abilityController.RebuildAbilities();
            }
        }

        public void ResetPlayer()
        {
            _maxHealth = GameManager.Data.BaseHealth;
            _maxMana = GameManager.Data.BaseMana;

            _resources = GameManager.Data.StartingResources;
            _currentHealth = _maxHealth;
            _selectedWeapon = GameManager.Data.DefaultWeapon;
            _spells = new SpellData[GameManager.Data.MaxSlots];
            var startingSpells = GameManager.Data.StartingSpells;
            for (int i = 0; i < startingSpells.Count && i < _spells.Length; i++)
            {
                _spells[i] = startingSpells[i];
            }

            _relics = new RelicData[GameManager.Data.MaxRelicSlots];
            var startingRelics = GameManager.Data.StartingRelics;
            if (startingRelics != null)
            {
                for (int i = 0; i < startingRelics.Count && i < _relics.Length; i++)
                {
                    _relics[i] = startingRelics[i];
                }
            }

            _currentMana = _maxMana;
            _manaRegenAccumulator = 0f;

            this.GetSingleton<SpellBarUI>()?.Refresh();
            this.GetSingleton<RelicBarUI>()?.Refresh();

            _abilityController.RebuildAbilities();
        }

        private void Update()
        {
            if (GameManager.State == GameState.Playing)
            {
                _manaRegenAccumulator += GameManager.Data.BaseManaRegen * Time.deltaTime;
                if (_manaRegenAccumulator >= 1f)
                {
                    int amount = Mathf.FloorToInt(_manaRegenAccumulator);
                    _currentMana = Mathf.Min(_currentMana + amount, _maxMana);
                    _manaRegenAccumulator -= amount;
                }
            }
        }

        public void AddResources(int amount) => _resources += amount;

        public bool SpendResources(int amount)
        {
            if (_resources >= amount)
            {
                _resources -= amount;
                return true;
            }
            return false;
        }

        public bool TrySpendMana(int amount)
        {
            var cost = amount;
            if (_currentMana >= cost)
            {
                _currentMana -= cost;
                return true;
            }
            return false;
        }

        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;
            if (_currentHealth <= 0)
            {
                GameManager.GameOver();
            }
        }

        public void SelectWeapon(ProjectileData weapon) => _selectedWeapon = weapon;
    }
}