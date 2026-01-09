using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.PlanetDefense
{
    public class SpellTileUI : BaseTileUI
    {
        public SpellData Spell => Item as SpellData;

        protected PlayerCaster PlayerCaster => this.GetSingleton<PlayerCaster>();

        public void Init(SpellData spell, bool isShopItem, int slotIndex = -1, UnityAction onClick = null)
        {
            base.Init(spell, isShopItem, slotIndex, onClick);
        }

        protected override void OnSell()
        {
            // Sell
            Player.AddResources(Spell.ShopCost); // 100% refund for now
            PlayerCaster.SetSpell(null, _slotIndex);

            if (ShopUI) ShopUI.SetDirty();
        }
    }
}
