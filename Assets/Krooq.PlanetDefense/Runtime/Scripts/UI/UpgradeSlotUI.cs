using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.PlanetDefense
{
    public class UpgradeSlotUI : MonoBehaviour, IDropHandler
    {
        [SerializeField] private int _slotIndex;
        [SerializeField] private Image _icon;

        protected ShopUI ShopUI => this.GetSingleton<ShopUI>();
        protected GameManager GameManager => this.GetSingleton<GameManager>();

        public int SlotIndex => _slotIndex;

        public void Init(int index)
        {
            _slotIndex = index;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent<UpgradeTileUI>(out var tileUI))
            {
                // Handle Drop Logic
                // If tileUI is from Shop -> Buy
                // If tileUI is from another Slot -> Swap or Move (not implemented yet, assuming Shop -> Slot for now)

                if (tileUI.IsShopItem)
                {
                    if (GameManager.SpendResources(tileUI.TileData.Cost))
                    {
                        // Add to this slot
                        // If slot is occupied, replace? Or fail?
                        // For simplicity, let's say we replace (sell old one?) or just overwrite.
                        // But ActiveUpgrades is a List. We need to ensure it has capacity or fill with nulls.

                        // Ensure list is big enough
                        while (GameManager.ActiveUpgrades.Count <= _slotIndex)
                        {
                            GameManager.ActiveUpgrades.Add(null);
                        }

                        var oldTile = GameManager.ActiveUpgrades[_slotIndex];
                        if (oldTile != null)
                        {
                            // Refund?
                            GameManager.AddResources(oldTile.Cost / 2);
                        }

                        GameManager.ActiveUpgrades[_slotIndex] = tileUI.TileData;

                        // Refresh UI handled by ShopUI Update loop
                        // But we should probably trigger an event or set dirty
                        if (ShopUI) ShopUI.SetDirty();
                        var upgradeUI = this.GetSingleton<UpgradeUI>();
                        if (upgradeUI) upgradeUI.Refresh();
                    }
                }
                else
                {
                    // Moving from another slot?
                    // Not implemented yet based on requirements, but good to have structure.
                }
            }
        }
    }
}
