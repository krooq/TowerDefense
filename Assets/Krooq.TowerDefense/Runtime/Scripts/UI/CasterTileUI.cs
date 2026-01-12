using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.TowerDefense
{
    public class CasterTileUI : BaseTileUI
    {
        public CasterData Caster => Item as CasterData;

        public void Init(CasterData caster, bool isShopItem, int slotIndex = -1, UnityAction onClick = null)
        {
            base.Init(caster, isShopItem, slotIndex, onClick);
        }

        protected override void OnSell()
        {
            // Sell
            Player.AddResources(Caster.ShopCost); // 100% refund for now
            Player.SetCaster(_slotIndex, null);

            if (ShopUI) ShopUI.SetDirty();

            var casterBarUI = this.GetSingleton<CasterBarUI>();
            // if (casterBarUI) casterBarUI.Refresh();
        }
    }
}
