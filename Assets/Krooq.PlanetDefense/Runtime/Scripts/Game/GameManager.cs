using UnityEngine;
using System.Collections.Generic;
using Krooq.Core;
using Krooq.Common;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Linq;

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

        [SerializeField, ReadOnly] private int _resources;
        [SerializeField, ReadOnly] private int _wave = 1;
        [SerializeField, ReadOnly] private GameState _currentState;
        [SerializeField, ReadOnly] private int _baseHealth;
        [SerializeField, ReadOnly] private List<Threat> _threats = new();
        [SerializeField, ReadOnly] private List<Modifier> _modifiers = new();
        [SerializeField, ReadOnly] private ProjectileWeaponData _selectedWeapon;

        public GameData Data => _gameData;
        public int Resources => _resources;
        public GameState State => _currentState;

        public IEnumerable<Modifier> ActiveModifiers => _modifiers.Where(m => m != null);
        public ProjectileWeaponData SelectedWeapon => _selectedWeapon;


        protected MultiGameObjectPool Pool => this.GetSingleton<MultiGameObjectPool>();
        protected WaveManager WaveManager => this.GetSingleton<WaveManager>();

        public int ThreatCount => _threats.Count;
        public bool HasThreats => _threats.Count > 0;

        public void Register(Threat threat)
        {
            if (!_threats.Contains(threat)) _threats.Add(threat);
        }

        public void Unregister(Threat threat)
        {
            if (_threats.Contains(threat)) _threats.Remove(threat);
        }

        protected void Awake()
        {
            if (_gameData != null)
            {
                _resources = _gameData.StartingResources;
                _baseHealth = _gameData.BaseHealth;
                _selectedWeapon = _gameData.DefaultWeapon;
                _modifiers = new List<Modifier>(new Modifier[_gameData.MaxSlots]);
            }
        }

        protected void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            _resources = _gameData.StartingResources;
            _wave = 1;
            _baseHealth = _gameData.BaseHealth;

            for (int i = 0; i < _modifiers.Count; i++)
            {
                if (i < _gameData.StartingModifiers.Count)
                {
                    _modifiers[i] = _gameData.StartingModifiers[i];
                }
                else
                {
                    _modifiers[i] = null;
                }
            }

            // Add some default modifiers for testing if needed, or let player buy them

            StartWave();
        }

        public void StartWave()
        {
            _currentState = GameState.Playing;
            WaveManager.StartWave(_wave);
        }

        public void EndWave()
        {
            _currentState = GameState.Shop;
            // Show shop UI
        }

        public void NextWave()
        {
            _wave++;
            StartWave();
        }

        public void AddResources(int amount)
        {
            _resources += amount;
        }

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

        public void SelectWeapon(ProjectileWeaponData weapon)
        {
            _selectedWeapon = weapon;
        }

        public void SetModifier(int index, Modifier modifier)
        {
            if (index >= 0 && index < _modifiers.Count)
            {
                var oldModifier = _modifiers[index];
                if (oldModifier != null)
                {
                    AddResources(oldModifier.Cost / 2);
                }

                _modifiers[index] = modifier;
            }
        }

        public Projectile SpawnProjectile()
        {
            if (_selectedWeapon == null || _selectedWeapon.ProjectilePrefab == null) return null;
            return Pool.Get(_selectedWeapon.ProjectilePrefab);
        }

        public Threat SpawnThreat() => Pool.Get(_gameData.ThreatPrefab);

        public ModifierTileUI SpawnModifierTileUI(ModifierTileUI prefab) => Pool.Get(prefab);

        public ModifierSlotUI SpawnModifierSlotUI(ModifierSlotUI prefab) => Pool.Get(prefab);

        public void Despawn(GameObject obj) => Pool.Release(obj);
        public void Despawn(Component obj) => Pool.Release(obj.gameObject);
    }
}
