using UnityEngine;
using UnityEngine.Events;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.PlanetDefense
{
    public class RelicTileUI : BaseTileUI
    {
        public RelicData Relic => Item as RelicData;

        public void Init(RelicData relic, bool isShopItem, int slotIndex = -1, UnityAction onClick = null)
        {
            base.Init(relic, isShopItem, slotIndex, onClick);
        }

        protected override void OnSell()
        {
            Player.AddResources(Relic.ShopCost);
            Player.SetRelic(_slotIndex, null);

            if (ShopUI) ShopUI.SetDirty();
            var relicBarUI = this.GetSingleton<RelicBarUI>();
            if (relicBarUI) relicBarUI.Refresh();
        }
    }
}
