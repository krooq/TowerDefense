using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.TowerDefense
{
    public class CasterSlotUI : BaseSlotUI
    {
        public override void OnDrop(PointerEventData eventData)
        {
            if (ShopUI) ShopUI.SetDirty();
            if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent<CasterTileUI>(out var tileUI))
            {
                if (tileUI.IsShopItem)
                {
                    if (Player.SpendResources(tileUI.Caster.ShopCost))
                    {
                        Player.SetCaster(_slotIndex, tileUI.Caster);

                        var casterBarUI = this.GetSingleton<CasterBarUI>();
                        // if (casterBarUI) casterBarUI.Refresh();
                    }
                }
            }
        }
    }
}
