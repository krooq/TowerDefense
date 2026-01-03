using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;

namespace Krooq.PlanetDefense
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] private Transform _itemContainer;

        [SerializeField, ReadOnly] private bool _dirty = true;

        protected UpgradeUI UpgradeUI => this.GetSingleton<UpgradeUI>();
        protected GameManager GameManager => this.GetSingleton<GameManager>();

        protected CanvasGroup CanvasGroup => this.GetCachedComponent<CanvasGroup>();

        protected void Update()
        {
            if (GameManager.State == GameState.Shop)
            {
                if (CanvasGroup.alpha == 0f)
                {
                    CanvasGroup.alpha = 1f;
                    CanvasGroup.interactable = true;
                    CanvasGroup.blocksRaycasts = true;
                    _dirty = true;
                }

                if (_dirty)
                {
                    UpdateUI();
                    _dirty = false;
                }
            }
            else
            {
                CanvasGroup.alpha = 0f;
                CanvasGroup.interactable = false;
                CanvasGroup.blocksRaycasts = false;
            }
        }

        public void SetDirty()
        {
            _dirty = true;
        }

        protected void UpdateUI()
        {
            // Clear Available container
            for (var i = _itemContainer.childCount - 1; i >= 0; i--) GameManager.Despawn(_itemContainer.GetChild(i).gameObject);

            // Refresh Active Slots
            if (UpgradeUI != null) UpgradeUI.Refresh();

            // Populate Available
            if (GameManager.Data.AvailableUpgrades != null)
            {
                foreach (var tile in GameManager.Data.AvailableUpgrades)
                {
                    var ui = GameManager.SpawnUpgradeTileUI(GameManager.Data.UpgradeTilePrefab);
                    ui.transform.SetParent(_itemContainer, false);
                    ui.Init(tile, true);
                }
            }
        }
    }
}
