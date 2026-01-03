using UnityEngine;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public class UpgradeUI : MonoBehaviour
    {
        [SerializeField] private Transform _upgradeSlotContainer;

        protected GameManager GameManager => this.GetSingleton<GameManager>();

        protected void OnEnable()
        {
            for (int i = 0; i < GameManager.Data.MaxSlots; i++)
            {
                var slot = GameManager.SpawnUpgradeSlotUI(GameManager.Data.UpgradeSlotPrefab);
                slot.transform.SetParent(_upgradeSlotContainer, false);
                slot.Init(i);
            }
        }

        public void Refresh()
        {
            // Clear Active Slots (remove children of slots)
            foreach (Transform slotTransform in _upgradeSlotContainer)
            {
                for (var i = slotTransform.childCount - 1; i >= 0; i--) GameManager.Despawn(slotTransform.GetChild(i).gameObject);
            }

            // Populate Active Slots
            for (var i = 0; i < GameManager.ActiveUpgrades.Count; i++)
            {
                var tile = GameManager.ActiveUpgrades[i];
                if (tile != null && i < _upgradeSlotContainer.childCount)
                {
                    var slot = _upgradeSlotContainer.GetChild(i);
                    var ui = GameManager.SpawnUpgradeTileUI(GameManager.Data.UpgradeTilePrefab);
                    ui.transform.SetParent(slot, false);
                    ui.Init(tile, false);
                }
            }
        }
    }
}
