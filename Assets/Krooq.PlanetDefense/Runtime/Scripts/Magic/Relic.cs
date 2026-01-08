using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    [CreateAssetMenu(fileName = "New Relic", menuName = "PlanetDefense/Relic")]
    public class Relic : ScriptableObject, IShopItem
    {
        [SerializeField] private string _relicName;
        [SerializeField, TextArea] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _shopCost;
        [SerializeField] private List<string> _tags = new();

        [SerializeField] private List<AbilityDefinition> _abilities = new();

        public string Name => _relicName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public int ShopCost => _shopCost;
        public IReadOnlyList<string> Tags => _tags;
        public IReadOnlyList<AbilityDefinition> Abilities => _abilities;
    }
}
