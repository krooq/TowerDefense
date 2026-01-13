using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Krooq.TowerDefense
{
    [CreateAssetMenu(fileName = "Relic", menuName = "Tower Defense/Relic")]
    public class RelicData : ScriptableObject, IShopItem, IAbilitySource
    {
        [SerializeField] private string _relicName;
        [SerializeField, TextArea] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _shopCost;
        [SerializeField] private List<string> _tags = new();

        [SerializeField] private List<AbilityData> _abilities = new();

        public string Name => _relicName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public int ShopCost => _shopCost;
        public IReadOnlyList<string> Tags => _tags;
        public IReadOnlyList<AbilityData> Abilities => _abilities;
    }
}
