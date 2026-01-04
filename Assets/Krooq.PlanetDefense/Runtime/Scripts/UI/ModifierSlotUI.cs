using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Krooq.Common;
using Krooq.Core;

namespace Krooq.PlanetDefense
{
    public class ModifierSlotUI : MonoBehaviour, IDropHandler
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

        protected void OnDisable()
        {
            _slotIndex = -1;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent<ModifierTileUI>(out var tileUI))
            {
                // Handle Drop Logic
                // If tileUI is from Shop -> Buy
                // If tileUI is from another Slot -> Swap or Move (not implemented yet, assuming Shop -> Slot for now)

                if (tileUI.IsShopItem)
                {
                    if (GameManager.SpendResources(tileUI.Modifier.Cost))
                    {
                        // Add to this slot
                        // If slot is occupied, replace? Or fail?
                        // For simplicity, let's say we replace (sell old one?) or just overwrite.
                        // But ActiveModifiers is a List. We need to ensure it has capacity or fill with nulls.

                        GameManager.SetModifier(_slotIndex, tileUI.Modifier);

                        // Refresh UI handled by ShopUI Update loop
                        // But we should probably trigger an event or set dirty
                        if (ShopUI) ShopUI.SetDirty();
                        var modifierUI = this.GetSingleton<ModifierUI>();
                        if (modifierUI) modifierUI.Refresh();
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
