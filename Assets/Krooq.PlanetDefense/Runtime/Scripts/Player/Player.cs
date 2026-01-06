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
        [SerializeField, ReadOnly] private Spell[] _spells = new Spell[4];
        [SerializeField, ReadOnly] private ProjectileWeaponData _selectedWeapon;

        private float _manaRegenAccumulator;

        public PlayerInputs Inputs => this.GetCachedComponent<PlayerInputs>();
        protected GameManager GameManager => this.GetSingleton<GameManager>();

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public int CurrentMana => _currentMana;
        public int MaxMana => _maxMana;
        public int Resources => _resources;
        public IReadOnlyList<Spell> Spells => _spells;
        public ProjectileWeaponData SelectedWeapon => _selectedWeapon;

        private void Start()
        {
            if (GameManager != null && GameManager.Data != null)
            {
                ResetPlayer();
            }
        }

        public void ResetPlayer()
        {
            _maxHealth = GameManager.Data.BaseHealth;
            _maxMana = GameManager.Data.BaseMana;

            _resources = GameManager.Data.StartingResources;
            _currentHealth = _maxHealth;
            _selectedWeapon = GameManager.Data.DefaultWeapon;
            _spells = new Spell[GameManager.Data.MaxSlots];
            var startingSpells = GameManager.Data.StartingSpells;
            for (int i = 0; i < startingSpells.Count && i < _spells.Length; i++)
            {
                _spells[i] = startingSpells[i];
            }
            _currentMana = _maxMana;
            _manaRegenAccumulator = 0f;

            this.GetSingleton<SpellBarUI>()?.Refresh();
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

        public bool TrySpendMana(float amount)
        {
            int cost = Mathf.CeilToInt(amount);
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

        public void SelectWeapon(ProjectileWeaponData weapon) => _selectedWeapon = weapon;

        public void SetSpell(int index, Spell spell)
        {
            if (index >= 0 && index < _spells.Length)
            {
                _spells[index] = spell;
            }
        }
    }
}