using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.TowerDefense
{
    public class SpellSlotUI : BaseSlotUI
    {
        protected PlayerCaster PlayerCaster => this.GetSingleton<PlayerCaster>();

        public override void OnDrop(PointerEventData eventData)
        {
            if (ShopUI) ShopUI.SetDirty();
            if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent<SpellTileUI>(out var tileUI))
            {
                if (tileUI.IsShopItem)
                {
                    if (Player.SpendResources(tileUI.Spell.ShopCost))
                    {
                        PlayerCaster.SetSpell(tileUI.Spell, _slotIndex);
                    }
                }
            }
        }
    }
}
