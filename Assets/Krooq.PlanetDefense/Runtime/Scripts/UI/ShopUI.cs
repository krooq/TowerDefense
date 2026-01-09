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

        protected RelicBarUI RelicBarUI => this.GetSingleton<RelicBarUI>();
        protected CasterBarUI TowerBarUI => this.GetSingleton<CasterBarUI>();
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
            // if (RelicBarUI != null) RelicBarUI.Refresh();
            // if (TowerBarUI != null) TowerBarUI.Refresh();

            // Populate Available Spells
            if (GameManager.Data.Spells != null)
            {
                foreach (var tile in GameManager.Data.Spells)
                {
                    var ui = GameManager.Spawn(GameManager.Data.SpellTilePrefab);
                    ui.transform.SetParent(_itemContainer, false);
                    ui.Init(tile, true);
                }
            }

            // Populate Available Relics
            if (GameManager.Data.Relics != null)
            {
                foreach (var tile in GameManager.Data.Relics)
                {
                    var ui = GameManager.Spawn(GameManager.Data.RelicTilePrefab);
                    ui.transform.SetParent(_itemContainer, false);
                    ui.Init(tile, true);
                }
            }

            // Populate Available Towers
            if (GameManager.Data.Casters != null)
            {
                foreach (var tile in GameManager.Data.Casters)
                {
                    var ui = GameManager.Spawn(GameManager.Data.CasterTilePrefab);
                    ui.transform.SetParent(_itemContainer, false);
                    ui.Init(tile, true);
                }
            }
        }
    }
}
