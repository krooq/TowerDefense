using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Krooq.TowerDefense
{
    [CreateAssetMenu(fileName = "New Spell", menuName = "Tower Defense/Spell")]
    public class SpellData : ScriptableObject, IShopItem, IAbilitySource
    {
        [SerializeField] private string _spellName;
        [SerializeField, TextArea] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _shopCost;
        [SerializeField] private float _cooldown = 1f;
        [SerializeField] private float _castDuration = 1f;
        [SerializeField] private float _range = 10f;
        [SerializeField] private TargetingStrategyType _targetingStrategyType;
        [SerializeField] private List<string> _tags = new();

        [SerializeField] private List<AbilityData> _abilities = new();

        public string SpellName => _spellName;
        public string Name => _spellName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public int ShopCost => _shopCost;
        public float Cooldown => _cooldown;
        public float CastDuration => _castDuration;
        public float Range => _range;
        public TargetingStrategyType TargetingStrategyType => _targetingStrategyType;
        public IReadOnlyList<string> Tags => _tags;
        public IReadOnlyList<AbilityData> Abilities => _abilities;
    }
}
