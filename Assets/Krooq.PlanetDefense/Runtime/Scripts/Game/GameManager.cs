using UnityEngine;
using System.Collections.Generic;
using Krooq.Core;
using Krooq.Common;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public enum GameState
    {
        Menu,
        Playing,
        Shop,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameData _gameData;

        [SerializeField, ReadOnly] private int _currentResources;
        [SerializeField, ReadOnly] private int _currentWave = 1;
        [SerializeField, ReadOnly] private GameState _currentState;
        [SerializeField, ReadOnly] private int _baseHealth;
        [SerializeField, ReadOnly] private HashSet<Meteor> _activeMeteors = new();
        [SerializeField, ReadOnly] private List<UpgradeTile> _activeUpgrades = new();

        public GameData Data => _gameData;
        public int Resources => _currentResources;
        public GameState State => _currentState;

        public List<UpgradeTile> ActiveUpgrades => _activeUpgrades;


        protected MultiGameObjectPool Pool => this.GetSingleton<MultiGameObjectPool>();
        protected WaveManager WaveManager => this.GetSingleton<WaveManager>();

        public int MeteorCount => _activeMeteors.Count;
        public bool HasMeteors => _activeMeteors.Count > 0;

        public void RegisterMeteor(Meteor meteor)
        {
            if (!_activeMeteors.Contains(meteor)) _activeMeteors.Add(meteor);
        }

        public void UnregisterMeteor(Meteor meteor)
        {
            if (_activeMeteors.Contains(meteor)) _activeMeteors.Remove(meteor);
        }

        protected void Awake()
        {
            if (_gameData != null)
            {
                _currentResources = _gameData.StartingResources;
                _baseHealth = _gameData.BaseHealth;
            }
        }

        protected void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            _currentResources = _gameData.StartingResources;
            _currentWave = 1;
            _baseHealth = _gameData.BaseHealth;
            ActiveUpgrades.Clear();

            // Add some default upgrades for testing if needed, or let player buy them

            StartWave();
        }

        public void StartWave()
        {
            _currentState = GameState.Playing;
            WaveManager.StartWave(_currentWave);
        }

        public void EndWave()
        {
            _currentState = GameState.Shop;
            // Show shop UI
        }

        public void NextWave()
        {
            _currentWave++;
            StartWave();
        }

        public void AddResources(int amount)
        {
            _currentResources += amount;
        }

        public bool SpendResources(int amount)
        {
            if (_currentResources >= amount)
            {
                _currentResources -= amount;
                return true;
            }
            return false;
        }

        public void TakeDamage(int amount)
        {
            _baseHealth -= amount;
            if (_baseHealth <= 0)
            {
                GameOver();
            }
        }

        protected void GameOver()
        {
            _currentState = GameState.GameOver;
            Debug.Log("Game Over");
        }

        public Projectile SpawnProjectile() => Pool.Get(_gameData.ProjectilePrefab);

        public Meteor SpawnMeteor() => Pool.Get(_gameData.MeteorPrefab);

        public UpgradeTileUI SpawnUpgradeTileUI(UpgradeTileUI prefab) => Pool.Get(prefab);

        public UpgradeSlotUI SpawnUpgradeSlotUI(UpgradeSlotUI prefab) => Pool.Get(prefab);

        public void Despawn(GameObject obj) => Pool.Release(obj);
        public void Despawn(Component obj) => Pool.Release(obj.gameObject);
    }
}
