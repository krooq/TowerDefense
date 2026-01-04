using UnityEngine;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;
using System.Linq;

namespace Krooq.PlanetDefense
{
    public class ModifierUI : MonoBehaviour
    {
        [SerializeField] private Transform _modifierSlotContainer;

        protected GameManager GameManager => this.GetSingleton<GameManager>();

        protected void OnEnable()
        {
            for (int i = 0; i < GameManager.Data.MaxSlots; i++)
            {
                var slot = GameManager.SpawnModifierSlotUI(GameManager.Data.ModifierSlotPrefab);
                slot.transform.SetParent(_modifierSlotContainer, false);
                slot.Init(i);
            }
        }

        public void Refresh()
        {
            // Clear Active Slots (remove children of slots)
            foreach (Transform slotTransform in _modifierSlotContainer)
            {
                for (var i = slotTransform.childCount - 1; i >= 0; i--) GameManager.Despawn(slotTransform.GetChild(i).gameObject);
            }

            // Populate Active Slots
            foreach (var (modifier, i) in GameManager.ActiveModifiers.WithIndex())
            {
                if (modifier != null && i < _modifierSlotContainer.childCount)
                {
                    var slot = _modifierSlotContainer.GetChild(i);
                    var ui = GameManager.SpawnModifierTileUI(GameManager.Data.ModifierTilePrefab);
                    ui.transform.SetParent(slot, false);
                    ui.Init(modifier, false);
                }
            }
        }
    }
}
