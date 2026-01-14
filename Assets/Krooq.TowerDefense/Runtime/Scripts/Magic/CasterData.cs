using UnityEngine;
using System.Collections.Generic;

namespace Krooq.TowerDefense
{
    [CreateAssetMenu(fileName = "Caster", menuName = "Tower Defense/Caster")]
    public class CasterData : ScriptableObject, IShopItem
    {
        [SerializeField] private string _name;
        [SerializeField, TextArea] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _shopCost;
        [SerializeField] private GameObject _modelPrefab;

        [Header("Stats")]
        [SerializeField] private List<SpellData> _spells = new();
        [SerializeField] private TargetingStrategyType _targetingStrategyType;
        [SerializeField] private List<AbilityData> _abilities = new();

        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;
        public int ShopCost => _shopCost;
        public GameObject ModelPrefab => _modelPrefab;

        public List<SpellData> Spells => _spells;
        public TargetingStrategyType TargetingStrategyType => _targetingStrategyType;
        public List<AbilityData> Abilities => _abilities;
    }
}
