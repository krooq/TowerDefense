using UnityEngine;

namespace Krooq.PlanetDefense
{
    [CreateAssetMenu(fileName = "SpellCasterData", menuName = "PlanetDefense/SpellCasterData")]
    public class SpellCasterData : ScriptableObject, IShopItem
    {
        [SerializeField] private string _name;
        [SerializeField, TextArea] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _shopCost;
        [SerializeField] private NonPlayerSpellCaster _prefab;
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private GameObject _slotPrefab;

        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;
        public int ShopCost => _shopCost;
        public NonPlayerSpellCaster Prefab => _prefab;
        public GameObject TilePrefab => _tilePrefab;
        public GameObject SlotPrefab => _slotPrefab;
    }
}
