using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
using Krooq.Common;

namespace Krooq.PlanetDefense
{
    [CreateAssetMenu(fileName = "GameData", menuName = "PlanetDefense/GameData")]
    public class GameData : ScriptableObject
    {
        [Header("Player Stats")]
        [SerializeField] private int _startingResources = 0;
        [SerializeField] private int _baseHealth = 10;
        [SerializeField] private int _baseMana = 100;
        [SerializeField] private int _baseManaRegen = 5;
        [SerializeField] private List<SpellData> _startingSpells;

        [Header("Weapons")]
        [SerializeField] private List<ProjectileData> _availableWeapons;
        [SerializeField] private ProjectileData _defaultWeapon;

        [Header("Projectile Stats")]
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

        [Header("Cannon")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 10f;

        [Header("Threats")]
        [SerializeField] private Threat _threatPrefab;
        [SerializeField] private List<ThreatData> _threats;
        [SerializeField] private float _threatSpawnHeight = 10f;
        [SerializeField] private float _threatSpawnWidth = 16f;
        [SerializeField] private float _groundUnitSpawnHeightMin = 3f;
        [SerializeField] private float _groundUnitSpawnHeightMax = 7f;
        [SerializeField] private float _groundLevelY = 2f;

        public Threat ThreatPrefab => _threatPrefab;
        public List<ThreatData> Threats => _threats;
        public float GroundUnitSpawnHeightMin => _groundUnitSpawnHeightMin;
        public float GroundUnitSpawnHeightMax => _groundUnitSpawnHeightMax;
        public float GroundLevelY => _groundLevelY;

        [Header("Wave Configuration")]
        [SerializeField] private int _baseWaveSize = 5;
        [SerializeField] private int _threatsPerWave = 5;
        [SerializeField] private float _baseSpawnRate = 2f;
        [SerializeField] private float _spawnRateDecreasePerWave = 0.1f;
        [SerializeField] private float _minSpawnRate = 0.2f;

        [Header("Shop")]
        [SerializeField] private List<SpellData> _availableSpells;
        [SerializeField] private int _maxSlots = 4;
        [SerializeField] private SpellTileUI _spellTilePrefab;
        [SerializeField] private SpellSlotUI _spellSlotPrefab;

        [Header("Audio")]
        [SerializeField] private AudioSource _audioSourcePrefab;


        public int StartingResources => _startingResources;
        public int BaseHealth => _baseHealth;
        public int BaseMana => _baseMana;
        public int BaseManaRegen => _baseManaRegen;
        public IReadOnlyList<SpellData> StartingSpells => _startingSpells;
        public IReadOnlyList<ProjectileData> AvailableWeapons => _availableWeapons;
        public ProjectileData DefaultWeapon => _defaultWeapon;

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

        public float MoveSpeed => _moveSpeed;
        public float RotationSpeed => _rotationSpeed;

        public float ThreatSpawnHeight => _threatSpawnHeight;
        public float ThreatSpawnWidth => _threatSpawnWidth;

        public int BaseWaveSize => _baseWaveSize;
        public int ThreatsPerWave => _threatsPerWave;
        public float BaseSpawnRate => _baseSpawnRate;
        public float SpawnRateDecreasePerWave => _spawnRateDecreasePerWave;
        public float MinSpawnRate => _minSpawnRate;

        public List<SpellData> AvailableSpells => _availableSpells;
        public int MaxSlots => _maxSlots;
        public SpellTileUI SpellTilePrefab => _spellTilePrefab;
        public SpellSlotUI SpellSlotPrefab => _spellSlotPrefab;
        public AudioSource AudioSourcePrefab => _audioSourcePrefab;

        [Header("Relics")]
        [SerializeField] private List<RelicData> _availableRelics;
        [SerializeField] private int _maxRelicSlots = 3;
        [SerializeField] private RelicTileUI _relicTilePrefab;
        [SerializeField] private RelicSlotUI _relicSlotPrefab;
        [SerializeField] private List<RelicData> _startingRelics;

        public List<RelicData> AvailableRelics => _availableRelics;
        public int MaxRelicSlots => _maxRelicSlots;
        public RelicTileUI RelicTilePrefab => _relicTilePrefab;
        public RelicSlotUI RelicSlotPrefab => _relicSlotPrefab;
        public List<RelicData> StartingRelics => _startingRelics;

        [Header("Towers")]
        [SerializeField] private List<CasterData> _availableTowers;
        [SerializeField] private int _maxTowerSlots = 3;
        [SerializeField] private CasterTileUI _towerTilePrefab;
        [SerializeField] private CasterSlotUI _towerSlotPrefab;
        [SerializeField] private List<CasterData> _startingTowers;

        public List<CasterData> AvailableTowers => _availableTowers;
        public int MaxCasterSlots => _maxTowerSlots;
        public CasterTileUI CasterTilePrefab => _towerTilePrefab;
        public CasterSlotUI CasterSlotPrefab => _towerSlotPrefab;
        public List<CasterData> StartingTowers => _startingTowers;
    }
}

