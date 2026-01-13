using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
using Krooq.Common;

namespace Krooq.TowerDefense
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Tower Defense/GameData")]
    public class GameData : ScriptableObject
    {
        [Header("Player")]
        [SerializeField] private int _startingResources = 0;
        [SerializeField] private int _baseHealth = 10;
        [SerializeField] private int _baseMana = 100;
        [SerializeField] private CasterData _playerCasterData;
        public int StartingResources => _startingResources;
        public int BaseHealth => _baseHealth;
        public int BaseMana => _baseMana;
        public CasterData PlayerCasterData => _playerCasterData;


        [Header("Audio")]
        [SerializeField] private AudioSource _audioSourcePrefab;
        public AudioSource AudioSourcePrefab => _audioSourcePrefab;


        [Header("Projectiles")]
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private StatData _damageStat;
        [SerializeField] private StatData _speedStat;
        [SerializeField] private StatData _fireRateStat;
        [SerializeField] private StatData _sizeStat;
        [SerializeField] private StatData _pierceStat;
        [SerializeField] private StatData _lifetimeStat;
        [SerializeField] private StatData _explosionRadiusStat;
        [SerializeField] private StatData _explosionDamageMultStat;
        [SerializeField] private StatData _splitCountStat;
        public Projectile ProjectilePrefab => _projectilePrefab;
        public StatData DamageStat => _damageStat;
        public StatData SpeedStat => _speedStat;
        public StatData FireRateStat => _fireRateStat;
        public StatData SizeStat => _sizeStat;
        public StatData PierceStat => _pierceStat;
        public StatData LifetimeStat => _lifetimeStat;
        public StatData ExplosionRadiusStat => _explosionRadiusStat;
        public StatData ExplosionDamageMultStat => _explosionDamageMultStat;
        public StatData SplitCountStat => _splitCountStat;

        [Header("Threats")]
        [SerializeField] private Threat _threatPrefab;
        [SerializeField] private List<ThreatData> _threats = new();
        [SerializeField] private float _horizonLevel = 1f;
        [SerializeField] private float _groundLevel = 0f;
        [SerializeField] private AnimationCurve _waveDifficultyCurve = AnimationCurve.Linear(1, 10, 20, 100);
        [Tooltip("Time between groups of enemies spawning")]
        [SerializeField] private float _baseSpawnRate = 2f;
        [Tooltip("How much shorter the wait between groups becomes each wave")]
        [SerializeField] private float _spawnRateDecreasePerWave = 0.1f;
        [SerializeField] private float _minSpawnRate = 0.2f;
        public Threat ThreatPrefab => _threatPrefab;
        public List<ThreatData> Threats => _threats;
        public float HorizonLevel => _horizonLevel;
        public float GroundLevel => _groundLevel;
        public AnimationCurve WaveDifficultyCurve => _waveDifficultyCurve;
        public float BaseSpawnRate => _baseSpawnRate;
        public float SpawnRateDecreasePerWave => _spawnRateDecreasePerWave;
        public float MinSpawnRate => _minSpawnRate;


        [Header("Spells")]
        [SerializeField] private List<SpellData> _spells;
        [SerializeField] private SpellTileUI _spellTilePrefab;
        [SerializeField] private SpellSlotUI _spellSlotPrefab;
        public List<SpellData> Spells => _spells;
        public SpellTileUI SpellTilePrefab => _spellTilePrefab;
        public SpellSlotUI SpellSlotPrefab => _spellSlotPrefab;


        [Header("Relics")]
        [SerializeField] private List<RelicData> _relics = new();
        [SerializeField] private int _maxRelicSlots = 3;
        [SerializeField] private RelicTileUI _relicTilePrefab;
        [SerializeField] private RelicSlotUI _relicSlotPrefab;
        [SerializeField] private List<RelicData> _startingRelics = new();
        public List<RelicData> Relics => _relics;
        public int MaxRelicSlots => _maxRelicSlots;
        public RelicTileUI RelicTilePrefab => _relicTilePrefab;
        public RelicSlotUI RelicSlotPrefab => _relicSlotPrefab;
        public List<RelicData> StartingRelics => _startingRelics;


        [Header("Casters")]
        [SerializeField] private Caster _casterPrefab;
        [SerializeField] private List<CasterData> _casters;
        [SerializeField] private int _maxCasterSlots = 3;
        [SerializeField] private CasterTileUI _casterTilePrefab;
        [SerializeField] private CasterSlotUI _casterSlotPrefab;
        [SerializeField] private List<CasterData> _startingCasters;
        public Caster CasterPrefab => _casterPrefab;
        public List<CasterData> Casters => _casters;
        public int MaxCasterSlots => _maxCasterSlots;
        public CasterTileUI CasterTilePrefab => _casterTilePrefab;
        public CasterSlotUI CasterSlotPrefab => _casterSlotPrefab;
        public List<CasterData> StartingCasters => _startingCasters;

        [Header("Tower Interactions")]
        [SerializeField] private TowerEnhancementUI _towerEnhancementUIPrefab;
        public TowerEnhancementUI TowerEnhancementUIPrefab => _towerEnhancementUIPrefab;
    }
}

