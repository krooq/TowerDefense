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
        [SerializeField] private float _manaCost;
        [SerializeField] private float _cooldown;
        [SerializeField] private float _range = 10f;
        [SerializeField] private List<string> _tags = new();

        [SerializeField] private List<AbilityData> _abilities = new();

        public string SpellName => _spellName;
        public string Name => _spellName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public int ShopCost => _shopCost;
        public float ManaCost => _manaCost;
        public float Cooldown => _cooldown;
        public float Range => _range;
        public IReadOnlyList<string> Tags => _tags;
        public IReadOnlyList<AbilityData> Abilities => _abilities;
    }
}
