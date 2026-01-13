using UnityEngine;

namespace Krooq.TowerDefense
{
    public enum ThreatAttackType
    {
        Melee,
        Ranged
    }

    [CreateAssetMenu(fileName = "ThreatData", menuName = "Tower Defense/ThreatData")]
    public class ThreatData : ScriptableObject
    {
        [SerializeField] private float _speed = 2f;
        [SerializeField] private float _health = 10f;
        [SerializeField] private int _resources = 10;
        [SerializeField] private float _damage = 1f;
        [SerializeField] private float _attackRate = 1f;
        [SerializeField] private float _attackRange = 1f;
        [SerializeField] private ThreatAttackType _attackType = ThreatAttackType.Melee;
        [SerializeField] private int _powerLevel = 1;
        [SerializeField] private int _minGroupSize = 1;
        [SerializeField] private int _maxGroupSize = 1;
        [SerializeField] private float _groupSpawnInterval = 0.5f;
        [SerializeField] private int _minWave = 1;
        [SerializeField] private ThreatMovementType _movementType;
        [SerializeField] private ThreatModel _modelPrefab;

        public float Speed => _speed;
        public float Health => _health;
        public int Resources => _resources;
        public float Damage => _damage;
        public float AttackRate => _attackRate;
        public float AttackRange => _attackRange;
        public ThreatAttackType AttackType => _attackType;
        public int PowerLevel => _powerLevel;
        public int MinGroupSize => _minGroupSize;
        public int MaxGroupSize => _maxGroupSize;
        public float GroupSpawnInterval => _groupSpawnInterval;
        public int MinWave => _minWave;
        public ThreatMovementType MovementType => _movementType;
        public ThreatModel ModelPrefab => _modelPrefab;
    }
}
