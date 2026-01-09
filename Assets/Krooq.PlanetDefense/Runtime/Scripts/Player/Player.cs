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
        [SerializeField, ReadOnly] private float _currentMana;
        [SerializeField, ReadOnly] private int _maxMana;
        [SerializeField, ReadOnly] private int _resources;
        [SerializeField, ReadOnly] private SpellData[] _spells = new SpellData[4];
        [SerializeField, ReadOnly] private RelicData[] _relics;
        [SerializeField, ReadOnly] private CasterData[] _casters;
        [SerializeField, ReadOnly] private ProjectileData _selectedWeapon;
        [SerializeField, ReadOnly] private Transform[] _casterSlots;

        private Caster[] _spawnedCasters;

        public PlayerInputs Inputs => this.GetCachedComponent<PlayerInputs>();
        protected GameManager GameManager => this.GetSingleton<GameManager>();

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public int CurrentMana => (int)_currentMana;
        public int MaxMana => _maxMana;
        public int Resources => _resources;
        public IReadOnlyList<SpellData> Spells => _spells;
        public IReadOnlyList<RelicData> Relics => _relics;
        public IReadOnlyList<CasterData> Casters => _casters;
        public ProjectileData SelectedWeapon => _selectedWeapon;

        public PlayerCaster PlayerCaster => this.GetCachedComponent<PlayerCaster>();
        public AbilityController AbilityController => this.GetCachedComponent<AbilityController>();

        private void Start()
        {
            Init();
        }

        protected void Init()
        {
            ResetPlayer();
            AbilityController.Init(PlayerCaster);
        }


        public void SetSpell(int index, SpellData spell)
        {
            if (index >= 0 && index < _spells.Length)
            {
                _spells[index] = spell;
                AbilityController.RebuildAbilities();
            }
        }

        public void SetRelic(int index, RelicData relic)
        {
            if (index >= 0 && index < _relics.Length)
            {
                _relics[index] = relic;
                AbilityController.RebuildAbilities();
            }
        }

        public void SetCaster(int index, CasterData tower)
        {
            if (index >= 0 && index < _casters.Length)
            {
                // Clean up existing instance
                if (_spawnedCasters != null && index < _spawnedCasters.Length && _spawnedCasters[index] != null)
                {
                    GameManager.Despawn(_spawnedCasters[index]);
                    _spawnedCasters[index] = null;
                }

                _casters[index] = tower;

                // Spawn new instance
                if (tower != null)
                {
                    SpawnCaster(index, tower);
                }
            }
        }

        private void SpawnCaster(int index, CasterData data)
        {
            if (_spawnedCasters == null) _spawnedCasters = new Caster[_casters.Length];

            // Check if we have a valid slot transform
            if (_casterSlots == null || index < 0 || index >= _casterSlots.Length || _casterSlots[index] == null)
            {
                Debug.LogWarning($"No caster slot defined for index {index}");
                return;
            }

            var instance = GameManager.Spawn(data.Prefab);
            instance.transform.SetPositionAndRotation(_casterSlots[index].position, _casterSlots[index].rotation);
            instance.transform.SetParent(_casterSlots[index], true);
            instance.Init(data);

            if (index < _spawnedCasters.Length)
            {
                _spawnedCasters[index] = instance;
            }
        }

        public void ResetPlayer()
        {
            if (GameManager == null) return;
            if (GameManager.Data == null) return;

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

            _casters = new CasterData[GameManager.Data.MaxCasterSlots];

            // Clean up old towers if resetting
            if (_spawnedCasters != null)
            {
                foreach (var tower in _spawnedCasters)
                {
                    if (tower != null) GameManager.Despawn(tower);
                }
            }
            _spawnedCasters = new Caster[_casters.Length];

            var startingTowers = GameManager.Data.StartingTowers;
            if (startingTowers != null)
            {
                for (int i = 0; i < startingTowers.Count && i < _casters.Length; i++)
                {
                    SetCaster(i, startingTowers[i]);
                }
            }

            _currentMana = _maxMana;

            this.GetSingleton<SpellBarUI>().Refresh();
            this.GetSingleton<RelicBarUI>().Refresh();
            this.GetSingleton<CasterBarUI>().Refresh();

            AbilityController.RebuildAbilities();
        }

        private void Update()
        {
            if (GameManager.State == GameState.Playing)
            {
                _currentMana += GameManager.Data.BaseManaRegen * Time.deltaTime;
                if (_currentMana > _maxMana) _currentMana = _maxMana;
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