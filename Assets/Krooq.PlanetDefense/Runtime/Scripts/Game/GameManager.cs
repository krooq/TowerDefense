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

        [Header("State")]
        [SerializeField, ReadOnly] private int _wave = 1;
        [SerializeField, ReadOnly] private GameState _currentState;
        [SerializeField, ReadOnly] private List<Threat> _threats = new();

        public GameData Data => _gameData;
        public int Resources => Player.Resources;
        public GameState State => _currentState;
        public int Wave => _wave;

        public IReadOnlyList<Threat> Threats => _threats;
        public ProjectileData SelectedWeapon => Player.SelectedWeapon;

        protected MultiGameObjectPool Pool => this.GetSingleton<MultiGameObjectPool>();
        protected WaveManager WaveManager => this.GetSingleton<WaveManager>();
        protected Player Player => this.GetSingleton<Player>();
        protected GameEventManager GameEventManager => this.GetSingleton<GameEventManager>();

        public int ThreatCount => _threats.Count;
        public bool HasThreats => _threats.Count > 0;

        private void Start()
        {
            StartGame();
        }

        private void Update()
        {
            // Mana regen handled by Player
        }

        public void Register(Threat threat)
        {
            if (!_threats.Contains(threat)) _threats.Add(threat);
        }

        public void Unregister(Threat threat)
        {
            if (_threats.Contains(threat)) _threats.Remove(threat);
        }

        public void StartGame()
        {
            _wave = 1;

            for (int i = _threats.Count - 1; i >= 0; i--)
            {
                if (_threats[i] != null) Despawn(_threats[i].gameObject);
            }
            _threats.Clear();

            if (Player) Player.ResetPlayer();
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

        public void GameOver()
        {
            _currentState = GameState.GameOver;
            Debug.Log("Game Over");
            GameEventManager.FireEvent(this, new GameEndedEvent());
        }

        public T Spawn<T>(T prefab) where T : Object => Pool.Get(prefab);
        public void Despawn(Object obj) => Pool.Release(obj);
    }
}