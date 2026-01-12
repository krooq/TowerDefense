using UnityEngine;

namespace Krooq.TowerDefense
{
    public interface IShopItem
    {
        string Name { get; }
        string Description { get; }
        Sprite Icon { get; }
        int ShopCost { get; }
    }
}
