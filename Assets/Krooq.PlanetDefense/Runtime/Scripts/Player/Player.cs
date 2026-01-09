using Krooq.Core;
using Krooq.Common;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerTargetingReticle _targetingReticle;

        [SerializeField, ReadOnly] private int _currentHealth;
        [SerializeField, ReadOnly] private int _maxHealth;
        [SerializeField, ReadOnly] private int _resources;
        [SerializeField, ReadOnly] private RelicData[] _relics;
        [SerializeField, ReadOnly] private CasterData[] _casters;
        [SerializeField, ReadOnly] private ProjectileData _selectedWeapon;
        [SerializeField, ReadOnly] private Transform[] _casterSlots;

        private Caster[] _spawnedCasters;

        protected GameManager GameManager => this.GetSingleton<GameManager>();

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public int Resources => _resources;
        public IReadOnlyList<RelicData> Relics => _relics;
        public IReadOnlyList<CasterData> Casters => _casters;
        public ProjectileData SelectedWeapon => _selectedWeapon;

        public PlayerInputs Inputs => this.GetCachedComponent<PlayerInputs>();
        public PlayerCaster PlayerCaster => this.GetCachedComponent<PlayerCaster>();
        public AbilityController AbilityController => this.GetCachedComponent<AbilityController>();
        public PlayerTargetingReticle TargetingReticle => _targetingReticle;

        protected void Init()
        {
            ResetPlayer();
            AbilityController.Init(PlayerCaster);
            PlayerCaster.Init(GameManager.Data.PlayerCasterData);
        }

        private void Start()
        {
            Init();
        }

        protected void Update()
        {
            if (GameManager.State != GameState.Playing) return;
            HandleAiming();
        }

        protected void HandleAiming()
        {
            if (_targetingReticle == null) return;
            PlayerCaster.Aim(_targetingReticle.TargetPosition);
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

            var instance = GameManager.Spawn(GameManager.Data.CasterPrefab);
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

            _resources = GameManager.Data.StartingResources;
            _currentHealth = _maxHealth;
            _relics = new RelicData[GameManager.Data.MaxRelicSlots];
            var startingRelics = GameManager.Data.StartingRelics;
            for (int i = 0; i < startingRelics.Count && i < _relics.Length; i++)
                _relics[i] = startingRelics[i];

            _casters = new CasterData[GameManager.Data.MaxCasterSlots];

            // Clean up old casters if resetting
            if (_spawnedCasters != null)
            {
                foreach (var caster in _spawnedCasters)
                {
                    if (caster != null) GameManager.Despawn(caster);
                }
            }
            _spawnedCasters = new Caster[_casters.Length];

            var startingCasters = GameManager.Data.StartingCasters;
            if (startingCasters != null)
            {
                for (int i = 0; i < startingCasters.Count && i < _casters.Length; i++)
                {
                    SetCaster(i, startingCasters[i]);
                }
            }
            // this.GetSingleton<RelicBarUI>().Refresh();
            // this.GetSingleton<CasterBarUI>().Refresh();
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