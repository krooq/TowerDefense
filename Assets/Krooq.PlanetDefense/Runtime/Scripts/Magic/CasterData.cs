using UnityEngine;
using System.Collections.Generic;

namespace Krooq.PlanetDefense
{
    [CreateAssetMenu(fileName = "CasterData", menuName = "PlanetDefense/CasterData")]
    public class CasterData : ScriptableObject, IShopItem
    {
        [SerializeField] private string _name;
        [SerializeField, TextArea] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _shopCost;
        [SerializeField] private GameObject _modelPrefab;

        [Header("Stats")]
        [SerializeField] private SpellData _initialSpell;
        [SerializeField] private int _maxMana = 50;
        [SerializeField] private float _manaRegen = 5f;
        [SerializeField] private TargetingData _targetingStrategy;
        [SerializeField] private float _range = 10f;
        [SerializeField] private List<AbilityData> _abilities = new();

        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;
        public int ShopCost => _shopCost;
        public GameObject ModelPrefab => _modelPrefab;

        public SpellData InitialSpell => _initialSpell;
        public int MaxMana => _maxMana;
        public float ManaRegen => _manaRegen;
        public TargetingData TargetingStrategy => _targetingStrategy;
        public float Range => _range;
        public List<AbilityData> Abilities => _abilities;
    }
}
