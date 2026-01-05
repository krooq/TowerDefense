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
        [SerializeField] private List<Modifier> _startingModifiers;

        [Header("Weapons")]
        [SerializeField] private List<ProjectileWeaponData> _availableWeapons;
        [SerializeField] private ProjectileWeaponData _defaultWeapon;

        [Header("Projectile Stats")]
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
        [SerializeField] private float _threatSpawnHeight = 10f;
        [SerializeField] private float _threatSpawnWidth = 16f;
        [SerializeField] private float _threatBaseSpeed = 2f;
        [SerializeField] private int _threatBaseHealth = 10;
        [SerializeField] private int _resourcesPerThreat = 1;

        [Header("Wave Configuration")]
        [SerializeField] private int _baseWaveSize = 5;
        [SerializeField] private int _threatsPerWave = 5;
        [SerializeField] private float _baseSpawnRate = 2f;
        [SerializeField] private float _spawnRateDecreasePerWave = 0.1f;
        [SerializeField] private float _minSpawnRate = 0.2f;

        [Header("Shop")]
        [SerializeField] private List<Modifier> _availableModifiers;
        [SerializeField] private int _maxSlots = 5;
        [SerializeField] private ModifierTileUI _modifierTilePrefab;
        [SerializeField] private ModifierSlotUI _modifierSlotPrefab;

        [Header("Audio")]
        [SerializeField] private AudioSource _audioSourcePrefab;

        public int StartingResources => _startingResources;
        public int BaseHealth => _baseHealth;
        public IReadOnlyList<Modifier> StartingModifiers => _startingModifiers;
        public IReadOnlyList<ProjectileWeaponData> AvailableWeapons => _availableWeapons;
        public ProjectileWeaponData DefaultWeapon => _defaultWeapon;

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

        public Threat ThreatPrefab => _threatPrefab;
        public float ThreatSpawnHeight => _threatSpawnHeight;
        public float ThreatSpawnWidth => _threatSpawnWidth;
        public float ThreatBaseSpeed => _threatBaseSpeed;
        public int ThreatBaseHealth => _threatBaseHealth;
        public int ResourcesPerThreat => _resourcesPerThreat;

        public int BaseWaveSize => _baseWaveSize;
        public int ThreatsPerWave => _threatsPerWave;
        public float BaseSpawnRate => _baseSpawnRate;
        public float SpawnRateDecreasePerWave => _spawnRateDecreasePerWave;
        public float MinSpawnRate => _minSpawnRate;

        public List<Modifier> AvailableModifiers => _availableModifiers;
        public int MaxSlots => _maxSlots;
        public ModifierTileUI ModifierTilePrefab => _modifierTilePrefab;
        public ModifierSlotUI ModifierSlotPrefab => _modifierSlotPrefab;
        public AudioSource AudioSourcePrefab => _audioSourcePrefab;
    }
}
