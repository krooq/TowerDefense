using UnityEngine;
using System.Collections.Generic;

namespace Krooq.PlanetDefense
{
    [CreateAssetMenu(fileName = "GameData", menuName = "PlanetDefense/GameData")]
    public class GameData : ScriptableObject
    {
        [Header("Player Stats")]
        [SerializeField] private int _startingResources = 100;
        [SerializeField] private int _baseHealth = 10;

        [Header("Cannon")]
        [SerializeField] private float _fireRate = 0.5f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _projectileLifetime = 30f;
        [SerializeField] private Projectile _projectilePrefab;

        [Header("Meteors")]
        [SerializeField] private Meteor _meteorPrefab;
        [SerializeField] private float _meteorSpawnHeight = 10f;
        [SerializeField] private float _meteorSpawnWidth = 16f;
        [SerializeField] private float _meteorBaseSpeed = 2f;
        [SerializeField] private int _meteorBaseHealth = 10;
        [SerializeField] private int _resourcesPerMeteor = 10;

        [Header("Shop")]
        [SerializeField] private List<UpgradeTile> _availableUpgrades;
        [SerializeField] private int _maxSlots = 5;
        [SerializeField] private UpgradeTileUI _upgradeTilePrefab;
        [SerializeField] private UpgradeSlotUI _upgradeSlotPrefab;

        public int StartingResources => _startingResources;
        public int BaseHealth => _baseHealth;
        public float FireRate => _fireRate;
        public float RotationSpeed => _rotationSpeed;
        public float ProjectileLifetime => _projectileLifetime;
        public Projectile ProjectilePrefab => _projectilePrefab;
        public Meteor MeteorPrefab => _meteorPrefab;
        public float MeteorSpawnHeight => _meteorSpawnHeight;
        public float MeteorSpawnWidth => _meteorSpawnWidth;
        public float MeteorBaseSpeed => _meteorBaseSpeed;
        public int MeteorBaseHealth => _meteorBaseHealth;
        public int ResourcesPerMeteor => _resourcesPerMeteor;
        public List<UpgradeTile> AvailableUpgrades => _availableUpgrades;
        public int MaxSlots => _maxSlots;
        public UpgradeTileUI UpgradeTilePrefab => _upgradeTilePrefab;
        public UpgradeSlotUI UpgradeSlotPrefab => _upgradeSlotPrefab;
    }
}
