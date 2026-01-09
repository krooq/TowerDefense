using UnityEngine;
using UnityEngine.EventSystems;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.PlanetDefense
{
    public class RelicSlotUI : BaseSlotUI
    {
        public override void OnDrop(PointerEventData eventData)
        {
            if (ShopUI) ShopUI.SetDirty();
            if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent<RelicTileUI>(out var tileUI))
            {
                if (tileUI.IsShopItem)
                {
                    if (Player.SpendResources(tileUI.Relic.ShopCost))
                    {
                        Player.SetRelic(_slotIndex, tileUI.Relic);

                        var relicBarUI = this.GetSingleton<RelicBarUI>();
                        // if (relicBarUI) relicBarUI.Refresh();
                    }
                }
            }
        }
    }
}
