using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    [CreateAssetMenu(fileName = "New Spell", menuName = "PlanetDefense/Spell")]
    public class Spell : ScriptableObject, IShopItem
    {
        [SerializeField] private string _spellName;
        [SerializeField, TextArea] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _shopCost;
        [SerializeField] private float _manaCost;
        [SerializeField] private float _cooldown;
        [SerializeField] private List<string> _tags = new();

        [SerializeField] private List<AbilityDefinition> _abilities = new();

        public string SpellName => _spellName;
        public string Name => _spellName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public int ShopCost => _shopCost;
        public float ManaCost => _manaCost;
        public float Cooldown => _cooldown;
        public IReadOnlyList<string> Tags => _tags;
        public IReadOnlyList<AbilityDefinition> Abilities => _abilities;
    }
}
